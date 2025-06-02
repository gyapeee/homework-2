# homework-2
SDET Homework


Build project
 - cd Homework2.Tests/
 - dotnet build

Install playwright
 - pwsh bin/Debug/net8.0/playwright.ps1 install

Run tests
 - dotnet test --no-build --verbosity normal --settings Homework2.Tests/.runsettings

Generate Allure Report
 - allure generate allure-results -o allure-report

Open the allure report
 - allure open allure-report