using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Homework2.Tests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        private readonly ILocator _emailInput;
        private readonly ILocator _passwordInput;
        private readonly ILocator _loginButton;

        public LoginPage(IPage page)
        {
            _page = page;
            _emailInput = _page.Locator("[name='Email Address']"); // Adjust locators as per your app
            _passwordInput = _page.Locator("[name='Password']");
            _loginButton = _page.Locator("#next");
        }

        public async Task GoToAsync(string url)
        {
            await _page.GotoAsync(url);
        }

        public async Task LoginAsync(string email, string password)
        {
            await _emailInput.FillAsync(email);
            await _passwordInput.FillAsync(password);
            await _loginButton.ClickAsync();
        }
    }
}