
namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class Utils
	{
		private static OutputWindowHelper _outputWindowHelper;

		internal static OutputWindowHelper GetOutputWindowHelper()
		{
			if (_outputWindowHelper == null)
				_outputWindowHelper = new OutputWindowHelper();
			return _outputWindowHelper;
		}
	}
}
