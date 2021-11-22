using System.Xml.Serialization;

namespace CarDealer.DTO.Input
{
    [XmlType("partId")]
    public class CarPartsModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}