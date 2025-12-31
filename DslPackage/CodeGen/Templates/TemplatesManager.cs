using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Templates
{
	internal class TemplatesManager
	{
		private const string cResourceRootPath = "Dyvenix.GenIt.DslPackage.Resources.Templates";
		private readonly string _templatesFolderPath;
		private readonly string _templateFilename;
		private string _template;

		public TemplatesManager(string templatesFolderPath, string templateFilename)
		{
			_templatesFolderPath = FileHelper.GetAbsolutePath(templatesFolderPath);
			_templateFilename = templateFilename;
		}

		internal void Validate(List<string> errors)
		{
			if (string.IsNullOrEmpty(_templateFilename))
				errors.Add("TemplateFilename not provided.");

			if (string.IsNullOrEmpty(_templatesFolderPath)) // Empty means embedded resource
			{
				try
				{
					_template = LoadFileFromResource(_templateFilename);
				}
				catch (Exception ex)
				{
					errors.Add($"Failed to load embedded template '{_templateFilename}': {ex.Message}");
				}
			}
			else if (!Directory.Exists(_templatesFolderPath))
				errors.Add("TemplatesFolder does not exist. Please select a valid folder.");
			else
			{
				try
				{
					var filepath = Path.Combine(_templatesFolderPath, _templateFilename);
					_template = File.ReadAllText(filepath);
				}
				catch (Exception ex)
				{
					errors.Add($"Failed to load embedded template '{_templateFilename}': {ex.Message}");
				}
			}
		}

		private string LoadFileFromResource(string templateFilename)
		{
			var resourceName = $"{cResourceRootPath}.{templateFilename}";

			var assembly = Assembly.GetExecutingAssembly();
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new InvalidOperationException("Resource not found: " + resourceName);

				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public string GetTemplate()
		{
			// Return a fresh copy each time
			return $"{_template}";
		}
	}
}
