using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Homework2.Tests.Pages
{
    public class SelectJurisdictionPage
    {
        private readonly IPage _page;

        // Extracted constants for locators and text
        private const string CountrySelectorAddButton = ".country-selector-add-button";
        private const string CountryListItem = ".country-list-item";
        private const string ConfirmButtonName = "Confirm";
        private const string SearchCountryPlaceholder = "Search country";
        private const string TaxRegistrationServiceLabel = "Do you need a tax registration service? {0}";
        private const string OutstandingTaxReturnsLabel = "Do you have any outstanding tax returns that we should file? {0}";
        private const string RetroactivePeriodLabel = "Please select the first retroactive period";
        private const string TermsAndConditionsLabel = "I accept the Terms and Conditions";
        private const string PrivacyPolicyLabel = "I accept the Privacy Policy";
        private const string PayMonthlyOptionName = "Pay monthly";
        private const string MonthlyFeeSelector = ".monthly-fee-display";
        private const string AnnualFeeSelector = ".annual-fee-display";
        private const string DateFormat = "yyyy-MM-dd";
        private const int MaxRandomDaysBack = 365;

        public SelectJurisdictionPage(IPage page)
        {
            _page = page;
        }

        public async Task SelectTargetCountry(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await ClickAddCountryButton();
                await SelectFirstAvailableCountry();
            }
            await ConfirmSelection();
        }

        public async Task SelectSpecificCountry(string countryName)
        {
            await SearchForCountry(countryName);
            await SelectCountryByName(countryName);
            await ConfirmSelection();
        }

        public async Task SelectTaxRegistrationServiceOption(string option)
        {
            await _page.GetByLabel(string.Format(TaxRegistrationServiceLabel, option)).ClickAsync();
        }

        public async Task SelectOutstandingTaxReturnsOption(string option)
        {
            await _page.GetByLabel(string.Format(OutstandingTaxReturnsLabel, option)).ClickAsync();
        }

        public async Task SelectRandomRetroactivePeriod()
        {
            var randomDate = GenerateRandomPastDate();
            var formattedDate = FormatDateForInput(randomDate);
            
            await _page.GetByLabel(RetroactivePeriodLabel).FillAsync(formattedDate);
            await _page.Keyboard.PressAsync("Enter");
        }

        public async Task AcceptTermsAndConditions()
        {
            await _page.GetByLabel(TermsAndConditionsLabel).ClickAsync();
        }

        public async Task AcceptPrivacyPolicy()
        {
            await _page.GetByLabel(PrivacyPolicyLabel).ClickAsync();
        }

        public async Task SelectPayMonthlyOption()
        {
            await _page.GetByRole(AriaRole.Radio, new() { Name = PayMonthlyOptionName }).ClickAsync();
        }

        public async Task VerifySubscriptionFees()
        {
            var monthlyFee = await GetFeeAmount(MonthlyFeeSelector);
            var annualFee = await GetFeeAmount(AnnualFeeSelector);

            AssertFeeValues(monthlyFee, annualFee);
        }

        // Extracted helper methods
        private async Task ClickAddCountryButton()
        {
            await _page.Locator(CountrySelectorAddButton).ClickAsync();
        }

        private async Task SelectFirstAvailableCountry()
        {
            await _page.Locator(CountryListItem).First.ClickAsync();
        }

        private async Task ConfirmSelection()
        {
            await _page.GetByRole(AriaRole.Button, new() { Name = ConfirmButtonName }).ClickAsync();
        }

        private async Task SearchForCountry(string countryName)
        {
            await _page.GetByPlaceholder(SearchCountryPlaceholder).FillAsync(countryName);
        }

        private async Task SelectCountryByName(string countryName)
        {
            await _page.Locator($"text={countryName}").ClickAsync();
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
            var feeText = await _page.Locator(selector).InnerTextAsync();
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
            return decimal.TryParse(cleanedString, out decimal value) ? value : 0m;
        }
    }
}