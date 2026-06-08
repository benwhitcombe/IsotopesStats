$a = [System.Reflection.Assembly]::LoadFrom("E:\gemini\IsotopesStats\src\Website\bin\Debug\net8.0\Supabase.Functions.dll")
foreach($t in $a.GetTypes()) {
  Write-Host $t.FullName
}
