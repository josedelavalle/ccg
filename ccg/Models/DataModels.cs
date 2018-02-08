using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ccg.Models
{
    public class News
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
    }

    public class CaseStudies
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
    }

    public class Locations
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Image { get; set; }
        public List<decimal> Coordinates { get; set; }
    }

}