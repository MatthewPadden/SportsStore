using System.Linq;

using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.UnitTests.TestHelpers
{
    public static class Helper
    {
        public static Mock<IProductRepository> CreateMockProductRepository(int numberOfItems)
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            Product[] products = new Product[numberOfItems];
            for (int i = 0; i < numberOfItems; i++)
            {
                products[i] = new Product { ProductID = i + 1, Name = "P" + (i + 1).ToString() };
            }

            mock.Setup(m => m.Products).Returns(products.AsQueryable());
            return mock;
        }
    }
}
