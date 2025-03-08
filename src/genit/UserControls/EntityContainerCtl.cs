﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dyvenix.Genit.Models;

namespace Dyvenix.Genit.UserControls
{
	public partial class EntityContainerCtl : UserControl
	{
		private const int cIdxMain = 0;
		private const int cIdxProperties = 1;
		private const int cIdxSvcMethods = 2;

		private int _currIdx = 0;
		private readonly EntityModel _entity;
		private readonly List<EntityEditorItem> _childEditors = new List<EntityEditorItem>();

		public EntityContainerCtl(EntityModel entity)
		{
			InitializeComponent();
			_entity = entity;
			Initialize();
		}

		private void Initialize()
		{
			_childEditors.Add(new EntityEditorItem(nbMain, new EntityMainEditCtl()));
			_childEditors.Add(new EntityEditorItem(nbProperties, new PropertyEditCtl()));
			_childEditors.Add(new EntityEditorItem(nbSvcMethods, new SvcMethodsEditCtl()));

			foreach (var childEditor in _childEditors) {
				var ctl = childEditor.Ctl;
				ctl.Top = toolStrip1.Height;
				ctl.Left = 0;
				ctl.Width = this.Width;
				ctl.Height = this.Height - toolStrip1.Height;
				ctl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
				Controls.Add(ctl);

				var entityEditCtl = ctl as EntityEditCtlBase;
				if (entityEditCtl != null)
					entityEditCtl.Initialize(_entity);
			}
		
			SelectControl(cIdxMain);
		}

		private void nbMain_Click(object sender, EventArgs e)
		{
			SelectControl(cIdxMain);
		}

		private void nbProperties_Click(object sender, EventArgs e)
		{
			SelectControl(cIdxProperties);
		}

		private void nbSvcMethods_Click(object sender, EventArgs e)
		{
			SelectControl(cIdxSvcMethods);
		}

		private void SelectControl(int idx)
		{
			this.SuspendLayout();

			for (var i = 0; i < _childEditors.Count; i++) {
				var selected = (i == idx);
				var btn = _childEditors[i].Button;
				var ctl = _childEditors[i].Ctl;

				if (selected) {
					btn.BackColor = SystemColors.ActiveCaption;
				} else {
					btn.BackColor = Color.Transparent;
				}

				_childEditors[i].Ctl.Visible = selected;
			}

			this.ResumeLayout();
		}

		//private void nbMain_EnabledChanged(object sender, EventArgs e)
		//{
		//	var btn = (ToolStripButton)sender;
		//	if (btn.Enabled) {
		//		btn.ForeColor = SystemColors.ControlText;
		//		btn.BackColor = SystemColors.ActiveCaption;
		//	} else {
		//		btn.ForeColor = Color.OrangeRed;
		//		btn.BackColor = SystemColors.ActiveCaption;
		//	}
		//}
	}

	public class EntityEditorItem
	{
		public EntityEditorItem(ToolStripButton button, Control ctl)
		{
			Button = button;
			Ctl = ctl;
		}

		public ToolStripButton Button { get; set; }
		public Control Ctl { get; set; }
	}
}
