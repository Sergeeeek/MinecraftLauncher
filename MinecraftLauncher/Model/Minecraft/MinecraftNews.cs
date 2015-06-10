using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MinecraftLauncher.Model.Minecraft
{
    public class MinecraftNews
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription 
        {
            get
            {
                return Description.Substring(0, Description.Length < ShortenLength ? Description.Length : ShortenLength) + (Description.Length > ShortenLength ? "..." : "");
            } 
        }
        int ShortenLength = 200;

        public static async Task<List<MinecraftNews>> GetNews()
        {
            try
            {
                var doc = new HtmlDocument();
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var html = await client.DownloadStringTaskAsync(new Uri("http://mc.warzone.su/"));
                    doc.LoadHtml(html);
                }

                var nodes = doc.DocumentNode.SelectNodes(@"//div[@class='news-box']");

                var news = from a in nodes
                           select new MinecraftNews()
                           {
                               Title = HtmlEntity.DeEntitize(a.SelectSingleNode(@"div[@class='news-header']/b").InnerText),
                               Description = HtmlEntity.DeEntitize(a.SelectSingleNode(@"div[@class='news-text']").InnerText)
                           };

                return news.ToList();
            }
            catch
            {
                return null;
            }
        }

        public override string ToString()
        {
            return Title + Description;
        }
    }

    public interface INewsService
    {
        ObservableCollection<MinecraftNews> News { get; }
    }

    public class NewsService : INewsService
    {
        public ObservableCollection<MinecraftNews> News { get; private set; }

        public NewsService()
        {
            
        }
    }
}
