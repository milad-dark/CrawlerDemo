using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CrawlerDemo
{
    public static class Crawler
    {
        private static HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            web.AutoDetectEncoding = true;
            HtmlDocument doc = web.Load(url);
            return doc;
        }

        public static List<string> GetNodsLink(string url)
        {
            var nodLink = new List<string>();
            var doc = GetDocument(url);

            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class,\"browse-post-list\")]");
            var nodeList = nodes.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("post-card-item"));
            //.Select(s=>s.SelectNodes(@"//div[contains(@class,""post-card-item"")]/div/a"));

            var baseUri = new Uri(url);
            foreach (var node in nodeList)
            {
                var href = node.ChildNodes["div"].ChildNodes["a"].Attributes["href"].Value;
                nodLink.Add(new Uri(baseUri, href).AbsoluteUri);
            }
            return nodLink;
        }

        public static List<Divar> GetNodeDetails(List<string> urls)
        {
            var items = new List<Divar>();
            foreach (var url in urls)
            {
                var document = GetDocument(url);
                var titleXPath = "//div[contains(@class,\"kt-page-title__title\")]";
                var priceXPath = "//div[contains(@class,\"kt-base-row__end\")]/p[@class=\"kt-unexpandable-row__value\"]";
                var hrefXPath = "//a";
                var imgXPath = "//img[contains(@class,\"kt-image-block__image\")]";

                var divar = new Divar();
                divar.Title = document.DocumentNode.SelectSingleNode(titleXPath).InnerText;
                divar.Price = document.DocumentNode.SelectSingleNode(priceXPath).InnerText;
                divar.Link = url;
                //document.DocumentNode.SelectSingleNode(hrefXPath).Attributes["href"].Value;
                divar.ImageUrl = document.DocumentNode.SelectSingleNode(imgXPath) != null ? document.DocumentNode.SelectSingleNode(imgXPath).Attributes["src"].Value : "No Image";
                items.Add(divar);
            }
            return items;
        }

        public static void exportToCSV(List<Divar> divars)
        {
            using (var writer = new StreamWriter("./divar-items.csv", false, System.Text.Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
            {
                csv.WriteRecords(divars);
            }
        }
    }
}
