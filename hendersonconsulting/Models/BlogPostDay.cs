using System.Collections.Generic;

namespace HendersonConsulting.Models
{
    public class BlogPostDay
    {
        public List<BlogPostItem> BlogPostList { get; set; }
        public int Day { get; set; }
        public string DayName { get; set; }
        public string Prefix { get; set; }
    }
}