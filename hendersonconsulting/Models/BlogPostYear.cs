using System.Collections.Generic;

namespace HendersonConsulting.Models
{
    public class BlogPostYear
    {
        public List<BlogPostMonth> Months { get; set; }
        public string Prefix { get; set; }

        public int Year { get; set; }
    }
}