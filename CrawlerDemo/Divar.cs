using System.Diagnostics;

namespace CrawlerDemo
{
    [DebuggerDisplay("{Model}, {Price}")]
    public class Divar
    {

        public string Title { get; set; }
        public string Price { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }


    }
}
