using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Homework2.Tests.Pages
{
    public class SignupPage
    {
        private readonly IPage _page;

        public SignupPage(IPage page)
        {
            _page = page;
        }

        public async Task GoToAsync(string url)
        {
            await _page.GotoAsync(url);
        }

        // Add any specific elements/methods for the signup page if needed for this flow
    }
}