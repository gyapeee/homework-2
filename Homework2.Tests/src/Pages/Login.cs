using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public class Login : PageBase
{
    private readonly ILocator _emailInput;
    private readonly ILocator _loginButton;
    private readonly ILocator _passwordInput;

    public Login(IPage page) : base(page)
    {
        _emailInput = Page.Locator("[name='Email Address']"); // Adjust locators as per your app
        _passwordInput = Page.Locator("[name='Password']");
        _loginButton = Page.Locator("#next");
    }

    protected override string PageUrl => "https://app.taxually.com/login";

    public async Task GoToAsync(string url)
    {
        await Page.GotoAsync(url);
    }

    public async Task LoginAsync(string email, string password)
    {
        await _emailInput.FillAsync(email);
        await _passwordInput.FillAsync(password);
        await _loginButton.ClickAsync();
    }
}