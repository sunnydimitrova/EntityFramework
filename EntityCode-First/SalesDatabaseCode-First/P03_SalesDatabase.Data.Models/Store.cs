﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P03_SalesDatabase.Data.Models
{
    public class Store
    {
        public Store()
        {
            this.Sales = new HashSet<Sale>();
        }
        [Key]
        public int StoreId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
