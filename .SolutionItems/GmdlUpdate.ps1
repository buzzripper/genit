# GmdlUpdate.ps1
# Updates a .gmdl file's modelRoot attributes with predefined values
# Usage: .\GmdlUpdate.ps1 -FilePath "path\to\file.gmdl"

param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath
)

# Values to set (from Test.gmdl)
$entitiesOutputFolder = "Common\Data.Shared\Entities"
$entitiesNamespace = "yvenix.App1.Data.Shared.Entities"
$dbContextOutputFolder = "Common\Data"
$dbContextNamespace = "Dyvenix.App1.Data"
$enumsOutputFolder = "Common\Data.Shared\Entities"
$enumsNamespace = "yvenix.App1.Data.Shared.Entities"

# Check if file exists
if (-not (Test-Path $FilePath)) {
    Write-Error "File not found: $FilePath"
    exit 1
}

try {
    # Load the XML file
    [xml]$xml = Get-Content -Path $FilePath -Encoding UTF8

    # Get the modelRoot element
    $modelRoot = $xml.modelRoot

    if ($null -eq $modelRoot) {
        Write-Error "modelRoot element not found in file"
        exit 1
    }

    # Update the attributes
    $modelRoot.SetAttribute("entitiesOutputFolder", $entitiesOutputFolder)
    $modelRoot.SetAttribute("entitiesNamespace", $entitiesNamespace)
    $modelRoot.SetAttribute("dbContextOutputFolder", $dbContextOutputFolder)
    $modelRoot.SetAttribute("dbContextNamespace", $dbContextNamespace)
    $modelRoot.SetAttribute("enumsOutputFolder", $enumsOutputFolder)
    $modelRoot.SetAttribute("enumsNamespace", $enumsNamespace)

    # Save the file
    $xml.Save($FilePath)

    Write-Host "Successfully updated: $FilePath"
    Write-Host "  entitiesOutputFolder = $entitiesOutputFolder"
    Write-Host "  entitiesNamespace = $entitiesNamespace"
    Write-Host "  dbContextOutputFolder = $dbContextOutputFolder"
    Write-Host "  dbContextNamespace = $dbContextNamespace"
    Write-Host "  enumsOutputFolder = $enumsOutputFolder"
    Write-Host "  enumsNamespace = $enumsNamespace"
}
catch {
    Write-Error "Error processing file: $_"
    exit 1
}
