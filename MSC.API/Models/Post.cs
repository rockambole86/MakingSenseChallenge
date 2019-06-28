using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSC.API.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public string Html { get; set; }
        public int VisitCount { get; set; }
        public byte Status { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateBy { get; set; }

        [NotMapped]
        public virtual ICollection<PostTag> PostTag { get; set; }

        [NotMapped]
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
