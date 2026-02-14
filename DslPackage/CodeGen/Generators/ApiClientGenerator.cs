using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class ApiClientGenerator
	{
		internal void GenerateCode(EntityModel entity, ServiceModel service, ModuleModel module)
		{
			var apiClientName = $"{entity.Name}ApiClient";

			// Usings
			var usings = new List<string>();
			usings.AddIfNotExists($"{module.ModelRoot.CommonNamespace}.Shared.ApiClients");
			usings.AddIfNotExists($"{module.ModelRoot.CommonNamespace}.Shared.Requests");
			usings.AddIfNotExists(entity.ModelRoot.EntitiesNamespace);
			usings.AddIfNotExists($"{module.Namespace}.Shared.Contracts.{service.Version}");
			if (service.UpdateMethods.Any() || service.ReadMethods.Any(m => m.UseRequest))
				usings.AddIfNotExists($"{module.RequestNamespace}.{service.Version}");
			if (service.ReadMethods.Any(m => m.InclPaging))
				usings.AddIfNotExists($"{module.ModelRoot.CommonNamespace}.Shared.DTOs");

			// Interface signatures
			var interfaceOutput = new List<string>();

			// Declaration
			var declaration = new List<string>();
			declaration.AddLine(0, $"public partial class {apiClientName} : ApiClientBase, I{entity.Name}Service");

			// Constructor
			var constructor = new List<string>();
			constructor.AddLine(0, $"public {apiClientName}(HttpClient httpClient) : base(httpClient)");
			constructor.AddLine(0, "{");
			constructor.AddLine(0, "}");

			// Create
			var createMethodsOutput = new List<string>();
			if (service.InclCreate)
			{
				createMethodsOutput.AddLine();
				createMethodsOutput.AddLine(0, "#region Create");
				this.GenerateCreateMethod(entity, createMethodsOutput, interfaceOutput);
				createMethodsOutput.AddLine();
				createMethodsOutput.AddLine(0, "#endregion");
			}

			// Delete
			var deleteMethodsOutput = new List<string>();
			if (service.InclDelete)
			{
				deleteMethodsOutput.AddLine();
				deleteMethodsOutput.AddLine(0, "#region Delete");
				this.GenerateDeleteMethod(entity, deleteMethodsOutput, interfaceOutput);
				deleteMethodsOutput.AddLine();
				deleteMethodsOutput.AddLine(0, "#endregion");
			}

			// Update methods
			var updMethodsOutput = new List<string>();
			if (service.InclUpdate || service.UpdateMethods.Any())
			{
				// Full udpate
				if (service.InclUpdate)
					this.GenerateFullUpdateMethod(entity, updMethodsOutput, interfaceOutput);
				// Normal updates
				foreach (UpdateMethodModel method in service.UpdateMethods)
					this.GenerateUpdateMethod(entity, method, service, updMethodsOutput, interfaceOutput);
			}

			// Read methods - single
			var singleMethodsOutput = new List<string>();
			if (service.ReadMethods.Where(m => !m.IsList).Any())
			{
				foreach (ReadMethodModel singleMethod in service.ReadMethods.Where(m => !m.IsList))
					this.GenerateReadMethod(entity, singleMethod, singleMethodsOutput, interfaceOutput);
			}

			// Read methods - list
			var listMethodsOutput = new List<string>();
			if (service.ReadMethods.Where(m => m.IsList).Any())
			{
				foreach (ReadMethodModel listMethod in service.ReadMethods.Where(m => m.IsList))
					this.GenerateReadMethod(entity, listMethod, listMethodsOutput, interfaceOutput);
			}

			// Write the file
			var fileContent = new List<string>();

			if (entity.ModelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);
			fileContent.AddLines(0, usings.Select(u => $"using {u};").ToList());
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.Shared.ApiClients.{service.Version};");
			fileContent.AddLine();
			fileContent.AddLines(0, declaration);
			fileContent.AddLine(0, "{");
			fileContent.AddLines(1, constructor);

			if (createMethodsOutput.Count > 0)
				fileContent.AddLines(1, createMethodsOutput);

			if (deleteMethodsOutput.Count > 0)
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

			if (singleMethodsOutput.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "#region Read Methods - Single");
			}
			fileContent.AddLines(1, singleMethodsOutput);
			if (singleMethodsOutput.Count > 0)
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

			fileContent.AddLine(0, "}");

			var fileContents = fileContent.AsString();

			var outputDir = Path.Combine(module.ApiClientsFolder, $"{service.Version}");
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{apiClientName}.g.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for controller: {apiClientName}");
		}

		private void GenerateCreateMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
		{
			var tc = 0;
			var className = entity.Name;
			var varName = className.ToCamelCase();

			// Interface
			var signature = $"Task Create{className}({className} {varName})";
			interfaceOutput.Add(signature);

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({varName});");
			output.AddLine();
			output.AddLine(tc + 1, $"await PostAsync(\"api/v1/{className}/Create{className}\", {varName});");
			output.AddLine(tc, "}");
		}

		private void GenerateDeleteMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
		{
			var tc = 0;
			var className = entity.Name;
			var varName = className.ToCamelCase();

			// Interface
			var signature = $"Task Delete{className}(Guid id)";
			interfaceOutput.Add(signature);

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, "if (id == Guid.Empty)");
			output.AddLine(tc + 2, "throw new ArgumentNullException(nameof(id));");
			output.AddLine();
			output.AddLine(tc + 1, "var deleteReq = new DeleteReq { Id = id };	");
			output.AddLine(tc + 1, $"await DeleteAsync<bool>($\"api/v1/{className}/Delete{className}\", deleteReq);");
			output.AddLine(tc, "}");
		}

		private void GenerateFullUpdateMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
		{
			var tc = 0;
			var className = entity.Name;
			var varName = className.ToCamelCase();
			var returnType = entity.InclRowVersion ? "<byte[]>" : null;
			var returnStr = entity.InclRowVersion ? "return " : null;
			var returnContent = entity.InclRowVersion ? $" {varName}.RowVersion" : null;
			var resultType = entity.InclRowVersion ? "<byte[]>" : null;

			// Interface
			var signature = $"Task{returnType} Update{className}({className} {varName})";
			interfaceOutput.Add(signature);

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({varName});");
			output.AddLine(tc + 1, $"{returnStr}await PutAsync{returnType}(\"api/v1/{className}/Update{className}\", {varName});");
			output.AddLine(tc, "}");
		}

		private void GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method, ServiceModel service, List<string> output, List<string> interfaceOutput)
		{
			var tc = 0;

			var returnType = entity.InclRowVersion ? "<byte[]>" : null;
			var returnStr = entity.InclRowVersion ? "return " : null;
			var signature = $"Task{returnType} {method.Name}({method.Name}Req request)";

			// Interface
			interfaceOutput.Add(signature);

			// Method
			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"{returnStr}await PatchAsync{returnType}($\"api/{service.Version}/{entity.Name}/{method.Name}\", request);");
			output.AddLine(tc, "}");
		}

		private void GenerateReadMethod(EntityModel entity, ReadMethodModel method, List<string> output, List<string> interfaceOutput)
		{
			var tc = 0;
			var className = entity.Name;
			var varName = className.ToCamelCase();

			output.AddLine();

			// Attributes
			if (method.Attributes.Any())
				foreach (var attr in method.Attributes)
					output.AddLine(tc, $"[{attr}]");

			// Build signature
			var sbSigArgs = new StringBuilder();
			var sbRoute = new StringBuilder();
			var sbQry = new StringBuilder();
			if (method.UseRequest)
			{
				sbSigArgs.Append($"{method.Name}Req request");
			}
			else
			{
				// Required params first, in url segments, and then optional as query params
				string nullStr = null;
				foreach (var reqFilterProp in method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal).ToList())
				{
					// Args
					if (sbSigArgs.Length > 0)
						sbSigArgs.Append(", ");
					sbSigArgs.Append($"{reqFilterProp.PropertyModel.CSType}{nullStr} {reqFilterProp.PropertyModel.ArgName}");
					// Query
					sbRoute.Append($"/{{{reqFilterProp.PropertyModel.ArgName}}}");
				}

				foreach (var optFilterProp in method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal).ToList())
				{
					// Args
					if (sbSigArgs.Length > 0)
						sbSigArgs.Append(", ");
					sbSigArgs.Append($"{optFilterProp.PropertyModel.CSType}? {optFilterProp.PropertyModel.ArgName} = null");

					if (sbQry.Length == 0)
						sbQry.Append("?");
					else
						sbQry.Append("&");
					sbQry.Append($"{optFilterProp.PropertyModel.ArgName}={{{optFilterProp.PropertyModel.ArgName}}}");
				}
			}

			string returnType = method.InclPaging ? $"ListPage<{entity.Name}>" : method.IsList ? $"List<{entity.Name}>" : entity.Name;
			var signature = $"Task<{returnType}> {method.Name}({sbSigArgs})";

			// Interface
			interfaceOutput.Add(signature);

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"return await GetAsync<{returnType}>($\"api/v1/{className}/{method.Name}{sbRoute}{sbQry}\");");
			output.AddLine(tc, "}");
		}
	}
}