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

$configuration = "Debug"
$propertyFlags = "WarningLevel=1;TreatWarningsAsErrors=true"

if($Debug -ne $true){
  # Release configuration flags
  $configuration = "Release"
  $propertyFlags = "Configuration=Release;WarningLevel=0;TreatWarningsAsErrors=false"
}

$solution = "DotNetSDataClient.sln"
& msbuild $solution /maxcpucount:"${Env:NUMBER_OF_PROCESSORS}" /nologo /property:"${propertyFlags}" /target:Rebuild
Test-ExitCode -Message "Error building ${solution}"
