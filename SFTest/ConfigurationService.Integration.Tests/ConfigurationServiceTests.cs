using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigurationService.Controllers;

namespace ConfigurationService.Integration.Tests
{
    [TestClass]
    public class ConfigurationServiceTests
    {
        [TestMethod]
        public void GetConfig()
        {
            var controller = new ConfigController(new CustomConfigProvider());
        }
    }
}
