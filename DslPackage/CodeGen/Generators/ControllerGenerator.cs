//using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
//{
//	internal class ControllerGenerator
//	{
//		private readonly ModelRoot _modelRoot;
//		private readonly List<EntityModel> _entities;
//		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
//		private readonly List<string> _usings = new List<string>();

//		internal ControllerGenerator(ModelRoot modelRoot)
//		{
//			// Convenience vars
//			_modelRoot = modelRoot;
//			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
//			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
//			{
//				if (!_modules.ContainsKey(module.Name))
//					_modules.Add(module.Name, module);
//			}
//		}

//		private void ResetUsings(EntityModel entity, ServiceModel serviceModel, ModuleModel module)
//		{
//			_usings.Clear();

//			// Default usings
//			_usings.AddLine(0, "Microsoft.AspNetCore.Mvc");
//			_usings.AddLine(0, "Microsoft.AspNetCore.Authorization");
//			_usings.AddLine(0, "Dyvenix.App1.Common.Api.Filters");
//			_usings.AddLines(0, _modelRoot.UsingsList);
//			_usings.Add(_modelRoot.EntitiesNamespace);
//			_usings.Add($"{module.Namespace}.Services.{serviceModel.Version}");

//			foreach (var u in entity.UsingsList)
//				_usings.AddIfNotExists(u);

//			foreach (var u in serviceModel.ControllerUsingsList)
//				_usings.AddIfNotExists(u);

//			if (serviceModel.ReadMethods.Any(m => m.UseRequest))
//				_usings.AddIfNotExists($"{module.RequestNamespace}.{serviceModel.Version}");
//		}

//		internal void Validate(List<string> errors)
//		{
//			//foreach (var entity in _entities)
//			//{
//			//	if (!entity.GenerateCode || entity.ServiceModels.Count == 0)
//			//		continue;
//			//	if (string.IsNullOrEmpty(entity.Module))
//			//		errors.Add($"Entity '{entity.Name}' does not have a Module assigned. Please set it in the Entity properties.");
//			//}
//		}

//		internal void GenerateCode()
//		{
//			foreach (var entity in _entities.Where(e => e.GenerateCode))
//			{
//				foreach (var serviceModel in entity.ServiceModels)
//				{
//					GenerateController(entity, serviceModel);
//				}
//			}
//		}

//		private void GenerateController(EntityModel entity, ServiceModel serviceModel)
//		{
//			var module = _modules[entity.Module];
//			var controllerOutputDir = Path.Combine(module.ControllersFolder, $"{serviceModel.Version}");
//			ResetUsings(entity, serviceModel, module);
//			Directory.CreateDirectory(controllerOutputDir);  // Ensure output dir exists
//			var controllerName = $"{entity.Name}Controller";
//			var serviceName = $"{entity.Name}Service";
//			var serviceVarName = serviceName.ToCamelCase();

//			// Attributes
//			var attrs = BuildControllerAttributes(serviceModel, module, serviceName);

//			// Declaration
//			var declaration = new List<string>();
//			declaration.AddLine(0, $"public class {controllerName} : ControllerBase");

//			// Constructor
//			var constructor = new List<string>();
//			constructor.AddLine(0, $"public {controllerName}(I{serviceName} {serviceVarName})");
//			constructor.AddLine(0, "{");
//			constructor.AddLine(1, $"_{serviceVarName} = {serviceVarName};");
//			constructor.AddLine(0, "}");

//			// Create
//			var createMethodsOutput = new List<string>();
//			if (serviceModel.InclCreate)
//				this.GenerateCreateControllerMethod(entity, serviceModel, serviceVarName, createMethodsOutput);

//			// Delete
//			var deleteMethodsOutput = new List<string>();
//			if (serviceModel.InclDelete)
//				this.GenerateDeleteControllerMethod(entity, serviceModel, serviceVarName, deleteMethodsOutput);

//			// Update methods
//			var updMethodsOutput = new List<string>();
//			if (serviceModel.InclUpdate || serviceModel.UpdateMethods.Any())
//			{
//				// Full update method
//				if (serviceModel.InclUpdate)
//					this.GenerateFullUpdateControllerMethod(entity, serviceModel, serviceVarName, updMethodsOutput);

//				// Normal update methods
//				foreach (UpdateMethodModel updMethod in serviceModel.UpdateMethods)
//				{
//					if (updMethodsOutput.Count > 0)
//						updMethodsOutput.AddLine();
//					this.GenerateUpdateMethod(entity, updMethod, serviceVarName, updMethodsOutput);
//				}
//			}

