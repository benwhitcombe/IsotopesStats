const fs = require('fs');

async function main() {
  const envPath = 'e:/gemini/IsotopesStats/supabase/.env';
  let supabaseUrl = '';
  let supabaseServiceKey = '';
  
  if (fs.existsSync(envPath)) {
    const lines = fs.readFileSync(envPath, 'utf8').split('\n');
    for (const line of lines) {
      if (line.startsWith('SUPABASE_URL=')) supabaseUrl = line.split('=')[1].trim();
      if (line.startsWith('SUPABASE_SERVICE_ROLE_KEY=')) supabaseServiceKey = line.split('=')[1].trim();
    }
  }

  if (!supabaseUrl) {
    console.log("NO URL");
    return;
  }

  const response = await fetch(`${supabaseUrl}/auth/v1/admin/generate_link`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${supabaseServiceKey}`
    },
    body: JSON.stringify({
      type: 'recovery',
      email: 'redstonerentals@hotmail.com'
    })
  });

  const data = await response.json();
  if (data.properties && data.properties.action_link) {
    console.log("LINK:", data.properties.action_link);
  } else {
    console.log("Error:", data);
  }
}
main();
