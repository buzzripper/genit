﻿namespace Dyvenix.Genit.UserControls;

partial class EntityGenEditCtl
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
		label1 = new System.Windows.Forms.Label();
		txtOutputFolder = new System.Windows.Forms.TextBox();
		lblName = new System.Windows.Forms.Label();
		ckbEnabled = new System.Windows.Forms.CheckBox();
		ckbInclHeader = new System.Windows.Forms.CheckBox();
		btnBrowseFolder = new System.Windows.Forms.Button();
		folderDlg = new System.Windows.Forms.FolderBrowserDialog();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new System.Drawing.Point(46, 77);
		label1.Name = "label1";
		label1.Size = new System.Drawing.Size(99, 19);
		label1.TabIndex = 0;
		label1.Text = "Output Folder:";
		label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// txtOutputFolder
		// 
		txtOutputFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		txtOutputFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtOutputFolder.Location = new System.Drawing.Point(151, 76);
		txtOutputFolder.Name = "txtOutputFolder";
		txtOutputFolder.Size = new System.Drawing.Size(479, 25);
		txtOutputFolder.TabIndex = 1;
		txtOutputFolder.TextChanged += txtOutputFolder_TextChanged;
		// 
		// lblName
		// 
		lblName.AutoSize = true;
		lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		lblName.Location = new System.Drawing.Point(1, 2);
		lblName.Name = "lblName";
		lblName.Size = new System.Drawing.Size(123, 21);
		lblName.TabIndex = 2;
		lblName.Text = "Entity Generator";
		// 
		// ckbEnabled
		// 
		ckbEnabled.AutoSize = true;
		ckbEnabled.Location = new System.Drawing.Point(167, 153);
		ckbEnabled.Name = "ckbEnabled";
		ckbEnabled.Size = new System.Drawing.Size(76, 23);
		ckbEnabled.TabIndex = 3;
		ckbEnabled.Text = "Enabled";
		ckbEnabled.UseVisualStyleBackColor = true;
		ckbEnabled.CheckedChanged += ckbEnabled_CheckedChanged;
		// 
		// ckbInclHeader
		// 
		ckbInclHeader.AutoSize = true;
		ckbInclHeader.Location = new System.Drawing.Point(328, 153);
		ckbInclHeader.Name = "ckbInclHeader";
		ckbInclHeader.Size = new System.Drawing.Size(120, 23);
		ckbInclHeader.TabIndex = 4;
		ckbInclHeader.Text = "Include Header";
		ckbInclHeader.UseVisualStyleBackColor = true;
		ckbInclHeader.CheckedChanged += ckbInclHeader_CheckedChanged;
		// 
		// btnBrowseFolder
		// 
		btnBrowseFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		btnBrowseFolder.Location = new System.Drawing.Point(636, 76);
		btnBrowseFolder.Name = "btnBrowseFolder";
		btnBrowseFolder.Size = new System.Drawing.Size(24, 22);
		btnBrowseFolder.TabIndex = 6;
		btnBrowseFolder.Text = "...";
		btnBrowseFolder.UseVisualStyleBackColor = true;
		btnBrowseFolder.Click += btnBrowseFolder_Click;
		// 
		// EntityGenEditCtl
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		Controls.Add(btnBrowseFolder);
		Controls.Add(ckbInclHeader);
		Controls.Add(ckbEnabled);
		Controls.Add(lblName);
		Controls.Add(txtOutputFolder);
		Controls.Add(label1);
		Font = new System.Drawing.Font("Segoe UI", 10F);
		Name = "EntityGenEditCtl";
		Size = new System.Drawing.Size(719, 389);
		Load += EntityGenEditCtl_Load;
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.TextBox txtOutputFolder;
	private System.Windows.Forms.Label lblName;
	private System.Windows.Forms.CheckBox ckbEnabled;
	private System.Windows.Forms.CheckBox ckbInclHeader;
	private System.Windows.Forms.Button btnBrowseFolder;
	private System.Windows.Forms.FolderBrowserDialog folderDlg;
}
