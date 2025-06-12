using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public abstract class PageBase(IPage page)
{
    protected abstract string PageUrl { get; }
    protected IPage CleanPage => page;

    public async Task VerifyPageUrlAsync()
    {
        await Assertions.Expect(CleanPage).ToHaveURLAsync(PageUrl);
    }
}