using System.Linq;
using System.Web.Mvc;

using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }

        public ViewResult List(string category, int page = 1)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = repository.Products
                .Where(p => category == null || category == string.Empty || p.Category == category) // if category is not null/empty, get where Category == category
                .OrderBy(p => p.ProductID) // order by ProductID
                .Skip((page - 1) * PageSize)
                .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null || category == string.Empty ?
                        repository.Products.Count() :
                        repository.Products.Where(e => e.Category == category).Count()
                },

                CurrentCategory = category
            };

            return View(model);
        }
    }
}