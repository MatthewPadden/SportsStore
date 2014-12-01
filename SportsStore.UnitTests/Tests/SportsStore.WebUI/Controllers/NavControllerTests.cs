using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests.Tests.SportsStore.WebUI.Controllers
{
    public class NavControllerTests
    {
        [TestClass]
        public class TheMenuMethod
        {
            [TestMethod]
            public void Can_Create_Categories()
            {
                // Arrange
                // create the mock repository
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                    new Product { ProductID = 2, Name = "P2", Category = "Apples" },
                    new Product { ProductID = 4, Name = "P3", Category = "Plums" },
                    new Product { ProductID = 5, Name = "P4", Category = "Oranges" },
                }.AsQueryable());

                // create the controller
                NavController target = new NavController(mock.Object);
 
                // Act
                string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

                // Assert
                Assert.AreEqual(results.Length, 3);
                Assert.AreEqual(results[0], "Apples");
                Assert.AreEqual(results[1], "Oranges");
                Assert.AreEqual(results[2], "Plums");
            }
        }
    }
}
