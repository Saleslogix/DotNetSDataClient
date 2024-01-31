param(
  [bool]$Debug=$True
)

Write-Host "Debug Build?: ${Debug}"

function Test-ExitCode
{
  param([string]$Message)
  if ($LastExitCode -ne 0) {
    Write-Host $Message
    Exit 1
  }
}

$propertyFlags = "WarningLevel=1;TreatWarningsAsErrors=true"
$standard_configuration = "Debug"

if($Debug -ne $true){
  # Release configuration flags
  $standard_configuration = "Release"
  $propertyFlags = "Configuration=Release;WarningLevel=0;TreatWarningsAsErrors=false"
}

Remove-Item .\Saleslogix.SData.Client\obj -Recurse -Force -ErrorAction Ignore
$framework_solution = "DotNetSDataClient.Framework.sln"
& msbuild $framework_solution /maxcpucount:"${Env:NUMBER_OF_PROCESSORS}" /nologo /property:"${propertyFlags}" /target:Rebuild
Test-ExitCode -Message "Error building ${framework_solution}"


$standard_solution = "DotNetSDataClient.Standard.sln"
& dotnet build $standard_solution --configuration "${standard_configuration}" --nologo --property WarningLevel=0 -v q
