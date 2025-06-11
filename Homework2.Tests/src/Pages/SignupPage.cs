using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public class SignupPage(IPage page) : BasePage(page)
{
    protected override string PageUrl => "https://app.taxually.com/verify";

    public async Task GoToAsync(string url)
    {
        await Page.GotoAsync(url);
    }
}