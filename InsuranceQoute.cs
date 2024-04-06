using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using SeleniumExtras.WaitHelpers;


namespace Assignment4
{
    public class InsuranceQuoteTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("http://localhost/prog8171a04/getQuote.html");
        }

     
        [Test]
        public void InsuranceQuote02_ValidDataAge25Exp3Accidents4()
        {
            FillForm("SyedSuhaib", "Hussain", "25", "3", "4");
            AssertInvalidSubmission("accidents", "No Insurance for you!!  Too many accidents - go take a course!");
        }
        [Test]
        public void InsuranceQuote03_ValidDataAge35Exp9Accidents2()
        {
            FillForm("FirstName", "LastName", "35", "9", "2");
    
            AssertValidSubmission("$3910"); 
        }

        [Test]
        public void InsuranceQuote04_InvalidPhoneNumber()
        {
            FillForm("Jane", "Doe", "27", "3", "0", phoneNumber: "1234567890"); 
            AssertFieldErrorMessage("phone", "Phone Number must follow the patterns 111-111-1111 or (111)111-1111");
        }

        [Test]
        public void InsuranceQuote05_InvalidEmail()
        {
            FillForm("FirstName", "LastName", "28", "3", "0", email: "invalid@");
            AssertFieldErrorMessage("email", "Must be a valid email address");
        }

        [Test]
        public void InsuranceQuote06_InvalidPostalCode()
        {
            FillForm("FirstName", "LastName", "35", "15", "1", postalCode: "123 456");
            AssertFieldErrorMessage("postalCode", "Postal Code must follow the pattern");
        }

        [Test]
        public void InsuranceQuote07_AgeOmitted()
        {
            FillForm("FirstName", "LastName", "", "5", "0");
            AssertFieldErrorMessage("age", "Age (>=16) is required");
        }

        [Test]
        public void InsuranceQuote08_AccidentsOmitted()
        {
            FillForm("FirstName", "LastName", "37", "8", "");
            AssertFieldErrorMessage("accidents", "Number of accidents is required");
        }

        [Test]
        public void InsuranceQuote09_ExperienceOmitted()
        {
            FillForm("FirstName", "LastName", "45", "", "0");
            AssertFieldErrorMessage("experience", "Years of experience is required");
        }

     
        [Test]
        public void InsuranceQuote10_NoDrivingExperienceHighAge()
        {
            FillForm("FirstName", "LastName", "50", "0", "0");
            AssertValidSubmission("$7000"); // No experience rate
        }

        [Test]
        public void InsuranceQuote11_YoungDriverHighExperience()
        {
            FillForm("FirstName", "LastName", "16", "0", "0");
            AssertFieldErrorMessage("experience", "Invalid experience for given age"); // Invalid as per business rules
        }

        [Test]
        public void InsuranceQuote12_ValidDataNoAccidentsOlderDriver()
        {
            FillForm("FirstName", "LastName", "65", "40", "0");
            AssertValidSubmission("$2840"); // Reduced rate for older drivers with experience
        }

        [Test]
        public void InsuranceQuote13_YoungDriverManyAccidents()
        {
            FillForm("FirstName", "LastName", "20", "2", "5");
            AssertFieldErrorMessage("accidents", "No Insurance for you!!");
        }

        [Test]
        public void InsuranceQuote14_InvalidAgeTooLow()
        {
            FillForm("FirstName", "LastName", "15", "0", "0"); // Age below minimum
            AssertFieldErrorMessage("age", "Age must be 16 or older");
        }

        [Test]
        public void InsuranceQuote15_MaxExperienceNewDriver()
        {
            FillForm("FirstName", "LastName", "35", "19", "0");
            AssertFieldErrorMessage("experience", "Invalid experience for given age"); // Experience too high for age
        }

        private void FillForm(
            string firstName,
            string lastName,
            string age,
            string experience,
            string accidents,
            string address = "123 Main St",
            string city = "YourCity",
            string province = "ON",
            string postalCode = "A1A 1A1",
            string phoneNumber = "123-123-1234",
            string email = "suhaib@mail.com")
        {
            driver.FindElement(By.Id("firstName")).SendKeys(firstName);
            driver.FindElement(By.Id("lastName")).SendKeys(lastName);
            driver.FindElement(By.Id("address")).SendKeys(address);
            driver.FindElement(By.Id("city")).SendKeys(city);
            new SelectElement(driver.FindElement(By.Id("province"))).SelectByText(province);
            driver.FindElement(By.Id("postalCode")).SendKeys(postalCode);
            driver.FindElement(By.Id("phone")).SendKeys(phoneNumber);
            driver.FindElement(By.Id("email")).SendKeys(email);
            driver.FindElement(By.Id("age")).SendKeys(age);
            driver.FindElement(By.Id("experience")).SendKeys(experience);
            driver.FindElement(By.Id("accidents")).SendKeys(accidents);
            driver.FindElement(By.Id("btnSubmit")).Click();
        }

        private void AssertValidSubmission(string expectedQuote)
        {
            // Wait for the final quote to be visible and then assert its value
            var finalQuote = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("finalQuote")))[0].GetAttribute("value");
            Assert.AreEqual(expectedQuote, finalQuote, $"Expected quote was {expectedQuote}, but found {finalQuote}.");
        }

        private void AssertInvalidSubmission(string fieldId, string expectedErrorMessage)
        {
            // Wait for the error message to appear and then assert
            string errorElementId = $"{fieldId}-error"; 
            var errorMessageElement = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(errorElementId)))[0];
            string actualErrorMessage = errorMessageElement.Text;
            Assert.IsTrue(actualErrorMessage.Contains(expectedErrorMessage), $"Expected error message for '{fieldId}' to be '{expectedErrorMessage}', but was '{actualErrorMessage}'.");
        }

        private void AssertFieldErrorMessage(string fieldId, string expectedErrorMessage)
        {
            // Check for an error message directly below the input field
            string errorElementId = $"{fieldId}-error"; 
            var errorMessageElement = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(errorElementId)))[0];
            string actualErrorMessage = errorMessageElement.Text;
            Assert.IsTrue(actualErrorMessage.Contains(expectedErrorMessage), $"Expected error message for '{fieldId}' to be '{expectedErrorMessage}', but was '{actualErrorMessage}'.");
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
