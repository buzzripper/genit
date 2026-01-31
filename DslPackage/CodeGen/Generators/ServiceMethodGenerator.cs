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
			var signature = $"Task<Guid> Create{className}({className} {varName})";
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
			output.AddLine(tc + 2, $"return {varName}.Id;");
			output.AddLine();
			output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
			output.AddLine(tc + 2, "throw new ConcurrencyApiException(\"The item was modified or deleted by another user.\");");
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
			var signature = $"Task<bool> Delete{className}(Guid id)";
			interfaceOutput.Add($"{signature};");

			output.AddLine();
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var result = await _db.{className}.Where(a => a.Id == id).ExecuteDeleteAsync();");
			output.AddLine(tc + 1, $"return result == 1;");
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
			var returnType = entity.InclRowVersion ? "Task<byte[]>" : "Task";
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
				output.AddLine(tc + 2, $"return {varName}.RowVersion;");
				output.AddLine();
			}
			output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
			output.AddLine(tc + 2, $"throw new ConcurrencyApiException(\"The item was modified or deleted by another user.\");");
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

			var returnType = entity.InclRowVersion ? "Task<byte[]>" : "Task";
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
			{
				output.AddLine(tc + 2, $"return {varName}.RowVersion;");
				output.AddLine();
			}
			output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
			output.AddLine(tc + 2, "throw new ConcurrencyApiException(\"The item was modified or deleted by another user.\");");
			output.AddLine(tc + 1, "}");
			output.AddLine(tc, "}");
		}

		internal void GenerateReadMethod(EntityModel entity, ReadMethodModel method, List<string> output, List<string> interfaceOutput)
		{
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
			string returnType = method.IsList ? $"Task<List<{entity.Name}>>" : $"Task<{entity.Name}>";

			var sbSigArgs = new StringBuilder();
			var c = 0;

			// Required properties first
			foreach (var filterProp in method.FilterProperties.Where(fp => !fp.IsInternal && !fp.IsOptional))
			{
				if (c++ > 0)
					sbSigArgs.Append(", ");
				sbSigArgs.Append($"{filterProp.PropertyModel.CSType} {filterProp.PropertyModel.ArgName}");
			}

			// Optional properties next
			foreach (var filterProp in method.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional))
			{
				if (c++ > 0)
					sbSigArgs.Append(", ");
				//var nullChar =  filterProp.PropertyModel.PrimitiveType?.Id != PrimitiveType.String.Id ? "?" : string.Empty;
				var nullChar = "?";
				sbSigArgs.Append($"{filterProp.PropertyModel.DataType}{nullChar} {filterProp.PropertyModel.ArgName} = null");
			}

			// Finally paging
			if (method.InclPaging)
			{
				if (sbSigArgs.Length > 0)
					sbSigArgs.Append(", ");
				sbSigArgs.Append("int pageSize = 0, int pageOffset = 0");
			}

			var signature = $"{returnType} {method.Name}({sbSigArgs.ToString()})";

			// Interface
			interfaceOutput.Add($"{signature};");

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var dbQuery = _db.{entity.Name}.AsQueryable();");
			output.AddLine();

			// Include any nav properties
			foreach (var inclNavProp in method.InclNavPropertiesList)
				output.AddLine(tc + 1, $"dbQuery = dbQuery.Include(x => x.{inclNavProp});");

			// Filters
			if (method.FilterProperties.Any())
			{
				// Required
				foreach (var filterProp in method.FilterProperties.Where(fp => !fp.IsInternal && !fp.IsOptional))
					GenerateFilter(filterProp, entity, output, 0);

				// Optional
				var optFilterProps = method.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional);
				if (optFilterProps.Any())
				{
					output.AddLine(2, "// Optional");
					foreach (var filterProp in optFilterProps)
						GenerateFilter(filterProp, entity, output, 0);
				}

				// Internal
				var intFilterProps = method.FilterProperties.Where(fp => fp.IsInternal);
				if (intFilterProps.Any())
				{
					output.AddLine();
					output.AddLine(2, "// Internal");
					foreach (var filterProp in intFilterProps)
						GenerateInternalFilter(filterProp, entity, output, 2);
				}
			}

			if (method.InclPaging)
			{
				output.AddLine(tc + 1, $"if (pageSize > 0)");
				output.AddLine(tc + 2, $"dbQuery = dbQuery.Skip(pageOffset * pageSize).Take(pageSize);");
			}

			output.AddLine();
			if (method.IsList)
			{
				output.AddLine(tc + 1, $"return await dbQuery.AsNoTracking().ToListAsync();");
			}
			else
			{
				output.AddLine(tc + 1, $"return await dbQuery.AsNoTracking().FirstOrDefaultAsync();");
			}

			output.AddLine(tc, "}");
		}

		private void GenerateFilter(FilterPropertyModel filterProp, EntityModel entity, List<string> output, int tc)
		{
			if (filterProp.PropertyModel.DataType == DataTypes.String)
			{
				output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace({filterProp.PropertyModel.ArgName}))");
				output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.PropertyModel.Name}, {filterProp.PropertyModel.ArgName}));");
			}
			else
			{
				var indent = tc + 1;
				if (filterProp.IsOptional)
				{
					output.AddLine(indent, $"if ({filterProp.PropertyModel.ArgName}.HasValue)");
					indent++;
				}
				output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {filterProp.PropertyModel.ArgName});");
			}
		}

		private void GenerateInternalFilter(FilterPropertyModel filterProp, EntityModel entity, List<string> output, int tc)
		{
			var indent = tc;

			if (filterProp.PropertyModel.DataType == DataTypes.String)
			{
				output.AddLine(indent, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.PropertyModel.Name}, \"{filterProp.InternalValue}\"));");

			}
			else
			{
				if (filterProp.IsOptional)
				{
					output.AddLine(indent, $"if ({filterProp.PropertyModel.ArgName}.HasValue)");
					indent++;
				}

				if (!PackageUtils.IsPrimitiveDataType(filterProp.PropertyModel.DataType))
				{
					output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {filterProp.PropertyModel.DataType}.{filterProp.InternalValue});");
				}
				else
				{
					output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == {filterProp.InternalValue});");
				}
			}
		}

		internal void GenerateQueryMethod(EntityModel entity, ReadMethodModel queryMethod, List<string> output, List<string> interfaceOutput)
		{
			var tc = 1;
			output.AddLine();
			var queryClassName = $"{queryMethod.Name}Query";

			// Attributes
			if (queryMethod.Attributes.Any())
				foreach (var attr in queryMethod.Attributes)
					output.AddLine(tc, $"[{attr}]");

			// Interface
			var signature = $"Task<EntityList<{entity.Name}>>{queryMethod.Name}({queryClassName} query)";
			interfaceOutput.Add($"{signature};");

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var dbQuery = _db.{entity.Name}.AsQueryable();");
			output.AddLine(tc + 1, $"var result = new EntityList<{entity.Name}>();");
			output.AddLine();

			output.AddLine(tc + 1, $"// Filters");
			foreach (var filterProp in queryMethod.FilterProperties)
			{
				if (PackageUtils.IsString(filterProp.PropertyModel.DataType))
				{
					output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace(query.{filterProp.PropertyModel.Name}))");
					output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.PropertyModel.Name}, query.{filterProp.PropertyModel.Name}));");
				}
				else if (filterProp.PropertyModel.DataType == DataTypes.Int32 || filterProp.PropertyModel.DataType == DataTypes.Boolean || filterProp.PropertyModel.DataType == DataTypes.Guid)
				{
					output.AddLine(tc + 1, $"if (query.{filterProp.PropertyModel.Name}.HasValue)");
					output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => x.{filterProp.PropertyModel.Name} == query.{filterProp.PropertyModel.Name});");
				}
			}

			if (queryMethod.InclPaging)
			{
				output.AddLine();
				output.AddLine(tc + 1, "// Paging");
				output.AddLine(tc + 1, "if (query.RecalcRowCount || query.GetRowCountOnly)");
				output.AddLine(tc + 2, "result.TotalRowCount = dbQuery.Count();");
				output.AddLine(tc + 1, "if (query.GetRowCountOnly)");
				output.AddLine(tc + 2, "return result;");
				output.AddLine(tc + 1, "if (query.PageSize > 0)");
				output.AddLine(tc + 2, "dbQuery = dbQuery.Skip(query.PageOffset).Take(query.PageSize);");
			}

			if (queryMethod.InclSorting)
			{
				output.AddLine();
				output.AddLine(tc + 1, "// Sorting");
				output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace(query.SortBy))");
				output.AddLine(tc + 2, $"this.AddSorting(ref dbQuery, query);");
			}

			output.AddLine();
			output.AddLine(tc + 1, "result.Data = await dbQuery.AsNoTracking().ToListAsync();");
			output.AddLine();
			output.AddLine(tc + 1, "return result;");
			output.AddLine(tc, "}");
		}

		internal void GenerateSortingMethod(EntityModel entity, List<string> output)
		{
			var tc = 1;
			output.AddLine();

			// Method
			output.AddLine(tc, $"private void AddSorting(ref IQueryable<{entity.Name}> dbQuery, ISortingQuery sortingQuery)");
			output.AddLine(tc, "{");

			var c = 0;
			foreach (var prop in entity.Properties)
			{
				if (c++ > 0)
					output.AddLine();

				output.AddLine(tc + 1, $"if (string.Equals(sortingQuery.SortBy, {entity.Name}.PropNames.{prop.Name}, StringComparison.OrdinalIgnoreCase))");
				output.AddLine(tc + 2, "if (sortingQuery.SortDesc)");
				output.AddLine(tc + 3, $"dbQuery.OrderByDescending(x => x.{prop.Name});");
				output.AddLine(tc + 2, "else");
				output.AddLine(tc + 3, $"dbQuery.OrderBy(x => x.{prop.Name});");
			}
			output.AddLine(tc, "}");
		}
	}

}