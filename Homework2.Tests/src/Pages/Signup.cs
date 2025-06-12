using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public class Signup(IPage page) : PageBase(page)
{
    protected override string PageUrl => "https://app.taxually.com/verify";

    public async Task GoToAsync(string url)
    {
        await CleanPage.GotoAsync(url);
    }
}