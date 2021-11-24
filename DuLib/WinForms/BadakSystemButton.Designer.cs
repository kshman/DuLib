namespace Du.WinForms
{
	partial class BadakSystemButton
	{
		/// <summary> 
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary> 
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.MinButton = new Du.WinForms.MinMaxCloseButton();
			this.MaxButton = new Du.WinForms.MinMaxCloseButton();
			this.CloseButton = new Du.WinForms.MinMaxCloseButton();
			this.SuspendLayout();
			// 
			// MinButton
			// 
			this.MinButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MinButton.BZBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.MinButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MinButton.ForeColor = System.Drawing.Color.White;
			this.MinButton.Location = new System.Drawing.Point(12, 0);
			this.MinButton.Margin = new System.Windows.Forms.Padding(4);
			this.MinButton.MinMaxCloseState = Du.WinForms.MinMaxCloseState.Minimize;
			this.MinButton.MouseClickColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(160)))));
			this.MinButton.MouseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.MinButton.Name = "MinButton";
			this.MinButton.Size = new System.Drawing.Size(42, 28);
			this.MinButton.TabIndex = 2;
			this.MinButton.TextLocation = new System.Drawing.Point(6, 10);
			this.MinButton.UseVisualStyleBackColor = true;
			this.MinButton.Click += new System.EventHandler(this.MinButton_Click);
			// 
			// MaxButton
			// 
			this.MaxButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MaxButton.BZBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.MaxButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.MaxButton.ForeColor = System.Drawing.Color.White;
			this.MaxButton.Location = new System.Drawing.Point(59, 0);
			this.MaxButton.Margin = new System.Windows.Forms.Padding(4);
			this.MaxButton.MinMaxCloseState = Du.WinForms.MinMaxCloseState.Normal;
			this.MaxButton.MouseClickColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(160)))));
			this.MaxButton.MouseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.MaxButton.Name = "MaxButton";
			this.MaxButton.Size = new System.Drawing.Size(42, 28);
			this.MaxButton.TabIndex = 1;
			this.MaxButton.TextLocation = new System.Drawing.Point(8, 6);
			this.MaxButton.UseVisualStyleBackColor = true;
			this.MaxButton.Click += new System.EventHandler(this.MaxButton_Click);
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.BZBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseButton.ForeColor = System.Drawing.Color.White;
			this.CloseButton.Location = new System.Drawing.Point(107, 0);
			this.CloseButton.Margin = new System.Windows.Forms.Padding(4);
			this.CloseButton.MinMaxCloseState = Du.WinForms.MinMaxCloseState.Close;
			this.CloseButton.MouseClickColor = System.Drawing.Color.Tomato;
			this.CloseButton.MouseHoverColor = System.Drawing.Color.Red;
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(42, 28);
			this.CloseButton.TabIndex = 0;
			this.CloseButton.TextLocation = new System.Drawing.Point(8, 6);
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// BadakSystemButton
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.MinButton);
			this.Controls.Add(this.MaxButton);
			this.Controls.Add(this.CloseButton);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MaximumSize = new System.Drawing.Size(150, 30);
			this.MinimumSize = new System.Drawing.Size(150, 30);
			this.Name = "BadakSystemButton";
			this.Size = new System.Drawing.Size(150, 30);
			this.ResumeLayout(false);

		}

		#endregion

		private MinMaxCloseButton MinButton;
		private MinMaxCloseButton MaxButton;
		private MinMaxCloseButton CloseButton;
	}
}
