using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.API.Models
{
    public class Blog
    {
        public string BlogId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishDate { get; set; }

        public virtual Author Author { get; set; }
    }
}
