using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CrawlerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            //startCrawlerasync();

            var links = Crawler.GetNodsLink("https://divar.ir/s/tehran/real-estate/ahang");
            Console.WriteLine(links.Count);
            var nodes = Crawler.GetNodeDetails(links);
            for (int i = 0; i < nodes.Count; i++)
            {
                Console.WriteLine("Model: " + nodes[i].Title);
                Console.WriteLine("Link: " + nodes[i].Link);
                Console.WriteLine("Price: " + nodes[i].Price);
                Console.WriteLine("ImageUrl: " + nodes[i].ImageUrl);
                Console.WriteLine("...................................");
            }

            Crawler.exportToCSV(nodes);


            Console.WriteLine("Successful....");
            Console.WriteLine("Press Enter to exit the program...");
            ConsoleKeyInfo keyinfor = Console.ReadKey(true);
            if (keyinfor.Key == ConsoleKey.Enter)
            {
                System.Environment.Exit(0);
            }
        }


        private static async Task startCrawlerasync()
        {
            //the url of the page we want to test
            var url = "https://divar.ir/s/tehran/real-estate/ahang";
            var webGet = new HtmlWeb();
            var htmlDocument = new HtmlDocument();
            htmlDocument = webGet.Load(url);

            // a list to add all the list of cars and the various prices 
            var cars = new List<Divar>();
            var divs =
            htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("post-list-grid"));
            //get All ChileNode For divar divs post-card-item
            //var allDivs = divs.FirstOrDefault().ChildNodes.Select(c => c.ChildNodes);

            var allDivs = divs.Select(c => c.ChildNodes.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("post-card-item"))).FirstOrDefault();

            foreach (var div in allDivs)
            {
                var title = div.Descendants("h2")
                .Where(node => node.GetAttributeValue("class", "").Equals("kt-post-card__title")).FirstOrDefault().InnerText;
                var price = div.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("kt-post-card__description")).FirstOrDefault().InnerText;
                var link = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;

                var test = div.Descendants("img")
                    .Where(node => node.GetAttributeValue("class", "").Contains("kt-image-block__image"));

                var img = div.Descendants("img").FirstOrDefault() != null ? div.Descendants("img")
                    .Where(node => node.GetAttributeValue("class", "").Contains("kt-image-block__image")).FirstOrDefault().ChildAttributes("data-src").FirstOrDefault().Value : "No Image";

                var divar = new Divar
                {
                    Title = title,
                    Price = price,
                    Link = link,
                    ImageUrl = img,
                };
                cars.Add(divar);
            }

            for (int i = 0; i < cars.Count; i++)
            {
                Console.WriteLine("Model: " + cars[i].Title);
                Console.WriteLine("Link: " + cars[i].Link);
                Console.WriteLine("Price: " + cars[i].Price);
                Console.WriteLine("ImageUrl: " + cars[i].ImageUrl);
                Console.WriteLine("...................................");
            }
        }
    }
}
