using System;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CustomCode
{
	internal class ModelValidator
	{
		internal void Validate(ModelRoot modelRoot)
		{
			if (modelRoot == null)
				throw new Exception("Could not access the model root.");

			ValidateEntities(modelRoot);

		}

		private void ValidateEntities(ModelRoot modelRoot)
		{
			if (string.IsNullOrEmpty(modelRoot.EntitiesOutputFolder))
				throw new Exception("EntitiesOutputFolder is not set. Please set it in the ModelRoot properties.");
			if (!Directory.Exists(modelRoot.EntitiesOutputFolder))
				throw new Exception("EntitiesOutputFolder does not exist. Please select a valid folder.");

			if (string.IsNullOrEmpty(modelRoot.EntitiesNamespace))
				throw new Exception("EntitiesNamespace is not set. Please set it in the ModelRoot properties.");

			var entities = modelRoot?.Types?.OfType<EntityModel>()?.ToList();
			if (entities == null || entities.Count == 0)
				throw new Exception("No entities found in the model. Add some entities first.");
		}
	}
}