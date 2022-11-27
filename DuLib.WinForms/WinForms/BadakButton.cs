using System.Drawing.Drawing2D;

namespace Du.WinForms;

/// <summary>
/// 바닥 버튼
/// </summary>
public class BadakButton : Button
{
	/// <summary>
	/// 활성화 모양
	/// </summary>
	[DefaultValue(false), Browsable(true), 
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), 
	 Description("활성화 모양을 설정합니다. 활성화일 때는 ActiveColor에 지정한 색깔이 모서리에 나옵니다")]
	public bool ActiveStyle { get; set; }

	/// <summary>
	/// 활성화 모양일 때 색깔
	/// </summary>
	[Browsable(true), 
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), 
	 Description("활성화 모양일 때 표시할 색깔입니다")]
	public Color ActiveColor { get; set; } = Color.Aquamarine;

	/// <summary>
	/// 바닥 버튼 생성자
	/// </summary>
	public BadakButton()
	{
		base.BackColor = Color.FromArgb(20, 20, 20);
		base.ForeColor = Color.White;

		FlatStyle = FlatStyle.Flat;
		FlatAppearance.BorderColor = Color.FromArgb(5, 5, 5);
	}

	/// <summary>
	/// 페인트 이벤트
	/// </summary>
	/// <param name="pevent"></param>
	protected override void OnPaint(PaintEventArgs pevent)
	{
		base.OnPaint(pevent);

		if (ActiveStyle)
		{
			var rt = ClientRectangle;
			rt.Inflate(-1, -1);

			var pen = new Pen(ActiveColor, 2.0f);
			pen.Alignment = PenAlignment.Center;

			pevent.Graphics.DrawRectangle(pen, rt);
		}
	}
}
