﻿namespace Dyvenix.Genit.UserControls;

partial class UpdateMethodsEditCtl
{
	/// <summary> 
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary> 
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Component Designer generated code

	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateMethodsEditCtl));
		splMain = new System.Windows.Forms.SplitContainer();
		grdMethods = new System.Windows.Forms.DataGridView();
		colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
		colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		colUseDto = new System.Windows.Forms.DataGridViewCheckBoxColumn();
		colDelete = new System.Windows.Forms.DataGridViewImageColumn();
		toolStrip1 = new System.Windows.Forms.ToolStrip();
		btnAdd = new System.Windows.Forms.ToolStripButton();
		btnUp = new System.Windows.Forms.ToolStripButton();
		btnDown = new System.Windows.Forms.ToolStripButton();
		clbUpdProperties = new System.Windows.Forms.CheckedListBox();
		toolStrip2 = new System.Windows.Forms.ToolStrip();
		toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
		bindingSrc = new System.Windows.Forms.BindingSource(components);
		((System.ComponentModel.ISupportInitialize)splMain).BeginInit();
		splMain.Panel1.SuspendLayout();
		splMain.Panel2.SuspendLayout();
		splMain.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)grdMethods).BeginInit();
		toolStrip1.SuspendLayout();
		toolStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)bindingSrc).BeginInit();
		SuspendLayout();
		// 
		// splMain
		// 
		splMain.AllowDrop = true;
		splMain.Dock = System.Windows.Forms.DockStyle.Fill;
		splMain.Location = new System.Drawing.Point(0, 0);
		splMain.Margin = new System.Windows.Forms.Padding(2);
		splMain.Name = "splMain";
		// 
		// splMain.Panel1
		// 
		splMain.Panel1.AllowDrop = true;
		splMain.Panel1.Controls.Add(grdMethods);
		splMain.Panel1.Controls.Add(toolStrip1);
		// 
		// splMain.Panel2
		// 
		splMain.Panel2.Controls.Add(clbUpdProperties);
		splMain.Panel2.Controls.Add(toolStrip2);
		splMain.Size = new System.Drawing.Size(1033, 537);
		splMain.SplitterDistance = 590;
		splMain.SplitterWidth = 6;
		splMain.TabIndex = 0;
		// 
		// grdMethods
		// 
		grdMethods.AllowUserToAddRows = false;
		grdMethods.AllowUserToDeleteRows = false;
		grdMethods.AllowUserToResizeRows = false;
		grdMethods.BackgroundColor = System.Drawing.SystemColors.Control;
		grdMethods.BorderStyle = System.Windows.Forms.BorderStyle.None;
		grdMethods.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		grdMethods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		grdMethods.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colId, colName, colUseDto, colDelete });
		grdMethods.Dock = System.Windows.Forms.DockStyle.Fill;
		grdMethods.Location = new System.Drawing.Point(0, 31);
		grdMethods.Margin = new System.Windows.Forms.Padding(2);
		grdMethods.MultiSelect = false;
		grdMethods.Name = "grdMethods";
		grdMethods.ReadOnly = true;
		grdMethods.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		grdMethods.RowHeadersWidth = 62;
		grdMethods.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
		grdMethods.ShowEditingIcon = false;
		grdMethods.Size = new System.Drawing.Size(590, 506);
		grdMethods.TabIndex = 3;
		grdMethods.CellClick += grdMethods_CellClick;
		grdMethods.CellMouseEnter += grdMethods_CellMouseEnter;
		grdMethods.CellMouseLeave += grdMethods_CellMouseLeave;
		grdMethods.DataError += grdMethods_DataError;
		grdMethods.SelectionChanged += grdMethods_SelectionChanged;
		// 
		// colId
		// 
		colId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		colId.DataPropertyName = "Id";
		colId.HeaderText = "Id";
		colId.MinimumWidth = 40;
		colId.Name = "colId";
		colId.ReadOnly = true;
		colId.Visible = false;
		// 
		// colName
		// 
		colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		colName.DataPropertyName = "Name";
		colName.HeaderText = "Name";
		colName.MinimumWidth = 100;
		colName.Name = "colName";
		colName.ReadOnly = true;
		// 
		// colUseDto
		// 
		colUseDto.DataPropertyName = "UseDto";
		colUseDto.HeaderText = "Use DTO";
		colUseDto.MinimumWidth = 100;
		colUseDto.Name = "colUseDto";
		colUseDto.ReadOnly = true;
		colUseDto.Resizable = System.Windows.Forms.DataGridViewTriState.False;
		// 
		// colDelete
		// 
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle1.NullValue = resources.GetObject("dataGridViewCellStyle1.NullValue");
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
		colDelete.DefaultCellStyle = dataGridViewCellStyle1;
		colDelete.HeaderText = "";
		colDelete.Image = (System.Drawing.Image)resources.GetObject("colDelete.Image");
		colDelete.MinimumWidth = 75;
		colDelete.Name = "colDelete";
		colDelete.ReadOnly = true;
		colDelete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
		colDelete.Width = 75;
		// 
		// toolStrip1
		// 
		toolStrip1.Font = new System.Drawing.Font("Segoe UI", 10F);
		toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
		toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnAdd, btnUp, btnDown });
		toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
		toolStrip1.Location = new System.Drawing.Point(0, 0);
		toolStrip1.Name = "toolStrip1";
		toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
		toolStrip1.Size = new System.Drawing.Size(590, 31);
		toolStrip1.TabIndex = 2;
		toolStrip1.Text = "toolStrip1";
		// 
		// btnAdd
		// 
		btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		btnAdd.Image = (System.Drawing.Image)resources.GetObject("btnAdd.Image");
		btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
		btnAdd.Name = "btnAdd";
		btnAdd.Size = new System.Drawing.Size(28, 28);
		btnAdd.Text = "toolStripButton1";
		btnAdd.Click += btnAdd_Click;
		// 
		// btnUp
		// 
		btnUp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		btnUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		btnUp.Image = (System.Drawing.Image)resources.GetObject("btnUp.Image");
		btnUp.ImageTransparentColor = System.Drawing.Color.Magenta;
		btnUp.Name = "btnUp";
		btnUp.Size = new System.Drawing.Size(28, 28);
		btnUp.Text = "toolStripButton1";
		// 
		// btnDown
		// 
		btnDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
		btnDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		btnDown.Image = (System.Drawing.Image)resources.GetObject("btnDown.Image");
		btnDown.ImageTransparentColor = System.Drawing.Color.Magenta;
		btnDown.Name = "btnDown";
		btnDown.Size = new System.Drawing.Size(28, 28);
		btnDown.Text = "toolStripButton1";
		// 
		// clbUpdProperties
		// 
		clbUpdProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		clbUpdProperties.Dock = System.Windows.Forms.DockStyle.Fill;
		clbUpdProperties.FormattingEnabled = true;
		clbUpdProperties.Location = new System.Drawing.Point(0, 25);
		clbUpdProperties.Name = "clbUpdProperties";
		clbUpdProperties.Size = new System.Drawing.Size(437, 512);
		clbUpdProperties.TabIndex = 8;
		clbUpdProperties.ItemCheck += clbUpdProperties_ItemCheck;
		// 
		// toolStrip2
		// 
		toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripLabel2 });
		toolStrip2.Location = new System.Drawing.Point(0, 0);
		toolStrip2.Name = "toolStrip2";
		toolStrip2.Size = new System.Drawing.Size(437, 25);
		toolStrip2.TabIndex = 7;
		toolStrip2.Text = "toolStrip2";
		// 
		// toolStripLabel2
		// 
		toolStripLabel2.Name = "toolStripLabel2";
		toolStripLabel2.Size = new System.Drawing.Size(101, 22);
		toolStripLabel2.Text = "Update Properties";
		// 
		// UpdateMethodsEditCtl
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		Controls.Add(splMain);
		Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		Margin = new System.Windows.Forms.Padding(2);
		Name = "UpdateMethodsEditCtl";
		Size = new System.Drawing.Size(1033, 537);
		Load += ServiceMethodsEditCtl_Load;
		splMain.Panel1.ResumeLayout(false);
		splMain.Panel1.PerformLayout();
		splMain.Panel2.ResumeLayout(false);
		splMain.Panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)splMain).EndInit();
		splMain.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)grdMethods).EndInit();
		toolStrip1.ResumeLayout(false);
		toolStrip1.PerformLayout();
		toolStrip2.ResumeLayout(false);
		toolStrip2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)bindingSrc).EndInit();
		ResumeLayout(false);
	}

	#endregion

	private System.Windows.Forms.SplitContainer splMain;
	private System.Windows.Forms.ToolStrip toolStrip1;
	private System.Windows.Forms.ToolStripButton btnAdd;
	private System.Windows.Forms.DataGridView grdMethods;
	private System.Windows.Forms.BindingSource bindingSrc;
	private System.Windows.Forms.SplitContainer splLists;
	private System.Windows.Forms.DataGridViewCheckBoxColumn colInclSorting;
	private System.Windows.Forms.DataGridViewCheckBoxColumn colInclPaging;
	private System.Windows.Forms.DataGridViewCheckBoxColumn colUseQuery;
	private System.Windows.Forms.DataGridViewLinkColumn colAttrs;
	private System.Windows.Forms.ToolStrip toolStrip2;
	private System.Windows.Forms.ToolStripLabel toolStripLabel2;
	private System.Windows.Forms.CheckedListBox clbUpdProperties;
	private System.Windows.Forms.ToolStripButton btnUp;
	private System.Windows.Forms.ToolStripButton btnDown;
	private System.Windows.Forms.DataGridViewTextBoxColumn colId;
	private System.Windows.Forms.DataGridViewTextBoxColumn colName;
	private System.Windows.Forms.DataGridViewCheckBoxColumn colUseDto;
	private System.Windows.Forms.DataGridViewImageColumn colDelete;
}
