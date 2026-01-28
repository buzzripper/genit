# GmdlUpdate.ps1
# Updates a .gmdl file's modelRoot attributes with predefined values
# Usage: .\GmdlUpdate.ps1 -FilePath "path\to\file.gmdl"

param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath
)

# Values to set (from Test.gmdl)
$entitiesOutputFolder = "Common\Data.Shared\Entities"
$entitiesNamespace = "Dyvenix.App1.Data.Shared.Entities"
$dbContextOutputFolder = "Common\Data"
$dbContextNamespace = "Dyvenix.App1.Data"
$enumsOutputFolder = "Common\Data.Shared\Entities"
$enumsNamespace = "Dyvenix.App1.Data.Shared.Entities"
$dbContextName = "App1Db"

# Module values to set
$moduleName = "Auth"
$moduleRootFolder = "Auth\Auth.Api"
$moduleNamespace = "Dyvenix.App1.Auth.Api"
$moduleDtoNamespace = "Dyvenix.App1.Auth.Shared.DTOs"
$moduleDtoOutputFolder = "Auth\Auth.Shared\DTOs"
$moduleQueryNamespace = "Dyvenix.App1.Auth.Shared.Queries"
$moduleQueryOutputFolder = "Auth\Auth.Shared\Queries"

# Check if file exists
if (-not (Test-Path $FilePath)) {
    Write-Error "File not found: $FilePath"
    Read-Host "Press Enter to exit"
    exit 1
}

try {
    # Load the XML file
    [xml]$xml = Get-Content -Path $FilePath -Encoding UTF8

    # Get the modelRoot element
    $modelRoot = $xml.modelRoot

    if ($null -eq $modelRoot) {
        Write-Error "modelRoot element not found in file"
        Read-Host "Press Enter to exit"
        exit 1
    }

    # Update the attributes
    $modelRoot.SetAttribute("entitiesOutputFolder", $entitiesOutputFolder)
    $modelRoot.SetAttribute("entitiesNamespace", $entitiesNamespace)
    $modelRoot.SetAttribute("dbContextOutputFolder", $dbContextOutputFolder)
    $modelRoot.SetAttribute("dbContextNamespace", $dbContextNamespace)
    $modelRoot.SetAttribute("enumsOutputFolder", $enumsOutputFolder)
    $modelRoot.SetAttribute("enumsNamespace", $enumsNamespace)
    $modelRoot.SetAttribute("dbContextName", $dbContextName)

    # Update moduleModel element (name = "Auth")
    # The .gmdl uses a default XML namespace, so XPath queries must use a namespace manager.
    $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $ns.AddNamespace("g", $xml.DocumentElement.NamespaceURI)
    $authModule = $xml.SelectSingleNode("//g:moduleModel[@name='$moduleName']", $ns)
    if ($null -eq $authModule) {
        Write-Error "moduleModel element with name '$moduleName' not found in file"
        Read-Host "Press Enter to exit"
        exit 1
    }

    $authModule.SetAttribute("rootFolder", $moduleRootFolder)
    $authModule.SetAttribute("namespace", $moduleNamespace)
    $authModule.SetAttribute("dtoNamespace", $moduleDtoNamespace)
    $authModule.SetAttribute("dtoOutputFolder", $moduleDtoOutputFolder)
    $authModule.SetAttribute("queryNamespace", $moduleQueryNamespace)
    $authModule.SetAttribute("queryOutputFolder", $moduleQueryOutputFolder)

    # Save the file
    $xml.Save($FilePath)

    Write-Host "Successfully updated: $FilePath"
    Write-Host "  entitiesOutputFolder = $entitiesOutputFolder"
    Write-Host "  entitiesNamespace = $entitiesNamespace"
    Write-Host "  dbContextOutputFolder = $dbContextOutputFolder"
    Write-Host "  dbContextNamespace = $dbContextNamespace"
    Write-Host "  enumsOutputFolder = $enumsOutputFolder"
    Write-Host "  enumsNamespace = $enumsNamespace"
    Write-Host "  dbContextName = $dbContextName"

    Write-Host "  moduleModel(name=$moduleName).rootFolder = $moduleRootFolder"
    Write-Host "  moduleModel(name=$moduleName).namespace = $moduleNamespace"
    Write-Host "  moduleModel(name=$moduleName).dtoNamespace = $moduleDtoNamespace"
    Write-Host "  moduleModel(name=$moduleName).dtoOutputFolder = $moduleDtoOutputFolder"
    Write-Host "  moduleModel(name=$moduleName).queryNamespace = $moduleQueryNamespace"
    Write-Host "  moduleModel(name=$moduleName).queryOutputFolder = $moduleQueryOutputFolder"
}
catch {
    Write-Error "Error processing file: $_"
    Read-Host "Press Enter to exit"
    exit 1
}
