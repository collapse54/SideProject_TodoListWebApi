using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Thing
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int? Finish { get; set; }
        public DateTime? AddDate { get; set; }
    }

    public class RecycleThing
    {
        [Required]
        public string Title { get; set; }
    }
}
