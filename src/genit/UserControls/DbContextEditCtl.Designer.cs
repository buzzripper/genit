﻿namespace Dyvenix.Genit.UserControls;

partial class DbContextEditCtl
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
		lblName = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		stringListEditor1 = new StringListEditor();
		label4 = new System.Windows.Forms.Label();
		txtName = new System.Windows.Forms.TextBox();
		txtContextNamespace = new System.Windows.Forms.TextBox();
		txtEntitiesNamespace = new System.Windows.Forms.TextBox();
		txtEnumsNamespace = new System.Windows.Forms.TextBox();
		label5 = new System.Windows.Forms.Label();
		txtServicesNamespace = new System.Windows.Forms.TextBox();
		label6 = new System.Windows.Forms.Label();
		txtQueriesNamespace = new System.Windows.Forms.TextBox();
		label7 = new System.Windows.Forms.Label();
		txtControllersNamespace = new System.Windows.Forms.TextBox();
		label8 = new System.Windows.Forms.Label();
		txtApiClientsNamespace = new System.Windows.Forms.TextBox();
		label9 = new System.Windows.Forms.Label();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		label1.Location = new System.Drawing.Point(0, 0);
		label1.Name = "label1";
		label1.Size = new System.Drawing.Size(83, 21);
		label1.TabIndex = 0;
		label1.Text = "DbContext";
		// 
		// lblName
		// 
		lblName.AutoSize = true;
		lblName.Font = new System.Drawing.Font("Segoe UI", 10F);
		lblName.Location = new System.Drawing.Point(107, 54);
		lblName.Name = "lblName";
		lblName.Size = new System.Drawing.Size(48, 19);
		lblName.TabIndex = 1;
		lblName.Text = "Name:";
		lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Font = new System.Drawing.Font("Segoe UI", 10F);
		label2.Location = new System.Drawing.Point(25, 88);
		label2.Name = "label2";
		label2.Size = new System.Drawing.Size(134, 19);
		label2.TabIndex = 3;
		label2.Text = "Context Namespace:";
		label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Font = new System.Drawing.Font("Segoe UI", 10F);
		label3.Location = new System.Drawing.Point(29, 120);
		label3.Name = "label3";
		label3.Size = new System.Drawing.Size(130, 19);
		label3.TabIndex = 5;
		label3.Text = "Entities Namespace:";
		label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// stringListEditor1
		// 
		stringListEditor1.BackColor = System.Drawing.SystemColors.Control;
		stringListEditor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		stringListEditor1.Font = new System.Drawing.Font("Segoe UI", 10F);
		stringListEditor1.Location = new System.Drawing.Point(161, 322);
		stringListEditor1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		stringListEditor1.Name = "stringListEditor1";
		stringListEditor1.Size = new System.Drawing.Size(331, 152);
		stringListEditor1.TabIndex = 7;
		stringListEditor1.ToolbarDockStyle = System.Windows.Forms.DockStyle.Right;
		// 
		// label4
		// 
		label4.AutoSize = true;
		label4.Font = new System.Drawing.Font("Segoe UI", 10F);
		label4.Location = new System.Drawing.Point(40, 323);
		label4.Name = "label4";
		label4.Size = new System.Drawing.Size(119, 19);
		label4.TabIndex = 8;
		label4.Text = "Additional Usings:";
		label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// txtName
		// 
		txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtName.Location = new System.Drawing.Point(161, 53);
		txtName.Name = "txtName";
		txtName.Size = new System.Drawing.Size(331, 25);
		txtName.TabIndex = 11;
		txtName.TextChanged += txtName_TextChanged;
		// 
		// txtContextNamespace
		// 
		txtContextNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtContextNamespace.Location = new System.Drawing.Point(161, 86);
		txtContextNamespace.Name = "txtContextNamespace";
		txtContextNamespace.Size = new System.Drawing.Size(331, 25);
		txtContextNamespace.TabIndex = 12;
		txtContextNamespace.TextChanged += txtContextNamespace_TextChanged;
		// 
		// txtEntitiesNamespace
		// 
		txtEntitiesNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtEntitiesNamespace.Location = new System.Drawing.Point(161, 119);
		txtEntitiesNamespace.Name = "txtEntitiesNamespace";
		txtEntitiesNamespace.Size = new System.Drawing.Size(331, 25);
		txtEntitiesNamespace.TabIndex = 13;
		txtEntitiesNamespace.TextChanged += txtEntitiesNamespace_TextChanged;
		// 
		// txtEnumsNamespace
		// 
		txtEnumsNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtEnumsNamespace.Location = new System.Drawing.Point(161, 152);
		txtEnumsNamespace.Name = "txtEnumsNamespace";
		txtEnumsNamespace.Size = new System.Drawing.Size(331, 25);
		txtEnumsNamespace.TabIndex = 15;
		txtEnumsNamespace.TextChanged += txtEnumsNamespace_TextChanged;
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Font = new System.Drawing.Font("Segoe UI", 10F);
		label5.Location = new System.Drawing.Point(32, 154);
		label5.Name = "label5";
		label5.Size = new System.Drawing.Size(127, 19);
		label5.TabIndex = 14;
		label5.Text = "Enums Namespace:";
		label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// txtServicesNamespace
		// 
		txtServicesNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtServicesNamespace.Location = new System.Drawing.Point(161, 185);
		txtServicesNamespace.Name = "txtServicesNamespace";
		txtServicesNamespace.Size = new System.Drawing.Size(331, 25);
		txtServicesNamespace.TabIndex = 17;
		txtServicesNamespace.TextChanged += txtServicesNamespace_TextChanged;
		// 
		// label6
		// 
		label6.AutoSize = true;
		label6.Font = new System.Drawing.Font("Segoe UI", 10F);
		label6.Location = new System.Drawing.Point(25, 188);
		label6.Name = "label6";
		label6.Size = new System.Drawing.Size(134, 19);
		label6.TabIndex = 16;
		label6.Text = "Services Namespace:";
		label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// txtQueriesNamespace
		// 
		txtQueriesNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtQueriesNamespace.Location = new System.Drawing.Point(161, 218);
		txtQueriesNamespace.Name = "txtQueriesNamespace";
		txtQueriesNamespace.Size = new System.Drawing.Size(331, 25);
		txtQueriesNamespace.TabIndex = 19;
		txtQueriesNamespace.TextChanged += txtQueriesNamespace_TextChanged;
		// 
		// label7
		// 
		label7.AutoSize = true;
		label7.Font = new System.Drawing.Font("Segoe UI", 10F);
		label7.Location = new System.Drawing.Point(26, 219);
		label7.Name = "label7";
		label7.Size = new System.Drawing.Size(133, 19);
		label7.TabIndex = 18;
		label7.Text = "Queries Namespace:";
		label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// txtControllersNamespace
		// 
		txtControllersNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtControllersNamespace.Location = new System.Drawing.Point(161, 251);
		txtControllersNamespace.Name = "txtControllersNamespace";
		txtControllersNamespace.Size = new System.Drawing.Size(331, 25);
		txtControllersNamespace.TabIndex = 21;
		txtControllersNamespace.TextChanged += txtControllersNamespace_TextChanged;
		// 
		// label8
		// 
		label8.AutoSize = true;
		label8.Font = new System.Drawing.Font("Segoe UI", 10F);
		label8.Location = new System.Drawing.Point(6, 252);
		label8.Name = "label8";
		label8.Size = new System.Drawing.Size(153, 19);
		label8.TabIndex = 20;
		label8.Text = "Controllers Namespace:";
		label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// txtApiClientsNamespace
		// 
		txtApiClientsNamespace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		txtApiClientsNamespace.Location = new System.Drawing.Point(161, 285);
		txtApiClientsNamespace.Name = "txtApiClientsNamespace";
		txtApiClientsNamespace.Size = new System.Drawing.Size(331, 25);
		txtApiClientsNamespace.TabIndex = 23;
		txtApiClientsNamespace.TextChanged += txtApiClientsNamespace_TextChanged;
		// 
		// label9
		// 
		label9.AutoSize = true;
		label9.Font = new System.Drawing.Font("Segoe UI", 10F);
		label9.Location = new System.Drawing.Point(6, 287);
		label9.Name = "label9";
		label9.Size = new System.Drawing.Size(151, 19);
		label9.TabIndex = 22;
		label9.Text = "Api Clients Namespace:";
		label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 
		// DbContextEditCtl
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		BackColor = System.Drawing.SystemColors.ActiveBorder;
		Controls.Add(txtApiClientsNamespace);
		Controls.Add(label9);
		Controls.Add(txtControllersNamespace);
		Controls.Add(label8);
		Controls.Add(txtQueriesNamespace);
		Controls.Add(label7);
		Controls.Add(txtServicesNamespace);
		Controls.Add(label6);
		Controls.Add(txtEnumsNamespace);
		Controls.Add(label5);
		Controls.Add(txtEntitiesNamespace);
		Controls.Add(txtContextNamespace);
		Controls.Add(txtName);
		Controls.Add(label4);
		Controls.Add(stringListEditor1);
		Controls.Add(label1);
		Controls.Add(label3);
		Controls.Add(label2);
		Controls.Add(lblName);
		Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		Name = "DbContextEditCtl";
		Size = new System.Drawing.Size(623, 550);
		Load += DbContextEditCtl_Load;
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Label lblName;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label3;
	private StringListEditor stringListEditor1;
	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.TextBox txtName;
	private System.Windows.Forms.TextBox txtContextNamespace;
	private System.Windows.Forms.TextBox txtEntitiesNamespace;
	private System.Windows.Forms.TextBox txtEnumsNamespace;
	private System.Windows.Forms.Label label5;
	private System.Windows.Forms.TextBox txtServicesNamespace;
	private System.Windows.Forms.Label label6;
	private System.Windows.Forms.TextBox txtQueriesNamespace;
	private System.Windows.Forms.Label label7;
	private System.Windows.Forms.TextBox txtControllersNamespace;
	private System.Windows.Forms.Label label8;
	private System.Windows.Forms.TextBox txtApiClientsNamespace;
	private System.Windows.Forms.Label label9;
}
