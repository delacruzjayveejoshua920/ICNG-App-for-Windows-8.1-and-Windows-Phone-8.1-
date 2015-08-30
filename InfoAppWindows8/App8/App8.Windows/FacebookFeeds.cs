using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;


namespace App8
{
    public class FacebookFeeds
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Generator { get; set; }
        public string lastBuildDate { get; set; }
        public string webmaster { get; set; }
        public Array[] Pubdate { get; set; }
        public Array[] Author { get; set; }
        
    }
}
