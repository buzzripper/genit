using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class CodeGenUtils
	{
		private const string _fileHeaderFmt = @"//------------------------------------------------------------------------------------------------------------
// This file was auto-generated on {0}. Any changes made to it will be lost.
//------------------------------------------------------------------------------------------------------------";

		public const string NullableEnableDirective = "#nullable enable";

		public static string FileHeader
		{
			get
			{
				return string.Format(_fileHeaderFmt, DateTime.Now);
			}
		}

		public static string ResolveRelativePath(string path)
		{
			if (string.IsNullOrWhiteSpace(PackageUtils.SolutionRootPath) || string.IsNullOrWhiteSpace(path))
				return path;

			if (Path.IsPathRooted(path))
				return path;

			var bp = Path.GetDirectoryName(PackageUtils.SolutionRootPath);   // In case it's a filepath

			return Path.GetFullPath(Path.Combine(bp, path));
		}

		public static string FormatToken(string tokenTitle)
		{
			return $"${{{{{tokenTitle}}}}}";
		}

		public static string ToCamelCase(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			if (input.Length == 1)
				return input.ToLower();

			var firstChar = input.Substring(0, 1).ToLower();
			return $"{firstChar}{input.Substring(1)}";
		}

		public static List<EntityModel> SortEntitiesForDeletion(List<EntityModel> entities)
		{
			var entityMap = entities.ToDictionary(e => e.Name, e => e);
			var visited = new HashSet<string>();
			var inProgress = new HashSet<string>();
			var sorted = new List<EntityModel>();

			foreach (var entity in entities)
				SortForDeletionVisit(entity.Name, entityMap, visited, inProgress, sorted);

			return sorted;
		}

		public static List<EntityModel> SortEntitiesForCreation(List<EntityModel> entities)
		{
			return SortEntitiesForDeletion(entities).Reverse<EntityModel>().ToList();
		}

		private static void SortForDeletionVisit(string name, Dictionary<string, EntityModel> entityMap, HashSet<string> visited, HashSet<string> inProgress, List<EntityModel> sorted)
		{
			if (visited.Contains(name))
				return;
			if (inProgress.Contains(name))
				return;

			inProgress.Add(name);

			var entity = entityMap[name];
			foreach (var navProp in entity.NavigationProperties)
			{
				var targetName = navProp.TargetEntityName;
				if (string.IsNullOrEmpty(targetName) || !entityMap.ContainsKey(targetName) || targetName == name)
					continue;

				SortForDeletionVisit(targetName, entityMap, visited, inProgress, sorted);
			}

			inProgress.Remove(name);
			visited.Add(name);
			sorted.Add(entity);
		}
	}
}
