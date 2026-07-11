using System;
using System.ComponentModel.Design;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Command, group and toolbar identifiers for the multi-view document toolbar.
	/// These MUST match the symbol values declared in Commands.vsct.
	/// </summary>
	internal static class GenItMultiViewCommands
	{
		/// <summary>Command set guid shared with the rest of the GenIt package.</summary>
		public static readonly Guid CommandSetGuid = new Guid(Constants.GenItCommandSetId);

		// Toolbar + group (see Commands.vsct)
		public const int ViewToolbarId = 0x11000;
		public const int ViewToolbarGroupId = 0x21000;

		// Command ids (see Commands.vsct)
		public const int ViewSelectorComboId = 0x0200;
		public const int ViewSelectorComboGetListId = 0x0201;
		public const int NewViewId = 0x0202;
		public const int RenameViewId = 0x0203;
		public const int DeleteViewId = 0x0204;

		/// <summary>The document toolbar shown on the GenIt diagram window.</summary>
		public static readonly CommandID ViewToolbar = new CommandID(CommandSetGuid, ViewToolbarId);

		/// <summary>The view-selector dropdown combo.</summary>
		public static readonly CommandID ViewSelectorCombo = new CommandID(CommandSetGuid, ViewSelectorComboId);

		/// <summary>Companion "get list" command that supplies the combo's items.</summary>
		public static readonly CommandID ViewSelectorComboGetList = new CommandID(CommandSetGuid, ViewSelectorComboGetListId);

		/// <summary>Creates a new view.</summary>
		public static readonly CommandID NewView = new CommandID(CommandSetGuid, NewViewId);

		/// <summary>Renames the current view.</summary>
		public static readonly CommandID RenameView = new CommandID(CommandSetGuid, RenameViewId);

		/// <summary>Deletes the current view.</summary>
		public static readonly CommandID DeleteView = new CommandID(CommandSetGuid, DeleteViewId);
	}
}
