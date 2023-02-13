using Microsoft.JSInterop;

namespace Du.Blazor.Services;

/// <summary>
/// 쿠키 서비스. <br/>
/// 이거 쓰다 오류나면 자바스크립트 추가 안했을 가능성이 크다
/// </summary>
internal class CookieService : ICookieService
{
	private IDictionary<string, string>? _cookies;
	private readonly IJSRuntime _js;

	public CookieService(IJSRuntime js) =>
		_js = js;

	private async Task WriteAsync(string name, string value, int? days = null) =>
		await _js.InvokeAsync<string>("DUCKIE.wr", name, value, days);

	private async Task<IDictionary<string, string>> ReadAsync() =>
		(await _js.InvokeAsync<string>("DUCKIE.rd"))
		.Split(';')
		.Select(x => x.Split('='))
		.ToDictionary(k => k.First().Trim(), v => v.Last().Trim());

	private async Task<IDictionary<string, string>> InstanceAsync(bool refresh = false)
	{
		if (refresh)
			_cookies = await ReadAsync();
		else
			_cookies ??= await ReadAsync();
		return _cookies;
	}

	// 얻기
	public async Task<string?> GetAsync(string name, bool refresh = false) =>
		(await InstanceAsync(refresh)).TryGetValue(name, out var value) ? value : null;

	// 쓰기
	public async Task SetAsync(string name, string value, int? days = null) =>
		await WriteAsync(name, value, days);
}

/// <summary>
/// 쿠키 서비스 인터페이스
/// </summary>
public interface ICookieService
{
	/// <summary>
	/// 쿠키를 얻자
	/// </summary>
	/// <param name="name">쿠키 이름</param>
	/// <param name="refresh">내부 데이터를 갱신하려면 true</param>
	/// <returns>얻은 값. 없으면 당근 null</returns>
	Task<string?> GetAsync(string name, bool refresh = false);
	/// <summary>
	/// 쿠키를 넣자
	/// </summary>
	/// <param name="name">쿠키 이름</param>
	/// <param name="value">넣을 값</param>
	/// <param name="days">보존 기간. 기본값이 null로 무제한일껄</param>
	/// <returns></returns>
	Task SetAsync(string name, string value, int? days = null);
}

/// <summary>
/// 쿠키 서비스의 서비스
/// </summary>
public static class CookieServiceExtension
{
	public static IServiceCollection AddDuCookieService(this IServiceCollection services)
	{
		if (services==null)
			throw new ArgumentNullException(nameof(services));

		services.AddTransient<ICookieService, CookieService>();

		return services;
	}
}
