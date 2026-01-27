using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
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
			output.AddLine(tc + 2, $"using var db = _dbContextFactory.CreateDbContext();");
			output.AddLine(tc + 2, $"db.Add({varName});");
			output.AddLine(tc + 2, "await db.SaveChangesAsync();");
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
			output.AddLine(tc + 1, $"using var db = _dbContextFactory.CreateDbContext();");
			output.AddLine();
			output.AddLine(tc + 1, $"var result = await db.{className}.Where(a => a.Id == id).ExecuteDeleteAsync();");
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
			output.AddLine(tc + 1, $"using var db = _dbContextFactory.CreateDbContext();");
			output.AddLine();
			output.AddLine(tc + 1, "try {");
			output.AddLine(tc + 2, $"db.Attach({varName});");
			output.AddLine(tc + 2, $"db.Entry({varName}).State = EntityState.Modified;");
			output.AddLine(tc + 2, $"await db.SaveChangesAsync();");
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
			var updateProps = new List<(UpdatePropertyModel UpdProp, PropertyModel PropModel)>();
			foreach (var updProp in method.UpdateProperties.Where(p => !p.IsOptional))
			{
				var propModel = updProp.PropertyModel ?? entity.Properties.FirstOrDefault(p => p.Name == updProp.Name);
				updateProps.Add((updProp, propModel));
			}
			foreach (var updProp in method.UpdateProperties.Where(p => p.IsOptional))
			{
				var propModel = updProp.PropertyModel ?? entity.Properties.FirstOrDefault(p => p.Name == updProp.Name);
				updateProps.Add((updProp, propModel));
			}

			// Build signature
			var sbSigArgs = new StringBuilder();
			sbSigArgs.Append("Guid id");
			if (entity.InclRowVersion)
				sbSigArgs.Append(", byte[] rowVersion");
			var argNames = new List<string>();
			foreach (var (updProp, propModel) in updateProps)
			{
				var argName = CodeGenUtils.ToCamelCase(propModel.Name);
				sbSigArgs.Append($", {propModel.CSType} {argName}");
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
			foreach (var (updProp, propModel) in updateProps.Where(p => !p.UpdProp.IsOptional && p.PropModel.DataType == DataTypes.String))
				output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({propModel.ArgName});");
			output.AddLine();
			output.AddLine(tc + 1, "try {");
			output.AddLine(tc + 2, $"var {varName} = new {entity.Name} {{");
			output.AddLine(tc + 3, $"Id = id,");
			if (entity.InclRowVersion)
				output.AddLine(tc + 3, $"RowVersion = rowVersion,");
			foreach (var (updProp, propModel) in updateProps)
				output.AddLine(tc + 3, $"{propModel.Name} = {propModel.ArgName},");
			output.AddLine(tc + 2, "};");
			output.AddLine();

			output.AddLine(tc + 2, "using var db = _dbContextFactory.CreateDbContext();");
			output.AddLine(tc + 2, $"db.Attach({varName});");
			foreach (var (updProp, propModel) in updateProps)
				output.AddLine(tc + 2, $"db.Entry({varName}).Property(u => u.{propModel.Name}).IsModified = true;");
			output.AddLine();
			output.AddLine(tc + 2, "await db.SaveChangesAsync();");
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
				sbSigArgs.Append($"{filterProp.ResolvedPropertyModel.CSType} {filterProp.ResolvedPropertyModel.ArgName}");
			}

			// Optional properties next
			foreach (var filterProp in method.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional))
			{
				if (c++ > 0)
					sbSigArgs.Append(", ");
				//var nullChar =  filterProp.PropertyModel.PrimitiveType?.Id != PrimitiveType.String.Id ? "?" : string.Empty;
				var nullChar = "?";
				sbSigArgs.Append($"{filterProp.ResolvedPropertyModel.DataType}{nullChar} {filterProp.PropertyModel.ArgName} = null");
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
			interfaceOutput.Add(signature);

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var dbQuery = _dbContextFactory.CreateDbContext().{entity.Name}.AsQueryable();");
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
			if (filterProp.ResolvedPropertyModel.DataType == DataTypes.String)
			{
				output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace({filterProp.ResolvedPropertyModel.ArgName}))");
				output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.ResolvedPropertyModel.Name}, {filterProp.ResolvedPropertyModel.ArgName}));");
			}
			else
			{
				var indent = tc + 1;
				if (filterProp.IsOptional)
				{
					output.AddLine(indent, $"if ({filterProp.ResolvedPropertyModel.ArgName}.HasValue)");
					indent++;
				}
				output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.ResolvedPropertyModel.Name} == {filterProp.ResolvedPropertyModel.ArgName});");
			}
		}

		private void GenerateInternalFilter(FilterPropertyModel filterProp, EntityModel entity, List<string> output, int tc)
		{
			var indent = tc;

			if (filterProp.ResolvedPropertyModel.DataType == DataTypes.String)
			{
				output.AddLine(indent, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.ResolvedPropertyModel.Name}, \"{filterProp.InternalValue}\"));");

			}
			else
			{
				if (filterProp.IsOptional)
				{
					output.AddLine(indent, $"if ({filterProp.ResolvedPropertyModel.ArgName}.HasValue)");
					indent++;
				}

				if (!PackageUtils.IsPrimitiveDataType(filterProp.ResolvedPropertyModel.DataType))
				{
					output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.ResolvedPropertyModel.Name} == {filterProp.ResolvedPropertyModel.DataType}.{filterProp.InternalValue});");
				}
				else
				{
					output.AddLine(indent, $"dbQuery = dbQuery.Where(x => x.{filterProp.ResolvedPropertyModel.Name} == {filterProp.InternalValue});");
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
			interfaceOutput.Add(signature);

			// Method
			output.AddLine(tc, $"public async {signature}");
			output.AddLine(tc, "{");
			output.AddLine(tc + 1, $"var dbQuery = _dbContextFactory.CreateDbContext().{entity.Name}.AsQueryable();");
			output.AddLine(tc + 1, $"var result = new EntityList<{entity.Name}>();");
			output.AddLine();

			output.AddLine(tc + 1, $"// Filters");
			foreach (var filterProp in queryMethod.FilterProperties)
			{
				if (PackageUtils.IsString(filterProp.ResolvedPropertyModel.DataType))
				{
					output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace(query.{filterProp.ResolvedPropertyModel.Name}))");
					output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => EF.Functions.Like(x.{filterProp.ResolvedPropertyModel.Name}, query.{filterProp.ResolvedPropertyModel.Name}));");
				}
				else if (filterProp.ResolvedPropertyModel.DataType == DataTypes.Int32 || filterProp.ResolvedPropertyModel.DataType == DataTypes.Boolean || filterProp.ResolvedPropertyModel.DataType == DataTypes.Guid)
				{
					output.AddLine(tc + 1, $"if (query.{filterProp.ResolvedPropertyModel.Name}.HasValue)");
					output.AddLine(tc + 2, $"dbQuery = dbQuery.Where(x => x.{filterProp.ResolvedPropertyModel.Name} == query.{filterProp.ResolvedPropertyModel.Name});");
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