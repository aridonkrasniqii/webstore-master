using WebStore.Models;

namespace WebStore.DAL
{
    public class WebStoreInitializer
    {
        public  static void Seed(WebStoreContext context)
        {
            var categories = new List<Category>
            {
                new() { Name = "Books", Description = "Words on a page in a binding." },
                new() { Name = "Bolts", Description = "We have lots of bolts." },
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var bookCategory = categories.First(c => c.Name == "Books");
            var boltCategory = categories.First(c => c.Name == "Bolts");
            var products = new List<Product>
            {
                new()
                {
                    Name = "Introduction to Network Security by Douglas Jacobson",
                    Description =
                        "Unlike data communications of the past, today’s networks consist of numerous devices that handle the data as it passes from the sender to the receiver. However, security concerns are frequently raised in circumstances where interconnected computers use a network not controlled by any one entity or organization. Introduction to Network Security examines various network protocols, focusing on vulnerabilities, exploits, attacks, and methods to mitigate an attack. ",
                    ReleaseDate = DateTime.Parse("2008-11-18 06:00"), StockCount = 100, Category = bookCategory, Cost = 4995
                },
                new() { Name = "M2 5mm", Description = "M2 5mm", Category = boltCategory, Cost = 5}
            };
            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            var firstBook = context.Products.First(p => p.Category == boltCategory);
            var firstBolt = context.Products.First(p => p.Category == boltCategory);
            var orders = new List<Order>
            {
                new() { State = OrderState.Processing, PlacedBy = "Administrator" },
                new() { State = OrderState.Processing, PlacedBy = "Administrator" },
                new() { State = OrderState.Processing, PlacedBy = "Administrator" },
            };
            orders.ForEach(o => context.Orders.Add(o));
            context.SaveChanges();

            var orderItems = new List<OrderItem>
            {
                new() {OrderId = 1, Product = firstBook},
                new() {OrderId = 2, Product = firstBolt},
                new() { OrderId = 3, Product = firstBolt},
                new() { OrderId = 3, Product = firstBook},
            };
            context.SaveChanges();

            var reviews = new List<Review>
            {
                new() { ProductId = 1, Stars = 5, Text = "Good", Title = "Good Book" }
            };
            reviews.ForEach(r => context.Reviews.Add(r));
            context.SaveChanges();
        }

        public static void CreateDbIfNotExist(WebApplication host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<WebStoreContext>();
                Seed(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred when creating the database.");
            }
        }
    }
}
