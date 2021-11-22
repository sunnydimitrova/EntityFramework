﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisinorsMailsDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"The [A-Z]{1}[a-z]+")]
        public string Nickname { get; set; }

        [Range(18, 65)]
        public int Age { get; set; }

        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        public decimal? Bail { get; set; }

        public int? CellId { get; set; }

        public MailImportModel[] Mails { get; set; }

    }


    //    ⦁	Id – integer, Primary Key
    //⦁	FullName – text with min length 3 and max length 20 (required)
    //⦁	Nickname – text starting with "The " and a single word only of letters with an uppercase letter for beginning(example: The Prisoner) (required)
    //⦁	Age – integer in the range[18, 65] (required)
    //⦁	IncarcerationDate – Date(required)
    //⦁	ReleaseDate– Date
    //⦁	Bail– decimal (non-negative, minimum value: 0)
    //⦁	CellId - integer, foreign key
    //⦁	Cell – the prisoner's cell
    //⦁	Mails - collection of type Mail
    //⦁	PrisonerOfficers - collection of type OfficerPrisoner

}
