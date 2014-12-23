using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests.Tests.SportsStore.WebUI.Controllers
{
    public class CartControllerTests
    {
        [TestClass]
        public class TheIndexMethod
        {
            [TestMethod]
            public void Can_View_Cart_Contents()
            {
                // Arrange
                Cart cart = new Cart();
                CartController target = new CartController(null, null);

                // Act
                CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

                // Assert
                Assert.AreSame(result.Cart, cart);
                Assert.AreEqual(result.ReturnUrl, "myUrl");
            }
        }

        [TestClass]
        public class TheAddToCartMethod
        {
            [TestMethod]
            public void Can_Add_To_Cart()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1", Category = "Apples" }
                    }.AsQueryable());

                Cart cart = new Cart();
                CartController target = new CartController(mock.Object, null);

                // Act
                target.AddToCart(cart, 1, null);

                // Assert
                Assert.AreEqual(cart.Lines.Count(), 1);
                Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
            }

            [TestMethod]
            public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
            {
                // Arrange
                Mock<IProductRepository> mock = new Mock<IProductRepository>();
                mock.Setup(m => m.Products).Returns(new Product[]
                    {
                        new Product { ProductID = 1, Name = "P1", Category = "Apples" }
                    }.AsQueryable());

                Cart cart = new Cart();
                CartController target = new CartController(mock.Object, null);

                // Act
                RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

                // Assert
                Assert.AreEqual(result.RouteValues["action"], "Index");
                Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
            }
        }

        [TestClass]
        public class TheCheckoutMethod
        {
            [TestMethod]
            public void Cannot_Checkout_Empty_Cart()
            {
                // Arrange
                Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
                Cart cart = new Cart();
                ShippingDetails shippingDetials = new ShippingDetails();
                CartController target = new CartController(null, mock.Object);

                // Act
                ViewResult result = target.Checkout(cart, shippingDetials);

                // Assert
                // check that the order has not been passed on to the processor
                mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            }

            [TestMethod]
            public void Cannot_Checkout_Invalid_ShippingDetails()
            {
                // Arrange
                Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
                Cart cart = new Cart();
                cart.AddItem(new Product(), 1);

                CartController target = new CartController(null, mock.Object);
                target.ModelState.AddModelError("error", "error");

                // Act
                ViewResult result = target.Checkout(cart, new ShippingDetails());

                // Assert
                mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
            }

            [TestMethod]
            public void Can_Checkout_And_Submit_Order()
            {
                // Assert
                Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
                Cart cart = new Cart();
                cart.AddItem(new Product(), 1);

                CartController target = new CartController(null, mock.Object);

                // Act
                ViewResult result = target.Checkout(cart, new ShippingDetails());

                // Assert
                mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
                Assert.AreEqual("Completed", result.ViewName);
                Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
            }
        }
    }
}
