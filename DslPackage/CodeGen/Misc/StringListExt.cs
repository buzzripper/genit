using System.Collections.Generic;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	public static class StringListExt
	{
		public static void AddIfNotExists(this List<string> list, string newValue)
		{
			if (!list.Contains(newValue))
				list.Add(newValue);
		}

		public static void AddLine(this List<string> list, int tabCount, StringBuilder sb)
		{
			AddLine(list, tabCount, sb.ToString());
		}

		public static void AddLine(this List<string> list, int tabCount, string line)
		{
			list.Add($"{Tabs(tabCount)}{line}");
		}

		public static void AddLine(this List<string> list)
		{
			list.Add(string.Empty);
		}

		public static string AsString(this List<string> list)
		{
			var sb = new StringBuilder();
			list.ForEach(x => sb.AppendLine(x));
			return sb.ToString();
		}

		private static string Tabs(int count)
		{
			return new string('\t', count);
		}


	}
}
