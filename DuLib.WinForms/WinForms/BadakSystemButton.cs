namespace Du.WinForms;

/// <summary>
/// 시스템 컨트롤 버튼
/// </summary>
public partial class BadakSystemButton : UserControl
{
	private BadakStateButton? MinButton;
	private BadakStateButton? MaxButton;
	private BadakStateButton? CloseButton;

	/// <summary>
	/// 필수 디자이너 변수입니다.
	/// </summary>
	private System.ComponentModel.IContainer? components = null;

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

	#region Windows Form 디자이너에서 생성한 코드

	/// <summary>
	/// 디자이너 지원에 필요한 메서드입니다.
	/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
	/// </summary>
	private void InitializeComponent()
	{
		this.MinButton = new Du.WinForms.BadakStateButton();
		this.MaxButton = new Du.WinForms.BadakStateButton();
		this.CloseButton = new Du.WinForms.BadakStateButton();
		this.SuspendLayout();
		// 
		// MinButton
		// 
		this.MinButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this.MinButton.BadakBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
		this.MinButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MinButton.ForeColor = System.Drawing.Color.White;
		this.MinButton.Location = new System.Drawing.Point(12, 0);
		this.MinButton.Margin = new System.Windows.Forms.Padding(4);
		this.MinButton.BadakState = Du.WinForms.BadakState.Minimize;
		this.MinButton.MouseClickColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(160)))));
		this.MinButton.MouseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
		this.MinButton.Name = "MinButton";
		this.MinButton.Size = new System.Drawing.Size(42, 28);
		this.MinButton.TabIndex = 2;
		this.MinButton.ShapeLocation = new System.Drawing.Point(6, 10);
		this.MinButton.UseVisualStyleBackColor = true;
		this.MinButton.Click += new System.EventHandler(this.MinButton_Click);
		// 
		// MaxButton
		// 
		this.MaxButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this.MaxButton.BadakBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
		this.MaxButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MaxButton.ForeColor = System.Drawing.Color.White;
		this.MaxButton.Location = new System.Drawing.Point(59, 0);
		this.MaxButton.Margin = new System.Windows.Forms.Padding(4);
		this.MaxButton.BadakState = Du.WinForms.BadakState.Normal;
		this.MaxButton.MouseClickColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(160)))));
		this.MaxButton.MouseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
		this.MaxButton.Name = "MaxButton";
		this.MaxButton.Size = new System.Drawing.Size(42, 28);
		this.MaxButton.TabIndex = 1;
		this.MaxButton.ShapeLocation = new System.Drawing.Point(8, 6);
		this.MaxButton.UseVisualStyleBackColor = true;
		this.MaxButton.Click += new System.EventHandler(this.MaxButton_Click);
		// 
		// CloseButton
		// 
		this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
		this.CloseButton.BadakBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
		this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.CloseButton.ForeColor = System.Drawing.Color.White;
		this.CloseButton.Location = new System.Drawing.Point(107, 0);
		this.CloseButton.Margin = new System.Windows.Forms.Padding(4);
		this.CloseButton.BadakState = Du.WinForms.BadakState.Close;
		this.CloseButton.MouseClickColor = System.Drawing.Color.Tomato;
		this.CloseButton.MouseHoverColor = System.Drawing.Color.Red;
		this.CloseButton.Name = "CloseButton";
		this.CloseButton.Size = new System.Drawing.Size(42, 28);
		this.CloseButton.TabIndex = 0;
		this.CloseButton.ShapeLocation = new System.Drawing.Point(8, 6);
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

	/// <summary>
	/// 생성자
	/// </summary>
	public BadakSystemButton()
	{
		InitializeComponent();
	}

	/// <summary>
	/// 이벤트를 받을 폼
	/// </summary>
	[Browsable(false)]
	public Form? Form { get; set; }

	/// <summary>
	/// 닫기 이벤트
	/// </summary>
	[Category("닫기 이벤트"), Description("닫기를 누르면 호출")]
	public event EventHandler? CloseOrder;

	/// <summary>
	/// 최소화를 보입니다
	/// </summary>
	#region 프로퍼티
	[Category("버튼"), Description("내리기를 보입니다")]
	public bool ShowMinimize
	{
		get => MinButton != null && MinButton.Visible;
		set
		{
			if (MinButton != null)
				MinButton.Visible = value;
		}
	}

	/// <summary>
	/// 최대화를 보입니다
	/// </summary>
	[Category("버튼"), Description("크게를 보입니다")]
	public bool ShowMaximize
	{
		get => MaxButton != null && MaxButton.Visible;
		set
		{
			if (MaxButton != null)
				MaxButton.Visible = value;
		}
	}

	/// <summary>
	/// 닫기를 보입니다
	/// </summary>
	[Category("버튼"), Description("닫기를 보입니다")]
	public bool ShowClose
	{
		get => CloseButton != null && CloseButton.Visible;
		set
		{
			if (CloseButton != null)
				CloseButton.Visible = value;
		}
	}
	#endregion

	#region UI 이벤트
	private void CloseButton_Click(object? sender, EventArgs _)
	{
		var e = new CancelEventArgs(false);
		CloseOrder?.Invoke(this, e);

		if (!e.Cancel && Form != null)
			Form.Close();
	}

	private void MaxButton_Click(object? sender, EventArgs _)
	{
		Maximize();
	}

	private void MinButton_Click(object? sender, EventArgs _)
	{
		Minimize();
	}
	#endregion

	#region 창관리 기능
	/// <summary>
	/// 최소화 한다
	/// </summary>
	public void Minimize()
	{
		if (Form == null)
			return;

		Form.WindowState = FormWindowState.Minimized;
	}

	/// <summary>
	/// 최대화/보통 전환한다
	/// </summary>
	public void Maximize()
	{
		if (Form == null)
			return;

		if (Form.WindowState == FormWindowState.Maximized)
		{
			Form.WindowState = FormWindowState.Normal;
			if (MaxButton != null)
				MaxButton.BadakState = BadakState.Normal;
		}
		else
		{
			Form.WindowState = FormWindowState.Maximized;
			if (MaxButton != null)
				MaxButton.BadakState = BadakState.Maximize;
		}
	}

	/// <summary>
	/// 강제 보틍오로
	/// </summary>
	public void ForceNormalize()
	{
		if (Form == null)
			return;

		Form.WindowState = FormWindowState.Normal;
		if (MaxButton != null)
			MaxButton.BadakState = BadakState.Normal;
	}

	/// <summary>
	/// 강제 최대로
	/// </summary>
	public void ForceMaximize()
	{
		if (Form == null)
			return;

		Form.WindowState = FormWindowState.Maximized;
		if (MaxButton != null)
			MaxButton.BadakState = BadakState.Maximize;
	}
	#endregion
}

