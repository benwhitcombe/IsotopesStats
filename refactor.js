const fs = require('fs');
const path = require('path');

const modelsDir = path.join(__dirname, 'IsotopesStats', 'Models');
const dtoDir = path.join(__dirname, 'SupabaseRepository', 'Models');
const mappingsDir = path.join(__dirname, 'SupabaseRepository', 'Mappings');

if (!fs.existsSync(dtoDir)) fs.mkdirSync(dtoDir, { recursive: true });
if (!fs.existsSync(mappingsDir)) fs.mkdirSync(mappingsDir, { recursive: true });

const files = fs.readdirSync(modelsDir).filter(f => f.endsWith('.cs') && f !== 'IEntity.cs');

let mappersCode = `using IsotopesStats.Models;
using SupabaseRepository.Models;

namespace SupabaseRepository.Mappings;

public static class ModelMappers
{
`;

// Build a list of all model names so we can correctly map nested objects.
const allModelNames = files.map(f => f.replace('.cs', ''));

files.forEach(file => {
    let content = fs.readFileSync(path.join(modelsDir, file), 'utf8');

    if (!content.includes(': BaseModel')) return;

    const classSigRegex = /public\s+class\s+([A-Za-z0-9_]+)\s*:\s*BaseModel(?:,\s*([A-Za-z0-9_]+))?/;
    const classMatch = content.match(classSigRegex);
    if (!classMatch) return;

    const className = classMatch[1];
    const extraInterface = classMatch[2]; 

    const tableMatch = content.match(/\[Table\([^\]]+\)\]\s*/);
    const tableAttr = tableMatch ? tableMatch[0].trim() : '';
    if (tableAttr) content = content.replace(tableMatch[0], '');

    const newClassDef = extraInterface ? `public record ${className} : ${extraInterface}` : `public record ${className}`;
    content = content.replace(classMatch[0], newClassDef);

    content = content.replace(/using\s+Postgrest\.Attributes;\s*\r?\n/g, '');
    content = content.replace(/using\s+Postgrest\.Models;\s*\r?\n/g, '');

    const cloneRegex1 = new RegExp(`\\s*public\\s+${className}\\s+Clone\\(\\)\\s*=>\\s*\\(${className}\\)this\\.MemberwiseClone\\(\\);`, 'g');
    const cloneRegex2 = new RegExp(`\\s*public\\s+${className}\\s+Clone\\(\\)\\s*{\\s*return\\s+\\(${className}\\)this\\.MemberwiseClone\\(\\);\\s*}`, 'g');
    content = content.replace(cloneRegex1, '');
    content = content.replace(cloneRegex2, '');

    const propRegex = /((?:\[[^\]]+\]\s*)*)public\s+([^ ]+)\s+([^ ]+)\s*{\s*get;\s*set;\s*}(?:\s*=\s*[^;]+;)?/g;
    
    let dtoProps = [];
    let mapToModelProps = [];
    let mapToDtoProps = [];
    let originalBlocks = [];
    
    let match;
    while ((match = propRegex.exec(content)) !== null) {
        const fullBlock = match[0];
        const attributesBlock = match[1];
        const propType = match[2];
        const propName = match[3];

        let hasPostgrestAttr = /\[(PrimaryKey|Column|Reference)/.test(attributesBlock);
        
        if (hasPostgrestAttr) {
            originalBlocks.push({
                fullBlock,
                attributesBlock,
                propType,
                propName
            });
        }
    }

    let modifiedContent = content;

    for (let block of originalBlocks) {
        let postgrestAttrs = [];
        let otherAttrs = [];
        
        const attrMatches = block.attributesBlock.match(/\[([^\]]+)\]/g) || [];
        for (let a of attrMatches) {
            if (a.startsWith('[PrimaryKey') || a.startsWith('[Column') || a.startsWith('[Reference')) {
                // If it's a reference, replace typeof(Model) with typeof(ModelDto)
                let transformedAttr = a;
                if (a.startsWith('[Reference(typeof(')) {
                    let refMatch = a.match(/typeof\(([A-Za-z0-9_]+)\)/);
                    if (refMatch && allModelNames.includes(refMatch[1])) {
                        transformedAttr = a.replace(`typeof(${refMatch[1]})`, `typeof(${refMatch[1]}Dto)`);
                    }
                }
                postgrestAttrs.push(`[${transformedAttr.substring(1, transformedAttr.length-1)}]`);
            } else {
                otherAttrs.push(`[${a.substring(1, a.length-1)}]`);
            }
        }
        
        let isNestedModel = false;
        let baseType = block.propType.replace('?', '');
        if (allModelNames.includes(baseType)) {
            isNestedModel = true;
        }

        let dtoPropType = block.propType;
        if (isNestedModel) {
            dtoPropType = dtoPropType.replace(baseType, `${baseType}Dto`);
        }

        // Construct DTO property
        let dtoProp = `    ${postgrestAttrs.join('\n    ')}\n    public ${dtoPropType} ${block.propName} { get; set; }`;
        let defMatch = block.fullBlock.match(/{\s*get;\s*set;\s*}(\s*=\s*[^;]+;)/);
        if (defMatch) {
            dtoProp += defMatch[1];
        }
        dtoProps.push(dtoProp.trim());

        // Map Property
        if (isNestedModel) {
            let isNullable = block.propType.includes('?');
            if (isNullable) {
                mapToModelProps.push(`${block.propName} = dto.${block.propName}?.ToModel()`);
                mapToDtoProps.push(`${block.propName} = model.${block.propName}?.ToDto()`);
            } else {
                mapToModelProps.push(`${block.propName} = dto.${block.propName}?.ToModel() ?? new ${baseType}()`);
                mapToDtoProps.push(`${block.propName} = model.${block.propName}?.ToDto() ?? new ${baseType}Dto()`);
            }
        } else {
            mapToModelProps.push(`${block.propName} = dto.${block.propName}`);
            mapToDtoProps.push(`${block.propName} = model.${block.propName}`);
        }

        // Construct modified original property
        let newPropBlock = ``;
        if (otherAttrs.length > 0) {
            newPropBlock += `${otherAttrs.join('\n    ')}\n    `;
        }
        newPropBlock += `public ${block.propType} ${block.propName} { get; set; }`;
        
        if (defMatch) {
            newPropBlock += defMatch[1];
        }
        newPropBlock = newPropBlock.trim();

        modifiedContent = modifiedContent.replace(block.fullBlock, newPropBlock);
    }

    let dtoContent = `using Postgrest.Attributes;
using Postgrest.Models;
using IsotopesStats.Models;

namespace SupabaseRepository.Models;

${tableAttr}
public class ${className}Dto : BaseModel
{
    ${dtoProps.join('\n\n    ')}
}
`;

    fs.writeFileSync(path.join(dtoDir, `${className}Dto.cs`), dtoContent, 'utf8');
    fs.writeFileSync(path.join(modelsDir, file), modifiedContent, 'utf8');

    mappersCode += `
    public static ${className} ToModel(this ${className}Dto dto)
    {
        if (dto == null) return null!;
        return new ${className}
        {
            ${mapToModelProps.join(',\n            ')}
        };
    }

    public static ${className}Dto ToDto(this ${className} model)
    {
        if (model == null) return null!;
        return new ${className}Dto
        {
            ${mapToDtoProps.join(',\n            ')}
        };
    }
`;
});

mappersCode += `
}
`;

fs.writeFileSync(path.join(mappingsDir, 'ModelMappers.cs'), mappersCode, 'utf8');

console.log('Refactoring with nested models complete.');