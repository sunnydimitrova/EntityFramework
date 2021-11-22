namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var departmentCellDto = JsonConvert.DeserializeObject<List<ImportDepartmenSellDto>>(jsonString);

            var departmentsWithCell = new List<Department>();

            foreach (var department in departmentCellDto)
            {
                if (!IsValid(department) || !IsValid(department.Cells.Any()) || 
                    !department.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var currentDepartment = new Department
                {
                    Name = department.Name,
                    Cells = department.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    }).ToList()
                };
                

                departmentsWithCell.Add(currentDepartment);

                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");

            }
            
            context.Departments.AddRange(departmentsWithCell);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            
            var prisinorsMailDto = JsonConvert.DeserializeObject<IEnumerable<ImportPrisinorsMailsDto>>(jsonString);

            var prisiners = new List<Prisoner>();

            foreach (var currentPrisiner in prisinorsMailDto)
            {
                if (!IsValid(currentPrisiner) || !currentPrisiner.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var incarcerationDate = DateTime.ParseExact(currentPrisiner.IncarcerationDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);

                var releaseDate = DateTime.TryParseExact
                    (currentPrisiner.ReleaseDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None,
                    out DateTime resultReleaseDate);

                var prisiner = new Prisoner
                {
                    FullName = currentPrisiner.FullName,
                    Nickname = currentPrisiner.Nickname,
                    Age = currentPrisiner.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate ? (DateTime?)resultReleaseDate : null,
                    Bail = currentPrisiner.Bail,
                    CellId = currentPrisiner.CellId,
                    Mails = currentPrisiner.Mails.Select(x => new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address
                    }).ToArray()
                };

                sb.AppendLine($"Imported {prisiner.FullName} {prisiner.Age} years old");
                prisiners.Add(prisiner);
            }
            context.Prisoners.AddRange(prisiners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(List<OfficerPrisinerImportModelDto>), new XmlRootAttribute("Officers"));

            var reader = new StringReader(xmlString);

            var offisersPrisonersDto = xmlSerializer.Deserialize(reader) as List<OfficerPrisinerImportModelDto>;

            var offiserPrisoners = new List<Officer>();

            foreach (var offiser in offisersPrisonersDto)
            {
                if (!IsValid(offiser))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var offiserPrisoner = new Officer
                {
                    FullName = offiser.FullName,
                    Salary = offiser.Salary,
                    Position = Enum.Parse<Position>(offiser.Position),
                    Weapon = Enum.Parse<Weapon>(offiser.Weapon),
                    DepartmentId = offiser.DepartmentId,
                    OfficerPrisoners = offiser.Prisoners.Select(x => new OfficerPrisoner
                    {
                        PrisonerId = x.Id
                    }).ToArray()
                };

                sb.AppendLine($"Imported {offiser.FullName} ({offiser.Prisoners.Length} prisoners)");
                offiserPrisoners.Add(offiserPrisoner);
            }

            context.Officers.AddRange(offiserPrisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
            
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}