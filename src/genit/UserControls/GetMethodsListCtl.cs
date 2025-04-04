﻿using Dyvenix.Genit.Misc;
using Dyvenix.Genit.Models;
using Dyvenix.Genit.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Dyvenix.Genit.UserControls;

public partial class GetMethodsListCtl : UserControlBase
{
	public event EventHandler<MethodAddedEventArgs> MethodAdded;
	public event EventHandler<MethodChangedEventArgs> MethodChanged;
	public event EventHandler<MethodDeletedEventArgs> MethodDeleted;

	#region Constants

	private const int cIdCol = 0;
	private const int cNameCol = 1;
	private const int cPropCol = 2;
	private const int cIsListCol = 3;
	private const int cInclSortingCol = 4;
	private const int cInclPagingCol = 5;
	private const int cAttrsCol = 6;
	private const int cDelCol = 7;

	#endregion

	#region Fields

	private ObservableCollection<ServiceMethodModel> _methods;

	#endregion

	#region Ctors / Init

	public GetMethodsListCtl()
	{
		InitializeComponent();
	}

	private void GetMethodsListCtl_Load(object sender, EventArgs e)
	{
		PositionGrid(toolStrip1.Dock);
		grdItems.AutoGenerateColumns = false;
	}

	public void SetData(ObservableCollection<ServiceMethodModel> methods, ObservableCollection<PropertyModel> properties)
	{
		var propItems = BuildPropertyBindingList(properties);
		var comboCol = grdItems.Columns[cPropCol] as DataGridViewComboBoxColumn;
		comboCol.DataSource = propItems;
		comboCol.DisplayMember = "Name";
		comboCol.ValueMember = "Id";

		_methods = methods;
		bindingSrc.DataSource = _methods;
		grdItems.DataSource = bindingSrc;
	}

	private BindingList<ListItem> BuildPropertyBindingList(ObservableCollection<PropertyModel> properties)
	{
		var propItems = new BindingList<ListItem>();

		propItems.Add(new ListItem(Guid.Empty, string.Empty));
		foreach (var prop in properties) {

			propItems.Add(new ListItem(prop.Id, prop.Name));
		}

		return propItems;
	}

	#endregion

	#region Properties

	#endregion

	#region Add

	private void btnAdd_Click(object sender, EventArgs e)
	{
		this.Add();
	}

	private void Add()
	{
		var method = new ServiceMethodModel(Guid.NewGuid()) {
			Name = "Get"
		};

		bindingSrc.Add(method);
	}

	#endregion

	#region Delete

	private void grdItems_SelectionChanged(object sender, EventArgs e)
	{
		btnDelete.Enabled = grdItems.SelectedCells.Count == 1;
	}

	private void btnDelete_Click(object sender, EventArgs e)
	{
		this.Delete();
	}

	private void Delete()
	{
		if (grdItems.SelectedCells.Count == 1) {
			var rowIdx = grdItems.SelectedCells[0].OwningRow.Index;
			var idValStr = grdItems.Rows[rowIdx].Cells[cIdCol].Value?.ToString();
			var method = _methods.FirstOrDefault(m => m.Id == Guid.Parse(idValStr));

			if (method != null) {
				bindingSrc.Remove(method);
				MethodDeleted?.Invoke(this, new MethodDeletedEventArgs(rowIdx));
			}
		}
	}

	#endregion

	#region Methods

	private void PositionGrid(DockStyle dock)
	{
		switch (dock) {
			case DockStyle.Top:
				grdItems.Top = toolStrip1.Height + 1;
				grdItems.Left = 0;
				grdItems.Width = this.Width;
				grdItems.Height = this.Height - toolStrip1.Height - 2;
				break;
			case DockStyle.Right:
				grdItems.Top = 0;
				grdItems.Left = 0;
				grdItems.Width = this.Width - toolStrip1.Width - 2;
				grdItems.Height = this.Height - 2;
				break;
			case DockStyle.Left:
				grdItems.Top = 0;
				grdItems.Left = toolStrip1.Width + 1;
				grdItems.Width = this.Width - toolStrip1.Width - 2;
				grdItems.Height = this.Height - 2;
				break;
			default: // DockStyle.Bottom:
				grdItems.Top = 0;
				grdItems.Left = 0;
				grdItems.Width = this.Width;
				grdItems.Height = this.Height - toolStrip1.Height - 2;
				break;
		}

		grdItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
	}

	private void grdItems_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Shift && e.KeyCode == Keys.Insert) {
			this.Add();

		} else if (e.Shift && e.KeyCode == Keys.Delete) {
			this.Delete();
		}
	}

	#endregion

	private void grdItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
	{
		MessageBox.Show($"Error in column {e.ColumnIndex}: {e.Exception.Message}");
	}

	private void grdItems_CellClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.RowIndex == -1)
			return;

		if (e.ColumnIndex == cAttrsCol) {
			var method = GetSvcMethodModel(e.RowIndex);
			this.StrListForm.Run("Attributes", method.Attributes);
			bindingSrc.ResetBindings(false);

		} else if (e.ColumnIndex == cDelCol) {
			if (MessageBox.Show("Confirm Delete", "Delete this item?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
				var method = GetSvcMethodModel(e.RowIndex);
				bindingSrc.Remove(method);
			}
		}
	}

	private ServiceMethodModel GetSvcMethodModel(int rowIndex)
	{
		var idValStr = grdItems.Rows[rowIndex].Cells[cIdCol].Value?.ToString();
		return _methods.FirstOrDefault(m => m.Id == Guid.Parse(idValStr));
	}

	private void grdItems_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
	{
		if ((e.ColumnIndex == this.grdItems.Columns[cAttrsCol].Index || e.ColumnIndex == this.grdItems.Columns[cDelCol].Index) && e.RowIndex > -1) {
			this.grdItems.Cursor = Cursors.Hand;
		}
	}

	private void grdItems_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
	{
		this.grdItems.Cursor = Cursors.Default;
	}
}

#region EventArg Classes

public class MethodAddedEventArgs : EventArgs
{
	public string Value { get; }

	public MethodAddedEventArgs(string value)
	{
		Value = value;
	}
}

public class MethodChangedEventArgs : EventArgs
{
	public int Index { get; }
	public string Value { get; }

	public MethodChangedEventArgs(int index, string value)
	{
		Index = index;
		Value = value;
	}
}

public class MethodDeletedEventArgs : EventArgs
{
	public int Index { get; }

	public MethodDeletedEventArgs(int index)
	{
		Index = index;
	}
}

#endregion