//			// Read methods - single
//			var singleReadMethodsOutput = new List<string>();
//			foreach (ReadMethodModel singleMethod in serviceModel.ReadMethods.Where(m => !m.IsList))
//			{
//				if (singleReadMethodsOutput.Count > 0)
//					singleReadMethodsOutput.AddLine();
//				this.GenerateReadSingleMethod(entity, singleMethod, serviceVarName, singleReadMethodsOutput);
//			}

//			// Read methods - list
//			var listMethodsOutput = new List<string>();
//			foreach (ReadMethodModel listMethod in serviceModel.ReadMethods.Where(m => m.IsList && !m.UseRequest))
//			{
//				if (listMethodsOutput.Count > 0)
//					listMethodsOutput.AddLine();
//				this.GenerateReadListMethod(entity, listMethod, serviceVarName, listMethodsOutput);
//			}

//			// Read methods - query
//			var queryMethodsOutput = new List<string>();
//			var sortingMethodOutput = new List<string>();
//			if (serviceModel.ReadMethods.Any(m => m.UseRequest))
//			{
//				if (queryMethodsOutput.Count > 0)
//					queryMethodsOutput.AddLine();
//				foreach (ReadMethodModel queryMethod in serviceModel.ReadMethods.Where(m => m.UseRequest))
//					this.GenerateQueryControllerMethod(entity, queryMethod, serviceVarName, queryMethodsOutput);
//			}

//			// Write the file

//			var fileContent = new List<string>();

//			if (_modelRoot.InclHeader)
//				fileContent.Add(CodeGenUtils.FileHeader);
//			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
//			fileContent.AddLine();
//			fileContent.AddLine(0, $"namespace {module.Namespace}.Controllers.{serviceModel.Version};");
//			fileContent.AddLine();
//			fileContent.AddLines(0, attrs);
//			fileContent.AddLines(0, declaration);
//			fileContent.AddLine(0, "{");
//			fileContent.AddLine(1, $"private readonly I{serviceName} _{serviceVarName};");
//			fileContent.AddLine();
//			fileContent.AddLines(1, constructor);

//			if (createMethodsOutput.Count > 0)
//				fileContent.AddLines(1, createMethodsOutput);
//			fileContent.AddLines(1, deleteMethodsOutput);

//			if (updMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#region Updates");
//			}
//			fileContent.AddLines(1, updMethodsOutput);
//			if (updMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#endregion");
//			}

//			if (singleReadMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#region Read Methods - Single");
//			}
//			fileContent.AddLines(1, singleReadMethodsOutput);
//			if (updMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#endregion");
//			}

//			if (listMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#region Read Methods - List");
//			}
//			fileContent.AddLines(1, listMethodsOutput);
//			if (updMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#endregion");
//			}

//			if (queryMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#region Query Methods");
//			}
//			fileContent.AddLines(0, queryMethodsOutput);
//			if (updMethodsOutput.Count > 0)
//			{
//				fileContent.AddLine();
//				fileContent.AddLine(1, "#endregion");
//			}

//			fileContent.AddLine(0, "}");

//			var fileContents = fileContent.AsString();

//			var outputDir = Path.Combine(module.ControllersFolder, $"{serviceModel.Version}");
//			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
//			var outputFilepath = Path.Combine(outputDir, $"{controllerName}.cs");

//			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

//			OutputHelper.Write($"Completed code gen for controller: {controllerName}");
//		}

//		private List<string> BuildControllerAttributes(ServiceModel serviceModel, ModuleModel module, string serviceClassName)
//		{
//			var attrs = new List<string>();

//			attrs.AddLine(0, "[ApiController]");
//			attrs.AddLine(0, $"[ServiceFilter(typeof(ApiExceptionFilter<{serviceClassName}>))]");
//			attrs.AddLine(0, $"[Asp.Versioning.ApiVersion(\"{serviceModel.Version}\")]");
//			attrs.AddLine(0, $"[Route(\"api/{module.Name.ToLower()}/{{version:apiVersion}}/[controller]\")]");
//			attrs.AddLine(0, $"[Route(\"api/{module.Name.ToLower()}/[controller]\")]");

//			foreach (var a in serviceModel.ControllerAttributesList)
//				attrs.AddIfNotExists(a);

//			return attrs;
//		}

