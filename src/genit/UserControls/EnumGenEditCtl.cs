﻿using Dyvenix.Genit.Misc;
using Dyvenix.Genit.Models.Generators;
using System;
using System.Windows.Forms;

namespace Dyvenix.Genit.UserControls
{
	public partial class EnumGenEditCtl : UserControlBase
	{
		#region Fields

		private EnumGenModel _enumGenMdl;

		#endregion

		#region Ctors / Init

		public EnumGenEditCtl()
		{
			InitializeComponent();
		}

		public EnumGenEditCtl(EnumGenModel enumGenMdl) : this()
		{
			_enumGenMdl = enumGenMdl;
		}

		private void EnumGenEditCtl_Load(object sender, EventArgs e)
		{
			Populate();
		}

		private void Populate()
		{
			_suspendUpdates = true;

			txtOutputFolder.Text = _enumGenMdl.OutputFolder;
			txtEnumsNamespace.Text = _enumGenMdl.EnumsNamespace;
			ckbInclHeader.Checked = _enumGenMdl.InclHeader;
			ckbEnabled.Checked = _enumGenMdl.Enabled;

			_suspendUpdates = false;
		}

		#endregion

		private void txtOutputFolder_TextChanged(object sender, EventArgs e)
		{
			if (!_suspendUpdates)
				_enumGenMdl.OutputFolder = txtOutputFolder.Text;
		}


		private void txtEnumsNamespace_TextChanged(object sender, EventArgs e)
		{
			if (!_suspendUpdates)
				_enumGenMdl.EnumsNamespace = txtEnumsNamespace.Text;
		}

		private void ckbEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if (!_suspendUpdates)
				_enumGenMdl.Enabled = ckbEnabled.Checked;
		}

		private void ckbInclHeader_CheckedChanged(object sender, EventArgs e)
		{
			if (!_suspendUpdates)
				_enumGenMdl.InclHeader = ckbInclHeader.Checked;
		}

		private void btnBrowseFolder_Click(object sender, EventArgs e)
		{
			folderDlg.InitialDirectory = txtOutputFolder.Text;
			if (folderDlg.ShowDialog() == DialogResult.OK)
				txtOutputFolder.Text = Utils.ConvertToRelative(Globals.CurrDocFilepath, folderDlg.SelectedPath);
		}
	}
}
