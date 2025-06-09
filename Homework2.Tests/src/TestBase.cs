using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using Allure.NUnit;
using Allure.Net.Commons;
using Homework2.Tests.Pages;

namespace Homework2.Tests
{
    [AllureNUnit]
    public abstract class TestBase : PageTest
    {
        protected LoginPage LoginPage = null!;
        protected SignupPage SignupPage = null!;
        protected BusinessDetailsPage BusinessDetailsPage = null!;
        protected SelectJurisdictionPage SelectJurisdictionPage = null!;
        protected readonly TestCredentials Credentials = TestCredentials.FromEnvironmentOrDefaults();

        [SetUp]
        public async Task BaseSetUp()
        {
            // Start Playwright tracing
            await Context.Tracing.StartAsync(new() 
            { 
                Screenshots = true, 
                Snapshots = true, 
                Sources = true 
            });
                
            // Initialize Playwright selectors to use a custom attribute for test IDs
            Playwright.Selectors.SetTestIdAttribute("data-unique-id");

            // Initialize the page objects for the test
            LoginPage = new LoginPage(Page);
            SignupPage = new SignupPage(Page);
            BusinessDetailsPage = new BusinessDetailsPage(Page);
            SelectJurisdictionPage = new SelectJurisdictionPage(Page);

            // Additional setup that derived classes can override
            await AdditionalSetUp();
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            try
            {
                // Check if test failed and save trace if needed
                var testResult = TestContext.CurrentContext.Result;
                if (testResult.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    await SaveAndAttachPlaywrightTraceOnFailure();
                }
                else
                {
                    // Stop tracing without saving if test passed
                    await Context.Tracing.StopAsync();
                }
            }
            catch (Exception ex)
            {
                AllureApi.AddAttachment("TearDown Error", "text/plain", 
                    $"Error during teardown: {ex.Message}");
            }
            finally
            {
                await AdditionalTearDown();
                await Page.CloseAsync();
            }
        }

        /// <summary>
        /// Virtual method for additional setup in derived classes
        /// </summary>
        protected virtual async Task AdditionalSetUp()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Virtual method for additional teardown in derived classes
        /// </summary>
        protected virtual async Task AdditionalTearDown()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Executes a test step with automatic exception handling and trace saving
        /// </summary>
        protected async Task ExecuteTestWithTracing(Func<Task> testAction)
        {
            try
            {
                await testAction();
            }
            catch (Exception ex)
            {
                // Trace will be saved in TearDown if test failed
                AllureApi.AddAttachment("Test Exception Details", "text/plain", 
                    $"Exception: {ex.GetType().Name}\n" +
                    $"Message: {ex.Message}\n" +
                    $"StackTrace: {ex.StackTrace}");
                throw; // Re-throw to maintain test failure
            }
        }

        private async Task SaveAndAttachPlaywrightTraceOnFailure()
        {
            try
            {
                var testName = TestContext.CurrentContext.Test.Name;
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var traceName = $"trace-{testName}-{timestamp}.zip";
                
                await Context.Tracing.StopAsync(new() { Path = traceName });
        
                // Attach the trace file
                AllureApi.AddAttachment("Playwright Trace (Download)", "application/zip", traceName);
        
                AllureApi.Step("📊 How to analyze the trace", () =>
                {
                    AllureApi.Step("📥 Download trace file", () => { });
                    AllureApi.Step("🌐 Open https://trace.playwright.dev", () => { });
                    AllureApi.Step("📂 Drag & drop zip file", () => { });
                });

            }
            catch (Exception traceException)
            {
                AllureApi.AddAttachment("Trace Error", "text/plain", 
                    $"Failed to save trace: {traceException.Message}");
            }
        }
    }
}