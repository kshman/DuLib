namespace DuLib.WinForms
{
	partial class BadakForm
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.BadakTopPanel = new System.Windows.Forms.Panel();
			this.BadakMinMaxClosePanel = new System.Windows.Forms.Panel();
			this.BadakMinButton = new DuLib.WinForms.MinMaxCloseButton();
			this.BadakCloseButton = new DuLib.WinForms.MinMaxCloseButton();
			this.BadakMaxButton = new DuLib.WinForms.MinMaxCloseButton();
			this.BadakText = new System.Windows.Forms.Label();
			this.BadakTopPanel.SuspendLayout();
			this.BadakMinMaxClosePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// BadakTopPanel
			// 
			this.BadakTopPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.BadakTopPanel.Controls.Add(this.BadakMinMaxClosePanel);
			this.BadakTopPanel.Controls.Add(this.BadakText);
			this.BadakTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.BadakTopPanel.Location = new System.Drawing.Point(0, 0);
			this.BadakTopPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.BadakTopPanel.Name = "BadakTopPanel";
			this.BadakTopPanel.Size = new System.Drawing.Size(716, 70);
			this.BadakTopPanel.TabIndex = 1;
			this.BadakTopPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BadakTopPanel_MouseDown);
			this.BadakTopPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BadakTopPanel_MouseMove);
			this.BadakTopPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BadakTopPanel_MouseUp);
			// 
			// BadakMinMaxClosePanel
			// 
			this.BadakMinMaxClosePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BadakMinMaxClosePanel.Controls.Add(this.BadakMinButton);
			this.BadakMinMaxClosePanel.Controls.Add(this.BadakCloseButton);
			this.BadakMinMaxClosePanel.Controls.Add(this.BadakMaxButton);
			this.BadakMinMaxClosePanel.Location = new System.Drawing.Point(593, 0);
			this.BadakMinMaxClosePanel.Margin = new System.Windows.Forms.Padding(0);
			this.BadakMinMaxClosePanel.Name = "BadakMinMaxClosePanel";
			this.BadakMinMaxClosePanel.Size = new System.Drawing.Size(123, 26);
			this.BadakMinMaxClosePanel.TabIndex = 0;
			// 
			// BadakMinButton
			// 
			this.BadakMinButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BadakMinButton.BZBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.BadakMinButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BadakMinButton.ForeColor = System.Drawing.Color.White;
			this.BadakMinButton.Location = new System.Drawing.Point(9, 0);
			this.BadakMinButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.BadakMinButton.MinMaxCloseState = DuLib.WinForms.MinMaxCloseState.Minimize;
			this.BadakMinButton.MouseClickColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(160)))));
			this.BadakMinButton.MouseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.BadakMinButton.Name = "BadakMinButton";
			this.BadakMinButton.Size = new System.Drawing.Size(36, 22);
			this.BadakMinButton.TabIndex = 2;
			this.BadakMinButton.Text = "_";
			this.BadakMinButton.TextLocation = new System.Drawing.Point(6, 10);
			this.BadakMinButton.UseVisualStyleBackColor = true;
			this.BadakMinButton.Click += new System.EventHandler(this.BadakMinButton_Click);
			// 
			// BadakCloseButton
			// 
			this.BadakCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BadakCloseButton.BZBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.BadakCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BadakCloseButton.ForeColor = System.Drawing.Color.White;
			this.BadakCloseButton.Location = new System.Drawing.Point(87, 0);
			this.BadakCloseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.BadakCloseButton.MinMaxCloseState = DuLib.WinForms.MinMaxCloseState.Close;
			this.BadakCloseButton.MouseClickColor = System.Drawing.Color.Tomato;
			this.BadakCloseButton.MouseHoverColor = System.Drawing.Color.Red;
			this.BadakCloseButton.Name = "BadakCloseButton";
			this.BadakCloseButton.Size = new System.Drawing.Size(36, 22);
			this.BadakCloseButton.TabIndex = 4;
			this.BadakCloseButton.Text = "X";
			this.BadakCloseButton.TextLocation = new System.Drawing.Point(8, 6);
			this.BadakCloseButton.UseVisualStyleBackColor = true;
			this.BadakCloseButton.Click += new System.EventHandler(this.BadakCloseButton_Click);
			// 
			// BadakMaxButton
			// 
			this.BadakMaxButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BadakMaxButton.BZBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.BadakMaxButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BadakMaxButton.ForeColor = System.Drawing.Color.White;
			this.BadakMaxButton.Location = new System.Drawing.Point(48, 0);
			this.BadakMaxButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.BadakMaxButton.MinMaxCloseState = DuLib.WinForms.MinMaxCloseState.Normal;
			this.BadakMaxButton.MouseClickColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(160)))));
			this.BadakMaxButton.MouseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.BadakMaxButton.Name = "BadakMaxButton";
			this.BadakMaxButton.Size = new System.Drawing.Size(36, 22);
			this.BadakMaxButton.TabIndex = 3;
			this.BadakMaxButton.Text = "#";
			this.BadakMaxButton.TextLocation = new System.Drawing.Point(8, 6);
			this.BadakMaxButton.UseVisualStyleBackColor = true;
			this.BadakMaxButton.Click += new System.EventHandler(this.BadakMaxButton_Click);
			// 
			// BadakText
			// 
			this.BadakText.AutoSize = true;
			this.BadakText.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BadakText.ForeColor = System.Drawing.Color.White;
			this.BadakText.Location = new System.Drawing.Point(33, 19);
			this.BadakText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.BadakText.Name = "BadakText";
			this.BadakText.Size = new System.Drawing.Size(149, 39);
			this.BadakText.TabIndex = 5;
			this.BadakText.Text = "Title text";
			this.BadakText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BadakTopPanel_MouseDown);
			this.BadakText.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BadakTopPanel_MouseMove);
			this.BadakText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BadakTopPanel_MouseUp);
			// 
			// BadakForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.ClientSize = new System.Drawing.Size(716, 584);
			this.Controls.Add(this.BadakTopPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.MinimumSize = new System.Drawing.Size(400, 250);
			this.Name = "BadakForm";
			this.Text = "BadakForm";
			this.Load += new System.EventHandler(this.BasicForm_Load);
			this.BadakTopPanel.ResumeLayout(false);
			this.BadakTopPanel.PerformLayout();
			this.BadakMinMaxClosePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel BadakTopPanel;
		private System.Windows.Forms.Label BadakText;
		private DuLib.WinForms.MinMaxCloseButton BadakMinButton;
		private DuLib.WinForms.MinMaxCloseButton BadakMaxButton;
		private DuLib.WinForms.MinMaxCloseButton BadakCloseButton;
		protected System.Windows.Forms.Panel BadakMinMaxClosePanel;
	}
}
