using Allure.Net.Commons;
using Allure.NUnit;
using Homework2.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Homework2.Tests;

[AllureNUnit]
public abstract class TestBase
{
    protected readonly TestCredentials Credentials = TestCredentials.FromEnvironmentOrDefaults();

    protected BusinessDetails BusinessDetailsPage = null!;
    protected Login LoginPage = null!;
    protected SelectJurisdiction SelectJurisdictionPage = null!;
    protected Signup SignupPage = null!;

    private IBrowser? Browser { get; set; }
    private IBrowserContext? Context { get; set; }
    private IPage? Page { get; set; }

    [SetUp]
    public async Task SetUp()
    {
        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync();
        Context = await Browser.NewContextAsync();
        Page = await Context.NewPageAsync();

        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });


        playwright.Selectors.SetTestIdAttribute("data-unique-id");

        LoginPage = new Login(Page);
        SignupPage = new Signup(Page);
        BusinessDetailsPage = new BusinessDetails(Page);
        SelectJurisdictionPage = new SelectJurisdiction(Page);
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            var testResult = TestContext.CurrentContext.Result;
            if (testResult.Outcome.Status == TestStatus.Failed)
                await SaveAndAttachPlaywrightTraceOnFailure();
            else
                await Context!.Tracing.StopAsync();
        }
        catch (Exception ex)
        {
            AllureApi.AddAttachment("TearDown Error", "text/plain",
                $"Error during teardown: {ex.Message}");
        }
        finally
        {
            await Context!.CloseAsync();
            await Browser!.CloseAsync();
        }
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

            await Context!.Tracing.StopAsync(new TracingStopOptions { Path = traceName });

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