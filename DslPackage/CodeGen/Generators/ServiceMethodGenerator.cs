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
            var returnType = entity.InclRowVersion ? "Task<byte[]>" : "Task";
            var signature = $"{returnType} Create{className}({className} {varName})";

            output.AddLine();
            output.AddLine(tc, "#region Create");

            // Interface
            interfaceOutput.Add($"{signature};");

            output.AddLine();
            output.AddLine(tc, $"public async {signature}");
            output.AddLine(tc, "{");
            output.AddLine(tc + 1, $"ArgumentNullException.ThrowIfNull({varName});");
            output.AddLine();
            output.AddLine(tc + 1, "try {");
            output.AddLine(tc + 2, $"_db.Add({varName});");
            output.AddLine(tc + 2, "await _db.SaveChangesAsync();");

            if (entity.InclRowVersion)
                output.AddLine(tc + 2, $"return {varName}.RowVersion;");

            output.AddLine(tc + 1, "}");
            output.AddLine(tc + 1, "catch (DbUpdateConcurrencyException)");
            output.AddLine(tc + 1, "{");
            output.AddLine(tc + 2, "throw new ConcurrencyException(\"The item was modified or deleted by another user.\");");
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
            var signature = $"Task Delete{className}(Guid id)";
            interfaceOutput.Add($"{signature};");

            output.AddLine();
            output.AddLine(tc, $"public async {signature}");
            output.AddLine(tc, "{");
            output.AddLine(tc + 1, $"var rowsAffected = await _db.{className}.Where(a => a.Id == id).ExecuteDeleteAsync();");
            output.AddLine();
            output.AddLine(tc + 1, $"if (rowsAffected == 0)");
            output.AddLine(tc + 2, $"throw new NotFoundException($\"{className} {{id}} not found\");");
            output.AddLine(tc, "}");

            output.AddLine();
            output.AddLine(tc, "#endregion");
        }

        internal void GenerateFullUpdateMethod(EntityModel entity, List<string> output, List<string> interfaceOutput)
        {
            var tc = 1;
            var className = entity.Name;
            var varName = CodeGenUtils.ToCamelCase(className);
            var returnType = entity.InclRowVersion ? "Task<byte[]>" : "Task";
            var signature = $"{returnType} Update{className}({className} {varName})";

            // Interface
            interfaceOutput.Add($"{signature};");

            // Method body

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
                output.AddLine(tc + 2, $"return {varName}.RowVersion;");
            output.AddLine();
            output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
            output.AddLine(tc + 2, $"throw new ConcurrencyException(\"The item was modified or deleted by another user.\");");
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

            var resultType = entity.InclRowVersion ? "Task<byte[]>" : "Task";
            var signature = $"{resultType} {method.Name}({method.Name}Req request)";

            // Interface
            interfaceOutput.Add($"{signature};");

            // Method body
            output.AddLine(tc, $"public async {signature}");
            output.AddLine(tc, "{");
            output.AddLine(tc + 1, "ArgumentNullException.ThrowIfNull(request);");
            output.AddLine();
            output.AddLine(tc + 1, "try {");
            output.AddLine(tc + 2, $"var {varName} = new {entity.Name} {{");
            output.AddLine(tc + 3, $"Id = request.Id,");
            if (entity.InclRowVersion)
                output.AddLine(tc + 3, $"RowVersion = request.RowVersion,");
            foreach (var updProp in updateProps)
                output.AddLine(tc + 3, $"{updProp.PropertyModel.Name} = request.{updProp.PropertyModel.Name},");
            output.AddLine(tc + 2, "};");
            output.AddLine();

            output.AddLine(tc + 2, $"_db.Attach({varName});");
            foreach (var updProp in updateProps)
                output.AddLine(tc + 2, $"_db.Entry({varName}).Property(u => u.{updProp.PropertyModel.Name}).IsModified = true;");
            output.AddLine();
            output.AddLine(tc + 2, "await _db.SaveChangesAsync();");
            if (entity.InclRowVersion)
            {
                output.AddLine();
                output.AddLine(tc + 2, $"return {varName}.RowVersion;");
            }
            output.AddLine();
            output.AddLine(tc + 1, "} catch (DbUpdateConcurrencyException) {");
            output.AddLine(tc + 2, $"throw new ConcurrencyException(\"The item was modified or deleted by another user.\");");
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
            string returnType = method.IsSingle ? $"{method.ReturnDto.Name}" : method.InclPaging ? $"ListPage<{method.ReturnDto.Name}>" : $"IReadOnlyList<{method.ReturnDto.Name}>";

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

            if (method.InclPaging)
            {
                output.AddLine();
                output.AddLine(tc + 1, $"var listPage = new ListPage<{entity.Name}>();");

                output.AddLine();
                output.AddLine(tc + 1, "// Count (if requested)");
                output.AddLine(tc + 1, $"if ({reqVarName}.RecalcRowCount || {reqVarName}.GetRowCountOnly)");
                output.AddLine(tc + 1, "{");
                output.AddLine(tc + 2, "listPage.TotalRowCount = await dbQuery.CountAsync();");
                output.AddLine(tc + 2, "if (request.GetRowCountOnly)");
                output.AddLine(tc + 3, $"return listPage;");
                output.AddLine(tc + 1, "}");
                output.AddLine(tc + 1, "else if (!request.RecalcRowCount && !request.GetRowCountOnly)");
                output.AddLine(tc + 1, "{");
                output.AddLine(tc + 2, "listPage.TotalRowCount = -1;  // Make it clear that row count was not calculated");
                output.AddLine(tc + 1, "}");
            }

            if (method.InclSorting)
            {
                output.AddLine();
                output.AddLine(tc + 1, "// Sorting");
                output.AddLine(tc + 1, $"if (!string.IsNullOrWhiteSpace({reqVarName}.SortBy))");
                output.AddLine(tc + 2, $"dbQuery = this.AddSorting(ref dbQuery, {reqVarName});");
            }
            else if (method.InclPaging)
            {
                output.AddLine();
                output.AddLine(tc + 1, $"dbQuery = dbQuery.OrderBy(x => x.Id);  // Stable ordering for paging");
            }

            if (method.InclPaging)
            {
                output.AddLine();
                output.AddLine(tc + 1, $"if ({reqVarName}.PageSize > 0)");
                output.AddLine(tc + 2, $"dbQuery = dbQuery.Skip({reqVarName}.PageOffset * {reqVarName}.PageSize).Take({reqVarName}.PageSize);");
                output.AddLine();
                output.AddLine(tc + 1, $"listPage.Items = await dbQuery.ToListAsync();");
                output.AddLine();
                output.AddLine(tc + 1, $"return listPage;");
            }
            else if (method.IsList)
            {
                output.AddLine();
                output.AddLine(tc + 1, $"return await dbQuery.ToListAsync();");
            }
            else
            {
                output.AddLine();
                output.AddLine(tc + 1, $"return await dbQuery.FirstOrDefaultAsync();");
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

        internal void GenerateSortingMethod(EntityModel entity, List<string> output)
        {
            var tc = 0;
            output.AddLine();

            // Method
            output.AddLine(tc, $"private IQueryable<{entity.Name}> AddSorting(ref IQueryable<{entity.Name}> dbQuery, ISortingRequest sortingRequest)");
            output.AddLine(tc, "{");

            var c = 0;
            foreach (var prop in entity.Properties)
            {
                if (c++ > 0)
                    output.AddLine();

                output.AddLine(tc + 1, $"if (string.Equals(sortingRequest.SortBy, {entity.Name}.PropNames.{prop.Name}, StringComparison.OrdinalIgnoreCase))");
                output.AddLine(tc + 2, "if (sortingRequest.SortDesc)");
                output.AddLine(tc + 3, $"return dbQuery.OrderByDescending(x => x.{prop.Name});");
                output.AddLine(tc + 2, "else");
                output.AddLine(tc + 3, $"return dbQuery.OrderBy(x => x.{prop.Name});");
            }
            output.AddLine(tc + 1, "return dbQuery;");
            output.AddLine(tc, "}");
        }
    }
}