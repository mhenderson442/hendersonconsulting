using System.Collections.Generic;

namespace HendersonConsulting.Models
{
    public class BlogPostMonth
    {
        public List<BlogPostItem> BlogPostItems { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public string Prefix { get; set; }
    }
}