# Automated .NET Playwright Tests [![.NET Playwright Tests](https://github.com/gyapeee/homework-2/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gyapeee/homework-2/actions/workflows/dotnet.yml)

📊 **Live Allure Report**  
➡️ [🔍 Click here to view the latest test report](https://gyapeee.github.io/homework-2/allure/)

![How to find a trace of the failed test in Allure Report](doc/trace.gif)


This project contains automated tests using Playwright, NUnit and .Net with reporting via Allure. Test execution and configuration are managed using a `.runsettings` file, which sets environment variables and test options for consistent and reproducible runs.

## Quick Start

**Build project**
- dotnet build

**Install Playwright**
- pwsh bin/Debug/net8.0/playwright.ps1 install

**Run tests**
- dotnet test --settings Homework2.Tests/.runsettings

**Generate Allure Report**
- allure generate .\Homework2.Tests\bin\Debug\net8.0\allure-results\ -o allure-report --clean

**Open the Allure report**
- allure open allure-report



 ## Test Configuration with .runsettings

The project uses a `.runsettings` file (`Homework2.Tests/.runsettings`) to configure test execution. This file provides:

- **NUnit settings**: Controls the number of parallel test workers.
- **Environment variables**: Sets variables like `TEST_USERNAME`, `TEST_PASSWORD`, and Playwright/Allure configuration for the test environment.
- **Playwright settings**: Configures browser type, headless mode, slow motion, and developer tools for UI tests.

When running or debugging tests, the `.runsettings` file ensures all required environment variables and test options are set automatically.

**To use the .runsettings file:**
- Run tests with:
  ```
  dotnet test --no-build --verbosity normal --settings Homework2.Tests/.runsettings
  ```
- For debugging, uncomment `<PWDEBUG>1</PWDEBUG>` in the `.runsettings` file to enable Playwright debug mode (non-headless, with devtools).

 ## Findings
 # The design of the test case should be more robust
 - Every test should run in a clean environment: 
   - new user shall be created for a new test run this might solve the instability of this test case(why the Retry is required now)
   - or Saved date in forms shall be cleared, via api call or ui flow, eg.
     ![Clear from from UI](doc/clearn_form.gif)
   - if the test is to test that the registration date is saved, then the test case should be designed accordingly

