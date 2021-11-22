using CarDealer.Data;
using CarDealer.DTO.Input;
using CarDealer.DTO.Output;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            //CreateDatabase(context);

            //var inputXml = File.ReadAllText(@"../../../Datasets/sales.xml");

            var result = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(result);

        }

        public static void CreateDatabase(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSirializer = new XmlSerializer(typeof(SupplierImportModel[]), new XmlRootAttribute("Suppliers"));

            var textReader = new StringReader(inputXml);

            var suppliersDto = xmlSirializer.Deserialize(textReader)
                as SupplierImportModel[];

            var suppliers = suppliersDto
                .Select(x => new Supplier
                {
                    Name = x.Name,
                    IsImporter = x.IsImporter
                }).ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";

        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSirializer = new XmlSerializer(typeof(PartInputModel[]), new XmlRootAttribute("Parts"));

            var xmlReader = new StringReader(inputXml);

            var partsDto = xmlSirializer.Deserialize(xmlReader) as PartInputModel[];

            var suppliersId = context
                .Suppliers
                .Select(x => x.Id)
                .ToArray();

            var parts = partsDto
                .Where(x => suppliersId.Contains(x.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                }).ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerealizer = new XmlSerializer(typeof(CarInputModel[]), new XmlRootAttribute("Cars"));

            var xmlReader = new StringReader(inputXml);

            var carsDto = xmlSerealizer.Deserialize(xmlReader) as CarInputModel[];

            var cars = new List<Car>();
            var partsCars = new List<PartCar>();

            foreach (var carDto in carsDto)
            {
                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance
                };

                var parts = carDto
                    .Parts
                    .Where(x => context.Parts.Any(p => p.Id == x.Id))
                    .Select(x => x.Id)
                    .Distinct()
                    .ToList();

                foreach (var partId in parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = partId,
                        Car = car
                    };

                    partsCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partsCars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(CustomerInputModel[]), new XmlRootAttribute("Customers"));

            var reader = new StringReader(inputXml);

            var customerDto = xmlSerializer.Deserialize(reader) as CustomerInputModel[];
            

            var customers = customerDto
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                }).ToArray();
            

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(SaleInputModel[]), new XmlRootAttribute("Sales"));

            var reader = new StringReader(inputXml);

            var saleDto = xmlSerializer.Deserialize(reader) as SaleInputModel[];

            var cars = context.Cars.Select(x => x.Id).ToList();

            var sales = saleDto
                .Where(x => cars.Contains(x.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                }).ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(x => x.TravelledDistance > 2000000)
                .Select(x => new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute("cars"));

            var writer = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(writer, cars, ns);

            var result = writer.ToString().TrimEnd();

            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmws = context
                .Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new BMWOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(BMWOutputModel[]), new XmlRootAttribute("cars"));
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, bmws, ns);

            return textWriter.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new LocalSuppliersOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(LocalSuppliersOutputModel[]), new XmlRootAttribute("suppliers"));
            var textWriter = new StringWriter();
            var sn = new XmlSerializerNamespaces();
            sn.Add("", "");

            xmlSerializer.Serialize(textWriter, suppliers, sn);

            return textWriter.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context
                .Cars
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .Select(x => new CarWithPartsOutputModelcs
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars
                    .Select(pc => new PartOutputModel
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(pc => pc.Price)
                    .ToArray()
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarWithPartsOutputModelcs[]), new XmlRootAttribute("cars"));
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, ns);

            return textWriter.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Sales
                .Where(x => x.Customer.Sales.Count > 0)
                .Select(x => new SaleByCustomerOutputModel
                {
                    FullName = x.Customer.Name,
                    BoughtCars = x.Customer.Sales.Count,
                    SpentMoney = x.Customer.Sales.Select(s => s.Car).SelectMany(c => c.PartCars).Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SaleByCustomerOutputModel[]), new XmlRootAttribute("customers"));
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, customers, ns);

            return textWriter.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(x => new SaleDiscountOutputModel
                {
                    Car = new CarOutputModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(pc => pc.Part.Price) -
                    x.Car.PartCars.Sum(pc => pc.Part.Price) * x.Discount / 100m
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SaleDiscountOutputModel[]), new XmlRootAttribute("sales"));
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, sales, ns);

            return textWriter.ToString().TrimEnd();
        }
    }
}