using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            var result = GetSoldProducts(context);
            Console.WriteLine(result);
        }

        public static void CreateDatabase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(UserImportModel[]), new XmlRootAttribute("Users"));

            var textReader = new StringReader(inputXml);

            var usersDto = xmlSerializer.Deserialize(textReader) as UserImportModel[];

            var users = usersDto
                .Where(x => x.Age != null)
                .Select(x => new User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age
                }).ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";

        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ProductImportModel[]), new XmlRootAttribute("Products"));

            var textReader = new StringReader(inputXml);

            var productsDto = xmlSerializer.Deserialize(textReader) as ProductImportModel[];

            var products = productsDto
                .Select(x => new Product
                {
                    Name = x.Name,
                    Price = x.Price,
                    SellerId = x.SellerId,
                    BuyerId = x.BuyerId
                }).ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(CategoryImportModel[]), new XmlRootAttribute("Categories"));

            var textReader = new StringReader(inputXml);
            var cateroriesDto = xmlSerializer.Deserialize(textReader) 
                as CategoryImportModel[];

            var categories = cateroriesDto
                .Where(x => x.Name != null)
                .Select(x => new Category
                {
                    Name = x.Name
                }).ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(CategoryProductImportModel[]), new XmlRootAttribute("CategoryProducts"));

            var textReader = new StringReader(inputXml);
            var categoryProductsDto = xmlSerializer.Deserialize(textReader) as CategoryProductImportModel[];

            var categories = context.Categories.Select(x => x.Id).ToList();
            var products = context.Products.Select(x => x.Id).ToList();

            var categoryProducts = categoryProductsDto
                .Where(x => categories.Contains(x.CategoryId) && products.Contains(x.ProductId))
                .Select(x => new CategoryProduct
                {
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId
                }).ToArray();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";

        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductExportModelSecond
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();
            var xmlSerializer = new XmlSerializer(typeof(ProductExportModelSecond[]), new XmlRootAttribute("Products"));
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, products, ns);

            return textWriter.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context.
                Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(x => new SoldProductExportModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(ps => new ProductExportModelSecond
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    }).ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SoldProductExportModel[]), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, soldProducts, ns);

            return textWriter.ToString().TrimEnd();
        }
    }
}