using SoftJail.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficerPrisinerImportModelDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        [XmlElement("Name")]
        public string FullName { get; set; }

        [XmlElement("Money")]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        [XmlElement("Position")]
        [EnumDataType(typeof(Position))]
        public string Position { get; set; }

        [Required]
        [XmlElement("Weapon")]
        [EnumDataType(typeof(Weapon))]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public PrisinerImportModel[] Prisoners { get; set; }
    }

    //    ⦁	Id – integer, Primary Key
    //⦁	FullName – text with min length 3 and max length 30 (required)
    //⦁	Salary – decimal (non-negative, minimum value: 0) (required)
    //⦁	Position - Position enumeration with possible values: “Overseer, Guard, Watcher, Labour” (required)
    //⦁	Weapon - Weapon enumeration with possible values: “Knife, FlashPulse, ChainRifle, Pistol, Sniper” (required)
    //⦁	DepartmentId - integer, foreign key(required)
    //⦁	Department – the officer's department (required)
    //⦁	OfficerPrisoners - collection of type OfficerPrisoner

}
