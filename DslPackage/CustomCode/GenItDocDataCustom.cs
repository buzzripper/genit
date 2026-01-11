using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom DocData - currently using base class for all serialization.
	/// Multi-diagram support can be added here later if needed.
	/// </summary>
	internal partial class GenItDocData
	{
		// No custom overrides - use the generated base class Load/Save methods
		// which properly handle single-diagram serialization.
		
		// The base class in DocData.cs handles:
		// - Load() using GenItSerializationHelper.Instance.LoadModelAndDiagram()
		// - Save() using GenItSerializationHelper.Instance.SaveModelAndDiagram()
		// - SaveSubordinateFile() for diagram-only saves
	}
}
