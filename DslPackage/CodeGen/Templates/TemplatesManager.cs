using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Templates
{
	internal static class TemplatesManager
	{
		private static string _templatesPath = @"CodeGen\Templates";
		private static OutputWindowHelper _outWinHelper;

		public static void Initialize(string templatesPath)
		{
			_templatesPath = templatesPath;
			_outWinHelper = CodeGenUtils.OutputWindowHelper;
		}

		public static string GetTemplate(string templateName)
		{
			try
			{
				var templateFilepath = System.IO.Path.Combine(_templatesPath, templateName);
				return System.IO.File.ReadAllText(templateFilepath);
			}
			catch (Exception ex)
			{
				_outWinHelper.WriteError($"Failed to load template '{templateName}': {ex.Message}");
				return null;
			}
		}
	}
}
