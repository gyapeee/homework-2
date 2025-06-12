using Microsoft.Playwright;
using NUnit.Framework;

namespace Homework2.Tests.Pages;

public class SelectJurisdiction(IPage page) : BasePage(page)
{
    // Extracted constants for locators and text
    private const string TaxRegistrationServiceLabel = "Do you need a tax registration service? {0}";

    private const string OutstandingTaxReturnsLabel =
        "Do you have any outstanding tax returns that we should file? {0}";

    private const string RetroactivePeriodLabel = "Please select the first retroactive period";
    private const string TermsAndConditionsLabel = "I accept the Terms and Conditions";
    private const string PrivacyPolicyLabel = "I accept the Privacy Policy";
    private const string PayMonthlyOptionName = "Pay monthly";
    private const string MonthlyFeeSelector = ".monthly-fee-display";
    private const string AnnualFeeSelector = ".annual-fee-display";
    private const string DateFormat = "yyyy-MM-dd";
    private const int MaxRandomDaysBack = 365;
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

        await firstMatchingElement.CountryOption.First.ClickAsync();

        // Get POSTFIX from ID attribute (pl. VAT_AT -> AT)
        var containerId = await firstMatchingElement.Element.GetAttributeAsync("id");
        if (string.IsNullOrWhiteSpace(containerId) || !containerId.StartsWith("VAT_"))
            throw new InvalidOperationException($"Can't get the postfix from ID: '{containerId}'");

        var countryCode = containerId.Substring("VAT_".Length); // pl. "AT"

        await Page.PauseAsync();

        // input[type='radio'][value='true'][id='DE'][name='DE_jurisdiction-vat-registration-group']
        await Page.Locator(
                $"input[type='radio'][id='{countryCode}'][name='{countryCode}_jurisdiction-vat-registration-group']")
            .Locator("xpath=ancestor::label//span[text()='Yes']").ClickAsync();

        await Page.PauseAsync();

        await Page.Locator(
                $"input[type='radio'][id='{countryCode}'][name='{countryCode}_jurisdiction-vat-registration-group']")
            .Locator("xpath=ancestor::label//span[text()='No']").ClickAsync();

        await Page.PauseAsync();
    }


    private async Task WaitForVatIds()
    {
        await Page.WaitForSelectorAsync(VatIdSelector,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
    }

    public async Task SelectTaxRegistrationServiceOption(string option)
    {
        await Page.GetByLabel(string.Format(TaxRegistrationServiceLabel, option)).ClickAsync();
    }

    public async Task SelectOutstandingTaxReturnsOption(string option)
    {
        await Page.GetByLabel(string.Format(OutstandingTaxReturnsLabel, option)).ClickAsync();
    }

    public async Task SelectRandomRetroactivePeriod()
    {
        var randomDate = GenerateRandomPastDate();
        var formattedDate = FormatDateForInput(randomDate);

        await Page.GetByLabel(RetroactivePeriodLabel).FillAsync(formattedDate);
        await Page.Keyboard.PressAsync("Enter");
    }

    public async Task AcceptTermsAndConditions()
    {
        await Page.GetByLabel(TermsAndConditionsLabel).ClickAsync();
    }

    public async Task AcceptPrivacyPolicy()
    {
        await Page.GetByLabel(PrivacyPolicyLabel).ClickAsync();
    }

    public async Task SelectPayMonthlyOption()
    {
        await Page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = PayMonthlyOptionName }).ClickAsync();
    }

    public async Task VerifySubscriptionFees()
    {
        var monthlyFee = await GetFeeAmount(MonthlyFeeSelector);
        var annualFee = await GetFeeAmount(AnnualFeeSelector);

        AssertFeeValues(monthlyFee, annualFee);
    }

    private static DateTime GenerateRandomPastDate()
    {
        return DateTime.Now.AddDays(-new Random().Next(1, MaxRandomDaysBack));
    }

    private static string FormatDateForInput(DateTime date)
    {
        return date.ToString(DateFormat);
    }

    private async Task<decimal> GetFeeAmount(string selector)
    {
        var feeText = await Page.Locator(selector).InnerTextAsync();
        return ExtractDecimalFromCurrencyString(feeText);
    }

    private static void AssertFeeValues(decimal monthlyFee, decimal annualFee)
    {
        Assert.That(monthlyFee, Is.Not.EqualTo(0m), "Monthly fee should not be 0€");
        Assert.That(annualFee, Is.EqualTo(0m), "Annual fee should be 0€");
    }

    private static decimal ExtractDecimalFromCurrencyString(string currencyString)
    {
        var cleanedString = currencyString.Replace("€", "").Replace("$", "").Replace(",", "").Trim();
        return decimal.TryParse(cleanedString, out var value) ? value : 0m;
    }
}