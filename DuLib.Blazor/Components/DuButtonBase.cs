using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace DuLib.Blazor;

public class DuButtonBase : DuComponentParent
{
	[CascadingParameter] public EditContext? EditContext { get; set; }

	/// <summary>버튼 타입. <see cref="ButtonType"/> 참고</summary>
	[Parameter] public ButtonType? Type { get; set; }
	/// <summary>레이아웃 타입. <see cref="ComponentColor"/> 참고</summary>
	[Parameter] public ComponentColor Color { get; set; } = ComponentColor.Primary;
	/// <summary>컴포넌트 크기. <see cref="ComponentSize"/> 참고</summary>
	[Parameter] public ComponentSize Size { get; set; } = ComponentSize.Medium;
	/// <summary>아웃라인 적용.</summary>
	[Parameter] public bool Outline { get; set; }

	/// <summary>마우스 눌린 이벤트 지정.</summary>
	[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
	/// <summary>에디트 폼 ValidClick.</summary>
	[Parameter] public EventCallback<MouseEventArgs> OnValidClick { get; set; }
	/// <summary>에디트 폼 InvalidClick.</summary>
	[Parameter] public EventCallback<MouseEventArgs> OnInvalidClick { get; set; }

	private bool _handle_click;

	//
	protected override void OnInitialized()
	{
		Type ??= EditContext is null ? ButtonType.Button : ButtonType.Submit;

		base.OnInitialized();
	}

	//
	protected virtual Task InvokeOnClick(MouseEventArgs e) =>
		OnClick.InvokeAsync(e);

	//
	protected virtual Task InvokeOnValidClick(MouseEventArgs e) =>
		OnValidClick.InvokeAsync(e);

	//
	protected virtual Task InvokeOnInvalidClick(MouseEventArgs e) =>
		OnInvalidClick.InvokeAsync(e);

	// 마우스 핸들러
	protected async Task HandleOnClick(MouseEventArgs e)
	{
		if (!Enabled)
			return;

		if (!_handle_click)
		{
			_handle_click = true;

			if (OnClick.HasDelegate)
				await InvokeOnClick(e);
			else if (Type == ButtonType.Submit && EditContext != null &&
					 (OnValidClick.HasDelegate || OnInvalidClick.HasDelegate))
			{
				switch (EditContext.Validate())
				{
					case true when OnValidClick.HasDelegate:
						await InvokeOnValidClick(e);
						break;

					case false when OnInvalidClick.HasDelegate:
						await InvokeOnInvalidClick(e);
						break;
				}
			}

			_handle_click = false;
		}
	}

	//
	protected string? GetTypeHtml() =>
		Type switch
		{
			ButtonType.Button => "button",
			ButtonType.Submit => "submit",
			ButtonType.Reset => "reset",
			_ => null,
		};

	//
	protected string? GetColorCss()
	{
		if (Outline)
		{
			return Color switch
			{
				ComponentColor.Primary => "btn-outline-primary",
				ComponentColor.Secondary => "btn-outline-secondary",
				ComponentColor.Success => "btn-outline-success",
				ComponentColor.Danger => "btn-outline-danger",
				ComponentColor.Warning => "btn-outline-warning",
				ComponentColor.Info => "btn-outline-dark",
				ComponentColor.Light => "btn-outline-light",
				ComponentColor.Link => "btn-link",
				_ => null,
			};
		}
		else
		{
			return Color switch
			{
				ComponentColor.Primary => "btn-primary",
				ComponentColor.Secondary => "btn-secondary",
				ComponentColor.Success => "btn-success",
				ComponentColor.Danger => "btn-danger",
				ComponentColor.Warning => "btn-warning",
				ComponentColor.Info => "btn-dark",
				ComponentColor.Light => "btn-light",
				ComponentColor.Link => "btn-link",
				_ => null,
			};
		}
	}
}
