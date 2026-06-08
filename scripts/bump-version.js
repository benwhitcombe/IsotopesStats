const fs = require('fs');
const path = require('path');

// Go up one directory from 'scripts' to reach the project root
const indexPath = path.join(__dirname, '..', 'src', 'Website', 'wwwroot', 'index.html');

if (!fs.existsSync(indexPath)) {
    console.warn("bump-version: index.html not found, skipping.");
    process.exit(0);
}

let content = fs.readFileSync(indexPath, 'utf8');

const today = new Date();
const year = today.getFullYear();
const month = String(today.getMonth() + 1).padStart(2, '0');
const day = String(today.getDate()).padStart(2, '0');
const datePrefix = `${year}.${month}.${day}`;

const versionRegex = /<meta name="app-version" content="([^"]+)" \/>/;
const match = content.match(versionRegex);

if (match) {
    const currentVersion = match[1];
    let newVersion;
    
    if (currentVersion.startsWith(datePrefix)) {
        // Increment the build number if it's the same day
        const parts = currentVersion.split('.');
        const buildNum = parseInt(parts[3] || 0, 10);
        newVersion = `${datePrefix}.${buildNum + 1}`;
    } else {
        // Reset to 1 for a new day
        newVersion = `${datePrefix}.1`;
    }
    
    content = content.replace(versionRegex, `<meta name="app-version" content="${newVersion}" />`);
    fs.writeFileSync(indexPath, content, 'utf8');
    console.log(`[pre-commit hook] Bumped app-version to ${newVersion}`);
} else {
    console.warn("bump-version: <meta name=\"app-version\" ... /> tag not found in index.html.");
}
