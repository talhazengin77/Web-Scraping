using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HtmlAgilityPack;

namespace Web_Scraping
{
    internal class Program
    {
        static List<int> avgValues = new List<int>();
        static List<string> values = new List<string>();
        const string domain = "https://www.sahibinden.com/";
        public static HtmlDocument GetHomepage()
        {
            Uri url = new Uri(domain);
            WebClient client = new WebClient();                     //siteye erişim için client tanımladık.
            //client.Proxy = new WebProxy("193.53.87.220", 33128);  

            string html = client.DownloadString(url);               //Adresten istek yapı html kodlarını indiriyoruz.
            Thread.Sleep(2000);                                     // 'The remote server returned an error: (429) .' hatasından kaçınmak için 2 saniye delay koyuyoruz
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);                                     //İndirdiğimiz html kodlarını bir HtmlDocument nesnesine yüklüyoruz.

            return doc;
            
        }
        public static void GetDetailPage()
        {
            var links = GetHomepage().DocumentNode.SelectNodes("//ul[@class='vitrin-list clearfix']/li/a[@href]");  //Html dökümanı içindeki ul etiketinde class'ı vitrin-list clearfix olanları liste halinde alıyoruz
            FileStream fs = new FileStream("C:\\Users\\talha\\Desktop\\avarage.txt", FileMode.Append, FileAccess.Write, FileShare.Write); //dosya oluşturulur.
            StreamWriter sw = new StreamWriter(fs); //dosyanın içine yazmak için bir nesne oluşturduk.
            int sum = 0;
            foreach (var link in links)
            {
                HtmlAttribute attribute = link.Attributes["href"];

                if (attribute.Value.StartsWith("/ilan/"))
                {
                    Uri urunUrl = new Uri(domain + attribute.Value);                           //domain ile ilanın linkini birleştirdik.
                    WebClient webClient2 = new WebClient();                                    //siteye erişim için client tanımladık.
                    string urunHtml = webClient2.DownloadString(urunUrl);                      //html'e çevirdik.
                    //Thread.Sleep(2000);
                    HtmlAgilityPack.HtmlDocument urunDoc = new HtmlAgilityPack.HtmlDocument(); //HtmlDocument tipinde bir nesne oluşturduk.
                    urunDoc.LoadHtml(urunHtml);                                                //İndirdiğimiz html kodlarını bir HtmlDocument nesnesine yüklüyoruz.
                    HtmlNode[] urunLinks = urunDoc.DocumentNode.SelectNodes(@"//*[@id=""favoriteClassifiedPrice""]").ToArray();
                    foreach (var item in urunLinks)
                    {
                        values.Add(item.Attributes["value"].Value); //fiyat değerini listeye ekliyoruz. 
                    }
                }

                for (int i = 0; i < values.Count; i++)
                {
                    values[i] = values[i].Replace("TL", "");
                    values[i] = values[i].Replace(".", "");     // values listesindeki her bir eleman int'e dönüştürülebilmesi için TL ve . kaldırıldı.
                    Convert.ToInt32(values[i]);                 // int'e dönüştürüldü
                    avgValues.Add(Convert.ToInt32(values[i]));  // dönüştürülen elemanlar avgValues listesine eklendi.
                }

                for (int i = 0; i < avgValues.Count; i++)
                {
                    sum += avgValues[i];
                    Console.WriteLine("Avarage Price:" + sum / avgValues.Count);
                    sw.WriteLine(sum/avgValues.Count);
                }
                //string avg = avgValues.Average().ToString();
                //sw.WriteLine(avg);
            }
            sw.Close();
        }
        static void Main(string[] args)
        {
            GetDetailPage();
            Console.ReadLine();
        }
    }
}
