using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlAgilityPack;

namespace takasu
{

    class Program
    {


        static void Main(string[] args)
        {


            var url = "https://www.city-urayasu.ed.jp/takas-es/3015504/3015505/index.html";
            var document = GetDocument(url);

            List<string> links = new List<string>();

            Dictionary<string, string> pdfs_collection = new Dictionary<string, string>();

            HtmlNodeCollection nodeCollection = document.DocumentNode.SelectNodes("//div[@id='content']//li");


            foreach (HtmlNode node in nodeCollection)
            {

                var hrefs = node.Descendants("a").Select(node => node.GetAttributeValue("href", "")).ToList()[0];
                links.Add(hrefs);

            }


            foreach (string url_string in links)
            {

                //< a href = "../../../takas-es/3015504/3015505/3017118/index.html" > R3年度1学期 </ a >
                string full_url = "https://www.city-urayasu.ed.jp/" + url_string.Replace("../../../", "");


            
                foreach (var newpdf in GetPdf(full_url))
                {
                    pdfs_collection.Add(newpdf.Key, newpdf.Value);
                }

            }



            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            var jsonstr = JsonSerializer.Serialize(pdfs_collection,options);
            Console.WriteLine(jsonstr);

            var fadfa = 3;


        }


        static Dictionary<string, string> GetPdf(string url)
        {


            Dictionary<string, string> pdfs = new Dictionary<string, string>();
            var sub_document = GetDocument(url);

            HtmlNodeCollection nodeCollection = sub_document.DocumentNode.SelectNodes("//div[@id='content']//li");
            
            foreach (HtmlNode node in nodeCollection)
            {

                var hrefs = node.Descendants("a").Select(node => node.GetAttributeValue("href", "")).ToList()[0];
                
                string pdf_url = "https://www.city-urayasu.ed.jp/" + hrefs.Replace("../../../../", "");
               


                pdfs.Add(pdf_url, node.InnerText.Replace("\n","").Replace("\u3000",""));
                //pdfs.Add(pdf_url, node.InnerText.Replace("\\n", ""));
            }
            
            return pdfs;

        }



        static HtmlDocument GetDocument(string url)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            var web = new System.Net.WebClient();
            web.Encoding = Encoding.UTF8;
            doc.LoadHtml(web.DownloadString(url));
            return doc;
        }
    }
}
