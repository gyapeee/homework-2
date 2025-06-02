using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Homework2.Tests.Pages
{
    public class BusinessDetailsPage
    {
        private readonly IPage _page;
        private readonly ILocator _businessTypeDropdown;
        private readonly ILocator _companyNameInput;
        private readonly ILocator _companyEmailInput;
        private readonly ILocator _nextButton;

        public BusinessDetailsPage(IPage page)
        {
            _page = page;
            _businessTypeDropdown = _page.GetByLabel("Business type"); // Adjust locator
            _companyNameInput = _page.GetByLabel("Company name"); // Adjust locator
            _companyEmailInput = _page.GetByLabel("Company email"); // Adjust locator
            _nextButton = _page.GetByRole(AriaRole.Button, new() { Name = "Next" }); // Adjust locator
        }

        public async Task FillBusinessDetailsAsync(string businessType, string companyName, string companyEmail)
        {
            await _businessTypeDropdown.SelectOptionAsync(new[] { businessType });
            await _companyNameInput.FillAsync(companyName);
            await _companyEmailInput.FillAsync(companyEmail);
        }

        public async Task ClickNextAsync()
        {
            await _nextButton.ClickAsync();
        }
    }
}