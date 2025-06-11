using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public class LoginPage : BasePage
{
    protected override string PageUrl => "https://app.taxually.com/login";
    private readonly ILocator _emailInput;
    private readonly ILocator _passwordInput;
    private readonly ILocator _loginButton;

    public LoginPage(IPage page) : base(page)
    {
        _emailInput = Page.Locator("[name='Email Address']"); // Adjust locators as per your app
        _passwordInput = Page.Locator("[name='Password']");
        _loginButton = Page.Locator("#next");
    }

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