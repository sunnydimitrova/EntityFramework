using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public enum ResourceType
        {
            Video = 1,
            Presentation = 2,
            Document = 3,
            Other = 4
        }

        public int CourseId { get; set; }

        public Course Course { get; set; }

    }
}
