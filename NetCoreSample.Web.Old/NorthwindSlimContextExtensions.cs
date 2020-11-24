using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using NetCoreSample.Entities;

namespace NetCoreSample.Web
{
    public static class NorthwindSlimContextExtensions
    {
        public static void EnsureSeedData(this NorthwindSlimContext context)
        {
            context.Database.OpenConnection();
            try
            {
                if (!context.Categories.Any())
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.[Category] ON");
                    AddCategories(context);
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.[Category] OFF");
                }

                if (!context.Products.Any())
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.[Product] ON");
                    AddProducts(context);
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.[Product] OFF");
                }

                if (!context.Customers.Any())
                {
                    AddCustomers(context);
                    context.SaveChanges();
                }

                if (!context.Orders.Any())
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.[Order] ON");
                    AddOrders(context);
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.[Order] OFF");
                }
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        private static void AddOrders(NorthwindSlimContext context)
        {
            context.Orders.Add(new Order
            {
                CustomerId = "ALFKI",
                OrderId = 1,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 10M
                    },
                    new OrderDetail
                    {
                        ProductId = 2,
                        Quantity = 2,
                        UnitPrice = 20M
                    },
                }
            });
            context.Orders.Add(new Order
            {
                CustomerId = "ALFKI",
                OrderId = 2,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 3,
                        Quantity = 3,
                        UnitPrice = 30M
                    },
                }
            });
            context.Orders.Add(new Order
            {
                CustomerId = "ANATR",
                OrderId = 3,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 2,
                        Quantity = 4,
                        UnitPrice = 20M
                    },
                }
            });
            context.Orders.Add(new Order
            {
                CustomerId = "ANTON",
                OrderId = 4,
                OrderDate = DateTime.Today,
                ShippedDate = DateTime.Today,
                Freight = 41.34M,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = 3,
                        Quantity = 5,
                        UnitPrice = 30M
                    },
                }
            });
        }

        private static void AddCustomers(NorthwindSlimContext context)
        {
            context.Customers.Add(new Customer
            {
                CustomerId = "ALFKI",
                CompanyName = "Alfreds Futterkiste",
                ContactName = "Maria Anders",
                City = "Berlin",
                Country = "Germany"
            });
            context.Customers.Add(new Customer
            {
                CustomerId = "ANATR",
                CompanyName = "Ana Trujillo Emparedados y helados",
                ContactName = "Ana Trujillo",
                City = "México D.F.",
                Country = "Mexico"
            });
            context.Customers.Add(new Customer
            {
                CustomerId = "ANTON",
                CompanyName = "Antonio Moreno Taquería",
                ContactName = "Antonio Moreno",
                City = "México D.F.",
                Country = "Mexico"
            });
        }

        private static void AddProducts(NorthwindSlimContext context)
        {
            context.Products.Add(new Product
            {
                ProductId = 1,
                CategoryId = 1,
                ProductName = "Chai",
                UnitPrice = 23M
            });
            context.Products.Add(new Product
            {
                ProductId = 2,
                CategoryId = 1,
                ProductName = "Chang",
                UnitPrice = 23M
            });
            context.Products.Add(new Product
            {
                ProductId = 3,
                CategoryId = 2,
                ProductName = "Aniseed Syrup",
                UnitPrice = 23M
            });
            context.Products.Add(new Product
            {
                ProductId = 4,
                CategoryId = 2,
                ProductName = "Chef Anton's Cajun Seasoning",
                UnitPrice = 23M
            });
        }

        private static void AddCategories(NorthwindSlimContext context)
        {
            context.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "Beverages"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 2,
                CategoryName = "Condiments"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 3,
                CategoryName = "Confections"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 4,
                CategoryName = "Dairy Products"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 5,
                CategoryName = "Grains/Cereals"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 6,
                CategoryName = "Meat/Poultry"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 7,
                CategoryName = "Produce"
            });
            context.Categories.Add(new Category
            {
                CategoryId = 8,
                CategoryName = "Seafood"
            });
        }
    }
}