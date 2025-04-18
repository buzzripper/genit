﻿using Dyvenix.Genit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace Dyvenix.Genit.UserControls
{
	public partial class NavPropGridCtl : UserControl
	{
		public event EventHandler<NavPropertyEditEventArgs> NavPropertyEdit;

		#region Fields

		private ObservableCollection<NavPropertyModel> _navProperties;
		private List<NavPropGridRowCtl> _navPropGridRowCtls = new List<NavPropGridRowCtl>();

		#endregion

		#region Ctors / Init

		public NavPropGridCtl()
		{
			InitializeComponent();
		}

		private void NavPropGridCtl_Load(object sender, EventArgs e)
		{
			splMain.Height = lblPrimaryPropertyName.Height + 4;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<NavPropertyModel> DataSource
		{
			get { return _navProperties; }
			set {
				_navProperties = value;
				_navProperties.CollectionChanged += NavProperties_CollectionChanged;
				PopulateRows();
			}
		}

		#endregion

		#region Methods

		private void PopulateRows()
		{
			if (this.DesignMode)
				return;

			SuspendLayout();

			try {
				foreach (var row in _navPropGridRowCtls) {
					this.Controls.Remove(row);
					row.Dispose();
				}
				_navPropGridRowCtls.Clear();

				var count = 0;
				foreach (var navProperty in _navProperties) {
					NavPropGridRowCtl navPropGridRowCtl = new NavPropGridRowCtl(navProperty);
					navPropGridRowCtl.Top = splMain.Height + (count++ * 35);
					navPropGridRowCtl.Left = 0;
					navPropGridRowCtl.Width = this.Width;
					navPropGridRowCtl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					navPropGridRowCtl.NavigationPropertyChanged += NavPropGridRowCtl_NavigationPropertyChanged;
					navPropGridRowCtl.NavPropertyEdit += NavPropGridRowCtl_OnNavPropertyEdit;

					_navPropGridRowCtls.Add(navPropGridRowCtl);
					this.Controls.Add(navPropGridRowCtl);
				}

				splMain_SplitterMoved(null, null);
				splCardinality_SplitterMoved(null, null);
				splRelPropName_SplitterMoved(null, null);

			} catch (Exception ex) {
				MessageBox.Show(ex.Message);

			} finally {
				ResumeLayout();
			}
		}

		public void Reload()
		{
			PopulateRows();
		}

		#endregion

		#region UI Events

		private void NavPropGridRowCtl_NavigationPropertyChanged(object sender, NavPropChangedEventArgs e)
		{
			if (e.Action == NavPropChangedAction.Deleted) {
				_navProperties.Remove(e.NavPropertyModel);
				e.NavPropertyModel.FKEntity.Properties.Remove(e.NavPropertyModel.FKProperty);
			}
		}

		private void NavPropGridRowCtl_OnNavPropertyEdit(object sender, NavPropertyEditEventArgs e)
		{
			NavPropertyEdit?.Invoke(this, e);
		}

		private void NavProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
				PopulateRows();
		}

		private void splMain_SplitterMoved(object sender, SplitterEventArgs e)
		{
			foreach (var ctl in _navPropGridRowCtls)
				if (ctl is NavPropGridRowCtl) {
					((NavPropGridRowCtl)ctl).Col1Width = splMain.SplitterDistance;
					((NavPropGridRowCtl)ctl).Col2Width = splCardinality.SplitterDistance;
					((NavPropGridRowCtl)ctl).Col3Width = splRelPropName.SplitterDistance;
				}
		}

		private void splCardinality_SplitterMoved(object sender, SplitterEventArgs e)
		{
			foreach (var ctl in _navPropGridRowCtls)
				if (ctl is NavPropGridRowCtl) {
					((NavPropGridRowCtl)ctl).Col2Width = splCardinality.SplitterDistance;
					((NavPropGridRowCtl)ctl).Col3Width = splRelPropName.SplitterDistance;
				}
		}

		private void splRelPropName_SplitterMoved(object sender, SplitterEventArgs e)
		{
			foreach (var ctl in _navPropGridRowCtls)
				if (ctl is NavPropGridRowCtl) {
					((NavPropGridRowCtl)ctl).Col3Width = splRelPropName.SplitterDistance;
				}
		}

		#endregion
	}
}
