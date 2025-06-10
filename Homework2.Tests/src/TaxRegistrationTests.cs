using NUnit.Framework;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace Homework2.Tests
{
    [AllureSuite("Tax Registration")]
    public class TaxRegistrationTests : TestBase
    {
        [Test]
        [AllureName("Complete Tax Registration Flow")]
        [AllureEpic("Onboarding")]
        [AllureFeature("Business Registration")]
        [AllureStory("End-to-end registration process")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureDescription("This test verifies the complete tax registration flow from login to final subscription setup")]
        public async Task CompleteTaxRegistrationFlow()
        {
            await ExecuteTestWithTracing(async () =>
            {
                await AllureApi.Step("Navigate to login page and authenticate", async () =>
                {
                    await LoginPage.GoToAsync("https://app.taxually.com/");
                    await LoginPage.LoginAsync(Credentials.Email, Credentials.Password);
                    await Expect(Page).ToHaveURLAsync("https://app.taxually.com/verify");
            
                    AllureLifecycle.Instance.UpdateTestCase(x => 
                        x.parameters.Add(new Parameter { name = "Email", value = Credentials.Email }));
                });

                await AllureApi.Step("Navigate to business registration page", async () =>
                {
                    await SignupPage.GoToAsync("https://app.taxually.com/signup");
                    await Expect(Page).ToHaveURLAsync("https://app.taxually.com/app/registration/select-services/about-your-business");
                });

                await AllureApi.Step("Fill business details", async () =>
                {
                    await BusinessDetailsPage.FillBusinessDetailsAsync();
                    await BusinessDetailsPage.ClickNextAsync();
                });

                await AllureApi.Step("Select target jurisdiction", async () =>
                {
                    await AllureApi.Step("Select numb" +
                                         "er of target countries", async () =>
                    {
                        await SelectJurisdictionPage.SelectTargetCountry(1);
                        AllureLifecycle.Instance.UpdateTestCase(x => 
                            x.parameters.Add(new Parameter { name = "Number of countries", value = "1" }));
                    });

                    await AllureApi.Step("Select specific country: Germany", async () =>
                    {
                        await SelectJurisdictionPage.SelectSpecificCountry("Germany");
                        AllureLifecycle.Instance.UpdateTestCase(x => 
                            x.parameters.Add(new Parameter { name = "Selected country", value = "Germany" }));
                    });
                });

                // await AllureApi.Step("Configure tax registration service options", async () =>
                // {
                //     await AllureApi.Step("Set tax registration service to Yes", async () =>
                //     {
                //         await SelectJurisdictionPage.SelectTaxRegistrationServiceOption("Yes");
                //         // Remove this line when ready
                //         await Page.PauseAsync();
                //         AllureLifecycle.Instance.UpdateTestCase(x => 
                //             x.parameters.Add(new Parameter { name = "Tax registration service", value = "Yes" }));
                //         // Remove this line when ready
                //         await Page.PauseAsync();
                //     });
                //
                //     await AllureApi.Step("Change tax registration service to No", async () =>
                //     {
                //         await SelectJurisdictionPage.SelectTaxRegistrationServiceOption("No");
                //         AllureLifecycle.Instance.UpdateTestCase(x => 
                //             x.parameters.Add(new Parameter { name = "Tax registration service", value = "No" }));
                //     });
                // });
                //
                // await AllureApi.Step("Configure outstanding tax returns options", async () =>
                // {
                //     await AllureApi.Step("Set outstanding tax returns to Yes", async () =>
                //     {
                //         await SelectJurisdictionPage.SelectOutstandingTaxReturnsOption("Yes");
                //         AllureLifecycle.Instance.UpdateTestCase(x => 
                //             x.parameters.Add(new Parameter { name = "Outstanding tax returns", value = "Yes" }));
                //     });
                //
                //     await AllureApi.Step("Change outstanding tax returns to No", async () =>
                //     {
                //         await SelectJurisdictionPage.SelectOutstandingTaxReturnsOption("No");
                //         AllureLifecycle.Instance.UpdateTestCase(x => 
                //             x.parameters.Add(new Parameter { name = "Outstanding tax returns", value = "No" }));
                //     });
                // });
                //
                // await AllureApi.Step("Select retroactive period", async () =>
                // {
                //     await SelectJurisdictionPage.SelectRandomRetroactivePeriod();
                // });
                //
                // await AllureApi.Step("Accept terms and policies", async () =>
                // {
                //     await SelectJurisdictionPage.AcceptTermsAndConditions();
                //     await SelectJurisdictionPage.AcceptPrivacyPolicy();
                // });
                //
                // await AllureApi.Step("Configure subscription", async () =>
                // {
                //     await AllureApi.Step("Select monthly payment option", async () =>
                //     {
                //         await SelectJurisdictionPage.SelectPayMonthlyOption();
                //         AllureLifecycle.Instance.UpdateTestCase(x => 
                //             x.parameters.Add(new Parameter { name = "Payment frequency", value = "Monthly" }));
                //     });
                //
                //     await AllureApi.Step("Verify subscription fees", async () =>
                //     {
                //         await SelectJurisdictionPage.VerifySubscriptionFees();
                //     });
                // });
            });
        }
    }
}