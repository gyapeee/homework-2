using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Homework2.Tests.Pages
{
    public class BusinessDetailsPage : PageTest
    {
        private readonly IPage _page;
        private readonly ILocator _nextButton;
        private readonly ILocator _businessTypeEcommerceRadioButton;
        private readonly ILocator _businessLegalStatusDropdown;
        private readonly ILocator _individualOption;
        private readonly ILocator _countryDropdown;
        private readonly ILocator _nameInput;
        private readonly ILocator _countryOption;
        private readonly ILocator _streetInput;
        private readonly ILocator _houseNumber;
        private readonly ILocator _cityInput;
        private readonly ILocator _zipCodeInput;

        public BusinessDetailsPage(IPage page)
        {
            _page = page;
            _businessTypeEcommerceRadioButton = _page.GetByTestId("about-your-business_company-type-label-e-commerce");
            _businessLegalStatusDropdown = _page.GetByTestId("about-your-business_legal-status-select");
            _individualOption = _page.Locator("ng-dropdown-panel span",
                new() { HasTextString = "Individual/Sole Proprietor" });
            _countryDropdown = _page.GetByTestId("about-your-business_establishment-country-id-select");
            _nameInput = _page.GetByTestId("about-your-business_legal-name-of-business-input").Locator("input");
            _countryOption = _page.Locator("ng-dropdown-panel span", new() { HasTextString = "Hungary" });
            _streetInput = _page.GetByTestId("about-your-business_street-input").Locator("input");
            _houseNumber = _page.GetByTestId("about-your-business_house-number-input").Locator("input");
            _cityInput = _page.GetByTestId("about-your-business_city-input").Locator("input");
            _zipCodeInput = _page.GetByTestId("about-your-business_zip-code-input").Locator("input");
            _nextButton = _page.GetByTestId("about-your-business_next-btn");
        }

        public async Task FillBusinessDetailsAsync()
        {
            await _businessTypeEcommerceRadioButton.ClickAsync();
            await _businessLegalStatusDropdown.ClickAsync();
            await _individualOption.ClickAsync();
            await Expect(_nameInput).ToBeVisibleAsync(); // Wait for to be visible, then fill
            await _nameInput.FillAsync("David Kormos");
            await _countryDropdown.ClickAsync();
            await _countryOption.ClickAsync();
            await _streetInput.FillAsync("IX. Körzet");
            await _houseNumber.FillAsync("1435");
            await _cityInput.FillAsync("Ásotthalom");
            await _zipCodeInput.FillAsync("6783");
        }

        public async Task ClickNextAsync()
        {
            await _nextButton.ClickAsync();
        }
    }
}