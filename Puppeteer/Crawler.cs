using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace PuppeteerTool
{
    public static class Crawler
    {
        public static async Task<string> HasRechargeAct(List<string> hrefLinks)
        {
            using var fetcher = new BrowserFetcher();
            //fetcher.Platform = Platform.Linux;
            //await fetcher.DownloadAsync();
            LaunchOptions options = new()
            {
                Headless = true,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
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
    }
}
