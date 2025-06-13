using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public abstract class PageBase(IPage page)
{
    protected abstract string PageUrl { get; }
    protected IPage Page => page;

    public async Task VerifyPageUrlAsync()
    {
        await Assertions.Expect(Page).ToHaveURLAsync(PageUrl);
    }
}