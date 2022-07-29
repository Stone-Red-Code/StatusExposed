using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace StatusExposed.Test.StepDefinitions;

[Binding]
public sealed class BasicStepDefinitions
{
    private IWebDriver driver = null!;
    private const string baseUrl = "https://localhost:7264";

    [BeforeScenario]
    public void BeforeScenario()
    {
        ChromeOptions chromeOptions = new ChromeOptions();
        chromeOptions.AddArguments("headless", "incognito");

        driver = new ChromeDriver(Environment.CurrentDirectory, chromeOptions);
        driver.Navigate().GoToUrl(baseUrl);
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        driver.Quit();
    }

    [Then(@"the app is running")]
    public void ThenTheAppIsRunning()
    {
        IWebElement? html = driver.FindElement(By.TagName("html"));
        _ = html.Should().NotBeNull();
    }

    [Then(@"the navigation contains {int} items")]
    public void ThenTheNavigationContainsItems(int itemCount)
    {
        IWebElement? navMenu = driver.FindElement(By.Id("navmenu"));
        System.Collections.ObjectModel.ReadOnlyCollection<IWebElement>? NavMenuItems = navMenu.FindElements(By.ClassName("nav-item"));
        _ = NavMenuItems.Count.Should().Be(itemCount);
    }

    [Then(@"the {string} page is not accessible")]
    public void ThenAPageIsNotAccessible(string page)
    {
        driver.Navigate().GoToUrl(CombineUriToString(baseUrl, page));
        _ = driver.PageSource.Should().Contain("Sorry, you don't have the required permissions to see this.");
    }

    public static string CombineUriToString(string baseUri, string relativeOrAbsoluteUri)
    {
        return new Uri(new Uri(baseUri), relativeOrAbsoluteUri).ToString();
    }
}