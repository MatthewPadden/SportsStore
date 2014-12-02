using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests.Tests.SportsStore.WebUI.Controllers
{
    public class AdminControllerTests
    {
        [TestClass]
        public class TheIndexMethod
        {
            [TestMethod]
            public void Index_Contains_All_Products()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
                        new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
                        new Product { ProductID = 3, Name = "P3", Category = "Cat1" }
                    }.AsQueryable());

                AdminController target = new AdminController(mock.Object);

                // Act
                Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

                // Assert
                Assert.IsTrue(result.Length == 3);
                Assert.AreEqual(result[0].Name, "P1");
                Assert.AreEqual(result[1].Name, "P2");
                Assert.AreEqual(result[2].Name, "P3");
            }
        }

        [TestClass]
        public class TheEditMethod
        {
            [TestMethod]
            public void Can_Edit_Product()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1" },
                        new Product { ProductID = 2, Name = "P2" },
                        new Product { ProductID = 3, Name = "P3" }
                    }.AsQueryable());

                AdminController target = new AdminController(mock.Object);

                // Act
                Product p1 = target.Edit(1).ViewData.Model as Product;
                Product p2 = target.Edit(2).ViewData.Model as Product;
                Product p3 = target.Edit(3).ViewData.Model as Product;

                // Assert
                Assert.AreEqual(1, p1.ProductID);
                Assert.AreEqual(2, p2.ProductID);
                Assert.AreEqual(3, p3.ProductID);
            }

            [TestMethod]
            public void Cannot_Edit_Nonexistent_Product()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1" },
                        new Product { ProductID = 2, Name = "P2" },
                        new Product { ProductID = 3, Name = "P3" }
                    }.AsQueryable());

                AdminController target = new AdminController(mock.Object);

                // Act
                Product result = target.Edit(4).ViewData.Model as Product;

                // Assert
                Assert.IsNull(result);
            }

            [TestMethod]
            public void Can_Save_Valid_Changes()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                AdminController target = new AdminController(mock.Object);
                Product product = new Product { Name = "Test" };

                // Act
                ActionResult result = target.Edit(product);

                // Assert
                mock.Verify(m => m.SaveProduct(product));
                Assert.IsNotInstanceOfType(result, typeof(ViewResult));
            }

            [TestMethod]
            public void Cannot_Save_Invalid_Changes()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                AdminController target = new AdminController(mock.Object);
                Product product = new Product { Name = "Test" };
                target.ModelState.AddModelError("error", "error");

                // Act
                ActionResult result = target.Edit(product);

                // Assert
                mock.Verify(m => m.SaveProduct(product), Times.Never());
                Assert.IsInstanceOfType(result, typeof(ViewResult));
            }
        }

        [TestClass]
        public class TheDeleteMethod
        {
            [TestMethod]
            public void Can_Delete_Products()
            {
                // Arrange
                Product prod = new Product { ProductID = 2, Name = "Test" };
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1" },
                        prod,
                        new Product { ProductID = 3, Name = "P3"}
                    }.AsQueryable());

                AdminController target = new AdminController(mock.Object);

                // Act
                target.Delete(prod.ProductID);

                // Assert
                mock.Verify(m => m.DeleteProduct(prod.ProductID));
            }
        }
    }
}
