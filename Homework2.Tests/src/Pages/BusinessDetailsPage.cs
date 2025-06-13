using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public class BusinessDetails : PageBase
{
    private readonly ILocator _businessLegalStatusDropdown;
    private readonly ILocator _businessTypeEcommerceRadioButton;
    private readonly ILocator _cityInput;
    private readonly ILocator _countryDropdown;
    private readonly ILocator _countryOption;
    private readonly ILocator _houseNumber;
    private readonly ILocator _individualOption;
    private readonly ILocator _nameInput;

    private readonly ILocator _nextButton;
    private readonly ILocator _streetInput;
    private readonly ILocator _zipCodeInput;

    public BusinessDetails(IPage page) : base(page)
    {
        _businessTypeEcommerceRadioButton = Page.GetByTestId("about-your-business_company-type-label-e-commerce");
        _businessLegalStatusDropdown = Page.GetByTestId("about-your-business_legal-status-select");
        _individualOption = Page.Locator("ng-dropdown-panel span",
            new PageLocatorOptions { HasTextString = "Individual/Sole Proprietor" });
        _countryDropdown = Page.GetByTestId("about-your-business_establishment-country-id-select");
        _nameInput = Page.GetByTestId("about-your-business_legal-name-of-business-input").Locator("input");
        _countryOption =
            Page.Locator("ng-dropdown-panel span", new PageLocatorOptions { HasTextString = "Hungary" });
        _streetInput = Page.GetByTestId("about-your-business_street-input").Locator("input");
        _houseNumber = Page.GetByTestId("about-your-business_house-number-input").Locator("input");
        _cityInput = Page.GetByTestId("about-your-business_city-input").Locator("input");
        _zipCodeInput = Page.GetByTestId("about-your-business_zip-code-input").Locator("input");
        _nextButton = Page.GetByTestId("about-your-business_next-btn");
    }

    protected override string PageUrl =>
        "https://app.taxually.com/app/registration/select-services/about-your-business";

    public async Task FillBusinessDetailsAsync()
    {
        await _businessTypeEcommerceRadioButton.ClickAsync();
        await _businessLegalStatusDropdown.ClickAsync();
        await _individualOption.ClickAsync();
        await Assertions.Expect(_nameInput).ToBeVisibleAsync(); // Wait for to be visible, then fill
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