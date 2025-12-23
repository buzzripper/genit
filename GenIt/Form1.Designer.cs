namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			button1 = new Button();
			lbxEntities = new ListBox();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new Point(64, 23);
			button1.Name = "button1";
			button1.Size = new Size(179, 59);
			button1.TabIndex = 0;
			button1.Text = "Read EDML";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// lbxEntities
			// 
			lbxEntities.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			lbxEntities.FormattingEnabled = true;
			lbxEntities.IntegralHeight = false;
			lbxEntities.Location = new Point(38, 139);
			lbxEntities.Name = "lbxEntities";
			lbxEntities.Size = new Size(957, 790);
			lbxEntities.TabIndex = 1;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1030, 966);
			Controls.Add(lbxEntities);
			Controls.Add(button1);
			Name = "Form1";
			Text = "Form1";
			ResumeLayout(false);
		}

		#endregion

		private Button button1;
		private ListBox lbxEntities;
	}
}
