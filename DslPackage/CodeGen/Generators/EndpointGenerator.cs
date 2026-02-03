using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class EndpointGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal EndpointGenerator(ModelRoot modelRoot)
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
			_usings.AddLine(0, "Microsoft.AspNetCore.Mvc");
			_usings.AddLine(0, "Microsoft.AspNetCore.Authorization");
			_usings.AddLine(0, "Microsoft.AspNetCore.Routing");
			_usings.AddLine(0, "Microsoft.AspNetCore.Builder");
			_usings.AddLine(0, "Microsoft.AspNetCore.Http");

			_usings.AddLines(0, _modelRoot.UsingsList);
			_usings.Add(_modelRoot.EntitiesNamespace);
			_usings.Add($"{module.Namespace}.Services.v{serviceModel.Version}");
			_usings.AddLine(0, $"{module.Namespace}.Extensions");
			_usings.AddLine(0, $"Dyvenix.App1.Common.Api.Filters");

			foreach (var u in entity.UsingsList)
				_usings.AddIfNotExists(u);

			foreach (var u in serviceModel.ControllerUsingsList)
				_usings.AddIfNotExists(u);

			if (serviceModel.UpdateMethods.Any(m => m.UseDto))
				_usings.AddIfNotExists($"{module.DtoNamespace}.v{serviceModel.Version}");

			if (serviceModel.ReadMethods.Any(m => m.UseQuery))
				_usings.AddIfNotExists($"{module.QueryNamespace}.v{serviceModel.Version}");
		}

		internal void Validate(List<string> errors)
		{
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				foreach (var serviceModel in entity.ServiceModels)
				{
					GenerateEndpoints(entity, serviceModel);
				}
			}
		}

		private void GenerateEndpoints(EntityModel entity, ServiceModel serviceModel)
		{
			var module = _modules[entity.Module];
			ResetUsings(entity, serviceModel, module);
			var className = $"{entity.Name}Endpoints";
			var serviceName = $"{entity.Name}Service";
			var serviceVarName = serviceName.ToCamelCase();

			//// Attributes
			//var attrs = BuildEndpointsAttributes(serviceModel, module, serviceName);

			// Declaration
			var declaration = new List<string>();
			declaration.AddLine(0, $"public static class {className}");

			// Maps method
			var mapMethods = new List<string>();

			// Create
			var createMethodsOutput = new List<string>();
			if (serviceModel.InclCreate)
			{
				// Map
				mapMethods.AddLines(1, GenerateCreateMapMethod(entity, serviceModel));
				// Method
				this.GenerateCreateEndpointsMethod(entity, serviceModel, serviceVarName, createMethodsOutput);
			}

			// Delete
			var deleteMethodsOutput = new List<string>();
			if (serviceModel.InclDelete)
			{
				// Map
				mapMethods.AddLines(1, GenerateDeleteMapMethod(entity, serviceModel));
				// Method
				this.GenerateDeleteMethod(entity, serviceModel, serviceVarName, deleteMethodsOutput);
			}

			// Update methods
			var updMethodsOutput = new List<string>();
			if (serviceModel.InclUpdate || serviceModel.UpdateMethods.Any())
			{
				// Full update method
				if (serviceModel.InclUpdate)
				{
					// Map
					mapMethods.AddLines(1, GenerateFullUpdateMapMethod(entity, serviceModel));
					// Method
					this.GenerateFullUpdateMethod(entity, serviceModel, serviceVarName, updMethodsOutput);
				}

				// Normal update methods
				foreach (UpdateMethodModel updMethod in serviceModel.UpdateMethods)
				{
					if (updMethodsOutput.Count > 0)
						updMethodsOutput.AddLine();

					this.GenerateUpdateMethod(entity, updMethod, serviceVarName, updMethodsOutput, mapMethods);
				}
			}

			// Read methods - single
			var singleReadMethodsOutput = new List<string>();
			foreach (ReadMethodModel singleMethod in serviceModel.ReadMethods.Where(m => !m.IsList))
			{
				this.GenerateReadSingleMethod(entity, singleMethod, serviceVarName, singleReadMethodsOutput, mapMethods);
			}

			// Read methods - list
			var listMethodsOutput = new List<string>();
			foreach (ReadMethodModel listMethod in serviceModel.ReadMethods.Where(m => m.IsList && !m.UseQuery))
			{
				if (listMethodsOutput.Count > 0)
					listMethodsOutput.AddLine();
				this.GenerateReadListMethod(entity, listMethod, serviceVarName, listMethodsOutput, mapMethods);
			}

			// Read methods - query
			var queryMethodsOutput = new List<string>();
			var sortingMethodOutput = new List<string>();
			if (serviceModel.ReadMethods.Any(m => m.UseQuery))
			{
				if (queryMethodsOutput.Count > 0)
					queryMethodsOutput.AddLine();
				foreach (ReadMethodModel queryMethod in serviceModel.ReadMethods.Where(m => m.UseQuery))
					this.GenerateQueryMethod(entity, queryMethod, serviceVarName, queryMethodsOutput, mapMethods);
			}

			// Write the file

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.Controllers.v{serviceModel.Version};");
			fileContent.AddLine();
			//fileContent.AddLines(0, attrs);
			fileContent.AddLines(0, declaration);
			fileContent.AddLine(0, "{");
			fileContent.AddLines(1, GenerateMapEndpointsMethod(mapMethods, module, entity, serviceModel));

			if (createMethodsOutput.Count > 0)
				fileContent.AddLines(1, createMethodsOutput);
			fileContent.AddLines(1, deleteMethodsOutput);

			if (updMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#region Updates");
			}
			fileContent.AddLines(1, updMethodsOutput);
			if (updMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#endregion");
			}

			if (singleReadMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#region Read Methods - Single");
			}
			fileContent.AddLines(1, singleReadMethodsOutput);
			if (singleReadMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#endregion");
			}

			if (listMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#region Read Methods - List");
			}
			fileContent.AddLines(1, listMethodsOutput);
			if (listMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#endregion");
			}

			if (queryMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#region Query Methods");
			}
			fileContent.AddLines(0, queryMethodsOutput);
			if (queryMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#endregion");
			}

			fileContent.AddLine(0, "}");

			var fileContents = fileContent.AsString();

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.ApiRootFolder, "Endpoints", $"v{serviceModel.Version}");
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{className}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for controller: {className}");
		}

		//private List<string> BuildEndpointsAttributes(ServiceModel serviceModel, ModuleModel module, string serviceClassName)
		//{
		//	var attrs = new List<string>();

		//	attrs.AddLine(0, "[ApiController]");
		//	attrs.AddLine(0, $"[ServiceFilter(typeof(ApiExceptionFilter<{serviceClassName}>))]");
		//	attrs.AddLine(0, $"[Asp.Versioning.ApiVersion(\"{serviceModel.Version}\")]");
		//	attrs.AddLine(0, $"[Route(\"api/{module.Name.ToLower()}/v{{version:apiVersion}}/[controller]\")]");
		//	attrs.AddLine(0, $"[Route(\"api/{module.Name.ToLower()}/[controller]\")]");

		//	foreach (var a in serviceModel.ControllerAttributesList)
		//		attrs.AddIfNotExists(a);

		//	return attrs;
		//}

		internal List<string> GenerateMapEndpointsMethod(List<string> mapMethods, ModuleModel module, EntityModel entity, ServiceModel serviceModel)
		{
			var lines = new List<string>();

			lines.AddLine(0, $"public static IEndpointRouteBuilder Map{entity.Name}Endpoints(this IEndpointRouteBuilder app)");
			lines.AddLine(0, "{");
			lines.AddLine(1, $"var group = app.MapGroup(\"api/{module.Name.ToLower()}/{serviceModel.Version}/{entity.Name.ToLower()}\")");
			lines.AddLine(2, $".WithTags(\"{entity.Name}\");");
			lines.AddLines(1, mapMethods);
			lines.AddLine();
			lines.AddLine(1, "return app;");
			lines.AddLine(0, "}");

			return lines;
		}

		// Create

		internal List<string> GenerateCreateMapMethod(EntityModel entity, ServiceModel serviceModel)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, "// Create");
			lines.AddLine(0, $"group.MapPost(\"Create{entity.Name}\", Create{entity.Name})");
			lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
			lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
			if (serviceModel.CreatePermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", serviceModel.CreatePermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		internal void GenerateCreateEndpointsMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
		{
			output.AddLine();
			output.AddLine(0, "#region Create");
			output.AddLine();
			output.AddLine(0, $"public static async Task<IResult> Create{entity.Name}({entity.Name} {entity.Name.ToCamelCase()}, I{entity.Name}Service {svcVarName})");
			output.AddLine(0, "{");
			output.AddLine(0 + 1, $"var result = await {svcVarName}.Create{entity.Name}({entity.Name.ToCamelCase()});");
			output.AddLine(0 + 1, $"return result.ToHttpResult();");
			output.AddLine(0, "}");
			output.AddLine();
			output.AddLine(0, "#endregion");
		}

		// Delete

		internal List<string> GenerateDeleteMapMethod(EntityModel entity, ServiceModel serviceModel)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, "// Delete");
			lines.AddLine(0, $"group.MapPost(\"Delete{entity.Name}\", Delete{entity.Name})");
			lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
			lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
			if (serviceModel.DeletePermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", serviceModel.DeletePermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		internal void GenerateDeleteMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
		{
			output.AddLine();
			output.AddLine(0, "#region Delete");
			output.AddLine();
			output.AddLine(0, $"public static async Task<IResult> Delete{entity.Name}(I{entity.Name}Service {svcVarName}, Guid id)");
			output.AddLine(0, "{");
			output.AddLine(0 + 1, $"var result = await {svcVarName}.Delete{entity.Name}(id);");
			output.AddLine(0 + 1, $"return result.ToHttpResult();");
			output.AddLine(0, "}");
			output.AddLine();
			output.AddLine(0, "#endregion");
		}

		// FullUpdate

		private List<string> GenerateFullUpdateMapMethod(EntityModel entity, ServiceModel serviceModel)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, "// FullUpdate");
			lines.AddLine(0, $"group.MapPost(\"Update{entity.Name}\", Update{entity.Name})");
			lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
			lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
			if (serviceModel.UpdatePermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", serviceModel.UpdatePermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		private void GenerateFullUpdateMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
		{
			output.AddLine();
			output.AddLine(0, $"public static async Task<IResult> Update{entity.Name}(I{entity.Name}Service {svcVarName}, {entity.Name} {entity.Name.ToCamelCase()})");
			output.AddLine(0, "{");
			output.AddLine(1, $"var result = await {svcVarName}.Update{entity.Name}({entity.Name.ToCamelCase()});");
			output.AddLine(1, $"return result.ToHttpResult();");
			output.AddLine(0, "}");
		}

		// Update

		private List<string> GenerateUpdateMapMethod(string methodName, string methodUrl, UpdateMethodModel updMethodModel)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, $"group.MapPost(\"{methodName}\", {methodName})");
			lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
			lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
			if (updMethodModel.PermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", updMethodModel.PermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		internal void GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method, string svcVarName, List<string> output, List<string> mapMethods)
		{
			var tc = 0;

			var updateProps = new List<UpdatePropertyModel>();
			foreach (var updProp in method.UpdateProperties.Where(p => !p.IsOptional))
				updateProps.Add(updProp);
			foreach (var updProp in method.UpdateProperties.Where(p => p.IsOptional))
				updateProps.Add(updProp);

			var route = new StringBuilder();
			route.Append("\"[action]");
			var inputArgs = new StringBuilder();
			var args = new StringBuilder();

			if (method.UseDto)
			{
				inputArgs.Append($", [FromBody] {method.Name}Req request");

				args.Append("request.Id");
				if (entity.InclRowVersion)
					args.Append(", request.RowVersion");
				foreach (var updProp in updateProps)
					args.Append($", request.{updProp.PropertyModel.Name}");
			}
			else
			{
				route.Append($"/{{id}}");
				inputArgs.Append(", Guid id");
				if (entity.InclRowVersion)
				{
					route.Append($"/{{rowVersion}}");
					args.Append(", byte[] rowVersion");
				}
				foreach (var updProp in method.UpdateProperties)
				{
					route.Append($"/{{{updProp.PropertyModel.ArgName}}}");
					inputArgs.Append($", {updProp.PropertyModel.CSType} {updProp.PropertyModel.ArgName}");
				}

				args.Append("id");
				if (entity.InclRowVersion)
					args.Append(", rowVersion");
				foreach (var updProp in updateProps)
					args.Append($", {updProp.PropertyModel.ArgName}");
			}

			output.AddLine(tc, $"public static async Task<IResult> {method.Name}(I{entity.Name}Service {svcVarName}{inputArgs})");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var result = await {svcVarName}.{method.Name}({args});");
			output.AddLine(tc + 1, $"return result.ToHttpResult();");
			output.AddLine(tc, "}");

			mapMethods.AddLines(1, this.GenerateUpdateMapMethod(method.Name, route.ToString(), method));
		}

		// Read Single

		private List<string> GenerateReadSingleMapMethod(string methodName, string methodUrl, ReadMethodModel readMethodModel)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, $"group.MapGet(\"{methodName}\", {methodName})");
			lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
			lines.AddLine(1, $".Produces(StatusCodes.Status404NotFound)");
			if (readMethodModel.PermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", readMethodModel.PermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		internal void GenerateReadSingleMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output, List<string> mapMethods)
		{
			var tc = 0;

			// Attributes
			if (method.Attributes.Any())
				foreach (var attr in method.Attributes)
					output.AddLine(tc, $"[{attr}]");

			var filterArg = string.Empty;
			var filterRoute = string.Empty;
			var filterParams = string.Empty;
			if (method.FilterProperties.Count > 0)
			{
				filterArg = method.FilterProperties[0].PropertyModel.ArgName;
				filterRoute = $"/{{{filterArg}}}";
				filterParams = $"{method.FilterProperties[0].PropertyModel.CSType} {method.FilterProperties[0].PropertyModel.ArgName}";
			}

			output.AddLine();
			output.AddLine(tc, $"public static async Task<IResult> {method.Name}({filterParams}, I{entity.Name}Service {svcVarName})");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var result = await {svcVarName}.{method.Name}({filterArg});");
			output.AddLine(tc + 1, $"return result.ToHttpResult();");
			output.AddLine(tc, "}");

			mapMethods.AddLines(1, this.GenerateReadSingleMapMethod(method.Name, filterRoute, method));
		}

		// Read List

		private List<string> GenerateReadListMapMethod(string methodName, string methodUrl, ReadMethodModel readMethodModel)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, $"group.MapGet(\"{methodName}\", {methodName})");
			lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
			if (readMethodModel.PermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", readMethodModel.PermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		internal void GenerateReadListMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output, List<string> mapMethods)
		{
			var tc = 0;

			// Attributes
			if (method.Attributes.Any())
				foreach (var attr in method.Attributes)
					output.AddLine(tc, $"[{attr}]");

			StringBuilder sbRoute = new StringBuilder();
			StringBuilder sbArgs = new StringBuilder();

			var reqFilterProps = method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal);
			var optFilterProps = method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal);

			// Required
			foreach (var filterProp in reqFilterProps)
			{
				// Required arguments go in the route
				sbRoute.Append($"/{{{filterProp.PropertyModel.ArgName}}}");
				sbArgs.Append(", ");
				sbArgs.Append($"[FromRoute] {filterProp.PropertyModel.CSType} {filterProp.PropertyModel.ArgName}");
			}

			// Optional
			foreach (var filterProp in optFilterProps)
			{
				sbArgs.Append(", ");
				var nullChar = filterProp.PropertyModel.DataType != DataTypes.String ? "?" : string.Empty;
				sbArgs.Append($"[FromQuery] {filterProp.PropertyModel.CSType}{nullChar} {filterProp.PropertyModel.ArgName} = null");
			}

			// Paging is always optional
			if (method.InclPaging)
			{
				sbArgs.Append(", ");
				sbArgs.Append("[FromQuery] int pgSize = 0, [FromQuery] int pgOffset = 0");
			}

			// Vars
			StringBuilder sbVars = new StringBuilder();
			foreach (var filterProp in reqFilterProps)
			{
				if (sbVars.Length > 0)
					sbVars.Append(", ");
				sbVars.Append(filterProp.PropertyModel.ArgName);
			}
			foreach (var filterProp in optFilterProps)
			{
				if (sbVars.Length > 0)
					sbVars.Append(", ");
				sbVars.Append(filterProp.PropertyModel.ArgName);
			}
			if (method.InclPaging)
			{
				if (sbVars.Length > 0)
					sbVars.Append(", ");
				sbVars.Append("pgSize, pgOffset");
			}

			output.AddLine();
			output.AddLine(tc, $"public static async Task<IResult> {method.Name}(I{entity.Name}Service {svcVarName}{sbArgs})");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var result = await {svcVarName}.{method.Name}({sbVars});");
			output.AddLine(tc + 1, $"return result.ToHttpResult();");
			output.AddLine(tc, "}");

			mapMethods.AddLines(1, GenerateReadListMapMethod(method.Name, sbRoute.ToString(), method));
		}

		// Query

		private List<string> GenerateQueryMapMethod(string className, string methodName, ReadMethodModel queryMethod)
		{
			var lines = new List<string>();

			lines.AddLine();
			lines.AddLine(0, $"group.MapPost(\"{methodName}\", {methodName})");
			lines.AddLine(1, $".Produces<EntityList<{className}>>(StatusCodes.Status200OK)");
			if (queryMethod.PermissionsList.Count > 0)
				lines.AddLine(1, $".RequireAuthorization(\"{string.Join(",", queryMethod.PermissionsList)}\");");
			else
				lines.AppendToLast(";");

			return lines;
		}

		internal void GenerateQueryMethod(EntityModel entity, ReadMethodModel queryMethod, string svcVarName, List<string> output, List<string> mapMethods)
		{
			var tc = 1;
			output.AddLine();
			var queryClassName = $"{queryMethod.Name}Query";
			var queryVarName = queryClassName.ToCamelCase();

			// Attributes
			if (queryMethod.Attributes.Any())
				foreach (var attr in queryMethod.Attributes)
					output.AddLine(tc, $"[{attr}]");

			// Method
			output.AddLine(tc, $"public static async Task<IResult> {queryMethod.Name}(I{entity.Name}Service {svcVarName}, [FromBody] {queryClassName} {queryVarName})");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var result = await {svcVarName}.{queryMethod.Name}({queryVarName});");
			output.AddLine(tc + 1, $"return result.ToHttpResult();");
			output.AddLine(tc, "}");

			mapMethods.AddLines(1, GenerateQueryMapMethod(entity.Name, queryMethod.Name, queryMethod));
		}
	}
}