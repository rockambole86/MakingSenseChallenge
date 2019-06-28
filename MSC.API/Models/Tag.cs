using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSC.API.Models
{
    public partial class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public virtual ICollection<PostTag> PostTag { get; set; }
    }
}
