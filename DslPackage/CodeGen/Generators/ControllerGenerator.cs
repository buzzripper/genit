using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class ControllerGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal ControllerGenerator(ModelRoot modelRoot)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
			{
				if (!_modules.ContainsKey(module.Name))
					_modules.Add(module.Name, module);
			}
		}

		private void ResetUsings(EntityModel entity, ServiceModel serviceModel, ModuleModel module)
		{
			_usings.Clear();

			// Default usings
			_usings.AddLines(0, _modelRoot.UsingsList);
			_usings.Add(_modelRoot.EntitiesNamespace);

			foreach (var u in entity.UsingsList)
				_usings.AddIfNotExists(u);

			foreach (var u in serviceModel.ControllerUsingsList)
				_usings.AddIfNotExists(u);
		}

		internal void Validate(List<string> errors)
		{
			//foreach (var entity in _entities)
			//{
			//	if (!entity.GenerateCode || entity.ServiceModels.Count == 0)
			//		continue;
			//	if (string.IsNullOrEmpty(entity.Module))
			//		errors.Add($"Entity '{entity.Name}' does not have a Module assigned. Please set it in the Entity properties.");
			//}
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				foreach (var serviceModel in entity.ServiceModels)
				{
					GenerateController(entity, serviceModel);
				}
			}
		}

		private void GenerateController(EntityModel entity, ServiceModel serviceModel)
		{
			var module = _modules[entity.Module];
			var controllerOutputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RootFolder, "Controllers", serviceModel.Version);
			ResetUsings(entity, serviceModel, module);
			Directory.CreateDirectory(controllerOutputDir);  // Ensure output dir exists
			var controllerName = $"{entity.Name}Controller";
			var serviceName = $"{entity.Name}Service";
			var serviceVarName = serviceName.ToCamelCase();

			// Attributes
			var attrs = BuildControllerAttributes(serviceModel);

			// Declaration
			var declaration = new List<string>();
			declaration.AddLine(0, $"public class {controllerName} : ApiControllerBase<{controllerName}>");
			declaration.AddLine(0, "{");

			// Create
			var createMethodsOutput = new List<string>();
			if (serviceModel.InclCreate)
				this.GenerateCreateControllerMethod(entity, serviceVarName, createMethodsOutput);

			//// Delete
			//var deleteMethodsOutput = new List<string>();
			//if (entity.Service.InclDelete)
			//	this.GenerateDeleteControllerMethod(entity, serviceVarName, deleteMethodsOutput);

			//// Update methods
			//var updMethodsOutput = new List<string>();
			//if (entity.Service.InclUpdate || entity.Service.UpdateMethods.Any())
			//{
			//	updMethodsOutput.AddLine();
			//	updMethodsOutput.AddLine(1, "#region Update");

			//	// Full update method
			//	if (entity.Service.InclUpdate)
			//		this.GenerateFullUpdateControllerMethod(entity, serviceVarName, updMethodsOutput);

			//	// Normal update methods
			//	foreach (UpdateMethodModel updMethod in entity.Service.UpdateMethods)
			//	{
			//		if (updMethodsOutput.Count > 0)
			//			updMethodsOutput.AddLine();
			//		this.GenerateUpdateMethod(entity, updMethod, serviceVarName, updMethodsOutput);
			//	}

			//	updMethodsOutput.AddLine();
			//	updMethodsOutput.AddLine(1, "#endregion");
			//}

			//// Read methods - single
			//var singleMethodsOutput = new List<string>();
			//foreach (ReadMethodModel singleMethod in entity.Service.ReadMethods.Where(m => !m.IsList))
			//{
			//	if (singleMethodsOutput.Count > 0)
			//		singleMethodsOutput.AddLine();
			//	this.GenerateSingleControllerMethod(entity, singleMethod, serviceVarName, singleMethodsOutput);
			//}

			//// Read methods - list
			//var listMethodsOutput = new List<string>();
			//foreach (ReadMethodModel listMethod in entity.Service.ReadMethods.Where(m => m.IsList && !m.UseQuery))
			//{
			//	if (singleMethodsOutput.Count > 0)
			//		singleMethodsOutput.AddLine();
			//	this.GenerateListControllerMethod(entity, listMethod, serviceVarName, listMethodsOutput);
			//}

			//// Read methods - query
			//var queryMethodsOutput = new List<string>();
			//var sortingMethodOutput = new List<string>();
			//if (entity.Service.ReadMethods.Any(m => m.UseQuery))
			//{
			//	queryMethodsOutput.AddLine();
			//	queryMethodsOutput.AddLine(1, "#region Queries");
			//	foreach (ReadMethodModel queryMethod in entity.Service.ReadMethods.Where(m => m.UseQuery))
			//		this.GenerateQueryControllerMethod(entity, queryMethod, serviceVarName, queryMethodsOutput);
			//	queryMethodsOutput.AddLine();
			//	queryMethodsOutput.AddLine(1, "#endregion");
			//}

			// Write the file

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.Controllers.{serviceModel.Version};");
			fileContent.AddLine();
			fileContent.AddLines(0, attrs);
			fileContent.AddLines(0, declaration);
			fileContent.AddLine(0, "{");
			//fileContent.AddLines(0, fields);
			fileContent.AddLine(1, $"private readonly I{serviceName} _{serviceVarName}");
			if (createMethodsOutput.Count > 0)
				fileContent.AddLines(1, createMethodsOutput);
			//fileContent.AddLines(0, deleteMethodsOutput);
			//fileContent.AddLines(0, updMethodsOutput);
			//fileContent.AddLines(1, singleMethodsOutput);
			//fileContent.AddLines(1, listMethodsOutput);
			//fileContent.AddLines(0, queryMethodsOutput);
			fileContent.AddLine(0, "}");

			var fileContents = fileContent.AsString();

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RootFolder, "Controllers", serviceModel.Version);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{controllerName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private List<string> BuildControllerAttributes(ServiceModel serviceModel)
		{
			var attrs = new List<string>();

			foreach (var a in serviceModel.ControllerAttributesList)
				attrs.AddIfNotExists(a);

			return attrs.Select(a => $"[{a}]").ToList();
		}

		internal void GenerateCreateControllerMethod(EntityModel entity, string svcVarName, List<string> output)
		{
			var tc = 1;
			var className = entity.Name;

			output.AddLine();
			output.AddLine(tc, "#region Create");
			output.AddLine();
			output.AddLine(tc, $"[HttpPost, Route(\"[action]\")]");
			output.AddLine(tc, $"public async Task<ActionResult> Create{className}({className} {svcVarName})");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, "var apiResponse = CreateApiResponse<Guid>();");
			output.AddLine(tc + 1, "try {");
			output.AddLine(tc + 2, $"apiResponse.Data = await _{svcVarName}.Create{className}({svcVarName});");
			output.AddLine();
			output.AddLine(tc + 2, "return Ok(apiResponse);");
			output.AddLine();
			output.AddLine(tc + 1, "} catch (Exception ex) {");
			output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(apiResponse, ex);");
			output.AddLine(tc + 1, "}");
			output.AddLine(tc, "}");
			output.AddLine();
			output.AddLine(tc, "#endregion");
		}

		//internal void GenerateDeleteControllerMethod(EntityModel entity, string svcVarName, List<string> output)
		//{
		//	var tc = 1;
		//	var className = entity.Name;
		//	var varName = Utils.ToCamelCase(className);

		//	output.AddLine();
		//	output.AddLine(tc, "#region Delete");
		//	output.AddLine();
		//	output.AddLine(tc, $"[HttpPost, Route(\"[action]/{{id}}\")]");
		//	output.AddLine(tc, $"public async Task<ActionResult> Delete{className}(Guid id)");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, "var apiResponse = CreateApiResponse<bool>();");
		//	output.AddLine(tc + 1, "try {");
		//	output.AddLine(tc + 2, $"apiResponse.Data = await _{svcVarName}.Delete{className}(id);");
		//	output.AddLine();
		//	output.AddLine(tc + 2, "return Ok(apiResponse);");
		//	output.AddLine();
		//	output.AddLine(tc + 1, "} catch (Exception ex) {");
		//	output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(apiResponse, ex);");
		//	output.AddLine(tc + 1, "}");
		//	output.AddLine(tc, "}");
		//	output.AddLine();
		//	output.AddLine(tc, "#endregion");
		//}

		//internal void GenerateFullUpdateControllerMethod(EntityModel entity, string svcVarName, List<string> output)
		//{
		//	var tc = 1;
		//	var className = entity.Name;
		//	var varName = Utils.ToCamelCase(className);

		//	output.AddLine();
		//	output.AddLine(tc, $"[HttpPut, Route(\"[action]\")]");
		//	output.AddLine(tc, $"public async Task<ActionResult> Update{className}({className} {varName})");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, $"var apiResponse = CreateApiResponse<byte[]>();");
		//	output.AddLine(tc + 1, "try {");
		//	if (entity.InclRowVersion)
		//	{
		//		output.AddLine(tc + 2, $"apiResponse.Data = await _{svcVarName}.Update{className}({varName});");
		//	}
		//	else
		//	{
		//		output.AddLine(tc + 2, $"var apiResponse =new ApiResponse();");
		//		output.AddLine();
		//		output.AddLine(tc + 2, $"await _{svcVarName}.Update{className}({varName});");
		//	}
		//	output.AddLine();
		//	output.AddLine(tc + 2, "return Ok(apiResponse);");
		//	output.AddLine();
		//	output.AddLine(tc + 1, "} catch (Exception ex) {");
		//	output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(apiResponse, ex);");
		//	output.AddLine(tc + 1, "}");
		//	output.AddLine(tc, "}");
		//}

		//internal void GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method, string svcVarName, List<string> output)
		//{
		//	var tc = 1;

		//	var updateProps = new List<UpdatePropertyModel>();
		//	foreach (var updProp in method.UpdateProperties.Where(p => !p.IsOptional))
		//		updateProps.Add(updProp);
		//	foreach (var updProp in method.UpdateProperties.Where(p => p.IsOptional))
		//		updateProps.Add(updProp);

		//	var args = new StringBuilder();
		//	args.Append("request.Id");
		//	if (entity.InclRowVersion)
		//		args.Append(", request.RowVersion");
		//	foreach (var updProp in updateProps)
		//		args.Append($", request.{updProp.Property.Name}");

		//	output.AddLine(tc, "[HttpPatch, Route(\"[action]\")]");
		//	output.AddLine(tc, $"public async Task<ActionResult> {method.Name}({method.Name}Req request)");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, $"var apiResponse = CreateApiResponse<byte[]>();");
		//	output.AddLine(tc + 1, "try {");
		//	output.AddLine(tc + 2, $"apiResponse.Data = await _{svcVarName}.{method.Name}({args});");
		//	output.AddLine();
		//	output.AddLine(tc + 2, $"return Ok(apiResponse);");
		//	output.AddLine();
		//	output.AddLine(tc + 1, "} catch (Exception ex) {");
		//	output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(apiResponse, ex);");
		//	output.AddLine(tc + 1, "}");
		//	output.AddLine(tc, "}");
		//}

		//internal void GenerateSingleControllerMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output)
		//{
		//	var tc = 1;

		//	// Attributes
		//	if (method.Attributes.Any())
		//		foreach (var attr in method.Attributes)
		//			output.AddLine(tc, $"[{attr}]");

		//	var filterArg = string.Empty;
		//	var filterRoute = string.Empty;
		//	var filterParams = string.Empty;
		//	if (method.FilterProperties.Count > 0)
		//	{
		//		filterArg = method.FilterProperties[0].Property.ArgName;
		//		filterRoute = $"/{{{filterArg}}}";
		//		filterParams = $"{method.FilterProperties[0].Property.DatatypeName} {method.FilterProperties[0].Property.ArgName}";
		//	}

		//	output.AddLine(tc, $"[HttpGet, Route(\"[action]{filterRoute}\")]");
		//	output.AddLine(tc, $"public async Task<ActionResult<{entity.Name}>> {method.Name}({filterParams})");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, $"var apiResponse = CreateApiResponse<{entity.Name}>();");
		//	output.AddLine(tc + 1, "try {");
		//	output.AddLine(tc + 2, $"apiResponse.Data =  await _{svcVarName}.{method.Name}({filterArg});");
		//	output.AddLine(tc + 2, $"return Ok(apiResponse);");
		//	output.AddLine();
		//	output.AddLine(tc + 1, "} catch (Exception ex) {");
		//	output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(apiResponse, ex);");
		//	output.AddLine(tc + 1, "}");
		//	output.AddLine(tc, "}");
		//}

		//internal void GenerateListControllerMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output)
		//{
		//	var tc = 1;

		//	// Attributes
		//	if (method.Attributes.Any())
		//		foreach (var attr in method.Attributes)
		//			output.AddLine(tc, $"[{attr}]");

		//	StringBuilder sbRoute = new StringBuilder();
		//	StringBuilder sbArgs = new StringBuilder();

		//	var reqFilterProps = method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal);
		//	var optFilterProps = method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal);

		//	// Required
		//	foreach (var filterProp in reqFilterProps)
		//	{
		//		// Required arguments go in the route
		//		sbRoute.Append($"/{{{filterProp.Property.ArgName}}}");
		//		if (sbArgs.Length > 0)
		//			sbArgs.Append(", ");
		//		sbArgs.Append($"[FromRoute] {filterProp.Property.DatatypeName} {filterProp.Property.ArgName}");
		//	}

		//	// Optional
		//	foreach (var filterProp in optFilterProps)
		//	{
		//		if (sbArgs.Length > 0)
		//			sbArgs.Append(", ");
		//		var nullChar = filterProp.Property.PrimitiveType?.Id != PrimitiveType.String.Id ? "?" : string.Empty;
		//		sbArgs.Append($"[FromQuery] {filterProp.Property.DatatypeName}{nullChar} {filterProp.Property.ArgName} = null");
		//	}

		//	// Paging is always optional
		//	if (method.InclPaging)
		//	{
		//		if (sbArgs.Length > 0)
		//			sbArgs.Append(", ");
		//		sbArgs.Append("[FromQuery] int pgSize = 0, [FromQuery] int pgOffset = 0");
		//	}

		//	// Vars
		//	StringBuilder sbVars = new StringBuilder();
		//	foreach (var filterProp in reqFilterProps)
		//	{
		//		if (sbVars.Length > 0)
		//			sbVars.Append(", ");
		//		sbVars.Append(filterProp.Property.ArgName);
		//	}
		//	foreach (var filterProp in optFilterProps)
		//	{
		//		if (sbVars.Length > 0)
		//			sbVars.Append(", ");
		//		sbVars.Append(filterProp.Property.ArgName);
		//	}
		//	if (method.InclPaging)
		//	{
		//		if (sbVars.Length > 0)
		//			sbVars.Append(", ");
		//		sbVars.Append("pgSize, pgOffset");
		//	}

		//	output.AddLine(tc, $"[HttpGet, Route(\"[action]{sbRoute}\")]");
		//	output.AddLine(tc, $"public async Task<ActionResult<List<{entity.Name}>>> {method.Name}({sbArgs})");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, $"var apiResponse = CreateApiResponse<List<{entity.Name}>>();");
		//	output.AddLine(tc + 1, "try {");
		//	output.AddLine(tc + 2, $"apiResponse.Data =  await _{svcVarName}.{method.Name}({sbVars});");
		//	output.AddLine(tc + 2, $"return Ok(apiResponse);");
		//	output.AddLine();
		//	output.AddLine(tc + 1, "} catch (Exception ex) {");
		//	output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(apiResponse, ex);");
		//	output.AddLine(tc + 1, "}");
		//	output.AddLine(tc, "}");
		//}

		//internal void GenerateQueryControllerMethod(EntityModel entity, ReadMethodModel queryMethod, string svcVarName, List<string> output)
		//{
		//	var tc = 1;
		//	output.AddLine();
		//	var queryClassName = $"{queryMethod.Name}Query";
		//	var queryVarName = Utils.ToCamelCase(queryClassName);

		//	// Attributes
		//	if (queryMethod.Attributes.Any())
		//		foreach (var attr in queryMethod.Attributes)
		//			output.AddLine(tc, $"[{attr}]");

		//	// Method
		//	output.AddLine(tc, "[HttpPost, Route(\"[action]\")]");
		//	output.AddLine(tc, $"public async Task<ActionResult<EntityList<{entity.Name}>>> {queryMethod.Name}([FromBody] {queryClassName} {queryVarName})");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, "try {");
		//	output.AddLine(tc + 2, $"return await _{svcVarName}.{queryMethod.Name}({queryVarName});");
		//	output.AddLine(tc + 1, "} catch (Exception ex) {");
		//	output.AddLine(tc + 2, "return LogErrorAndReturnErrorResponse(ex);");
		//	output.AddLine(tc + 1, "}");
		//	output.AddLine(tc, "}");
		//}

		/*
				internal string ReplaceControllerTemplateTokens(string template, List<string> addlUsings, List<string> attrsOutput, string controllerVersion, string controllersNamespace, string controllerName, string serviceName, string serviceVarName, List<string> createMethodsOutput, List<string> deleteMethodsOutput, List<string> updMethodsOutput, List<string> singleMethodsOutput, List<string> listMethodsOutput, List<string> queryMethodsOutput, string entityName)
				{
					// Addl Usings
					var sb = new StringBuilder();
					addlUsings.ForEach(x =>
					{
						if (sb.Length > 0)
							sb.AppendLine();
						sb.Append($"using {x};");
					});
					template = template.Replace(Utils.FmtToken(cToken_AddlUsings), sb.ToString());

					// Class Attributes
					sb = new StringBuilder();
					attrsOutput.ForEach(x =>
					{
						if (sb.Length > 0)
							sb.AppendLine();
						sb.Append(x);
					});
					template = template.Replace(Utils.FmtToken(cToken_ControllerAttrs), sb.ToString());

					template = template.Replace(Utils.FmtToken(cToken_ControllerVersion), controllerVersion);

					// Various
					template = template.Replace(Utils.FmtToken(cToken_ControllersNs), controllersNamespace);
					template = template.Replace(Utils.FmtToken(cToken_ControllerName), controllerName);
					template = template.Replace(Utils.FmtToken(cToken_ServiceName), serviceName);
					template = template.Replace(Utils.FmtToken(cToken_ServiceVarName), serviceVarName);
					template = template.Replace(Utils.FmtToken(cToken_EntityName), entityName);

					// CUD Methods
					sb = new StringBuilder();
					createMethodsOutput.ForEach(x => sb.AppendLine(x));
					deleteMethodsOutput.ForEach(x => sb.AppendLine(x));
					updMethodsOutput.ForEach(x => sb.AppendLine(x));
					template = template.Replace(Utils.FmtToken(cToken_CudControllerMethods), sb.ToString());

					// Single Methods
					sb = new StringBuilder();
					singleMethodsOutput.ForEach(x => sb.AppendLine(x));
					template = template.Replace(Utils.FmtToken(cToken_SingleMethods), sb.ToString());

					// List Methods
					sb = new StringBuilder();
					listMethodsOutput.ForEach(x => sb.AppendLine(x));
					template = template.Replace(Utils.FmtToken(cToken_ListMethods), sb.ToString());

					// Query Methods
					sb = new StringBuilder();
					queryMethodsOutput.ForEach(x => sb.AppendLine(x));
					template = template.Replace(Utils.FmtToken(cToken_QueryMethods), sb.ToString());

					return template;
				}
		*/
	}
}