using System.Collections.Generic;

namespace HendersonConsulting.Models
{
    public class Category
    {
        public List<BlogPostItem> BlogPostItems { get; set; }
        public string Name { get; set; }
    }
}