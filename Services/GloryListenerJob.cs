using FluentScheduler;
using HtmlAgilityPack;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace WeridTool.Services
{
    public class GloryListenerJob : IJob
    {
        public void Execute()
        {
            // 获取活动列表
            List<string> hrefLinks = GetHrefLinks();
            // 筛选新活动
            hrefLinks = GetNewAct(hrefLinks);

            if (hrefLinks.Count == 0)
            {
                return;
            }
            var msg = "";
            foreach (string href in hrefLinks)
            {
                var (isAct, actMsg) = IsRechargeAct(href).Result;
                // 判断是否是充值活动
                if (isAct)
                {
                    msg += actMsg;
                }
            }
            // 更新活动标记
            var newestAct = hrefLinks.FirstOrDefault();
            WriteActFlag(newestAct);

            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }
            MailMessage mailMsg = new("placeholder@value.com", "supremelang@qq.com")
            {
                Subject = "王者荣耀充值活动",//邮件主题  
                IsBodyHtml = true,
                Body = msg//邮件正文  
            };
            new Notify().SendEmail(mailMsg);
        }

        private static List<string> GetHrefLinks()
        {
            // 使用HtmlAgilityPack解析HTML
            HtmlWeb web = new()
            {
                OverrideEncoding = Encoding.GetEncoding("gbk")
            };
            HtmlDocument doc = web.Load("https://pvp.qq.com/web201706/newsindex.shtml");

            // 获取指定h2标签下的所有li标签
            HtmlNodeCollection h2Nodes = doc.DocumentNode.SelectNodes("//h2[text()='活动']/following-sibling::ul[1]/li");

            List<string> hrefLinks = new();
            // 遍历li标签，获取href属性中的链接字符串
            if (h2Nodes != null)
            {
                foreach (HtmlNode liNode in h2Nodes)
                {
                    HtmlNode aNode = liNode.SelectSingleNode("a");
                    string href = aNode.GetAttributeValue("href", "");
                    hrefLinks.Add(href);
                }
            }
            return hrefLinks;
        }

        private static async Task<(bool, string)> IsRechargeAct(string href)
        {
            // 设置Headless Chrome路径
            BrowserFetcher bf = new();
            IEnumerable<InstalledBrowser> Browsers = bf.GetInstalledBrowsers();

            if (!Browsers.Any())
            {
                await bf.DownloadAsync();
            }
            LaunchOptions options = new() { Headless = true };
            using IBrowser browser = await Puppeteer.LaunchAsync(options);
            using IPage page = await browser.NewPageAsync();
            // 导航到页面
            await page.GoToAsync(href);

            await Task.Delay(5000);// 延迟5秒

            // 获取经过JavaScript处理后的HTML内容
            string html = await page.GetContentAsync();

            string keyword = "累计充值";
            var rechargeAct = html.Contains(keyword);
            if (!rechargeAct)
            {
                keyword = "每日充值";
                rechargeAct = html.Contains(keyword);
                if (!rechargeAct)
                {
                    return (false, "");
                }
            }
            string title = await page.GetTitleAsync();
            string msg = $"{keyword}<br/><br/>活动标题：{title}<br/><br/>";

            string pattern = $@"(?<={keyword}[\s\S]*span[^>]*>).*活动时间.*(?=</span>)";

            Match match = Regex.Match(html, pattern);
            if (match.Success)
            {
                // 提取活动时间
                string ActTime = match.Value;
                msg += ActTime + "<br/><br/>";
            }

            return (true, msg);
        }

        private static List<string> GetNewAct(List<string> hrefLinks)
        {
            var lastAct = ReadLastAct();

            var newAct = hrefLinks.TakeWhile(x => x != lastAct).ToList();
            return newAct;
        }

        private static string ReadLastAct()
        {
            string filePath = AppContext.BaseDirectory + "cursor.txt"; // 文件路径

            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                return fileContent;
            }
            return string.Empty;
        }

        private static void WriteActFlag(string? actFlag)
        {
            string filePath = AppContext.BaseDirectory + "cursor.txt"; // 文件路径
            File.WriteAllText(filePath, actFlag);
        }
    }
}
