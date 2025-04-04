﻿namespace Dyvenix.Genit.UserControls;

partial class EnumGenEditCtl
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
		btnBrowseTempleFilepath = new System.Windows.Forms.Button();
		txtTemplateFilepath = new System.Windows.Forms.TextBox();
		label3 = new System.Windows.Forms.Label();
		fileDlg = new System.Windows.Forms.OpenFileDialog();
		txtEnumsNamespace = new System.Windows.Forms.TextBox();
		label5 = new System.Windows.Forms.Label();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new System.Drawing.Point(65, 103);
		label1.Name = "label1";
		label1.Size = new System.Drawing.Size(99, 19);
		label1.TabIndex = 0;
		label1.Text = "Output Folder:";
		// 
		// txtOutputFolder
		// 
		txtOutputFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		txtOutputFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtOutputFolder.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		txtOutputFolder.Location = new System.Drawing.Point(170, 101);
		txtOutputFolder.Name = "txtOutputFolder";
		txtOutputFolder.Size = new System.Drawing.Size(595, 25);
		txtOutputFolder.TabIndex = 1;
		txtOutputFolder.TextChanged += txtOutputFolder_TextChanged;
		// 
		// lblName
		// 
		lblName.AutoSize = true;
		lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		lblName.Location = new System.Drawing.Point(1, 2);
		lblName.Name = "lblName";
		lblName.Size = new System.Drawing.Size(124, 21);
		lblName.TabIndex = 2;
		lblName.Text = "Enum Generator";
		// 
		// ckbEnabled
		// 
		ckbEnabled.AutoSize = true;
		ckbEnabled.Location = new System.Drawing.Point(727, 12);
		ckbEnabled.Name = "ckbEnabled";
		ckbEnabled.Size = new System.Drawing.Size(76, 23);
		ckbEnabled.TabIndex = 3;
		ckbEnabled.Text = "Enabled";
		ckbEnabled.UseVisualStyleBackColor = true;
		ckbEnabled.CheckedChanged += ckbEnabled_CheckedChanged;
		// 
		// ckbInclHeader
		// 
		ckbInclHeader.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		ckbInclHeader.AutoSize = true;
		ckbInclHeader.Location = new System.Drawing.Point(587, 12);
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
		btnBrowseFolder.Location = new System.Drawing.Point(771, 102);
		btnBrowseFolder.Name = "btnBrowseFolder";
		btnBrowseFolder.Size = new System.Drawing.Size(32, 32);
		btnBrowseFolder.TabIndex = 7;
		btnBrowseFolder.Text = "...";
		btnBrowseFolder.UseVisualStyleBackColor = true;
		btnBrowseFolder.Click += btnBrowseFolder_Click;
		// 
		// btnBrowseTempleFilepath
		// 
		btnBrowseTempleFilepath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		btnBrowseTempleFilepath.Location = new System.Drawing.Point(771, 60);
		btnBrowseTempleFilepath.Name = "btnBrowseTempleFilepath";
		btnBrowseTempleFilepath.Size = new System.Drawing.Size(32, 32);
		btnBrowseTempleFilepath.TabIndex = 11;
		btnBrowseTempleFilepath.Text = "...";
		btnBrowseTempleFilepath.UseVisualStyleBackColor = true;
		btnBrowseTempleFilepath.Click += btnBrowseTempleFilepath_Click;
		// 
		// txtTemplateFilepath
		// 
		txtTemplateFilepath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		txtTemplateFilepath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtTemplateFilepath.Location = new System.Drawing.Point(170, 59);
		txtTemplateFilepath.Name = "txtTemplateFilepath";
		txtTemplateFilepath.Size = new System.Drawing.Size(595, 25);
		txtTemplateFilepath.TabIndex = 10;
		txtTemplateFilepath.TextChanged += txtTemplateFilepath_TextChanged;
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Location = new System.Drawing.Point(73, 60);
		label3.Name = "label3";
		label3.Size = new System.Drawing.Size(91, 19);
		label3.TabIndex = 9;
		label3.Text = "Template File:";
		// 
		// fileDlg
		// 
		fileDlg.DefaultExt = "*.tmpl";
		fileDlg.Filter = "Template file (*.tmpl)|*.tmpl| All Files (*.*)|*.*";
		// 
		// txtEnumsNamespace
		// 
		txtEnumsNamespace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		txtEnumsNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtEnumsNamespace.Location = new System.Drawing.Point(170, 141);
		txtEnumsNamespace.Name = "txtEnumsNamespace";
		txtEnumsNamespace.Size = new System.Drawing.Size(595, 25);
		txtEnumsNamespace.TabIndex = 17;
		txtEnumsNamespace.TextChanged += txtEnumsNamespace_TextChanged;
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Font = new System.Drawing.Font("Segoe UI", 10F);
		label5.Location = new System.Drawing.Point(37, 142);
		label5.Name = "label5";
		label5.Size = new System.Drawing.Size(127, 19);
		label5.TabIndex = 16;
		label5.Text = "Enums Namespace:";
		label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// EnumGenEditCtl
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		Controls.Add(txtEnumsNamespace);
		Controls.Add(label5);
		Controls.Add(btnBrowseTempleFilepath);
		Controls.Add(txtTemplateFilepath);
		Controls.Add(label3);
		Controls.Add(btnBrowseFolder);
		Controls.Add(ckbInclHeader);
		Controls.Add(ckbEnabled);
		Controls.Add(lblName);
		Controls.Add(txtOutputFolder);
		Controls.Add(label1);
		Font = new System.Drawing.Font("Segoe UI", 10F);
		Name = "EnumGenEditCtl";
		Size = new System.Drawing.Size(849, 215);
		Load += EnumGenEditCtl_Load;
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
	private System.Windows.Forms.Button btnBrowseTempleFilepath;
	private System.Windows.Forms.TextBox txtTemplateFilepath;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.OpenFileDialog fileDlg;
	private System.Windows.Forms.TextBox txtEnumsNamespace;
	private System.Windows.Forms.Label label5;
}
