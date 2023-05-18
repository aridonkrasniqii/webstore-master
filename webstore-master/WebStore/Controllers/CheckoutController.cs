using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebStore.DAL;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly WebStoreContext _context;
        public CheckoutController(WebStoreContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            string? cookieCart = string.Empty;
            var hasValue = HttpContext.Request.Cookies.TryGetValue("cart", out cookieCart);
            string? cookieUser = string.Empty;
            var hasUser = HttpContext.Request.Cookies.TryGetValue("username", out cookieUser);

            if (!hasValue || !hasUser)
            {
                return View();
            }
            List<Product> products = new List<Product>();
            string cookie = cookieCart.Normalize();
            string user = cookieUser.Normalize();
            cookie = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
            var order = new Models.Order { PlacedBy = user, State = OrderState.Placed };
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var productId in cookie.Split(","))
            {
                var OrderItem = new OrderItem { OrderId = order.Id, ProductId = int.Parse(productId) };
                var p = _context.Products.First(p => p.Id == int.Parse(productId));
                p.StockCount -= 1;
                _context.OrderItems.Add(OrderItem);
            }
            Response.Cookies.Delete("cart");
            _context.SaveChanges();
            return View(order);
        }

        public IActionResult Cart()
        {
            string? cookieCart = "";
            var hasValue = HttpContext.Request.Cookies.TryGetValue("Cart", out cookieCart);
            List<Product> products = new List<Product>();   
            if (!hasValue)
            {
                return View(products);
            }
            else
            {
                string cookie = cookieCart.Normalize();
                cookie = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
                foreach (var productId in cookie.Split(","))
                {
                    var p = _context.Products.First(p => p.Id == int.Parse(productId));
                    products.Add(p);
                }
                return View(products);
            }
        }
    }
}
