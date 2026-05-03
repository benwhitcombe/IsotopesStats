using IsotopesStats.Domain.Models;
using IsotopesStats.SupabaseRepository.Mappings;
using Postgrest;
using Postgrest.Models;
using Postgrest.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IsotopesStats.SupabaseRepository.Repositories;

internal abstract class BaseRepository
{
    protected readonly Supabase.Client Supabase;
    protected readonly SupabaseMapper Mapper;

    protected BaseRepository(Supabase.Client supabase, SupabaseMapper mapper)
    {
        Supabase = supabase;
        Mapper = mapper;
    }

    protected async Task<List<ModelType>> GetListAsync<ModelType, DTOType>(
        Func<DTOType, ModelType> toModel,
        string orderColumn,
        Constants.Ordering ordering = Constants.Ordering.Ascending,
        bool onlyActive = true)
        where DTOType : BaseModel, new()
    {
        Table<DTOType> query = (Table<DTOType>)Supabase.From<DTOType>();
        
        if (onlyActive)
        {
            // Only apply the filter if the DTO has an IsDeleted property
            if (typeof(DTOType).GetProperty("IsDeleted") != null)
            {
                // Use string "false" for boolean filters to avoid Postgrest type exceptions in generic methods
                query = query.Filter("isdeleted", Constants.Operator.Equals, "false");
            }
        }

        ModeledResponse<DTOType> response = await query.Order(orderColumn, ordering).Get();
        return response.Models.Select(toModel).ToList();
    }

    protected async Task<int> InsertAsync<ModelType, DTOType>(ModelType model, Func<ModelType, DTOType> toDTO)
        where DTOType : BaseModel, new()
    {
        ModeledResponse<DTOType> response = await Supabase.From<DTOType>().Insert(toDTO(model));
        
        PropertyInfo? prop = typeof(DTOType).GetProperty("Id");
        object? value = prop?.GetValue(response.Model);
        return value is int intValue ? intValue : 0;
    }

    protected async Task UpdateAsync<ModelType, DTOType>(ModelType model, Func<ModelType, DTOType> toDTO)
        where DTOType : BaseModel, new()
    {
        await Supabase.From<DTOType>().Update(toDTO(model));
    }

    protected async Task SoftDeleteAsync<DTOType>(int id)
        where DTOType : BaseModel, new()
    {
        DTOType dto = new DTOType();
        PropertyInfo? idProp = typeof(DTOType).GetProperty("Id");
        PropertyInfo? delProp = typeof(DTOType).GetProperty("IsDeleted");
        
        idProp?.SetValue(dto, id);
        delProp?.SetValue(dto, true);
        
        await Supabase.From<DTOType>().Update(dto);
    }

    protected async Task<bool> IsUniqueAsync<DTOType>(string column, string value, int excludeId = 0)
        where DTOType : BaseModel, new()
    {
        Table<DTOType> query = (Table<DTOType>)Supabase.From<DTOType>()
            .Filter(column, Constants.Operator.Equals, value);

        // Only apply the filter if the DTO has an IsDeleted property
        if (typeof(DTOType).GetProperty("IsDeleted") != null)
        {
            query = query.Filter("isdeleted", Constants.Operator.Equals, "false");
        }

        if (excludeId != 0)
        {
            query = query.Filter("id", Constants.Operator.NotEqual, excludeId);
        }

        ModeledResponse<DTOType> response = await query.Get();
        return response.Models.Count == 0;
    }
}
