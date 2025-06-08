using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using Homework2.Tests.Pages;
using Allure.NUnit;


namespace Homework2.Tests
{
    [AllureNUnit]
    [AllureSuite("Tax Registration")]
    public class TaxRegistrationTests : PageTest
    {
        private LoginPage _loginPage = null!;
        private SignupPage _signupPage = null!;
        private BusinessDetailsPage _businessDetailsPage = null!;
        private SelectJurisdictionPage _selectJurisdictionPage = null!;
        private readonly TestCredentials _credentials = TestCredentials.FromEnvironmentOrDefaults();

        [SetUp]
        public void SetupPages()
        { 
            // Initialize Playwright selectors to use a custom attribute for test IDs
            // This is useful for selecting elements in tests without relying on class names or IDs that might change.
             Playwright.Selectors.SetTestIdAttribute("data-unique-id");

            // Initialize the page objects for the test
            // These pages should be defined in your project, and they should encapsulate the logic for interacting with the respective pages.
            // Ensure that the URLs and locators in these classes match the actual application structure.
            _loginPage = new LoginPage(Page);
            _signupPage = new SignupPage(Page);
            _businessDetailsPage = new BusinessDetailsPage(Page);
            _selectJurisdictionPage = new SelectJurisdictionPage(Page);
        }

        [TearDown]
        public async Task TearDown()
        {
            await Page.CloseAsync();
        }

        [Test]
        [AllureName("Complete Tax Registration Flow")]
        [AllureEpic("Onboarding")]
        [AllureFeature("Business Registration")]
        public async Task CompletePovertyQuestionsFlow()
        {
            // Step 1: Navigate to the login page and log in
            // Ensure the URL matches the actual login page URL.
            // The URL provided in the original code was "https://app.taxually.com/app/registration/select-services/about-your-business",
            await _loginPage.GoToAsync("https://app.taxually.com/");            
            await _loginPage.LoginAsync(_credentials.Email, _credentials.Password);
            await Expect(Page).ToHaveURLAsync("https://app.taxually.com/verify");

            // Step 2: Navigate to signup page (if not automatically redirected)
            // This step is a bit ambiguous. If login directly leads to signup, this might not be needed.
            // Assuming we might need to navigate explicitly after login for this specific flow.
            await _signupPage.GoToAsync("https://app.taxually.com/signup");
            await Expect(Page).ToHaveURLAsync("https://app.taxually.com/app/registration/select-services/about-your-business");
            
            // Step 3: On the "Tell us about your business" page
            await _businessDetailsPage.FillBusinessDetailsAsync();
            await _businessDetailsPage.ClickNextAsync();
            
            await Page.PauseAsync(); // Todo remove when ready
            
            // Step 4: On the "Select jurisdiction" page
            // Step 4a: Create a method that defines how many target countries are going to be selected
            // and use it to select at least one target country (Example: SelectTargetCountry(1))
            await _selectJurisdictionPage.SelectTargetCountry(1);
            
            // Step 4b: Create a method that uses the country name to select a specific country
            // Assuming we select "Germany" as an example. You might need to make this dynamic or configurable.
            await _selectJurisdictionPage.SelectSpecificCountry("Germany");
            
            // Step 4c: Under the selected country, select both combinations for the "Do you need a tax registration service?" question (Yes / No)
            await _selectJurisdictionPage.SelectTaxRegistrationServiceOption("Yes");
            await _selectJurisdictionPage.SelectTaxRegistrationServiceOption("No"); // Selecting both implies two distinct tests or a re-selection. Here, I'm showing re-selection for the current test.
            
            // Step 4d: Select both combinations for the "Do you have any outstanding tax returns that we should file?" question (Yes / No)
            await _selectJurisdictionPage.SelectOutstandingTaxReturnsOption("Yes");
            await _selectJurisdictionPage.SelectOutstandingTaxReturnsOption("No");
            
            // Step 4e: Select a random/any date for the "Please select the first retroactive period" question
            // Assuming there's a date picker or input. This will select a random date within the last year.
            await _selectJurisdictionPage.SelectRandomRetroactivePeriod();
            
            // Step 4f: Accept the Terms and Conditions and Privacy Policy
            await _selectJurisdictionPage.AcceptTermsAndConditions();
            await _selectJurisdictionPage.AcceptPrivacyPolicy();
            
            // Step 4g: Select the "Pay monthly" option from the Subscription summary
            await _selectJurisdictionPage.SelectPayMonthlyOption();
            
            // Step 4h: Check that the "Monthly fee" is not 0€ and the Annual fee is 0€
            await _selectJurisdictionPage.VerifySubscriptionFees();
        }
    }
}