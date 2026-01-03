<?xml version="1.0" encoding="utf-8"?>
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="9d433ece-11d0-4cbc-9a3b-82824193f347" Description="Description for Dyvenix.GenIt.GenIt" Name="GenIt" DisplayName="GenIt" Namespace="Dyvenix.GenIt" ProductName="GenIt" CompanyName="Dyvenix" PackageGuid="3aa8cbb2-f0c4-4628-bc99-158569733469" PackageNamespace="Dyvenix.GenIt" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="9913cfeb-b29a-4eb0-aba0-2e8c046b87e0" Description="" Name="NamedElement" DisplayName="Named Element" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt">
      <Properties>
        <DomainProperty Id="34e4a209-7491-4a75-9f70-f2b75ab46ed5" Description="" Name="Name" DisplayName="Name" DefaultValue="" Category="General" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="884883bf-3682-4dd8-8845-36fd3b212b3e" Description="" Name="ModelRoot" DisplayName="Model Root" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="2847358f-5cff-4639-ac36-424b18d1f769" Description="Description for Dyvenix.GenIt.ModelRoot.Entities Output Folder" Name="EntitiesOutputFolder" DisplayName="Entities Output Folder" Category="Entities">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9c8ff99e-ff07-4379-b55d-116f4d541c5f" Description="Description for Dyvenix.GenIt.ModelRoot.Entities Namespace" Name="EntitiesNamespace" DisplayName="Entities Namespace" Category="Entities">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2c2607f1-8c23-4964-8c11-d7b01cdfecd4" Description="Description for Dyvenix.GenIt.ModelRoot.Templates Folder" Name="TemplatesFolder" DisplayName="Templates Folder">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ffe37e03-971d-4dd8-9222-d0d5785bef4d" Description="Description for Dyvenix.GenIt.ModelRoot.Db Context Output Folder" Name="DbContextOutputFolder" DisplayName="Db Context Output Folder" Category="DbContext">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="94cef65b-5fb6-424b-8c28-0626951051f6" Description="Description for Dyvenix.GenIt.ModelRoot.Db Context Namespace" Name="DbContextNamespace" DisplayName="Db Context Namespace" Category="DbContext">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3" Description="Custom using statements for the DbContext (one per line)" Name="DbContextUsings" DisplayName="Db Context Usings" Category="DbContext">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c89338c6-f414-4b3d-8bef-c394af76864b" Description="Description for Dyvenix.GenIt.ModelRoot.Enums Output Folder" Name="EnumsOutputFolder" DisplayName="Enums Output Folder" Category="Enums">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="866010b0-d180-44b3-939b-48fb2eac91bb" Description="Description for Dyvenix.GenIt.ModelRoot.Enums Namespace" Name="EnumsNamespace" DisplayName="Enums Namespace" Category="Enums">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c62cace8-2504-4f5c-8071-aa0440e1e58e" Description="Description for Dyvenix.GenIt.ModelRoot.Enabled" Name="EntitiesEnabled" DisplayName="Enabled" DefaultValue="true" Category="Entities">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="18526060-805b-4f63-ba29-4b29c43b0498" Description="Description for Dyvenix.GenIt.ModelRoot.Enabled" Name="DbContextEnabled" DisplayName="Enabled" DefaultValue="true" Category="DbContext">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f1a3fdde-cea3-44f8-9748-27d429982104" Description="Description for Dyvenix.GenIt.ModelRoot.Enabled" Name="EnumsEnabled" DisplayName="Enabled" DefaultValue="true" Category="Enums">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1" Description="Background color for the diagram surface" Name="DiagramBackgroundColor" DisplayName="Diagram Background Color" DefaultValue="White" Category="Diagram">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2" Description="Color for association connector lines" Name="AssociationLineColor" DisplayName="Association Line Color" DefaultValue="113, 111, 110" Category="Diagram">
          <Type>
            <ExternalTypeMoniker Name="/System.Drawing/Color" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="734e8e6b-74f7-42c3-8b70-066d67928c78" Description="Description for Dyvenix.GenIt.ModelRoot.Incl Header" Name="InclHeader" DisplayName="Incl Header" DefaultValue="true">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Comment" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelRootHasComments.Comments</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ModelType" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelRootHasTypes.Types</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="a333bc7a-0057-4ca9-bdbe-19524aded57a" Description="" Name="EntityModel" DisplayName="Entity Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="a1a1a1a1-1111-1111-1111-111111111111" Description="Custom attributes for the entity (one per line)" Name="Attributes" DisplayName="Attributes" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a2a2a2a2-2222-2222-2222-222222222222" Description="Custom using statements for the entity (one per line)" Name="Usings" DisplayName="Usings" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a3a3a3a3-3333-3333-3333-333333333333" Description="" Name="Auditable" DisplayName="Auditable" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a4a4a4a4-4444-4444-4444-444444444444" Description="" Name="GenerateCode" DisplayName="Generate Code" DefaultValue="true" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a5a5a5a5-5555-5555-5555-555555555555" Description="" Name="InclRowVersion" DisplayName="Incl Row Version" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="PropertyModel" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasProperties.Properties</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="NavigationProperty" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasNavigationProperties.NavigationProperties</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ServiceModel" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>EntityModelHasServiceModels.ServiceModels</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="4df99ada-d0ce-4ea5-82a0-2d7409e38475" Description="A property of a class." Name="PropertyModel" DisplayName="Property Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="4ef9ef25-f6e7-46f2-9e48-ed9323cc081f" Description="" Name="DataType" DisplayName="Data Type" DefaultValue="String" Category="Type">
          <Type>
            <DomainEnumerationMoniker Name="DataType" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d1e2f3a4-b5c6-4d7e-8f9a-0b1c2d3e4f5a" Description="Name of the enum type when DataType is Enum" Name="EnumTypeName" DisplayName="Enum Type Name" DefaultValue="" Category="Type">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d" Description="Maximum length for string types" Name="Length" DisplayName="Length" DefaultValue="0" Category="Type">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b9a0da77-ee42-4cd5-a7d3-85a8d724691e" Description="" Name="InitialValue" DisplayName="Initial Value" DefaultValue="" Category="Value">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b1b1b1b1-1111-1111-1111-111111111111" Description="Custom attributes for the property (one per line)" Name="Attributes" DisplayName="Attributes" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b2b2b2b2-2222-2222-2222-222222222222" Description="Custom using statements for the property (one per line)" Name="Usings" DisplayName="Usings" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b3b3b3b3-3333-3333-3333-333333333333" Description="" Name="IsPrimaryKey" DisplayName="Is Primary Key" DefaultValue="false" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b4b4b4b4-4444-4444-4444-444444444444" Description="" Name="IsNullable" DisplayName="Is Nullable" DefaultValue="false" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b5b5b5b5-5555-5555-5555-555555555555" Description="" Name="IsIdentity" DisplayName="Is Identity" DefaultValue="false" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b6b6b6b6-6666-6666-6666-666666666666" Description="" Name="IsIndexed" DisplayName="Is Indexed" DefaultValue="false" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b7b7b7b7-7777-7777-7777-777777777777" Description="" Name="IsIndexUnique" DisplayName="Is Index Unique" DefaultValue="false" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b8b8b8b8-8888-8888-8888-888888888888" Description="" Name="IsIndexClustered" DisplayName="Is Index Clustered" DefaultValue="false" Category="Database">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e40dafb9-46ce-49f9-9932-23945a811c0c" Description="Description for Dyvenix.GenIt.PropertyModel.Is Foreign Key" Name="IsForeignKey" DisplayName="Is Foreign Key">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b9b9b9b9-9999-9999-9999-999999999999" Description="Indicates this is an auto-generated RowVersion property" Name="IsRowVersion" DisplayName="Is Row Version" DefaultValue="false" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="27906d32-ad8a-4efd-98c0-95e0c0798f7c" Description="" Name="Comment" DisplayName="Comment" Namespace="Dyvenix.GenIt">
      <Properties>
        <DomainProperty Id="46f793ed-fdff-47c7-b246-678c2ec68699" Description="" Name="Text" DisplayName="Text" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="2d30faa3-bcec-4005-86f9-5a8df285db7b" Description="An Operation of a Class." Name="Operation" DisplayName="Operation" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt">
      <Notes>Abstract base class of ClassOperation and InterfaceOperation.</Notes>
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="4e5f8ae0-e3b4-4d76-9405-c8bea0e4a58a" Description="" Name="Signature" DisplayName="Signature" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="5813ab92-155f-4833-9e7f-9d4b55444a88" Description="" Name="Concurrency" DisplayName="Concurrency" DefaultValue="Sequential">
          <Type>
            <DomainEnumerationMoniker Name="OperationConcurrency" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="05800dff-e9f0-4238-a80d-0cecc8bb383a" Description="" Name="Precondition" DisplayName="Precondition" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="6761ea2a-69f9-4ecf-b239-bb6b54e10229" Description="" Name="Postcondition" DisplayName="Postcondition" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="1529d3a7-27a1-4fb5-92bd-2911316b0a6a" Description="" Name="ModelInterface" DisplayName="Model Interface" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="InterfaceOperation" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>InterfaceHasOperation.Operations</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="51d76f1a-3480-468c-9a08-7e33c064a30e" Description="" Name="InterfaceOperation" DisplayName="Interface Operation" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="Operation" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="ac123494-47dc-4b1b-991a-a5c0493a0c99" Description="" Name="ModelType" DisplayName="Model Type" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Comment" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>CommentReferencesSubjects.Comments</DomainPath>
            <DomainPath>ModelRootHasTypes.ModelRoot/!ModelRoot/ModelRootHasComments.Comments</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="ac2e0a3e-9998-420d-82fd-8d40dbab20f7" Description="Element with a Description" Name="ClassModelElement" DisplayName="Class Model Element" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt">
      <Notes>Abstract base of all elements that have a Description property.</Notes>
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="77a2653e-d7ee-4729-9dd2-d1c73e8ebbf0" Description="This is a Description." Name="Description" DisplayName="Description" Category="General">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="b1c2d3e4-f5a6-4b7c-8d9e-0f1a2b3c4d5e" Description="A member of an enumeration." Name="EnumMember" DisplayName="Enum Member" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="c2d3e4f5-a6b7-4c8d-9e0f-1a2b3c4d5e6f" Description="The numeric value of the enum member" Name="Value" DisplayName="Value" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a" Description="An enumeration type." Name="EnumModel" DisplayName="Enum Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="e4f5a6b7-c8d9-4e0f-1a2b-3c4d5e6f7a8b" Description="Whether this enum is defined externally" Name="IsExternal" DisplayName="Is External" DefaultValue="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f5a6b7c8-d9e0-4f1a-2b3c-4d5e6f7a8b9c" Description="Whether this enum has the [Flags] attribute" Name="IsFlags" DisplayName="Is Flags" DefaultValue="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a6b7c8d9-e0f1-4a2b-3c4d-5e6f7a8b9c0d" Description="Whether code should be generated for this enum" Name="GenerateCode" DisplayName="Generate Code" DefaultValue="true" Category="Code">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="EnumMember" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>EnumHasMembers.Members</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="a1b2c3d4-e5f6-7890-abcd-ef1234567890" Description="A navigation property representing a relationship to another entity." Name="NavigationProperty" DisplayName="Navigation Property" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="b2c3d4e5-f6a7-8901-bcde-f23456789012" Description="Name of the target entity type" Name="TargetEntityName" DisplayName="Target Entity" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c3d4e5f6-a7b8-9012-cdef-345678901234" Description="Whether this navigation property is a collection" Name="IsCollection" DisplayName="Is Collection" DefaultValue="false">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="e97460a8-b0c2-4ad1-8647-ea4e27d2a20d" Description="Description for Dyvenix.GenIt.FilterPropertyModel" Name="FilterPropertyModel" DisplayName="Filter Property Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelRoot" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="a62c7c1d-5ab4-4917-bd52-a13a77f94d63" Description="" Name="IsOptional" DisplayName="Is Optional">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b4574f7b-e7cf-4741-9f42-456b2cbafe9e" Description="" Name="IsInternal" DisplayName="Is Internal">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fc2d5cff-abd9-4d0f-a208-dd028b12a0b3" Description="" Name="InternalValue" DisplayName="Internal Value">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="d1adda56-215a-4935-b298-b323e4db36ae" Description="Description for Dyvenix.GenIt.ReadMethodModel" Name="ReadMethodModel" DisplayName="Read Method Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelRoot" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="3eba03a0-4a38-4b84-b92f-df0b6eb699d5" Description="Description for Dyvenix.GenIt.ReadMethodModel.Item Id" Name="ItemId" DisplayName="Item Id">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="195dd15c-8511-4ac1-a981-4d3321013e64" Description="Description for Dyvenix.GenIt.ReadMethodModel.Incl Paging" Name="InclPaging" DisplayName="Incl Paging">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="14903356-51d9-4bdc-9d67-4cecc1d5236d" Description="Description for Dyvenix.GenIt.ReadMethodModel.Use Query" Name="UseQuery" DisplayName="Use Query">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="38e5475c-d4f4-4c0a-802f-1d7e1f992ba5" Description="Description for Dyvenix.GenIt.ReadMethodModel.Incl Sorting" Name="InclSorting" DisplayName="Incl Sorting">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="eb611af5-b2c6-40fc-a876-e09706222756" Description="Description for Dyvenix.GenIt.ReadMethodModel.Display Order" Name="DisplayOrder" DisplayName="Display Order">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="ae6548ac-6c00-4749-b7a6-ba456df529f8" Description="Description for Dyvenix.GenIt.ReadMethodModel.Attributes" Name="Attributes" DisplayName="Attributes">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="dfb40532-a576-4b0e-bebe-90c3c47c3ae3" Description="Description for Dyvenix.GenIt.UpdatePropertyModel" Name="UpdatePropertyModel" DisplayName="Update Property Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelRoot" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="f5660c8b-88ae-4d6d-8304-0d0800788262" Description="Description for Dyvenix.GenIt.UpdatePropertyModel.Is Optional" Name="IsOptional" DisplayName="Is Optional">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="0d0babe0-406d-490d-ae7a-47ad0ddb6081" Description="Description for Dyvenix.GenIt.UpdateMethodModel" Name="UpdateMethodModel" DisplayName="Update Method Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelRoot" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="880bb24a-b5b9-4816-9f0a-d95d5d7f210c" Description="Description for Dyvenix.GenIt.UpdateMethodModel.Item Id" Name="ItemId" DisplayName="Item Id">
          <Type>
            <ExternalTypeMoniker Name="/System/Guid" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e5e99a5a-7ed8-4c53-86b0-6cf9f4415a97" Description="Description for Dyvenix.GenIt.UpdateMethodModel.Use Dto" Name="UseDto" DisplayName="Use Dto">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2d1897ce-b2a7-4cfb-af36-b5f0c6c1cd13" Description="Description for Dyvenix.GenIt.UpdateMethodModel.Display Order" Name="DisplayOrder" DisplayName="Display Order">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="2d802e08-f549-40a0-8cd2-80ca5f72acbc" Description="Description for Dyvenix.GenIt.ServiceModel" Name="ServiceModel" DisplayName="Service Model" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ModelRoot" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="e8ddf73c-45d6-4deb-9842-3bea5db66783" Description="Description for Dyvenix.GenIt.ServiceModel.Enabled" Name="Enabled" DisplayName="Enabled">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4ab8767e-1987-49f2-9b86-f8e91cef4b2c" Description="Description for Dyvenix.GenIt.ServiceModel.Incl Create" Name="InclCreate" DisplayName="Incl Create">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="9775b002-4da4-48be-8b5b-9671a6809e89" Description="Description for Dyvenix.GenIt.ServiceModel.Incl Update" Name="InclUpdate" DisplayName="Incl Update">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="4d35b8b7-148d-44a9-9cca-71c517a19c6c" Description="Description for Dyvenix.GenIt.ServiceModel.Incl Delete" Name="InclDelete" DisplayName="Incl Delete">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f91b7374-a09f-48e5-8d9c-2f756727a568" Description="Description for Dyvenix.GenIt.ServiceModel.Incl Controller" Name="InclController" DisplayName="Incl Controller">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7308866d-339b-4ebb-8596-8b4ec03b74f5" Description="Description for Dyvenix.GenIt.ServiceModel.Version" Name="Version" DisplayName="Version">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="570fa364-ef5f-42ee-969d-e79e511a0832" Description="Description for Dyvenix.GenIt.ServiceModel.Service Usings" Name="ServiceUsings" DisplayName="Service Usings">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="561c9f21-eaa9-43ba-bb72-8c9b3d281a45" Description="Description for Dyvenix.GenIt.ServiceModel.Service Attributes" Name="ServiceAttributes" DisplayName="Service Attributes">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f56f4698-6675-4600-bfe4-baa46bdb88d9" Description="Description for Dyvenix.GenIt.ServiceModel.Controller Usings" Name="ControllerUsings" DisplayName="Controller Usings">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f06cdc0d-c358-4d2d-bcaa-498226202c8f" Description="Description for Dyvenix.GenIt.ServiceModel.Controller Attributes" Name="ControllerAttributes" DisplayName="Controller Attributes">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="4a55a93f-ffed-423c-ad69-a1b5c9c85a1e" Description="Associations between Classes." Name="Association" DisplayName="Association" Namespace="Dyvenix.GenIt" AllowsDuplicates="true">
      <Notes>This is the abstract base relationship of the several kinds of association between Classes.
      It defines the Properties that are attached to each association.</Notes>
      <Properties>
        <DomainProperty Id="7f3fc48a-1968-44e9-91aa-ee8d93c10f89" Description="" Name="SourceMultiplicity" DisplayName="Source Multiplicity" DefaultValue="One" Category="Source">
          <Type>
            <DomainEnumerationMoniker Name="Multiplicity" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="99ceb0e6-4515-4c03-96e9-b961d66611de" Description="" Name="SourceRoleName" DisplayName="Source Role Name" DefaultValue="" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="a7b8c9d0-1234-5678-9abc-def012345678" Description="Whether to generate a navigation property on the source entity" Name="GenSourceNavProperty" DisplayName="Gen Source Nav Property" DefaultValue="true" Category="Source">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="007c3000-0bf3-4179-a114-883ede04c3df" Description="" Name="TargetMultiplicity" DisplayName="Target Multiplicity" DefaultValue="Many" Category="Target">
          <Type>
            <DomainEnumerationMoniker Name="Multiplicity" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d0dfb7ed-2955-4348-b1e2-c8d8da294082" Description="" Name="TargetRoleName" DisplayName="Target Role Name" DefaultValue="" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b8c9d0e1-2345-6789-abcd-ef0123456789" Description="Whether to generate a navigation property on the target entity" Name="GenTargetNavProperty" DisplayName="Gen Target Nav Property" DefaultValue="false" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c9d0e1f2-3456-789a-bcde-f01234567890" Description="Name of the FK property on the target entity" Name="FkPropertyName" DisplayName="FK Property Name" DefaultValue="" Category="Target">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="e473ff4f-acab-4329-a392-a85c91dc86e6" Description="" Name="Source" DisplayName="Source" PropertyName="Targets" PropertyDisplayName="Targets">
          <Notes>The Targets property on a ModelClass will include all the elements targeted by every kind of Association.</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="8c669209-f190-4b19-9da5-6cfe3696e4fa" Description="" Name="Target" DisplayName="Target" PropertyName="Sources" PropertyDisplayName="Sources">
          <Notes>The Sources property on a EntityModel will include all the elements sourced by every kind of Association.</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="03a9c537-a07d-44d2-b9e9-7904b35de3dd" Description="" Name="ClassHasProperties" DisplayName="Class Has Properties" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="50d60282-915b-4102-96ea-d10a5ddc1dd4" Description="" Name="EntityModel" DisplayName="Entity Model" PropertyName="Properties" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Properties">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="36ae1ba4-57f2-4f07-af06-788b29609abb" Description="" Name="Attribute" DisplayName="Attribute" PropertyName="EntityModel" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Entity Model">
          <RolePlayer>
            <DomainClassMoniker Name="PropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="35b3681b-c415-4a5b-9cea-292282b31f57" Description="" Name="ModelRootHasComments" DisplayName="Model Root Has Comments" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="4c2a7d97-4eaa-4362-ba99-62cf697db3fd" Description="" Name="ModelRoot" DisplayName="Model Root" PropertyName="Comments" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Comments">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="f781ec71-927f-4ed1-a240-005eaa8fc5a5" Description="" Name="Comment" DisplayName="Comment" PropertyName="ModelRoot" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Model Root">
          <RolePlayer>
            <DomainClassMoniker Name="Comment" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="8e562d87-9e68-49dd-be03-fda08f1dd962" Description="Inheritance between Classes." Name="Generalization" DisplayName="Generalization" Namespace="Dyvenix.GenIt">
      <Properties>
        <DomainProperty Id="3279d758-d23a-46a1-8ede-3add260d77b2" Description="" Name="Discriminator" DisplayName="Discriminator" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="5ce4b528-2873-402a-9135-a0e9ebd0eb58" Description="" Name="Superclass" DisplayName="Superclass" PropertyName="Subclasses" PropertyDisplayName="Subclasses">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="e749d5da-fcb0-4724-bb41-4693cf9ee715" Description="" Name="Subclass" DisplayName="Subclass" PropertyName="Superclass" Multiplicity="ZeroOne" PropertyDisplayName="Superclass">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="2a551631-c1f0-43f3-82c5-2643ed5c0743" Description="" Name="ModelRootHasTypes" DisplayName="Model Root Has Types" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="e1f6d228-1927-45c1-8416-d2d564ed9818" Description="" Name="ModelRoot" DisplayName="Model Root" PropertyName="Types" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Types">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="73a07a95-e75b-4047-8b09-e39867694033" Description="" Name="Type" DisplayName="Type" PropertyName="ModelRoot" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="071232a2-4f5f-4ef1-b311-40f07892e04d" Description="" Name="InterfaceHasOperation" DisplayName="Interface Has Operation" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="daefc3db-ff63-4141-bdde-9b49ca0dbd8f" Description="" Name="Interface" DisplayName="Interface" PropertyName="Operations" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Operations">
          <RolePlayer>
            <DomainClassMoniker Name="ModelInterface" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="c06327a8-965b-4db0-8158-d803528c7580" Description="" Name="Operation" DisplayName="Operation" PropertyName="Interface" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Interface">
          <RolePlayer>
            <DomainClassMoniker Name="InterfaceOperation" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="9d833684-8699-445d-8d20-d09a6be822d1" Description="" Name="CommentReferencesSubjects" DisplayName="Comment References Subjects" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="cf4e3e38-999a-47bd-a73b-971c502cb8f7" Description="" Name="Comment" DisplayName="Comment" PropertyName="Subjects" PropertyDisplayName="Subjects">
          <RolePlayer>
            <DomainClassMoniker Name="Comment" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="c39cd6b7-e5f3-4e42-9b4a-c3b68ab8363d" Description="" Name="Subject" DisplayName="Subject" PropertyName="Comments" PropertyDisplayName="Comments">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="268379b1-6145-468b-a27f-c412e3c05e8c" Description="" Name="Implementation" DisplayName="Implementation" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="666eff36-18d9-4a5d-bb41-7bfa9f53e354" Description="" Name="Implement" DisplayName="Implement" PropertyName="Implementors" PropertyDisplayName="Implementors">
          <RolePlayer>
            <DomainClassMoniker Name="ModelInterface" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="bb243312-1df8-4883-8a04-5ad2ab7ffe93" Description="" Name="Implementor" DisplayName="Implementor" PropertyName="Implements" PropertyDisplayName="Implements">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b" Description="" Name="EnumHasMembers" DisplayName="Enum Has Members" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c" Description="" Name="EnumModel" DisplayName="Enum Model" PropertyName="Members" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Members">
          <RolePlayer>
            <DomainClassMoniker Name="EnumModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d" Description="" Name="Member" DisplayName="Member" PropertyName="EnumModel" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Enum Model">
          <RolePlayer>
            <DomainClassMoniker Name="EnumMember" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="d4e5f6a7-b8c9-0123-4567-89abcdef0123" Description="" Name="ClassHasNavigationProperties" DisplayName="Class Has Navigation Properties" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="e5f6a7b8-c9d0-1234-5678-9abcdef01234" Description="" Name="EntityModel" DisplayName="Entity Model" PropertyName="NavigationProperties" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Navigation Properties">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="f6a7b8c9-d0e1-2345-6789-abcdef012345" Description="" Name="NavigationProperty" DisplayName="Navigation Property" PropertyName="EntityModel" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Entity Model">
          <RolePlayer>
            <DomainClassMoniker Name="NavigationProperty" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="5284b992-a619-46e6-9ec8-41ca28fc0fa7" Description="Description for Dyvenix.GenIt.FilterPropertyModelHasProperty" Name="FilterPropertyModelHasProperty" DisplayName="Filter Property Model Has Property" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="17d276ff-cd02-40a8-99a1-29bdfff796bf" Description="Description for Dyvenix.GenIt.FilterPropertyModelHasProperty.FilterPropertyModel" Name="FilterPropertyModel" DisplayName="Filter Property Model" PropertyName="PropertyModel" Multiplicity="One" PropertyDisplayName="Property Model">
          <RolePlayer>
            <DomainClassMoniker Name="FilterPropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="0b288384-2de6-4188-b81d-dd343d2c541a" Description="Description for Dyvenix.GenIt.FilterPropertyModelHasProperty.PropertyModel" Name="PropertyModel" DisplayName="Property Model" PropertyName="FilterPropertyModel" Multiplicity="One" PropertyDisplayName="Filter Property Model">
          <RolePlayer>
            <DomainClassMoniker Name="PropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="32cb6cdb-1478-47f4-829d-937554a834f2" Description="Description for Dyvenix.GenIt.ReadMethodModelHasFilterProperties" Name="ReadMethodModelHasFilterProperties" DisplayName="Read Method Model Has Filter Properties" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="0f995100-09ad-4176-ade0-88937e672b83" Description="Description for Dyvenix.GenIt.ReadMethodModelHasFilterProperties.ReadMethodModel" Name="ReadMethodModel" DisplayName="Read Method Model" PropertyName="FilterPropertyModeled" PropertyDisplayName="Filter Property Modeled">
          <RolePlayer>
            <DomainClassMoniker Name="ReadMethodModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="a86dafce-f80a-4f02-93e8-25e90ad5cfa9" Description="Description for Dyvenix.GenIt.ReadMethodModelHasFilterProperties.FilterPropertyModel" Name="FilterPropertyModel" DisplayName="Filter Property Model" PropertyName="ReadMethodModeled" PropertyDisplayName="Read Method Modeled">
          <RolePlayer>
            <DomainClassMoniker Name="FilterPropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="d46acff7-473f-47fa-b427-5d628500c5a0" Description="Description for Dyvenix.GenIt.UpdatePropertyModelHasPropertyModel" Name="UpdatePropertyModelHasPropertyModel" DisplayName="Update Property Model Has Property Model" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="ad66d0c1-b9fe-4aa8-8638-8aa72d162dcf" Description="Description for Dyvenix.GenIt.UpdatePropertyModelHasPropertyModel.UpdatePropertyModel" Name="UpdatePropertyModel" DisplayName="Update Property Model" PropertyName="PropertyModel" Multiplicity="One" PropertyDisplayName="Property Model">
          <RolePlayer>
            <DomainClassMoniker Name="UpdatePropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="201fa5a7-23e9-4b3c-8be8-3aea92dc11e6" Description="Description for Dyvenix.GenIt.UpdatePropertyModelHasPropertyModel.PropertyModel" Name="PropertyModel" DisplayName="Property Model" PropertyName="UpdatePropertyModel" Multiplicity="One" PropertyDisplayName="Update Property Model">
          <RolePlayer>
            <DomainClassMoniker Name="PropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="b00efb37-ec59-40d2-af39-bdd942b5d4f4" Description="Description for Dyvenix.GenIt.UpdateMethodModelHasUpdateProperties" Name="UpdateMethodModelHasUpdateProperties" DisplayName="Update Method Model Has Update Properties" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="c15ab607-9c3b-49cb-b70a-f87377d319d7" Description="Description for Dyvenix.GenIt.UpdateMethodModelHasUpdateProperties.UpdateMethodModel" Name="UpdateMethodModel" DisplayName="Update Method Model" PropertyName="PropertyModels" Multiplicity="OneMany" PropertyDisplayName="Property Models">
          <RolePlayer>
            <DomainClassMoniker Name="UpdateMethodModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="cc20809a-f025-4bd0-bc76-5167422fd873" Description="Description for Dyvenix.GenIt.UpdateMethodModelHasUpdateProperties.UpdatePropertyModel" Name="UpdatePropertyModel" DisplayName="Update Property Model" PropertyName="UpdateMethodModeled" PropertyDisplayName="Update Method Modeled">
          <RolePlayer>
            <DomainClassMoniker Name="UpdatePropertyModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="7c30f2e5-9b25-42da-8022-ef5f910ee008" Description="Description for Dyvenix.GenIt.ServiceModelHasReadMethodModels" Name="ServiceModelHasReadMethodModels" DisplayName="Service Model Has Read Method Models" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="284505c4-32a3-4097-9cdb-16f78636473f" Description="Description for Dyvenix.GenIt.ServiceModelHasReadMethodModels.ServiceModel" Name="ServiceModel" DisplayName="Service Model" PropertyName="ReadMethods" PropertyDisplayName="Read Methods">
          <RolePlayer>
            <DomainClassMoniker Name="ServiceModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="c78062b8-cc0c-4f7f-b968-0ebdc5f82bc0" Description="Description for Dyvenix.GenIt.ServiceModelHasReadMethodModels.ReadMethodModel" Name="ReadMethodModel" DisplayName="Read Method Model" PropertyName="ServiceModeled" PropertyDisplayName="Service Modeled">
          <RolePlayer>
            <DomainClassMoniker Name="ReadMethodModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="2d057002-0041-4aa6-a842-c86b328c05ad" Description="Description for Dyvenix.GenIt.ServiceModelHasUpdateMethods" Name="ServiceModelHasUpdateMethods" DisplayName="Service Model Has Update Methods" Namespace="Dyvenix.GenIt">
      <Source>
        <DomainRole Id="46b0dfdc-44ac-4aaa-96f7-b9a089e1ca06" Description="Description for Dyvenix.GenIt.ServiceModelHasUpdateMethods.ServiceModel" Name="ServiceModel" DisplayName="Service Model" PropertyName="UpdateMethods" PropertyDisplayName="Update Methods">
          <RolePlayer>
            <DomainClassMoniker Name="ServiceModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="7e877f3f-d760-41f7-80bd-7f7437fd59f8" Description="Description for Dyvenix.GenIt.ServiceModelHasUpdateMethods.UpdateMethodModel" Name="UpdateMethodModel" DisplayName="Update Method Model" PropertyName="ServiceModeled" PropertyDisplayName="Service Modeled">
          <RolePlayer>
            <DomainClassMoniker Name="UpdateMethodModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="f7eaeabe-cc43-4898-9c7f-0576609f30e5" Description="Description for Dyvenix.GenIt.EntityModelHasServiceModels" Name="EntityModelHasServiceModels" DisplayName="Entity Model Has Service Models" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="4af6262f-0c9f-463a-a5ee-96eb899348b1" Description="Description for Dyvenix.GenIt.EntityModelHasServiceModels.EntityModel" Name="EntityModel" DisplayName="Entity Model" PropertyName="ServiceModels" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Service Models">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1eb2f065-fdd7-463f-b470-49e8fe5a43ee" Description="Description for Dyvenix.GenIt.EntityModelHasServiceModels.ServiceModel" Name="ServiceModel" DisplayName="Service Model" PropertyName="EntityModeled" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Entity Modeled">
          <RolePlayer>
            <DomainClassMoniker Name="ServiceModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
    <ExternalType Name="Color" Namespace="System.Drawing" />
    <DomainEnumeration Name="AccessModifier" Namespace="Dyvenix.GenIt" Description="" />
    <DomainEnumeration Name="TypeAccessModifier" Namespace="Dyvenix.GenIt" Description="" />
    <DomainEnumeration Name="InheritanceModifier" Namespace="Dyvenix.GenIt" Description="" />
    <DomainEnumeration Name="Multiplicity" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="Zero or one" Name="ZeroOne" Value="0" />
        <EnumerationLiteral Description="Exactly one" Name="One" Value="1" />
        <EnumerationLiteral Description="Zero or more" Name="Many" Value="2" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="OperationConcurrency" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="Sequential execution" Name="Sequential" Value="0" />
        <EnumerationLiteral Description="Guarded execution" Name="Guarded" Value="1" />
        <EnumerationLiteral Description="Concurrent execution" Name="Concurrent" Value="2" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="DataType" Namespace="Dyvenix.GenIt" Description="Common C# data types">
      <Literals>
        <EnumerationLiteral Description="System.String" Name="String" Value="0" />
        <EnumerationLiteral Description="System.Int32" Name="Int32" Value="5" />
        <EnumerationLiteral Description="System.Boolean" Name="Boolean" Value="1" />
        <EnumerationLiteral Description="System.Guid" Name="Guid" Value="17" />
        <EnumerationLiteral Description="System.DateTime" Name="DateTime" Value="14" />
        <EnumerationLiteral Description="System.Byte" Name="Byte" Value="2" />
        <EnumerationLiteral Description="System.SByte" Name="SByte" Value="3" />
        <EnumerationLiteral Description="System.Int16" Name="Int16" Value="4" />
        <EnumerationLiteral Description="System.Int64" Name="Int64" Value="6" />
        <EnumerationLiteral Description="System.UInt16" Name="UInt16" Value="7" />
        <EnumerationLiteral Description="System.UInt32" Name="UInt32" Value="8" />
        <EnumerationLiteral Description="System.UInt64" Name="UInt64" Value="9" />
        <EnumerationLiteral Description="System.Single" Name="Single" Value="10" />
        <EnumerationLiteral Description="System.Double" Name="Double" Value="11" />
        <EnumerationLiteral Description="System.Decimal" Name="Decimal" Value="12" />
        <EnumerationLiteral Description="System.Char" Name="Char" Value="13" />
        <EnumerationLiteral Description="System.DateTimeOffset" Name="DateTimeOffset" Value="15" />
        <EnumerationLiteral Description="System.TimeSpan" Name="TimeSpan" Value="16" />
        <EnumerationLiteral Description="Byte array (byte[])" Name="ByteArray" Value="18" />
        <EnumerationLiteral Description="System.Object" Name="Object" Value="19" />
        <EnumerationLiteral Description="Enum type - see EnumTypeName property" Name="Enum" Value="20" />
        <EnumerationLiteral Description="List of strings (List&lt;string&gt;)" Name="StringList" Value="21" />
      </Literals>
    </DomainEnumeration>
  </Types>
  <Shapes>
    <CompartmentShape Id="90f1ac4d-6ba9-4b0a-8559-20e1108d187b" Description="" Name="ClassShape" DisplayName="Class Shape" Namespace="Dyvenix.GenIt" FixedTooltipText="Class Shape" FillColor="211, 220, 239" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <Compartment TitleFillColor="235, 235, 235" Name="PropertiesCompartment" Title="Properties" />
      <Compartment TitleFillColor="235, 235, 235" Name="NavPropertiesCompartment" Title="Navigation Properties" />
      <Compartment Name="ServiceMethods" Title="Service Methods" />
    </CompartmentShape>
    <CompartmentShape Id="c8894909-29b3-4763-ab1e-bb10d40d8335" Description="" Name="InterfaceShape" DisplayName="Interface Shape" Namespace="Dyvenix.GenIt" FixedTooltipText="Interface Shape" FillColor="LightGray" InitialHeight="0.5" OutlineThickness="0.01" Geometry="RoundedRectangle">
      <Notes>This shape only has one compartment, so by default it would not show the compartment header.
      But we want it to look uniform with the ClassShape, so we set IsSingleCompartmentHeaderVisible.</Notes>
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Sterotype" DisplayName="Sterotype" DefaultText="&lt;&lt;Interface&gt;&gt;">
          <Notes>This decorator is fixed - not mapped to any property.</Notes>
        </TextDecorator>
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0.15">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="InterfaceShapeNameDecorator">
          <Notes>The VerticalOffset puts this decorator just below the stereotype, with normal font sizes.</Notes>
        </TextDecorator>
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <Compartment TitleFillColor="235, 235, 235" Name="OperationsCompartment" Title="Operations" />
    </CompartmentShape>
    <GeometryShape Id="fb9814ea-7fa8-483d-81b3-1744ce9368f5" Description="" Name="CommentBoxShape" DisplayName="Comment Box Shape" Namespace="Dyvenix.GenIt" FixedTooltipText="Comment Box Shape" FillColor="255, 255, 204" OutlineColor="204, 204, 102" InitialHeight="0.3" OutlineThickness="0.01" FillGradientMode="None" Geometry="Rectangle">
      <ShapeHasDecorators Position="Center" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Comment" DisplayName="Comment" DefaultText="BusinessRulesShapeNameDecorator" />
      </ShapeHasDecorators>
    </GeometryShape>
    <CompartmentShape Id="d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a" Description="" Name="EnumShape" DisplayName="Enum Shape" Namespace="Dyvenix.GenIt" FixedTooltipText="Enum Shape" FillColor="Goldenrod" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="EnumName" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <Compartment TitleFillColor="235, 235, 235" Name="MembersCompartment" Title="Members" />
    </CompartmentShape>
  </Shapes>
  <Connectors>
    <Connector Id="2a47bfc7-ca8d-42ba-bfdf-e4805a7ad87b" Description="" Name="AssociationConnector" DisplayName="Association Connector" Namespace="Dyvenix.GenIt" GeneratesDoubleDerived="true" FixedTooltipText="Association Connector" Color="113, 111, 110" Thickness="0.01" />
    <Connector Id="b3bba042-d28b-47b6-9a19-569fd62ec876" Description="" Name="GeneralizationConnector" DisplayName="Generalization Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="Generalization Connector" Color="113, 111, 110" SourceEndStyle="HollowArrow" Thickness="0.01" />
    <Connector Id="43c88c4d-0054-4bc1-84dd-7592973d5c05" Description="" Name="ImplementationConnector" DisplayName="Implementation Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="Implementation Connector" Color="113, 111, 110" DashStyle="Dash" SourceEndStyle="HollowArrow" Thickness="0.01" />
    <Connector Id="0485a32c-16a6-4fd4-880a-503be4641fad" Description="" Name="CommentConnector" DisplayName="Comment Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="Comment Connector" Color="113, 111, 110" DashStyle="Dot" Thickness="0.01" RoutingStyle="Straight" />
  </Connectors>
  <XmlSerializationBehavior Name="GenItSerializationBehavior" Namespace="Dyvenix.GenIt">
    <ClassData>
      <XmlClassData TypeName="NamedElement" MonikerAttributeName="" SerializeId="true" MonikerElementName="namedElementMoniker" ElementName="namedElement" MonikerTypeName="NamedElementMoniker">
        <DomainClassMoniker Name="NamedElement" />
        <ElementData>
          <XmlPropertyData XmlName="name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="NamedElement/Name" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Association" MonikerAttributeName="" SerializeId="true" MonikerElementName="associationMoniker" ElementName="association" MonikerTypeName="AssociationMoniker">
        <DomainRelationshipMoniker Name="Association" />
        <ElementData>
          <XmlPropertyData XmlName="sourceMultiplicity">
            <DomainPropertyMoniker Name="Association/SourceMultiplicity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceRoleName">
            <DomainPropertyMoniker Name="Association/SourceRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="genSourceNavProperty">
            <DomainPropertyMoniker Name="Association/GenSourceNavProperty" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetMultiplicity">
            <DomainPropertyMoniker Name="Association/TargetMultiplicity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetRoleName">
            <DomainPropertyMoniker Name="Association/TargetRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="genTargetNavProperty">
            <DomainPropertyMoniker Name="Association/GenTargetNavProperty" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="fkPropertyName">
            <DomainPropertyMoniker Name="Association/FkPropertyName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ClassHasProperties" MonikerAttributeName="" SerializeId="true" MonikerElementName="classHasPropertiesMoniker" ElementName="classHasProperties" MonikerTypeName="ClassHasPropertiesMoniker">
        <DomainRelationshipMoniker Name="ClassHasProperties" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRootHasComments" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelRootHasCommentsMoniker" ElementName="modelRootHasComments" MonikerTypeName="ModelRootHasCommentsMoniker">
        <DomainRelationshipMoniker Name="ModelRootHasComments" />
      </XmlClassData>
      <XmlClassData TypeName="Generalization" MonikerAttributeName="" SerializeId="true" MonikerElementName="generalizationMoniker" ElementName="generalization" MonikerTypeName="GeneralizationMoniker">
        <DomainRelationshipMoniker Name="Generalization" />
        <ElementData>
          <XmlPropertyData XmlName="discriminator">
            <DomainPropertyMoniker Name="Generalization/Discriminator" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="InterfaceHasOperation" MonikerAttributeName="" SerializeId="true" MonikerElementName="interfaceHasOperationMoniker" ElementName="interfaceHasOperation" MonikerTypeName="InterfaceHasOperationMoniker">
        <DomainRelationshipMoniker Name="InterfaceHasOperation" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRootHasTypes" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelRootHasTypesMoniker" ElementName="modelRootHasTypes" MonikerTypeName="ModelRootHasTypesMoniker">
        <DomainRelationshipMoniker Name="ModelRootHasTypes" />
      </XmlClassData>
      <XmlClassData TypeName="CommentReferencesSubjects" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentReferencesSubjectsMoniker" ElementName="commentReferencesSubjects" MonikerTypeName="CommentReferencesSubjectsMoniker">
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </XmlClassData>
      <XmlClassData TypeName="Implementation" MonikerAttributeName="" SerializeId="true" MonikerElementName="implementationMoniker" ElementName="implementation" MonikerTypeName="ImplementationMoniker">
        <DomainRelationshipMoniker Name="Implementation" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRoot" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelRootMoniker" ElementName="modelRoot" MonikerTypeName="ModelRootMoniker">
        <DomainClassMoniker Name="ModelRoot" />
        <ElementData>
          <XmlRelationshipData RoleElementName="comments">
            <DomainRelationshipMoniker Name="ModelRootHasComments" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="types">
            <DomainRelationshipMoniker Name="ModelRootHasTypes" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="entitiesOutputFolder">
            <DomainPropertyMoniker Name="ModelRoot/EntitiesOutputFolder" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="entitiesNamespace">
            <DomainPropertyMoniker Name="ModelRoot/EntitiesNamespace" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="templatesFolder">
            <DomainPropertyMoniker Name="ModelRoot/TemplatesFolder" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dbContextOutputFolder">
            <DomainPropertyMoniker Name="ModelRoot/DbContextOutputFolder" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dbContextNamespace">
            <DomainPropertyMoniker Name="ModelRoot/DbContextNamespace" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dbContextUsings">
            <DomainPropertyMoniker Name="ModelRoot/DbContextUsings" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="enumsOutputFolder">
            <DomainPropertyMoniker Name="ModelRoot/EnumsOutputFolder" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="enumsNamespace">
            <DomainPropertyMoniker Name="ModelRoot/EnumsNamespace" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="entitiesEnabled">
            <DomainPropertyMoniker Name="ModelRoot/EntitiesEnabled" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="dbContextEnabled">
            <DomainPropertyMoniker Name="ModelRoot/DbContextEnabled" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="enumsEnabled">
            <DomainPropertyMoniker Name="ModelRoot/EnumsEnabled" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="diagramBackgroundColor">
            <DomainPropertyMoniker Name="ModelRoot/DiagramBackgroundColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="associationLineColor">
            <DomainPropertyMoniker Name="ModelRoot/AssociationLineColor" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclHeader">
            <DomainPropertyMoniker Name="ModelRoot/InclHeader" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="EntityModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="entityModelMoniker" ElementName="entityModel" MonikerTypeName="EntityModelMoniker">
        <DomainClassMoniker Name="EntityModel" />
        <ElementData>
          <XmlPropertyData XmlName="attributes">
            <DomainPropertyMoniker Name="EntityModel/Attributes" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="usings">
            <DomainPropertyMoniker Name="EntityModel/Usings" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="auditable">
            <DomainPropertyMoniker Name="EntityModel/Auditable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="generateCode">
            <DomainPropertyMoniker Name="EntityModel/GenerateCode" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclRowVersion">
            <DomainPropertyMoniker Name="EntityModel/InclRowVersion" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="properties">
            <DomainRelationshipMoniker Name="ClassHasProperties" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="navigationProperties">
            <DomainRelationshipMoniker Name="ClassHasNavigationProperties" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="subclasses">
            <DomainRelationshipMoniker Name="Generalization" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="targets">
            <DomainRelationshipMoniker Name="Association" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="serviceModels">
            <DomainRelationshipMoniker Name="EntityModelHasServiceModels" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PropertyModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="propertyModelMoniker" ElementName="propertyModel" MonikerTypeName="PropertyModelMoniker">
        <DomainClassMoniker Name="PropertyModel" />
        <ElementData>
          <XmlPropertyData XmlName="dataType">
            <DomainPropertyMoniker Name="PropertyModel/DataType" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="enumTypeName">
            <DomainPropertyMoniker Name="PropertyModel/EnumTypeName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="length">
            <DomainPropertyMoniker Name="PropertyModel/Length" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="initialValue">
            <DomainPropertyMoniker Name="PropertyModel/InitialValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="attributes">
            <DomainPropertyMoniker Name="PropertyModel/Attributes" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="usings">
            <DomainPropertyMoniker Name="PropertyModel/Usings" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isPrimaryKey">
            <DomainPropertyMoniker Name="PropertyModel/IsPrimaryKey" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isNullable">
            <DomainPropertyMoniker Name="PropertyModel/IsNullable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isIdentity">
            <DomainPropertyMoniker Name="PropertyModel/IsIdentity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isIndexed">
            <DomainPropertyMoniker Name="PropertyModel/IsIndexed" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isIndexUnique">
            <DomainPropertyMoniker Name="PropertyModel/IsIndexUnique" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isIndexClustered">
            <DomainPropertyMoniker Name="PropertyModel/IsIndexClustered" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isForeignKey">
            <DomainPropertyMoniker Name="PropertyModel/IsForeignKey" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isRowVersion">
            <DomainPropertyMoniker Name="PropertyModel/IsRowVersion" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Comment" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentMoniker" ElementName="comment" MonikerTypeName="CommentMoniker">
        <DomainClassMoniker Name="Comment" />
        <ElementData>
          <XmlPropertyData XmlName="text">
            <DomainPropertyMoniker Name="Comment/Text" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="subjects">
            <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Operation" MonikerAttributeName="" SerializeId="true" MonikerElementName="operationMoniker" ElementName="operation" MonikerTypeName="OperationMoniker">
        <DomainClassMoniker Name="Operation" />
        <ElementData>
          <XmlPropertyData XmlName="signature">
            <DomainPropertyMoniker Name="Operation/Signature" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="concurrency">
            <DomainPropertyMoniker Name="Operation/Concurrency" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="precondition">
            <DomainPropertyMoniker Name="Operation/Precondition" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="postcondition">
            <DomainPropertyMoniker Name="Operation/Postcondition" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelInterface" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelInterfaceMoniker" ElementName="modelInterface" MonikerTypeName="ModelInterfaceMoniker">
        <DomainClassMoniker Name="ModelInterface" />
        <ElementData>
          <XmlRelationshipData RoleElementName="operations">
            <DomainRelationshipMoniker Name="InterfaceHasOperation" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="implementors">
            <DomainRelationshipMoniker Name="Implementation" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="InterfaceOperation" MonikerAttributeName="" SerializeId="true" MonikerElementName="interfaceOperationMoniker" ElementName="interfaceOperation" MonikerTypeName="InterfaceOperationMoniker">
        <DomainClassMoniker Name="InterfaceOperation" />
      </XmlClassData>
      <XmlClassData TypeName="ModelType" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelTypeMoniker" ElementName="modelType" MonikerTypeName="ModelTypeMoniker">
        <DomainClassMoniker Name="ModelType" />
      </XmlClassData>
      <XmlClassData TypeName="ClassModelElement" MonikerAttributeName="" SerializeId="true" MonikerElementName="classModelElementMoniker" ElementName="classModelElement" MonikerTypeName="ClassModelElementMoniker">
        <DomainClassMoniker Name="ClassModelElement" />
        <ElementData>
          <XmlPropertyData XmlName="description">
            <DomainPropertyMoniker Name="ClassModelElement/Description" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="EnumMember" MonikerAttributeName="" SerializeId="true" MonikerElementName="enumMemberMoniker" ElementName="enumMember" MonikerTypeName="EnumMemberMoniker">
        <DomainClassMoniker Name="EnumMember" />
        <ElementData>
          <XmlPropertyData XmlName="value">
            <DomainPropertyMoniker Name="EnumMember/Value" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="EnumModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="enumModelMoniker" ElementName="enumModel" MonikerTypeName="EnumModelMoniker">
        <DomainClassMoniker Name="EnumModel" />
        <ElementData>
          <XmlPropertyData XmlName="isExternal">
            <DomainPropertyMoniker Name="EnumModel/IsExternal" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isFlags">
            <DomainPropertyMoniker Name="EnumModel/IsFlags" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="generateCode">
            <DomainPropertyMoniker Name="EnumModel/GenerateCode" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="members">
            <DomainRelationshipMoniker Name="EnumHasMembers" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="EnumHasMembers" MonikerAttributeName="" SerializeId="true" MonikerElementName="enumHasMembersMoniker" ElementName="enumHasMembers" MonikerTypeName="EnumHasMembersMoniker">
        <DomainRelationshipMoniker Name="EnumHasMembers" />
      </XmlClassData>
      <XmlClassData TypeName="ClassHasNavigationProperties" MonikerAttributeName="" SerializeId="true" MonikerElementName="classHasNavigationPropertiesMoniker" ElementName="classHasNavigationProperties" MonikerTypeName="ClassHasNavigationPropertiesMoniker">
        <DomainRelationshipMoniker Name="ClassHasNavigationProperties" />
      </XmlClassData>
      <XmlClassData TypeName="NavigationProperty" MonikerAttributeName="" SerializeId="true" MonikerElementName="navigationPropertyMoniker" ElementName="navigationProperty" MonikerTypeName="NavigationPropertyMoniker">
        <DomainClassMoniker Name="NavigationProperty" />
        <ElementData>
          <XmlPropertyData XmlName="targetEntityName">
            <DomainPropertyMoniker Name="NavigationProperty/TargetEntityName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isCollection">
            <DomainPropertyMoniker Name="NavigationProperty/IsCollection" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CommentBoxShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentBoxShapeMoniker" ElementName="commentBoxShape" MonikerTypeName="CommentBoxShapeMoniker">
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </XmlClassData>
      <XmlClassData TypeName="AssociationConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="associationConnectorMoniker" ElementName="associationConnector" MonikerTypeName="AssociationConnectorMoniker">
        <ConnectorMoniker Name="AssociationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="GeneralizationConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="generalizationConnectorMoniker" ElementName="generalizationConnector" MonikerTypeName="GeneralizationConnectorMoniker">
        <ConnectorMoniker Name="GeneralizationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ImplementationConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="implementationConnectorMoniker" ElementName="implementationConnector" MonikerTypeName="ImplementationConnectorMoniker">
        <ConnectorMoniker Name="ImplementationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CommentConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentConnectorMoniker" ElementName="commentConnector" MonikerTypeName="CommentConnectorMoniker">
        <ConnectorMoniker Name="CommentConnector" />
      </XmlClassData>
      <XmlClassData TypeName="GenItDiagram" MonikerAttributeName="" SerializeId="true" MonikerElementName="genItDiagramMoniker" ElementName="genItDiagram" MonikerTypeName="GenItDiagramMoniker">
        <DiagramMoniker Name="GenItDiagram" />
      </XmlClassData>
      <XmlClassData TypeName="ClassShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="classShapeMoniker" ElementName="classShape" MonikerTypeName="ClassShapeMoniker">
        <CompartmentShapeMoniker Name="ClassShape" />
      </XmlClassData>
      <XmlClassData TypeName="InterfaceShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="interfaceShapeMoniker" ElementName="interfaceShape" MonikerTypeName="InterfaceShapeMoniker">
        <CompartmentShapeMoniker Name="InterfaceShape" />
      </XmlClassData>
      <XmlClassData TypeName="EnumShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="enumShapeMoniker" ElementName="enumShape" MonikerTypeName="EnumShapeMoniker">
        <CompartmentShapeMoniker Name="EnumShape" />
      </XmlClassData>
      <XmlClassData TypeName="FilterPropertyModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="filterPropertyModelMoniker" ElementName="filterPropertyModel" MonikerTypeName="FilterPropertyModelMoniker">
        <DomainClassMoniker Name="FilterPropertyModel" />
        <ElementData>
          <XmlPropertyData XmlName="isOptional">
            <DomainPropertyMoniker Name="FilterPropertyModel/IsOptional" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isInternal">
            <DomainPropertyMoniker Name="FilterPropertyModel/IsInternal" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="internalValue">
            <DomainPropertyMoniker Name="FilterPropertyModel/InternalValue" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="propertyModel">
            <DomainRelationshipMoniker Name="FilterPropertyModelHasProperty" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="FilterPropertyModelHasProperty" MonikerAttributeName="" SerializeId="true" MonikerElementName="filterPropertyModelHasPropertyMoniker" ElementName="filterPropertyModelHasProperty" MonikerTypeName="FilterPropertyModelHasPropertyMoniker">
        <DomainRelationshipMoniker Name="FilterPropertyModelHasProperty" />
      </XmlClassData>
      <XmlClassData TypeName="ReadMethodModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="readMethodModelMoniker" ElementName="readMethodModel" MonikerTypeName="ReadMethodModelMoniker">
        <DomainClassMoniker Name="ReadMethodModel" />
        <ElementData>
          <XmlPropertyData XmlName="itemId">
            <DomainPropertyMoniker Name="ReadMethodModel/ItemId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclPaging">
            <DomainPropertyMoniker Name="ReadMethodModel/InclPaging" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useQuery">
            <DomainPropertyMoniker Name="ReadMethodModel/UseQuery" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclSorting">
            <DomainPropertyMoniker Name="ReadMethodModel/InclSorting" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="displayOrder">
            <DomainPropertyMoniker Name="ReadMethodModel/DisplayOrder" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="attributes">
            <DomainPropertyMoniker Name="ReadMethodModel/Attributes" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="filterPropertyModeled">
            <DomainRelationshipMoniker Name="ReadMethodModelHasFilterProperties" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ReadMethodModelHasFilterProperties" MonikerAttributeName="" SerializeId="true" MonikerElementName="readMethodModelHasFilterPropertiesMoniker" ElementName="readMethodModelHasFilterProperties" MonikerTypeName="ReadMethodModelHasFilterPropertiesMoniker">
        <DomainRelationshipMoniker Name="ReadMethodModelHasFilterProperties" />
      </XmlClassData>
      <XmlClassData TypeName="UpdatePropertyModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="updatePropertyModelMoniker" ElementName="updatePropertyModel" MonikerTypeName="UpdatePropertyModelMoniker">
        <DomainClassMoniker Name="UpdatePropertyModel" />
        <ElementData>
          <XmlPropertyData XmlName="isOptional">
            <DomainPropertyMoniker Name="UpdatePropertyModel/IsOptional" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="propertyModel">
            <DomainRelationshipMoniker Name="UpdatePropertyModelHasPropertyModel" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="UpdatePropertyModelHasPropertyModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="updatePropertyModelHasPropertyModelMoniker" ElementName="updatePropertyModelHasPropertyModel" MonikerTypeName="UpdatePropertyModelHasPropertyModelMoniker">
        <DomainRelationshipMoniker Name="UpdatePropertyModelHasPropertyModel" />
      </XmlClassData>
      <XmlClassData TypeName="UpdateMethodModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="updateMethodModelMoniker" ElementName="updateMethodModel" MonikerTypeName="UpdateMethodModelMoniker">
        <DomainClassMoniker Name="UpdateMethodModel" />
        <ElementData>
          <XmlPropertyData XmlName="itemId">
            <DomainPropertyMoniker Name="UpdateMethodModel/ItemId" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="useDto">
            <DomainPropertyMoniker Name="UpdateMethodModel/UseDto" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="displayOrder">
            <DomainPropertyMoniker Name="UpdateMethodModel/DisplayOrder" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="propertyModels">
            <DomainRelationshipMoniker Name="UpdateMethodModelHasUpdateProperties" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="UpdateMethodModelHasUpdateProperties" MonikerAttributeName="" SerializeId="true" MonikerElementName="updateMethodModelHasUpdatePropertiesMoniker" ElementName="updateMethodModelHasUpdateProperties" MonikerTypeName="UpdateMethodModelHasUpdatePropertiesMoniker">
        <DomainRelationshipMoniker Name="UpdateMethodModelHasUpdateProperties" />
      </XmlClassData>
      <XmlClassData TypeName="ServiceModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="serviceModelMoniker" ElementName="serviceModel" MonikerTypeName="ServiceModelMoniker">
        <DomainClassMoniker Name="ServiceModel" />
        <ElementData>
          <XmlPropertyData XmlName="enabled">
            <DomainPropertyMoniker Name="ServiceModel/Enabled" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclCreate">
            <DomainPropertyMoniker Name="ServiceModel/InclCreate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclUpdate">
            <DomainPropertyMoniker Name="ServiceModel/InclUpdate" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclDelete">
            <DomainPropertyMoniker Name="ServiceModel/InclDelete" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="inclController">
            <DomainPropertyMoniker Name="ServiceModel/InclController" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="version">
            <DomainPropertyMoniker Name="ServiceModel/Version" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="serviceUsings">
            <DomainPropertyMoniker Name="ServiceModel/ServiceUsings" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="serviceAttributes">
            <DomainPropertyMoniker Name="ServiceModel/ServiceAttributes" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="controllerUsings">
            <DomainPropertyMoniker Name="ServiceModel/ControllerUsings" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="controllerAttributes">
            <DomainPropertyMoniker Name="ServiceModel/ControllerAttributes" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="readMethods">
            <DomainRelationshipMoniker Name="ServiceModelHasReadMethodModels" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="updateMethods">
            <DomainRelationshipMoniker Name="ServiceModelHasUpdateMethods" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ServiceModelHasReadMethodModels" MonikerAttributeName="" SerializeId="true" MonikerElementName="serviceModelHasReadMethodModelsMoniker" ElementName="serviceModelHasReadMethodModels" MonikerTypeName="ServiceModelHasReadMethodModelsMoniker">
        <DomainRelationshipMoniker Name="ServiceModelHasReadMethodModels" />
      </XmlClassData>
      <XmlClassData TypeName="ServiceModelHasUpdateMethods" MonikerAttributeName="" SerializeId="true" MonikerElementName="serviceModelHasUpdateMethodsMoniker" ElementName="serviceModelHasUpdateMethods" MonikerTypeName="ServiceModelHasUpdateMethodsMoniker">
        <DomainRelationshipMoniker Name="ServiceModelHasUpdateMethods" />
      </XmlClassData>
      <XmlClassData TypeName="EntityModelHasServiceModels" MonikerAttributeName="" SerializeId="true" MonikerElementName="entityModelHasServiceModelsMoniker" ElementName="entityModelHasServiceModels" MonikerTypeName="EntityModelHasServiceModelsMoniker">
        <DomainRelationshipMoniker Name="EntityModelHasServiceModels" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="GenItExplorer" />
  <ConnectionBuilders>
    <ConnectionBuilder Name="AssociationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="Association" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EntityModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EntityModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="GeneralizationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="Generalization" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EntityModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EntityModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="Implementation" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelInterface" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelType" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="CommentReferencesSubjectsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="Comment" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EntityModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelInterface" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EnumModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="FilterPropertyModelHasPropertyBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="FilterPropertyModelHasProperty" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="FilterPropertyModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="PropertyModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ReadMethodModelHasFilterPropertiesBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ReadMethodModelHasFilterProperties" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ReadMethodModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="FilterPropertyModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="UpdatePropertyModelHasPropertyModelBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="UpdatePropertyModelHasPropertyModel" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="UpdatePropertyModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="PropertyModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="UpdateMethodModelHasUpdatePropertiesBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="UpdateMethodModelHasUpdateProperties" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="UpdateMethodModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="UpdatePropertyModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ServiceModelHasReadMethodModelsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ServiceModelHasReadMethodModels" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ServiceModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ReadMethodModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ServiceModelHasUpdateMethodsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ServiceModelHasUpdateMethods" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ServiceModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="UpdateMethodModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="EntityModelHasServiceModelsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="EntityModelHasServiceModels" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="EntityModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ServiceModel" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
  </ConnectionBuilders>
  <Diagram Id="2f556c0f-616b-4b70-9e3e-12f9045d12d7" Description="" Name="GenItDiagram" DisplayName="Class Diagram" Namespace="Dyvenix.GenIt">
    <Class>
      <DomainClassMoniker Name="ModelRoot" />
    </Class>
    <ShapeMaps>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="EntityModel" />
        <ParentElementPath>
          <DomainPath>ModelRootHasTypes.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ClassShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ClassShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/PropertiesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ClassHasProperties.Properties/!Attribute</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/NavPropertiesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ClassHasNavigationProperties.NavigationProperties/!NavigationProperty</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/ServiceMethods" />
          <ElementsDisplayed>
            <DomainPath>EntityModelHasServiceModels.ServiceModels/!ServiceModel</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ServiceModel/Version" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="ModelInterface" />
        <ParentElementPath>
          <DomainPath>ModelRootHasTypes.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="InterfaceShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="InterfaceShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="InterfaceShape/OperationsCompartment" />
          <ElementsDisplayed>
            <DomainPath>InterfaceHasOperation.Operations/!Operation</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="Comment" />
        <ParentElementPath>
          <DomainPath>ModelRootHasComments.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CommentBoxShape/Comment" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Comment/Text" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </ShapeMap>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="EnumModel" />
        <ParentElementPath>
          <DomainPath>ModelRootHasTypes.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="EnumShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="EnumShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="EnumShape/MembersCompartment" />
          <ElementsDisplayed>
            <DomainPath>EnumHasMembers.Members/!Member</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="AssociationConnector" />
        <DomainRelationshipMoniker Name="Association" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="GeneralizationConnector" />
        <DomainRelationshipMoniker Name="Generalization" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ImplementationConnector" />
        <DomainRelationshipMoniker Name="Implementation" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="CommentConnector" />
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer CopyPasteGeneration="CopyPasteOnly" FileExtension="gmdl" EditorGuid="185dd91f-393a-4f15-8ba0-9406fa4ffc9b">
    <RootClass>
      <DomainClassMoniker Name="ModelRoot" />
    </RootClass>
    <XmlSerializationDefinition CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="GenItSerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="Class Diagrams">
      <ElementTool Name="EntityModel" ToolboxIcon="Resources\ClassTool.bmp" Caption="Entity" Tooltip="Create an Entity" HelpKeyword="EntityModelF1Keyword">
        <DomainClassMoniker Name="EntityModel" />
      </ElementTool>
      <ElementTool Name="EnumModel" ToolboxIcon="Resources\ClassTool.bmp" Caption="Enum" Tooltip="Create an Enumeration" HelpKeyword="EnumModelF1Keyword">
        <DomainClassMoniker Name="EnumModel" />
      </ElementTool>
      <ElementTool Name="ModelInterface" ToolboxIcon="Resources\InterfaceTool.bmp" Caption="Interface" Tooltip="Create an Interface" HelpKeyword="ModelInterfaceF1Keyword">
        <DomainClassMoniker Name="ModelInterface" />
      </ElementTool>
      <ElementTool Name="InterfaceOperation" ToolboxIcon="resources\interfaceoperationtool.bmp" Caption="Interface Operation" Tooltip="Create an Operation on an Interface" HelpKeyword="InterfaceOperationF1Keyword">
        <DomainClassMoniker Name="InterfaceOperation" />
      </ElementTool>
      <ConnectionTool Name="Association" ToolboxIcon="Resources\AssociationTool.bmp" Caption="Association" Tooltip="Create an Association link" HelpKeyword="ConnectAssociationF1Keyword">
        <ConnectionBuilderMoniker Name="GenIt/AssociationBuilder" />
      </ConnectionTool>
      <ElementTool Name="Comment" ToolboxIcon="resources\commenttool.bmp" Caption="Comment" Tooltip="Create a Comment" HelpKeyword="CommentF1Keyword">
        <DomainClassMoniker Name="Comment" />
      </ElementTool>
      <ConnectionTool Name="CommentsReferenceTypes" ToolboxIcon="resources\commentlinktool.bmp" Caption="Comment Link" Tooltip="Link a comment to an element" HelpKeyword="CommentsReferenceTypesF1Keyword">
        <ConnectionBuilderMoniker Name="GenIt/CommentReferencesSubjectsBuilder" />
      </ConnectionTool>
    </ToolboxTab>
    <Validation UsesMenu="false" UsesOpen="false" UsesSave="false" UsesLoad="false" />
    <DiagramMoniker Name="GenItDiagram" />
  </Designer>
  <Explorer ExplorerGuid="584bbce2-fa64-47c0-a8a5-f0500f850804" Title="">
    <ExplorerBehaviorMoniker Name="GenIt/GenItExplorer" />
  </Explorer>
</Dsl>