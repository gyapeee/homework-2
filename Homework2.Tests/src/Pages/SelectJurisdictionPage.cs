using System.Globalization;
using System.Text.RegularExpressions;
using Allure.Net.Commons;
using Microsoft.Playwright;

namespace Homework2.Tests.Pages;

public class SelectJurisdiction(IPage page) : PageBase(page)
{
    private static readonly string VatIdSelector = "[id^='VAT_']";

    protected override string PageUrl =>
        "https://app.taxually.com/app/registration/select-services/jurisdiction-selection";

    public async Task SelectTargetCountry(int count)
    {
        await WaitForVatIds();
        var elements = await Page.Locator(VatIdSelector).AllAsync();
        var totalAvailable = elements.Count;

        if (count > totalAvailable)
            throw new InvalidOperationException($"Requested number of elements ({count}) " +
                                                $"is greater than number of VAT_ elements ({totalAvailable}).");

        await Task.WhenAll(elements.Take(count).Select(e => e.ClickAsync()));
    }

    public async Task SelectSpecificCountryAndClickRadios(string countryName)
    {
        await WaitForVatIds();
        var vatElements = await Page.Locator(VatIdSelector).AllAsync();

        var matchingElements = await Task.WhenAll(vatElements.Select(async vatContainer => new
        {
            Element = vatContainer,
            CountryOption = vatContainer.Locator($"*:text('{countryName}')"),
            Count = await vatContainer.Locator($"*:text('{countryName}')").CountAsync()
        }));

        var firstMatchingElement = matchingElements.FirstOrDefault(e => e.Count > 0);
        if (firstMatchingElement == null)
            throw new InvalidOperationException(
                $"The given country ('{countryName}') is missing in VAT_ containers.");

        // Get the checkbox container within the matching VAT block
        var checkboxContainer = firstMatchingElement.Element.Locator(".tuui-checkbox-checkmark");

        // Ensure country name is visible (it could be hidden initially)
        await firstMatchingElement.CountryOption.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });

        // Click the label (the Germany text acts as the checkbox label)
        await firstMatchingElement.CountryOption.First.ClickAsync();

        // Wait until the checkbox gets the "checked" class
        await Assertions.Expect(checkboxContainer).ToHaveClassAsync(
            new Regex(".*tuui-checkbox-checkmark-checked.*"), new LocatorAssertionsToHaveClassOptions
            {
                Timeout = 5000
            });

        // Get POSTFIX from ID attribute (pl. VAT_AT -> AT)
        var containerId = await firstMatchingElement.Element.GetAttributeAsync("id");
        if (string.IsNullOrWhiteSpace(containerId) || !containerId.StartsWith("VAT_"))
            throw new InvalidOperationException($"Can't get the postfix from ID: '{containerId}'");

        var countryCode = containerId.Substring("VAT_".Length); // pl. "AT"

        await ClickOnAllRadioButtonsOfCountry(countryCode);
        await SelectDateAsFirstRetroactivePeriod();
    }

    private async Task ClickOnAllRadioButtonsOfCountry(string countryCode)
    {
        var registrationGroupYesLocator =
            $"//input[@type='radio' and @id='{countryCode}' and @name='{countryCode}_jurisdiction-vat-registration-group']/ancestor::label//small[text()='Help me to get a tax number']";
        await SelectRadioButton(registrationGroupYesLocator);

        var registrationGroupNoLocator =
            $"//input[@type='radio' and @id='{countryCode}' and @name='{countryCode}_jurisdiction-vat-registration-group']/ancestor::label//small[text()='I already have a tax number.']";
        await SelectRadioButton(registrationGroupNoLocator);

        var retroactiveGroupNoLocator =
            $"//input[@type='radio' and @id='{countryCode}' and @name='{countryCode}_jurisdiction-retroactive-group']/ancestor::label//small[text()='All tax returns filed']";
        await SelectRadioButton(retroactiveGroupNoLocator);

        var retroactiveGroupYesLocator =
            $"//input[@type='radio' and @id='{countryCode}' and @name='{countryCode}_jurisdiction-retroactive-group']/ancestor::label//small[text()='Need to file tax returns']";
        await SelectRadioButton(retroactiveGroupYesLocator);
    }

    private async Task SelectDateAsFirstRetroactivePeriod()
    {
        await Page.PauseAsync();
        await Page.Locator("[placeholder='YYYY-MM']").ClickAsync();
        await Page.PauseAsync();
        await Page.Locator("//span[normalize-space(text())='Jan']").ClickAsync();
        await Page.PauseAsync();
        // TODO 2025-January is not set although the date picker is closed
        await Assertions.Expect(Page.Locator("//span[normalize-space(text())='Feb']")).ToBeHiddenAsync();
    }

    public async Task ClickOnPayMonthlyIfNotSelected()
    {
        var sliderLocator =
            Page.Locator("[data-unique-id='subscription-summary_recurring-interval'] .slider.round");
        var parentLocator = Page.Locator("[data-unique-id='subscription-summary_recurring-interval']");
        var isSelected = await parentLocator.GetAttributeAsync("data-unique-meta_selected");

        if (isSelected == "true") await sliderLocator.ClickAsync();
    }

    public async Task ClickOnTermsIfNotSelected()
    {
        var menuLocator = Page.Locator("[data-unique-id='client-side-menu_terms-and-conditions']");
        var menuAttr = await menuLocator.GetAttributeAsync("data-unique-meta_selected");

        if (menuAttr == "false") await menuLocator.ClickAsync();
    }

    public async Task AssertMonthlyFeeIsGreaterThanZeroAsync()
    {
        await AllureApi.Step("Check that Monthly fee amount is greater than 0", async () =>
        {
            // Find the .amount child of the "Monthly fee" container
            var amountLocator = Page.Locator("xpath=//div[normalize-space(text())='Monthly fee']");

            // Wait for it to be visible
            await amountLocator.WaitForAsync();

            // Get the text content, e.g., "€0"
            var amountText = await amountLocator.InnerTextAsync();

            // Remove non-numeric characters (except decimal separator)
            var numericString = new string(amountText
                    .Where(c => char.IsDigit(c) || c == '.' || c == ',')
                    .ToArray())
                .Replace(",", "."); // Normalize comma to dot for decimal parsing

            // Parse and assert
            if (!decimal.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                throw new Exception($"Unable to parse amount: '{amountText}'");

            if (amount <= 0) throw new Exception($"Monthly fee is not greater than 0. Found: {amountText}");
        });
    }

    private async Task SelectRadioButton(string radioSelector)
    {
        await Page.WaitForSelectorAsync(radioSelector,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });

        await Page.Locator(radioSelector)
            .ClickAsync();
    }


    private async Task WaitForVatIds()
    {
        await Page.WaitForSelectorAsync(VatIdSelector,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
    }
}