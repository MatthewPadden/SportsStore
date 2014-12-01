using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SportsStore.WebUI.HTMLHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests.Tests.SportsStore.WebUI.HTMLHelpers
{
    public class PagingHelpersTests
    {
        [TestClass]
        public class ThePageLinksMethod
        {
            [TestMethod]
            public void Can_Generate_Page_Links()
            {
                // Arrange - Define an HTML helper. We need to do this in order to apply the extesion method
                HtmlHelper myHelper = null;

                // create paging info data
                PagingInfo pagingInfo = new PagingInfo
                {
                    CurrentPage = 2,
                    TotalItems = 28,
                    ItemsPerPage = 10
                };

                // set up the delegate using a lambda expression
                Func<int, string> pageUrlDelegate = i => "Page" + i;

                // Act
                MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

                // Assert
                Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
                    + @"<a class=""selected"" href=""Page2"">2</a>"
                    + @"<a href=""Page3"">3</a>");
            }
        }
    }
}
