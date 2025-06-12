using Allure.Net.Commons;
using Allure.NUnit;
using Homework2.Tests.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Homework2.Tests;

[AllureNUnit]
public abstract class TestBase : PageTest
{
    protected readonly TestCredentials Credentials = TestCredentials.FromEnvironmentOrDefaults();

    protected BusinessDetails BusinessDetails = null!;

    protected IBrowserContext CleanContext = null!;
    protected IPage CleanPage = null!;
    protected Login Login = null!;
    protected SelectJurisdiction SelectJurisdiction = null!;
    protected Signup Signup = null!;

    [SetUp]
    public async Task BaseSetUp()
    {
        CleanContext = await Browser.NewContextAsync();
        CleanPage = await CleanContext.NewPageAsync();

        await CleanContext.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });


        Playwright.Selectors.SetTestIdAttribute("data-unique-id");

        Login = new Login(CleanPage);
        Signup = new Signup(CleanPage);
        BusinessDetails = new BusinessDetails(CleanPage);
        SelectJurisdiction = new SelectJurisdiction(CleanPage);

        await AdditionalSetUp();
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        try
        {
            var testResult = TestContext.CurrentContext.Result;
            if (testResult.Outcome.Status == TestStatus.Failed)
                await SaveAndAttachPlaywrightTraceOnFailure();
            else
                await CleanContext.Tracing.StopAsync();
        }
        catch (Exception ex)
        {
            AllureApi.AddAttachment("TearDown Error", "text/plain",
                $"Error during teardown: {ex.Message}");
        }
        finally
        {
            await AdditionalTearDown();

            await CleanPage.CloseAsync();
            await CleanContext.CloseAsync();
        }
    }

    protected virtual async Task AdditionalSetUp()
    {
        await Task.CompletedTask;
    }

    protected virtual async Task AdditionalTearDown()
    {
        await Task.CompletedTask;
    }

    protected async Task ExecuteTestWithTracing(Func<Task> testAction)
    {
        try
        {
            await testAction();
        }
        catch (Exception ex)
        {
            AllureApi.AddAttachment("Test Exception Details", "text/plain",
                $"Exception: {ex.GetType().Name}\n" +
                $"Message: {ex.Message}\n" +
                $"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task SaveAndAttachPlaywrightTraceOnFailure()
    {
        try
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var traceName = $"trace-{testName}-{timestamp}.zip";

            await CleanContext.Tracing.StopAsync(new TracingStopOptions { Path = traceName });

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