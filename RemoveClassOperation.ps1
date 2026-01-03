$filePath = "D:\Code\buzzripper\genit\Dsl\DslDefinition.dsl"
$content = Get-Content $filePath -Raw -Encoding UTF8

Write-Host "Starting removal of ClassOperation from DslDefinition.dsl..."

# 1. Remove ClassOperation class definition (between Operation and ModelInterface)
Write-Host "1. Removing ClassOperation class definition..."
$pattern1 = '    <DomainClass Id="fe35f199-2784-422d-a1da-37f9f5f6270c" Description="" Name="ClassOperation" DisplayName="Class Operation" Namespace="Dyvenix.GenIt">\r?\n      <BaseClass>\r?\n        <DomainClassMoniker Name="Operation" />\r?\n      </BaseClass>\r?\n      <Properties>\r?\n        <DomainProperty Id="902d987c-217c-482e-a192-d4d832f7ac12" Description="" Name="IsAbstract" DisplayName="Is Abstract" DefaultValue="False">\r?\n          <Type>\r?\n            <ExternalTypeMoniker Name="/System/Boolean" />\r?\n          </Type>\r?\n        </DomainProperty>\r?\n      </Properties>\r?\n    </DomainClass>\r?\n'
$content = $content -replace $pattern1, ''

# 2. Remove ClassOperation ElementMergeDirective from EntityModel
Write-Host "2. Removing ClassOperation ElementMergeDirective..."
$pattern2 = '        <ElementMergeDirective>\r?\n          <Index>\r?\n            <DomainClassMoniker Name="ClassOperation" />\r?\n          </Index>\r?\n          <LinkCreationPaths>\r?\n            <DomainPath>ClassHasOperations\.Operations</DomainPath>\r?\n          </LinkCreationPaths>\r?\n        </ElementMergeDirective>\r?\n'
$content = $content -replace $pattern2, ''

# 3. Remove ClassHasOperations relationship
Write-Host "3. Removing ClassHasOperations relationship..."
$pattern3 = '    <DomainRelationship Id="4a1da39d-6ea0-4857-95ad-81f6633a64ed" Description="" Name="ClassHasOperations" DisplayName="Class Has Operations" Namespace="Dyvenix.GenIt" IsEmbedding="true">\r?\n      <Source>\r?\n        <DomainRole Id="2e1a6b41-4c61-4232-895c-e15334b06341" Description="" Name="EntityModel" DisplayName="EntityModel" PropertyName="Operations" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Operations">\r?\n          <RolePlayer>\r?\n            <DomainClassMoniker Name="EntityModel" />\r?\n          </RolePlayer>\r?\n        </DomainRole>\r?\n      </Source>\r?\n      <Target>\r?\n        <DomainRole Id="c1d3a745-01ec-4b47-b5cc-5038c42efd55" Description="" Name="Operation" DisplayName="Operation" PropertyName="EntityModel" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Entity Model">\r?\n          <RolePlayer>\r?\n            <DomainClassMoniker Name="ClassOperation" />\r?\n          </RolePlayer>\r?\n        </DomainRole>\r?\n      </Target>\r?\n    </DomainRelationship>\r?\n'
$content = $content -replace $pattern3, ''

# 4. Remove Operations compartment from ClassShape
Write-Host "4. Removing Operations compartment from ClassShape..."
$pattern4 = '      <Compartment TitleFillColor="235, 235, 235" Name="OperationsCompartment" Title="Operations" />\r?\n'
$content = $content -replace $pattern4, ''

# 5. Remove ClassHasOperations XmlClassData
Write-Host "5. Removing ClassHasOperations XmlClassData..."
$pattern5 = '      <XmlClassData TypeName="ClassHasOperations" MonikerAttributeName="" SerializeId="true" MonikerElementName="classHasOperationsMoniker" ElementName="classHasOperations" MonikerTypeName="ClassHasOperationsMoniker">\r?\n        <DomainRelationshipMoniker Name="ClassHasOperations" />\r?\n      </XmlClassData>\r?\n'
$content = $content -replace $pattern5, ''

# 6. Remove ClassOperation XmlClassData
Write-Host "6. Removing ClassOperation XmlClassData..."
$pattern6 = '      <XmlClassData TypeName="ClassOperation" MonikerAttributeName="" SerializeId="true" MonikerElementName="classOperationMoniker" ElementName="classOperation" MonikerTypeName="ClassOperationMoniker">\r?\n        <DomainClassMoniker Name="ClassOperation" />\r?\n        <ElementData>\r?\n          <XmlPropertyData XmlName="isAbstract">\r?\n            <DomainPropertyMoniker Name="ClassOperation/IsAbstract" />\r?\n          </XmlPropertyData>\r?\n        </ElementData>\r?\n      </XmlClassData>\r?\n'
$content = $content -replace $pattern6, ''

# 7. Remove operations XmlRelationshipData from EntityModel
Write-Host "7. Removing operations XmlRelationshipData from EntityModel..."
$pattern7 = '          <XmlRelationshipData RoleElementName="operations">\r?\n            <DomainRelationshipMoniker Name="ClassHasOperations" />\r?\n          </XmlRelationshipData>\r?\n'
$content = $content -replace $pattern7, ''

# 8. Remove Operations CompartmentMap from Diagram
Write-Host "8. Removing Operations CompartmentMap from Diagram..."
$pattern8 = '        <CompartmentMap>\r?\n          <CompartmentMoniker Name="ClassShape/OperationsCompartment" />\r?\n          <ElementsDisplayed>\r?\n            <DomainPath>ClassHasOperations\.Operations/!Operation</DomainPath>\r?\n          </ElementsDisplayed>\r?\n          <PropertyDisplayed>\r?\n            <PropertyPath>\r?\n              <DomainPropertyMoniker Name="NamedElement/Name" />\r?\n            </PropertyPath>\r?\n          </PropertyDisplayed>\r?\n        </CompartmentMap>\r?\n'
$content = $content -replace $pattern8, ''

# 9. Remove ClassOperation ElementTool from Designer
Write-Host "9. Removing ClassOperation ElementTool..."
$pattern9 = '      <ElementTool Name="ClassOperation" ToolboxIcon="resources\\operationtool\.bmp" Caption="Class Operation" Tooltip="Create an Operation on a Class" HelpKeyword="ClassOperationF1Keyword">\r?\n        <DomainClassMoniker Name="ClassOperation" />\r?\n      </ElementTool>\r?\n'
$content = $content -replace $pattern9, ''

# Save the file
Write-Host "Saving changes..."
$content | Out-File $filePath -Encoding UTF8 -NoNewline

Write-Host "`nDslDefinition.dsl has been successfully updated!"
Write-Host "All ClassOperation references have been removed."
Write-Host "`nNext steps:"
Write-Host "1. Open the solution in Visual Studio"
Write-Host "2. Right-click on DslDefinition.dsl in Solution Explorer"
Write-Host "3. Select 'Transform All Templates'"
Write-Host "4. Rebuild the solution"
