using FluentScheduler;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace WeirdTool.Services
{
    public class GloryListenerJob : IJob
    {
        public void Execute()
        {
            try
            {
                Console.WriteLine($"任务开始...");
                // 获取活动列表
                List<string> hrefLinks = GetHrefLinks();
                // 筛选新活动
                hrefLinks = GetNewAct(hrefLinks);
                Console.WriteLine($"检查到个{hrefLinks.Count}新活动");
                if (hrefLinks.Count == 0)
                {
                    return;
                }
                var newestAct = hrefLinks.FirstOrDefault();
                // 检查是否有充值活动
                var msg = HasRechargeAct(hrefLinks).Result;
               
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Console.WriteLine($"邮件内容：{msg}");
                    MailMessage mailMsg = new("placeholder@value.com", "supremelang@qq.com")
                    {
                        Subject = "王者荣耀活动",//邮件主题  
                        IsBodyHtml = true,
                        Body = msg//邮件正文  
                    };
                    new Notify().SendEmail(mailMsg);
                    //_ = new WxApi().SendMsgAsync(msg);
                }
                // 更新活动标记
                WriteActFlag(newestAct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
            Console.WriteLine($"任务结束");
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

        private static async Task<string> HasRechargeAct(List<string> hrefLinks)
        {
            using var fetcher = new BrowserFetcher();
            //fetcher.Platform = Platform.Linux;
            //await fetcher.DownloadAsync();
            LaunchOptions options = new()
            {
                Headless = true,
                //ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                Args = new string[] { "--disable-gpu", "--no-sandbox" }
            };
            Console.WriteLine("正在打开浏览器...");
            using IBrowser browser = await Puppeteer.LaunchAsync(options);
            Console.WriteLine("打开浏览器成功");
            //using IBrowser browser = await Puppeteer.ConnectAsync(new ConnectOptions
            //{
            //     BrowserWSEndpoint = "ws://speedrunners.cn:3000"
            //});
            using IPage page = await browser.NewPageAsync();

            string msg = "";
            foreach (string href in hrefLinks)
            {
                var (isAct, actMsg) = await IsRechargeAct(page, href);
                // 判断是否是充值活动
                if (isAct)
                {
                    msg += actMsg;
                }
                await Task.Delay(5000);// 延迟5秒
            }
            await browser.CloseAsync();
            return msg;
        }

        private static async Task<(bool, string)> IsRechargeAct(IPage page, string href)
        {
            // 导航到页面
            await page.GoToAsync(href);
            // 获取经过JavaScript处理后的HTML内容
            string html = await page.GetContentAsync();
            string[] actList = ["累计充值", "每日充值", "积分夺宝打折", "积分暴击"];

            string? keyword = actList.FirstOrDefault(html.Contains);
            if (keyword == null)
            {
                return (false, "");
            }

            string msg = $"【{keyword}】<br/><br/>";
            string pattern = $@"(?<={keyword}[\s\S]*span[^>]*>).*时间：.*(?=</span>)";
            Match match = Regex.Match(html, pattern);
            if (match.Success)
            {
                // 提取活动时间
                string ActTime = match.Groups[0].Value;
                msg += ActTime + "<br/><br/>";
            }
            string title = await page.GetTitleAsync();
            msg += $@"活动链接：<a href=""{href}""  target=""_blank"">{title}</a><br/><br/>";
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
