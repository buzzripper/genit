using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class ServiceMethodGenerator
	{
		internal void GenerateCreateMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
		{
			var tc = 1;
			var className = entity.Name;
			var varName = CodeGenUtils.ToCamelCase(className);

			output.AddLine();
			output.AddLine(tc, "#region Create");

			// Interface
			var signature = $"Task<Result<Guid>> Create{className}({className} {varName})";
			interfaceOutput.Add($"{signature};");

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({varName});");
			output.AddLine();
			output.AddLine(tc + 1, "try {");
			output.AddLine(tc + 2, $"_db.Add({varName});");
			output.AddLine(tc + 2, "await _db.SaveChangesAsync();");
			output.AddLine();
			output.AddLine(tc + 2, $"return Result<Guid>.Ok({varName}.Id);");
			output.AddLine();
			output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
			output.AddLine(tc + 2, "return Result<Guid>.Conflict(\"The item was modified or deleted by another user.\");");
			output.AddLine(tc + 1, "}");
			output.AddLine(tc, "}");

			output.AddLine();
			output.AddLine(tc, "#endregion");
		}

		internal void GenerateDeleteMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
		{
			var tc = 1;
			var className = entity.Name;
			var varName = CodeGenUtils.ToCamelCase(className);

			output.AddLine();
			output.AddLine(tc, "#region Delete");

			// Interface
			var signature = $"Task<Result> Delete{className}(Guid id)";
			interfaceOutput.Add($"{signature};");

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var rowsAffected = await _db.{className}.Where(a => a.Id == id).ExecuteDeleteAsync();");
			output.AddLine();
			output.AddLine(tc + 1, $"if (rowsAffected == 0)");
			output.AddLine(tc + 2, $"return Result.NotFound($\"{className} {{id}} not found\");");
			output.AddLine();
			output.AddLine(tc + 1, $"return Result.Ok();");
			output.AddLine(tc, "}");

			output.AddLine();
			output.AddLine(tc, "#endregion");
		}

		internal void GenerateFullUpdateMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
		{
			var tc = 1;
			var className = entity.Name;
			var varName = CodeGenUtils.ToCamelCase(className);

			// Interface
			var returnType = entity.InclRowVersion ? "Task<Result<byte[]>>" : "Task<Result>";
			var signature = $"{returnType} Update{className}({className} {varName})";
			interfaceOutput.Add($"{signature};");

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({varName});");
			output.AddLine();
			output.AddLine(tc + 1, "try {");
			output.AddLine(tc + 2, $"_db.Attach({varName});");
			output.AddLine(tc + 2, $"_db.Entry({varName}).State = EntityState.Modified;");
			output.AddLine(tc + 2, $"await _db.SaveChangesAsync();");
			output.AddLine();
			if (entity.InclRowVersion)
			{
				output.AddLine(tc + 2, $"return Result.Ok({varName}.RowVersion);");
				output.AddLine();
			}
			else
			{
				output.AddLine(tc + 1, "return Result.Ok();");
			}
			output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
			output.AddLine(tc + 2, $"return Result.Conflict(\"The item was modified or deleted by another user.\");");
			output.AddLine(tc + 1, "}");
			output.AddLine(tc, "}");
		}

		internal void GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method, List<string> output, List<string> interfaceOutput)
		{
			var tc = 1;
			var varName = CodeGenUtils.ToCamelCase(entity.Name);
			output.AddLine();

			// Build the list of update properties with resolved PropertyModel, required first then optional
			var updateProps = new List<UpdatePropertyModel>();
			updateProps.AddRange(method.UpdateProperties.Where(p => !p.IsOptional));
			updateProps.AddRange(method.UpdateProperties.Where(p => p.IsOptional));

			// Build signature
			var sbSigArgs = new StringBuilder();
			sbSigArgs.Append("Guid id");
			if (entity.InclRowVersion)
				sbSigArgs.Append(", byte[] rowVersion");
			var argNames = new List<string>();
			foreach (var updProp in updateProps)
			{
				var argName = CodeGenUtils.ToCamelCase(updProp.PropertyModel.Name);
				sbSigArgs.Append($", {updProp.PropertyModel.CSType} {argName}");
			}

			var returnType = entity.InclRowVersion ? "Task<Result<byte[]>>" : "Task<Result>";
			var signature = $"{returnType} {method.Name}({sbSigArgs.ToString()})";

			// Interface
			interfaceOutput.Add($"{signature};");

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			if (entity.InclRowVersion)
				output.AddLine(tc + 1, "ArgumentNullException.ThrowIfNull(rowVersion);");
			foreach (var updProp in updateProps.Where(p => !p.IsOptional && p.PropertyModel.DataType == DataTypes.String))
				output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({updProp.PropertyModel.ArgName});");
			output.AddLine();
			output.AddLine(tc + 1, "try {");
			output.AddLine(tc + 2, $"var {varName} = new {entity.Name} {{");
			output.AddLine(tc + 3, $"Id = id,");
			if (entity.InclRowVersion)
				output.AddLine(tc + 3, $"RowVersion = rowVersion,");
			foreach (var updProp in updateProps)
				output.AddLine(tc + 3, $"{updProp.PropertyModel.Name} = {updProp.PropertyModel.ArgName},");
			output.AddLine(tc + 2, "};");
			output.AddLine();

			output.AddLine(tc + 2, $"_db.Attach({varName});");
			foreach (var updProp in updateProps)
				output.AddLine(tc + 2, $"_db.Entry({varName}).Property(u => u.{updProp.PropertyModel.Name}).IsModified = true;");
			output.AddLine();
			output.AddLine(tc + 2, "await _db.SaveChangesAsync();");
			output.AddLine();
			if (entity.InclRowVersion)
				output.AddLine(tc + 2, $"return Result.Ok({varName}.RowVersion);");
			else
				output.AddLine(tc + 2, $"return Result.Ok();");
			output.AddLine();
			output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
			output.AddLine(tc + 2, "return Result.Conflict(\"The item was modified or deleted by another user.\");");
			output.AddLine(tc + 1, "}");
			output.AddLine(tc, "}");
		}

		internal void GenerateReadMethod(EntityModel entity, ReadMethodModel method, List<string> output, List<string> interfaceOutput)
		{
			var entityVarName = entity.Name.ToCamelCase();
			var reqClassName = $"{method.Name}Req";
			var reqVarName = "request";
			var tc = 0;

			output.AddLine();

			var invalidFilterProp = method.FilterProperties.FirstOrDefault(fp => fp.PropertyModel == null);
			if (invalidFilterProp != null)
				throw new InvalidOperationException($"Read method '{method.Name}' has a filter property with no linked PropertyModel. Fix the DSL model (select a Property for the filter) and try again.");

			// Attributes
			if (method.Attributes.Any())
				foreach (var attr in method.Attributes)
					output.AddLine(tc, $"[{attr}]");

			// Build signature
			string returnType = method.IsSingle ? $"Result<{entity.Name}>" : method.InclPaging ? $"Result<EntityList<{entity.Name}>>" : $"Result<List<{entity.Name}>>";

			var sbSigArgs = new StringBuilder();
			if (method.UseRequest)
			{
				sbSigArgs.Append($"{reqClassName} {reqVarName}");
			}
			else
			{
				// Required params first, in url segments, and then optional as query params
				var filterProps = method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal).ToList();
				filterProps.AddRange(method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal));
				foreach (var fp in filterProps)
				{
					var nullStr = fp.IsOptional ? "?" : null;
					if (sbSigArgs.Length > 0)
						sbSigArgs.Append(", ");
					sbSigArgs.Append($"{fp.PropertyModel.CSType}{nullStr} {fp.PropertyModel.ArgName}");
				}
			}

			var signature = $"Task<{returnType}> {method.Name}({sbSigArgs.ToString()})";

			// Interface
			interfaceOutput.Add($"{signature};");

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var dbQuery = _db.{entity.Name}.AsNoTracking();");

			// Include any nav properties
			foreach (var inclNavProp in method.InclNavPropertiesList)
				output.AddLine(tc + 1, $"dbQuery = dbQuery.Include(x => x.{inclNavProp});");

			// Filters
			if (method.FilterProperties.Any())
			{
				// Required
				foreach (var filterProp in method.FilterProperties.Where(fp => !fp.IsInternal && !fp.IsOptional))
				{
					var varName = method.UseRequest ? $"{reqVarName}.{filterProp.PropertyModel.Name}" : filterProp.PropertyModel.ArgName;
					GenerateFilter(filterProp, entity, varName, output);
				}

				// Optional
				var optFilterProps = method.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional);
				if (optFilterProps.Any())
				{
					output.AddLine(2, "// Optional");
					foreach (var filterProp in optFilterProps)
					{
						var varName = method.UseRequest ? $"{reqVarName}.{filterProp.PropertyModel.Name}" : filterProp.PropertyModel.ArgName;
						GenerateFilter(filterProp, entity, varName, output);
					}
				}

				// Internal
				var intFilterProps = method.FilterProperties.Where(fp => fp.IsInternal);
				if (intFilterProps.Any())
				{
					output.AddLine();
					output.AddLine(1, "// Internal");
					foreach (var filterProp in intFilterProps)
					{
						var varName = method.UseRequest ? $"{reqVarName}.{filterProp.PropertyModel.Name}" : filterProp.PropertyModel.ArgName;
						GenerateInternalFilter(filterProp, entity, varName, output);
					}
				}
			}

			if (method.InclSorting)
			{
				output.AddLine();
				output.AddLine(tc + 1, "// Sorting");
				output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace({reqVarName}.SortBy))");
				output.AddLine(tc + 2, $"this.AddSorting(ref dbQuery, {reqVarName});");
			}

			if (method.InclPaging)
			{
				output.AddLine();
				output.AddLine(tc + 1, $"var entityList = new EntityList<{entity.Name}>();");
				if (method.FilterProperties.Any())
					output.AddLine(tc + 1, "// Stable ordering for paging");
				foreach (var filterProp in method.FilterProperties)
					output.AddLine(tc + 1, $"dbQuery = dbQuery.OrderBy(x => x.{filterProp.PropertyModel.Name}).ThenBy(x => x.Id);");

				output.AddLine();
				output.AddLine(tc + 1, $"if ({reqVarName}.PageSize > 0)");
				output.AddLine(tc + 2, $"dbQuery = dbQuery.Skip({reqVarName}.PageOffset * {reqVarName}.PageSize).Take({reqVarName}.PageSize);");
				output.AddLine();
				output.AddLine(tc + 1, "// Count (only when requested)");
				output.AddLine(tc + 1, $"if ({reqVarName}.RecalcRowCount || {reqVarName}.GetRowCountOnly)");
				output.AddLine(tc + 1, "{");
				output.AddLine(tc + 2, "entityList.TotalRowCount = await dbQuery.CountAsync();");
				output.AddLine();
				output.AddLine(tc + 2, "if (request.GetRowCountOnly)");
				output.AddLine(tc + 3, $"return {returnType}.Ok(entityList);");
				output.AddLine(tc + 1, "}");
			}

			if (method.IsList)
			{
				output.AddLine();
				output.AddLine(tc + 1, $"var data = await dbQuery.ToListAsync();");
				output.AddLine();
				if (method.InclPaging)
					output.AddLine(tc + 1, $"return {returnType}.Ok(data.ToEntityList<{entity.Name}>());");
				else
					output.AddLine(tc + 1, $"return {returnType}.Ok(data);");
			}
			else
			{
				output.AddLine();
				output.AddLine(tc + 1, $"var {entityVarName} = await dbQuery.FirstOrDefaultAsync();");
				output.AddLine();
				output.AddLine(tc + 1, $"if ({entityVarName} is null)");
				output.AddLine(tc + 2, $"return {returnType}.NotFound($\"{entity.Name} not found\");");
				output.AddLine();
				output.AddLine(tc + 1, $"return {returnType}.Ok({entityVarName});");
			}

			output.AddLine(tc, "}");
		}

		private void GenerateFilter(FilterPropertyModel filterProp, EntityModel entity, string varName, List<string> output)
		{
			output.AddLine();
			if (filterProp.PropertyModel.DataType == DataTypes.String)
			{
				output.AddLine(1, $"if (!string.IsNullOrWhiteSpace({varName}))");
				if (filterProp.IsPartialMatch)
					output.AddLine(2, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.PropertyModel.Name}, $\"%{{{varName}}}%\"));");
				else
					output.AddLine(2, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {varName});");
			}
			else
			{
				var indent = 1;
				if (filterProp.IsOptional)
				{
					output.AddLine(indent, $"if ({varName}.HasValue)");
					indent++;
				}
				output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {varName});");
			}
		}

		private void GenerateInternalFilter(FilterPropertyModel filterProp, EntityModel entity, string varName, List<string> output)
		{
			var indent = 1;

			if (filterProp.PropertyModel.DataType == DataTypes.String)
			{
				output.AddLine(indent, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.PropertyModel.Name}, $\"%{filterProp.InternalValue}%\"));");
			}
			else
			{
				if (filterProp.IsOptional)
				{
					output.AddLine(indent, $"if ({varName}.HasValue)");
					indent++;
				}

				if (!DataTypes.IsPrimitive(filterProp.PropertyModel.DataType))
				{
					output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {filterProp.PropertyModel.DataType}.{filterProp.InternalValue});");
				}
				else
				{
					output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {filterProp.InternalValue});");
				}
			}
		}

		//internal void GenerateSearchMethod(EntityModel entity, ReadMethodModel searchMethod, List<string> output, List<string> interfaceOutput)
		//{
		//	var tc = 1;
		//	output.AddLine();
		//	var reqClassName = $"{searchMethod.Name}Req";

		//	// Attributes
		//	if (searchMethod.Attributes.Any())
		//		foreach (var attr in searchMethod.Attributes)
		//			output.AddLine(tc, $"[{attr}]");

		//	// Interface
		//	string returnType = !searchMethod.IsList ? $"Result<{entity.Name}>" : searchMethod.InclPaging ? $"Result<EntityList<{entity.Name}>>" : $"Result<List<{entity.Name}>>";
		//	var signature = $"Task<{returnType}>{searchMethod.Name}({reqClassName} request)";
		//	interfaceOutput.Add($"{signature};");

		//	// Method
		//	output.AddLine(tc, $"public async {signature}");
		//	output.AddLine(tc, "{");
		//	output.AddLine(tc + 1, $"IQueryable<{entity.Name}> dbQuery = _db.{entity.Name}.AsNoTracking();");
		//	if (searchMethod.FilterProperties.Any())
		//		output.AddLine(tc + 1, $"// Filters");
		//	foreach (var filterProp in searchMethod.FilterProperties)
		//	{
		//		if (DataTypes.IsString(filterProp.PropertyModel.DataType) && filterProp.IsPartialMatch)
		//		{
		//			output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace(request.{filterProp.PropertyModel.Name}))");
		//			output.AddLine(tc + 1, "{");
		//			output.AddLine(tc + 2, $"var pattern = $\"%{{request.{filterProp.PropertyModel.Name}}}%\";");
		//			output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.PropertyModel.Name}, pattern));");
		//			output.AddLine(tc + 1, "}");
		//		}
		//		else if (filterProp.PropertyModel.DataType == DataTypes.Int32 || filterProp.PropertyModel.DataType == DataTypes.Boolean || filterProp.PropertyModel.DataType == DataTypes.Guid)
		//		{
		//			output.AddLine(tc + 1, $"if (request.{filterProp.PropertyModel.Name}.HasValue)");
		//			output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == request.{filterProp.PropertyModel.Name});");
		//		}
		//	}

		//	if (searchMethod.InclSorting)
		//	{
		//		output.AddLine();
		//		output.AddLine(tc + 1, "// Sorting");
		//		output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace(request.SortBy))");
		//		output.AddLine(tc + 2, $"this.AddSorting(ref dbQuery, request);");
		//	}
		//	if (searchMethod.InclPaging)
		//	{
		//		output.AddLine();
		//		output.AddLine(tc + 1, $"var entityList = new EntityList<{entity.Name}>();");
		//		if (searchMethod.FilterProperties.Any())
		//			output.AddLine(tc + 1, "// Stable ordering for paging");
		//		foreach (var filterProp in searchMethod.FilterProperties)
		//			output.AddLine(tc + 1, $"dbQuery = dbQuery.OrderBy(x => x.{filterProp.PropertyModel.Name}).ThenBy(x => x.Id);");

		//		output.AddLine();
		//		output.AddLine(tc + 1, "// Count (only when requested)");
		//		output.AddLine(tc + 1, "if (request.RecalcRowCount || request.GetRowCountOnly)");
		//		output.AddLine(tc + 1, "{");
		//		output.AddLine(tc + 2, "entityList.TotalRowCount = await dbQuery.CountAsync();");
		//		output.AddLine();
		//		output.AddLine(tc + 2, "if (request.GetRowCountOnly)");
		//		output.AddLine(tc + 3, $"return {returnType}.Ok(entityList);");
		//		output.AddLine(tc + 1, "}");
		//	}

		//	if (searchMethod.InclPaging)
		//	{
		//		output.AddLine();
		//		output.AddLine(tc + 1, "// Paging");
		//		output.AddLine(tc + 1, "if (request.PageSize > 0)");
		//		output.AddLine(tc + 2, "dbQuery = dbQuery.Skip(request.PageOffset * request.PageSize).Take(request.PageSize);");
		//	}

		//	output.AddLine();
		//	output.AddLine(tc + 1, "// Data");

		//	if (!searchMethod.IsList)
		//	{
		//		output.AddLine(tc + 1, $"var {entity.Name.ToCamelCase()}= await dbQuery.FirstOrDefaultAsync();");
		//		output.AddLine();
		//		output.AddLine(tc + 1, $"return {returnType}.Ok({entity.Name.ToCamelCase()});");
		//	}
		//	else if (searchMethod.InclPaging)
		//	{
		//		output.AddLine(tc + 1, "entityList.Items = await dbQuery.ToListAsync();");
		//		output.AddLine();
		//		output.AddLine(tc + 1, $"return {returnType}.Ok(entityList);");
		//	}
		//	else
		//	{
		//		output.AddLine(tc + 1, "var items = await dbQuery.ToListAsync();");
		//		output.AddLine();
		//		output.AddLine(tc + 1, $"return {returnType}.Ok(items);");
		//	}
		//	output.AddLine(tc, "}");
		//}

		internal void GenerateSortingMethod(EntityModel entity, List<string> output)
		{
			var tc = 1;
			output.AddLine();

			// Method
			output.AddLine(tc, $"private void AddSorting(ref IQueryable<{entity.Name}> dbQuery, ISortingRequest sortingRequest)");
			output.AddLine(tc, "{");

			var c = 0;
			foreach (var prop in entity.Properties)
			{
				if (c++ > 0)
					output.AddLine();

				output.AddLine(tc + 1, $"if (string.Equals(sortingRequest.SortBy, {entity.Name}.PropNames.{prop.Name}, StringComparison.OrdinalIgnoreCase))");
				output.AddLine(tc + 2, "if (sortingRequest.SortDesc)");
				output.AddLine(tc + 3, $"dbQuery.OrderByDescending(x => x.{prop.Name});");
				output.AddLine(tc + 2, "else");
				output.AddLine(tc + 3, $"dbQuery.OrderBy(x => x.{prop.Name});");
			}
			output.AddLine(tc, "}");
		}
	}

}