//		internal void GenerateCreateControllerMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
//		{
//			output.AddLine();
//			output.AddLine(0, "#region Create");
//			output.AddLine();
//			output.AddLine(0, $"[HttpPost, Route(\"[action]\")]");
//			output.AddLine(0, $"[Authorize(\"{string.Join(",", serviceModel.CreatePermissionsList)}\")]");
//			output.AddLine(0, $"public async Task<ActionResult> Create{entity.Name}([FromBody] {entity.Name} {svcVarName})");
//			output.AddLine(0, "{");
//			output.AddLine(0 + 1, $"return Ok(await _{svcVarName}.Create{entity.Name}({svcVarName}));");
//			output.AddLine(0, "}");
//			output.AddLine();
//			output.AddLine(0, "#endregion");
//		}

//		internal void GenerateDeleteControllerMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
//		{
//			output.AddLine();
//			output.AddLine(0, "#region Delete");
//			output.AddLine();
//			output.AddLine(0, $"[HttpPost, Route(\"[action]\")]");
//			output.AddLine(0, $"[Authorize(\"{string.Join(",", serviceModel.DeletePermissionsList)}\")]");
//			output.AddLine(0, $"public async Task<ActionResult> Delete{entity.Name}(Guid id)");
//			output.AddLine(0, "{");
//			output.AddLine(0 + 1, $"return Ok(await _{svcVarName}.Delete{entity.Name}(id));");
//			output.AddLine(0, "}");
//			output.AddLine();
//			output.AddLine(0, "#endregion");
//		}

//		private void GenerateFullUpdateControllerMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
//		{
//			output.AddLine();
//			output.AddLine(0, $"[HttpPost, Route(\"[action]\")]");
//			output.AddLine(0, $"[Authorize(\"{string.Join(",", serviceModel.UpdatePermissionsList)}\")]");
//			output.AddLine(0, $"public async Task<ActionResult> Update{entity.Name}([FromBody] {entity.Name} {svcVarName})");
//			output.AddLine(0, "{");
//			output.AddLine(0 + 1, $"await _{svcVarName}.Update{entity.Name}({svcVarName});");
//			output.AddLine(0 + 1, $"return Ok();");
//			output.AddLine(0, "}");
//		}

//		internal void GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method, string svcVarName, List<string> output)
//		{
//			var tc = 0;

//			var updateProps = new List<UpdatePropertyModel>();
//			foreach (var updProp in method.UpdateProperties.Where(p => !p.IsOptional))
//				updateProps.Add(updProp);
//			foreach (var updProp in method.UpdateProperties.Where(p => p.IsOptional))
//				updateProps.Add(updProp);

//			var route = new StringBuilder();
//			route.Append("\"[action]");
//			var inputArgs = new StringBuilder();
//			var args = new StringBuilder();

//			route.Append($"/{{id}}");
//			inputArgs.Append("Guid id");
//			if (entity.InclRowVersion)
//			{
//				route.Append($"/{{rowVersion}}");
//				args.Append(", byte[] rowVersion");
//			}
//			foreach (var updProp in method.UpdateProperties)
//			{
//				route.Append($"/{{{updProp.PropertyModel.ArgName}}}");
//				inputArgs.Append($", {updProp.PropertyModel.CSType} {updProp.PropertyModel.ArgName}");
//			}

//			args.Append("id");
//			if (entity.InclRowVersion)
//				args.Append(", rowVersion");
//			foreach (var updProp in updateProps)
//				args.Append($", {updProp.PropertyModel.ArgName}");
//			route.Append("\"");

//			output.AddLine(tc, $"[HttpPatch, Route({route})]");
//			output.AddLine(tc, $"public async Task<ActionResult> {method.Name}({inputArgs})");
//			output.AddLine(tc, "{");
//			output.AddLine(tc + 1, $"await _{svcVarName}.{method.Name}({args});");
//			output.AddLine(tc + 1, $"return Ok();");
//			output.AddLine(tc, "}");
//		}

//		internal void GenerateReadSingleMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output)
//		{
//			var tc = 0;

//			// Attributes
//			if (method.Attributes.Any())
//				foreach (var attr in method.Attributes)
//					output.AddLine(tc, $"[{attr}]");

//			var filterArg = string.Empty;
//			var filterRoute = string.Empty;
//			var filterParams = string.Empty;
//			if (method.FilterProperties.Count > 0)
//			{
//				filterArg = method.FilterProperties[0].PropertyModel.ArgName;
//				filterRoute = $"/{{{filterArg}}}";
//				filterParams = $"{method.FilterProperties[0].PropertyModel.CSType} {method.FilterProperties[0].PropertyModel.ArgName}";
//			}

