using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.UnitTests.TestHelpers;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests.Tests.SportsStore.Web.UI.Controllers
{
    public class ProductControllerTests
    {
        [TestClass]
        public class TheListMethod
        {
            [TestMethod]
            public void Can_Paginate()
            {
                // Arrange
                Mock<IProductRepository> mock = Helper.CreateMockProductRepository(5);
                // create a controller and make the page size 3 items
                ProductController controller = new ProductController(mock.Object);
                controller.PageSize = 3;

                // Act
                ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

                // Asssert
                Product[] prodArray = result.Products.ToArray();
                Assert.IsTrue(prodArray.Length == 2);
                Assert.AreEqual(prodArray[0].Name, "P4");
                Assert.AreEqual(prodArray[1].Name, "P5");
            }

            [TestMethod]
            public void Can_Send_Pagination_View_Model()
            {
                // Arrange
                Mock<IProductRepository> mock = Helper.CreateMockProductRepository(5);
                ProductController controller = new ProductController(mock.Object);
                controller.PageSize = 3;

                // Act
                ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
                PagingInfo pageInfo = result.PagingInfo;

                // Assert
                Assert.AreEqual(pageInfo.CurrentPage, 2);
                Assert.AreEqual(pageInfo.ItemsPerPage, 3);
                Assert.AreEqual(pageInfo.TotalItems, 5);
                Assert.AreEqual(pageInfo.TotalPages, 2);
            }

            [TestMethod]
            public void Can_Filter_Products()
            {
                // Arrange
                // create the mock repository
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1", Category = "Cat1"},
                        new Product { ProductID = 2, Name = "P2", Category = "Cat2"},
                        new Product { ProductID = 3, Name = "P3", Category = "Cat1"},
                        new Product { ProductID = 4, Name = "P4", Category = "Cat2"},
                        new Product { ProductID = 5, Name = "P5", Category = "Cat3"}
                    }.AsQueryable());

                // create a controller and make the page size 3 items
                ProductController controller = new ProductController(mock.Object);
                controller.PageSize = 3;

                // Action
                Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

                // Assert
                Assert.AreEqual(result.Length, 2);
                Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
                Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
            }

            [TestMethod]
            public void Generate_Category_Specific_Product_Count()
            {
                // Arrange
                // create the mock repository
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1", Category = "Cat1"},
                        new Product { ProductID = 2, Name = "P2", Category = "Cat2"},
                        new Product { ProductID = 3, Name = "P3", Category = "Cat1"},
                        new Product { ProductID = 4, Name = "P4", Category = "Cat2"},
                        new Product { ProductID = 5, Name = "P5", Category = "Cat3"}
                    }.AsQueryable());

                // create a controller and make the page size 3 items
                ProductController target = new ProductController(mock.Object);
                target.PageSize = 3;

                // Action
                int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
                int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
                int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
                int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

                // Assert
                Assert.AreEqual(res1, 2);
                Assert.AreEqual(res2, 2);
                Assert.AreEqual(res3, 1);
                Assert.AreEqual(resAll, 5);
            }
        }

        [TestClass]
        public class TheGetImageMethod
        {
            [TestMethod]
            public void Can_Retrieve_Image_Data()
            {
                Product prod = new Product
                {
                    ProductID = 2,
                    Name = "Test",
                    ImageData = new byte[] { },
                    ImageMimeType = "image/png"
                };

                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1" },
                        prod,
                        new Product { ProductID = 3, Name = "P3" }
                    }.AsQueryable());

                ProductController target = new ProductController(mock.Object);

                // Act
                ActionResult result = target.GetImage(prod.ProductID);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(FileResult));
                Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
            }

            [TestMethod]
            public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1" },
                        new Product { ProductID = 2, Name = "P2" }
                    }.AsQueryable());

                ProductController target = new ProductController(mock.Object);

                // Act
                ActionResult result = target.GetImage(100);

                // Assert
                Assert.IsNull(result);
            }
        }
    }
}