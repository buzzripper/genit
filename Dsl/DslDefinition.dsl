<?xml version="1.0" encoding="utf-8"?>
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="9d433ece-11d0-4cbc-9a3b-82824193f347" Description="Description for Dyvenix.GenIt.GenIt" Name="GenIt" DisplayName="Class Diagrams" Namespace="Dyvenix.GenIt" ProductName="GenIt" CompanyName="Dyvenix" PackageGuid="3aa8cbb2-f0c4-4628-bc99-158569733469" PackageNamespace="Dyvenix.GenIt" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="9913cfeb-b29a-4eb0-aba0-2e8c046b87e0" Description="" Name="NamedElement" DisplayName="Named Element" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt">
      <Properties>
        <DomainProperty Id="34e4a209-7491-4a75-9f70-f2b75ab46ed5" Description="" Name="Name" DisplayName="Name" DefaultValue="" IsElementName="true">
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
        <DomainProperty Id="f387f050-11f9-456e-a9e6-be8dfd35fa9a" Description="" Name="Kind" DisplayName="Kind" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="1fb358c4-c63e-441b-bf67-00184bd796a6" Description="" Name="IsAbstract" DisplayName="Is Abstract" DefaultValue="None">
          <Type>
            <DomainEnumerationMoniker Name="InheritanceModifier" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ModelAttribute" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasAttributes.Attributes</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ClassOperation" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasOperations.Operations</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="4df99ada-d0ce-4ea5-82a0-2d7409e38475" Description="An attribute of a class." Name="ModelAttribute" DisplayName="Model Attribute" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="4ef9ef25-f6e7-46f2-9e48-ed9323cc081f" Description="" Name="Type" DisplayName="Type" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="b9a0da77-ee42-4cd5-a7d3-85a8d724691e" Description="" Name="InitialValue" DisplayName="Initial Value" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="25717fe9-d9b3-4b94-a58c-f82c1ee7397d" Description="" Name="Multiplicity" DisplayName="Multiplicity" DefaultValue="1">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
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
    <DomainClass Id="fe35f199-2784-422d-a1da-37f9f5f6270c" Description="" Name="ClassOperation" DisplayName="Class Operation" Namespace="Dyvenix.GenIt">
      <BaseClass>
        <DomainClassMoniker Name="Operation" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="902d987c-217c-482e-a192-d4d832f7ac12" Description="" Name="IsAbstract" DisplayName="Is Abstract" DefaultValue="False">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
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
        <DomainProperty Id="77a2653e-d7ee-4729-9dd2-d1c73e8ebbf0" Description="This is a Description." Name="Description" DisplayName="Description" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="4a55a93f-ffed-423c-ad69-a1b5c9c85a1e" Description="Associations between Classes." Name="Association" DisplayName="Association" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt" AllowsDuplicates="true">
      <Notes>This is the abstract base relationship of the several kinds of association between Classes.
      It defines the Properties that are attached to each association.</Notes>
      <Properties>
        <DomainProperty Id="7f3fc48a-1968-44e9-91aa-ee8d93c10f89" Description="" Name="SourceMultiplicity" DisplayName="Source Multiplicity" DefaultValue="One">
          <Type>
            <DomainEnumerationMoniker Name="Multiplicity" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="99ceb0e6-4515-4c03-96e9-b961d66611de" Description="" Name="SourceRoleName" DisplayName="Source Role Name" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="007c3000-0bf3-4179-a114-883ede04c3df" Description="" Name="TargetMultiplicity" DisplayName="Target Multiplicity" DefaultValue="Many">
          <Type>
            <DomainEnumerationMoniker Name="Multiplicity" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d0dfb7ed-2955-4348-b1e2-c8d8da294082" Description="" Name="TargetRoleName" DisplayName="Target Role Name" DefaultValue="">
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
    <DomainRelationship Id="c126dbfd-cb6b-4a84-b44b-b3426ae7df14" Description="" Name="UnidirectionalAssociation" DisplayName="Unidirectional Association" Namespace="Dyvenix.GenIt" AllowsDuplicates="true">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="Association" />
      </BaseRelationship>
      <Source>
        <DomainRole Id="8adeaf08-9a8e-4694-b408-31d33ead420a" Description="" Name="UnidirectionalSource" DisplayName="Unidirectional Source" PropertyName="UnidirectionalTargets" PropertyDisplayName="Unidirectional Targets">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="b491a3af-896f-4ddf-a6de-def4020b5620" Description="" Name="UnidirectionalTarget" DisplayName="Unidirectional Target" PropertyName="UnidirectionalSources" PropertyDisplayName="Unidirectional Sources">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="03a9c537-a07d-44d2-b9e9-7904b35de3dd" Description="" Name="ClassHasAttributes" DisplayName="Class Has Attributes" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="50d60282-915b-4102-96ea-d10a5ddc1dd4" Description="" Name="EntityModel" DisplayName="Entity Model" PropertyName="Attributes" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Attributes">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="36ae1ba4-57f2-4f07-af06-788b29609abb" Description="" Name="Attribute" DisplayName="Attribute" PropertyName="EntityModel" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Entity Model">
          <RolePlayer>
            <DomainClassMoniker Name="ModelAttribute" />
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
    <DomainRelationship Id="4a1da39d-6ea0-4857-95ad-81f6633a64ed" Description="" Name="ClassHasOperations" DisplayName="Class Has Operations" Namespace="Dyvenix.GenIt" IsEmbedding="true">
      <Source>
        <DomainRole Id="2e1a6b41-4c61-4232-895c-e15334b06341" Description="" Name="EntityModel" DisplayName="EntityModel" PropertyName="Operations" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Operations">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="c1d3a745-01ec-4b47-b5cc-5038c42efd55" Description="" Name="Operation" DisplayName="Operation" PropertyName="EntityModel" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Entity Model">
          <RolePlayer>
            <DomainClassMoniker Name="ClassOperation" />
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
    <DomainRelationship Id="51610fa6-0953-40ff-b031-b3de3be17962" Description="" Name="BidirectionalAssociation" DisplayName="Bidirectional Association" Namespace="Dyvenix.GenIt" AllowsDuplicates="true">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="Association" />
      </BaseRelationship>
      <Source>
        <DomainRole Id="a6840fdf-2a3e-4691-bb0d-c5aee9541b00" Description="" Name="BidirectionalSource" DisplayName="Bidirectional Source" PropertyName="BidirectionalTargets" PropertyDisplayName="Bidirectional Targets">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="30c8a91e-de98-441e-814f-2eef871179b9" Description="" Name="BidirectionalTarget" DisplayName="Bidirectional Target" PropertyName="BidirectionalSources" PropertyDisplayName="Bidirectional Sources">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
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
    <DomainRelationship Id="dc82326e-8d7b-4d53-b330-45c82c456b67" Description="" Name="Aggregation" DisplayName="Aggregation" Namespace="Dyvenix.GenIt" AllowsDuplicates="true">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="Association" />
      </BaseRelationship>
      <Source>
        <DomainRole Id="6e37a940-3e02-4c06-a51c-8ee0a30d470e" Description="" Name="AggregationSource" DisplayName="Aggregation Source" PropertyName="AggregationTargets" PropertyDisplayName="Aggregation Targets">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="07e94eb3-ae37-43ea-b00a-323dfcc8d12e" Description="" Name="AggregationTarget" DisplayName="Aggregation Target" PropertyName="AggregationSources" PropertyDisplayName="Aggregation Sources">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="746d4c6c-8d40-4e45-b739-69837cf3ffa2" Description="" Name="Composition" DisplayName="Composition" Namespace="Dyvenix.GenIt" AllowsDuplicates="true">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="Association" />
      </BaseRelationship>
      <Source>
        <DomainRole Id="32f48c4a-303c-40af-8d0a-45f7bc78913e" Description="" Name="CompositionSource" DisplayName="Composition Source" PropertyName="CompositionTargets" PropertyDisplayName="Composition Targets">
          <RolePlayer>
            <DomainClassMoniker Name="EntityModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="0bcd9a3c-5007-4343-bf09-101db668d479" Description="" Name="CompositionTarget" DisplayName="Composition Target" PropertyName="CompositionSources" PropertyDisplayName="Composition Sources">
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
    <DomainEnumeration Name="AccessModifier" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Public" Value="0" />
        <EnumerationLiteral Description="" Name="Assembly" Value="1" />
        <EnumerationLiteral Description="" Name="Private" Value="2" />
        <EnumerationLiteral Description="" Name="Family" Value="3" />
        <EnumerationLiteral Description="" Name="FamilyOrAssembly" Value="4" />
        <EnumerationLiteral Description="" Name="FamilyAndAssembly" Value="5" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="TypeAccessModifier" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Public" Value="0" />
        <EnumerationLiteral Description="" Name="Private" Value="1" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="InheritanceModifier" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="None" Value="0" />
        <EnumerationLiteral Description="" Name="Abstract" Value="1" />
        <EnumerationLiteral Description="" Name="Sealed" Value="2" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="Multiplicity" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Many" Value="0" />
        <EnumerationLiteral Description="" Name="One" Value="1" />
        <EnumerationLiteral Description="" Name="ZeroOrOne" Value="2" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="OperationConcurrency" Namespace="Dyvenix.GenIt" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Sequential" Value="0" />
        <EnumerationLiteral Description="" Name="Guarded" Value="1" />
        <EnumerationLiteral Description="" Name="Concurrent" Value="2" />
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
      <Compartment TitleFillColor="235, 235, 235" Name="AttributesCompartment" Title="Attributes" />
      <Compartment TitleFillColor="235, 235, 235" Name="OperationsCompartment" Title="Operations" />
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
  </Shapes>
  <Connectors>
    <Connector Id="2a47bfc7-ca8d-42ba-bfdf-e4805a7ad87b" Description="" Name="AssociationConnector" DisplayName="Association Connector" InheritanceModifier="Abstract" Namespace="Dyvenix.GenIt" GeneratesDoubleDerived="true" FixedTooltipText="Association Connector" Color="113, 111, 110" Thickness="0.01">
      <ConnectorHasDecorators Position="TargetBottom" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="TargetMultiplicity" DisplayName="Target Multiplicity" DefaultText="TargetMultiplicity" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="SourceBottom" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="SourceMultiplicity" DisplayName="Source Multiplicity" DefaultText="SourceMultiplicity" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="TargetRoleName" DisplayName="Target Role Name" DefaultText="TargetRoleName" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="SourceRoleName" DisplayName="Source Role Name" DefaultText="SourceRoleName" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="658f1224-1f44-4b74-b2bc-6c32bd8fdec4" Description="" Name="UnidirectionalConnector" DisplayName="Unidirectional Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="Unidirectional Connector" Color="113, 111, 110" TargetEndStyle="EmptyArrow" Thickness="0.01">
      <BaseConnector>
        <ConnectorMoniker Name="AssociationConnector" />
      </BaseConnector>
    </Connector>
    <Connector Id="a10ab6b0-ade6-4708-98db-5a6b18a6d6cd" Description="" Name="BidirectionalConnector" DisplayName="Bidirectional Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="" Color="113, 111, 110" Thickness="0.01">
      <BaseConnector>
        <ConnectorMoniker Name="AssociationConnector" />
      </BaseConnector>
    </Connector>
    <Connector Id="6c7a82f5-819d-48e2-a0b6-e624b3bfc813" Description="" Name="AggregationConnector" DisplayName="Aggregation Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="Aggregation Connector" Color="113, 111, 110" SourceEndStyle="EmptyDiamond" Thickness="0.01">
      <BaseConnector>
        <ConnectorMoniker Name="AssociationConnector" />
      </BaseConnector>
    </Connector>
    <Connector Id="a141fa25-2d5d-46e6-8623-2cf98cf8760d" Description="" Name="CompositionConnector" DisplayName="Composition Connector" Namespace="Dyvenix.GenIt" FixedTooltipText="Composition Connector" Color="113, 111, 110" SourceEndStyle="FilledDiamond" Thickness="0.01">
      <BaseConnector>
        <ConnectorMoniker Name="AssociationConnector" />
      </BaseConnector>
    </Connector>
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
          <XmlPropertyData XmlName="targetMultiplicity">
            <DomainPropertyMoniker Name="Association/TargetMultiplicity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetRoleName">
            <DomainPropertyMoniker Name="Association/TargetRoleName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ClassHasAttributes" MonikerAttributeName="" SerializeId="true" MonikerElementName="classHasAttributesMoniker" ElementName="classHasAttributes" MonikerTypeName="ClassHasAttributesMoniker">
        <DomainRelationshipMoniker Name="ClassHasAttributes" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRootHasComments" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelRootHasCommentsMoniker" ElementName="modelRootHasComments" MonikerTypeName="ModelRootHasCommentsMoniker">
        <DomainRelationshipMoniker Name="ModelRootHasComments" />
      </XmlClassData>
      <XmlClassData TypeName="ClassHasOperations" MonikerAttributeName="" SerializeId="true" MonikerElementName="classHasOperationsMoniker" ElementName="classHasOperations" MonikerTypeName="ClassHasOperationsMoniker">
        <DomainRelationshipMoniker Name="ClassHasOperations" />
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
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="EntityModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="entityModelMoniker" ElementName="entityModel" MonikerTypeName="EntityModelMoniker">
        <DomainClassMoniker Name="EntityModel" />
        <ElementData>
          <XmlPropertyData XmlName="kind">
            <DomainPropertyMoniker Name="EntityModel/Kind" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isAbstract">
            <DomainPropertyMoniker Name="EntityModel/IsAbstract" />
          </XmlPropertyData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="unidirectionalTargets">
            <DomainRelationshipMoniker Name="UnidirectionalAssociation" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="attributes">
            <DomainRelationshipMoniker Name="ClassHasAttributes" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="operations">
            <DomainRelationshipMoniker Name="ClassHasOperations" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="subclasses">
            <DomainRelationshipMoniker Name="Generalization" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="bidirectionalTargets">
            <DomainRelationshipMoniker Name="BidirectionalAssociation" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="aggregationTargets">
            <DomainRelationshipMoniker Name="Aggregation" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="compositionTargets">
            <DomainRelationshipMoniker Name="Composition" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelAttribute" MonikerAttributeName="" SerializeId="true" MonikerElementName="modelAttributeMoniker" ElementName="modelAttribute" MonikerTypeName="ModelAttributeMoniker">
        <DomainClassMoniker Name="ModelAttribute" />
        <ElementData>
          <XmlPropertyData XmlName="type">
            <DomainPropertyMoniker Name="ModelAttribute/Type" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="initialValue">
            <DomainPropertyMoniker Name="ModelAttribute/InitialValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="multiplicity">
            <DomainPropertyMoniker Name="ModelAttribute/Multiplicity" />
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
      <XmlClassData TypeName="ClassOperation" MonikerAttributeName="" SerializeId="true" MonikerElementName="classOperationMoniker" ElementName="classOperation" MonikerTypeName="ClassOperationMoniker">
        <DomainClassMoniker Name="ClassOperation" />
        <ElementData>
          <XmlPropertyData XmlName="isAbstract">
            <DomainPropertyMoniker Name="ClassOperation/IsAbstract" />
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
      <XmlClassData TypeName="UnidirectionalAssociation" MonikerAttributeName="" SerializeId="true" MonikerElementName="unidirectionalAssociationMoniker" ElementName="unidirectionalAssociation" MonikerTypeName="UnidirectionalAssociationMoniker">
        <DomainRelationshipMoniker Name="UnidirectionalAssociation" />
      </XmlClassData>
      <XmlClassData TypeName="BidirectionalAssociation" MonikerAttributeName="" SerializeId="true" MonikerElementName="bidirectionalAssociationMoniker" ElementName="bidirectionalAssociation" MonikerTypeName="BidirectionalAssociationMoniker">
        <DomainRelationshipMoniker Name="BidirectionalAssociation" />
      </XmlClassData>
      <XmlClassData TypeName="Aggregation" MonikerAttributeName="" SerializeId="true" MonikerElementName="aggregationMoniker" ElementName="aggregation" MonikerTypeName="AggregationMoniker">
        <DomainRelationshipMoniker Name="Aggregation" />
      </XmlClassData>
      <XmlClassData TypeName="Composition" MonikerAttributeName="" SerializeId="true" MonikerElementName="compositionMoniker" ElementName="composition" MonikerTypeName="CompositionMoniker">
        <DomainRelationshipMoniker Name="Composition" />
      </XmlClassData>
      <XmlClassData TypeName="ClassShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="classShapeMoniker" ElementName="classShape" MonikerTypeName="ClassShapeMoniker">
        <CompartmentShapeMoniker Name="ClassShape" />
      </XmlClassData>
      <XmlClassData TypeName="InterfaceShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="interfaceShapeMoniker" ElementName="interfaceShape" MonikerTypeName="InterfaceShapeMoniker">
        <CompartmentShapeMoniker Name="InterfaceShape" />
      </XmlClassData>
      <XmlClassData TypeName="CommentBoxShape" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentBoxShapeMoniker" ElementName="commentBoxShape" MonikerTypeName="CommentBoxShapeMoniker">
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </XmlClassData>
      <XmlClassData TypeName="AssociationConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="associationConnectorMoniker" ElementName="associationConnector" MonikerTypeName="AssociationConnectorMoniker">
        <ConnectorMoniker Name="AssociationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="UnidirectionalConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="unidirectionalConnectorMoniker" ElementName="unidirectionalConnector" MonikerTypeName="UnidirectionalConnectorMoniker">
        <ConnectorMoniker Name="UnidirectionalConnector" />
      </XmlClassData>
      <XmlClassData TypeName="BidirectionalConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="bidirectionalConnectorMoniker" ElementName="bidirectionalConnector" MonikerTypeName="BidirectionalConnectorMoniker">
        <ConnectorMoniker Name="BidirectionalConnector" />
      </XmlClassData>
      <XmlClassData TypeName="AggregationConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="aggregationConnectorMoniker" ElementName="aggregationConnector" MonikerTypeName="AggregationConnectorMoniker">
        <ConnectorMoniker Name="AggregationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CompositionConnector" MonikerAttributeName="" SerializeId="true" MonikerElementName="compositionConnectorMoniker" ElementName="compositionConnector" MonikerTypeName="CompositionConnectorMoniker">
        <ConnectorMoniker Name="CompositionConnector" />
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
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="GenItExplorer" />
  <ConnectionBuilders>
    <ConnectionBuilder Name="UnidirectionalAssociationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="UnidirectionalAssociation" />
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
    <ConnectionBuilder Name="BidirectionalAssociationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="BidirectionalAssociation" />
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
    <ConnectionBuilder Name="AggregationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="Aggregation" />
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
    <ConnectionBuilder Name="CompositionBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="Composition" />
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
          <CompartmentMoniker Name="ClassShape/AttributesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ClassHasAttributes.Attributes/!Attribute</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/OperationsCompartment" />
          <ElementsDisplayed>
            <DomainPath>ClassHasOperations.Operations/!Operation</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
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
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="BidirectionalConnector" />
        <DomainRelationshipMoniker Name="BidirectionalAssociation" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="UnidirectionalConnector" />
        <DomainRelationshipMoniker Name="UnidirectionalAssociation" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="AggregationConnector" />
        <DomainRelationshipMoniker Name="Aggregation" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="CompositionConnector" />
        <DomainRelationshipMoniker Name="Composition" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetMultiplicity" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/SourceRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/SourceRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="AssociationConnector/TargetRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Association/TargetRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="CommentConnector" />
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="GeneralizationConnector" />
        <DomainRelationshipMoniker Name="Generalization" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ImplementationConnector" />
        <DomainRelationshipMoniker Name="Implementation" />
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
      <ElementTool Name="Attribute" ToolboxIcon="resources\attributetool.bmp" Caption="Attribute" Tooltip="Create an Attribute on a Class" HelpKeyword="AttributeF1Keyword">
        <DomainClassMoniker Name="ModelAttribute" />
      </ElementTool>
      <ElementTool Name="ClassOperation" ToolboxIcon="resources\operationtool.bmp" Caption="Class Operation" Tooltip="Create an Operation on a Class" HelpKeyword="ClassOperationF1Keyword">
        <DomainClassMoniker Name="ClassOperation" />
      </ElementTool>
      <ElementTool Name="ModelInterface" ToolboxIcon="Resources\InterfaceTool.bmp" Caption="Interface" Tooltip="Create an Interface" HelpKeyword="ModelInterfaceF1Keyword">
        <DomainClassMoniker Name="ModelInterface" />
      </ElementTool>
      <ElementTool Name="InterfaceOperation" ToolboxIcon="resources\interfaceoperationtool.bmp" Caption="Interface Operation" Tooltip="Create an Operation on an Interface" HelpKeyword="InterfaceOperationF1Keyword">
        <DomainClassMoniker Name="InterfaceOperation" />
      </ElementTool>
      <ConnectionTool Name="UnidirectionalAssociation" ToolboxIcon="Resources\UnidirectionTool.bmp" Caption="Unidirectional Association" Tooltip="Create a Unidirectional link" HelpKeyword="ConnectUnidirectionalAssociationF1Keyword">
        <ConnectionBuilderMoniker Name="GenIt/UnidirectionalAssociationBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="BidirectionalAssociation" ToolboxIcon="Resources\AssociationTool.bmp" Caption="Bidirectional Association" Tooltip="Create a Bidirectional link" HelpKeyword="ConnectBidirectionalAssociationF1Keyword">
        <ConnectionBuilderMoniker Name="GenIt/BidirectionalAssociationBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="Aggregation" ToolboxIcon="Resources\AggregationTool.bmp" Caption="Aggregation" Tooltip="Create an Aggregation link" HelpKeyword="AggregationF1Keyword">
        <ConnectionBuilderMoniker Name="GenIt/AggregationBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="Composition" ToolboxIcon="Resources\CompositeTool.bmp" Caption="Composition" Tooltip="Create a Composition link" HelpKeyword="CompositionF1Keyword">
        <ConnectionBuilderMoniker Name="GenIt/CompositionBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="Generalization" ToolboxIcon="resources\generalizationtool.bmp" Caption="Inheritance" Tooltip="Create a Generalization or Implementation link" HelpKeyword="GeneralizationF1Keyword" ReversesDirection="true">
        <ConnectionBuilderMoniker Name="GenIt/GeneralizationBuilder" />
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