//			output.AddLine();
//			output.AddLine(tc, $"[HttpGet, Route(\"[action]{filterRoute}\")]");
//			output.AddLine(tc, $"public async Task<ActionResult<{entity.Name}>> {method.Name}({filterParams})");
//			output.AddLine(tc, "{");
//			output.AddLine(tc + 1, $"return Ok(await _{svcVarName}.{method.Name}({filterArg}));");
//			output.AddLine(tc, "}");
//		}

//		internal void GenerateReadListMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output)
//		{
//			var tc = 0;

//			// Attributes
//			if (method.Attributes.Any())
//				foreach (var attr in method.Attributes)
//					output.AddLine(tc, $"[{attr}]");

//			StringBuilder sbRoute = new StringBuilder();
//			StringBuilder sbArgs = new StringBuilder();

//			var reqFilterProps = method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal);
//			var optFilterProps = method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal);

//			// Required
//			foreach (var filterProp in reqFilterProps)
//			{
//				// Required arguments go in the route
//				sbRoute.Append($"/{{{filterProp.PropertyModel.ArgName}}}");
//				if (sbArgs.Length > 0)
//					sbArgs.Append(", ");
//				sbArgs.Append($"[FromRoute] {filterProp.PropertyModel.CSType} {filterProp.PropertyModel.ArgName}");
//			}

//			// Optional
//			foreach (var filterProp in optFilterProps)
//			{
//				if (sbArgs.Length > 0)
//					sbArgs.Append(", ");
//				var nullChar = filterProp.PropertyModel.DataType != DataTypes.String ? "?" : string.Empty;
//				sbArgs.Append($"[FromQuery] {filterProp.PropertyModel.CSType}{nullChar} {filterProp.PropertyModel.ArgName} = null");
//			}

//			// Paging is always optional
//			if (method.InclPaging)
//			{
//				if (sbArgs.Length > 0)
//					sbArgs.Append(", ");
//				sbArgs.Append("[FromQuery] int pgSize = 0, [FromQuery] int pgOffset = 0");
//			}

//			// Vars
//			StringBuilder sbVars = new StringBuilder();
//			foreach (var filterProp in reqFilterProps)
//			{
//				if (sbVars.Length > 0)
//					sbVars.Append(", ");
//				sbVars.Append(filterProp.PropertyModel.ArgName);
//			}
//			foreach (var filterProp in optFilterProps)
//			{
//				if (sbVars.Length > 0)
//					sbVars.Append(", ");
//				sbVars.Append(filterProp.PropertyModel.ArgName);
//			}
//			if (method.InclPaging)
//			{
//				if (sbVars.Length > 0)
//					sbVars.Append(", ");
//				sbVars.Append("pgSize, pgOffset");
//			}

//			output.AddLine();
//			output.AddLine(tc, $"[HttpGet, Route(\"[action]{sbRoute}\")]");
//			output.AddLine(tc, $"public async Task<ActionResult<List<{entity.Name}>>> {method.Name}({sbArgs})");
//			output.AddLine(tc, "{");
//			output.AddLine(tc + 1, $"return Ok(await _{svcVarName}.{method.Name}({sbVars}));");
//			output.AddLine(tc, "}");
//		}

//		internal void GenerateQueryControllerMethod(EntityModel entity, ReadMethodModel queryMethod, string svcVarName, List<string> output)
//		{
//			var tc = 1;
//			output.AddLine();
//			var queryClassName = $"{queryMethod.Name}Query";
//			var queryVarName = queryClassName.ToCamelCase();

//			// Attributes
//			if (queryMethod.Attributes.Any())
//				foreach (var attr in queryMethod.Attributes)
//					output.AddLine(tc, $"[{attr}]");

//			// Method
//			output.AddLine(tc, "[HttpPost, Route(\"[action]\")]");
//			output.AddLine(tc, $"public async Task<ActionResult<EntityList<{entity.Name}>>> {queryMethod.Name}([FromBody] {queryClassName} {queryVarName})");
//			output.AddLine(tc, "{");
//			output.AddLine(tc + 1, $"return Ok(await _{svcVarName}.{queryMethod.Name}({queryVarName}));");
//			output.AddLine(tc, "}");
//		}
//	}
//}