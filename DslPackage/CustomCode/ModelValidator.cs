using System;

namespace Dyvenix.GenIt.DslPackage.CustomCode
{
	internal class ModelValidator
	{
		internal void Validate(ModelRoot modelRoot)
		{
			if (modelRoot == null)
				throw new Exception("Could not access the model root.");

			// ValidateEntities(modelRoot); // TODO: Implement validation
		}
	}
}