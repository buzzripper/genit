﻿using Dyvenix.Genit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Dyvenix.Genit.UserControls
{
	public partial class EntityContainerCtl : UserControl
	{
		#region Fields

		private const int cIdxMain = 0;
		private const int cIdxSvcMethods = 1;

		private readonly List<EntityEditorItem> _childEditors = new List<EntityEditorItem>();

		#endregion

		#region Ctors / Init

		public EntityContainerCtl(EntityModel entity)
		{
			InitializeComponent();
			Entity = entity;
			Initialize();
		}

		private void Initialize()
		{
			lblEntityName.Text = Entity.Name;
			Entity.PropertyChanged += _entity_PropertyChanged;

			_childEditors.Add(new EntityEditorItem(nbMain, new EntityMainEditCtl(Entity)));
			_childEditors.Add(new EntityEditorItem(nbSvcMethods, new ServiceEditCtl(Entity)));

			foreach (var childEditor in _childEditors) {
				var ctl = childEditor.Ctl;
				ctl.Top = toolStrip1.Height;
				ctl.Left = 0;
				ctl.Width = this.Width;
				ctl.Height = this.Height - toolStrip1.Height;
				ctl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

				this.Controls.Add(ctl);
			}

			SelectControl(cIdxMain);
		}

		#endregion

		#region Properties

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EntityModel Entity { get; private set; }

		#endregion

		private void _entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
				lblEntityName.Text = Entity.Name;
		}

		private void nbMain_Click(object sender, EventArgs e)
		{
			SelectControl(cIdxMain);
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

			//foreach (var ce in _childEditors)
			//	if (ce.Ctl is EntityMainEditCtl)
			//		((EntityMainEditCtl)ce.Ctl).InitializeGrid();
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
