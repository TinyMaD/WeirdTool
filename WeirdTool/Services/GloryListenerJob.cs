//using WeChatMP;
using FluentScheduler;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WeirdTool.Services
{
    public class GloryListenerJob : IJob
    {
        public void Execute()
        {
            Console.WriteLine($"任务开始...");
            // 获取活动列表
            List<string> hrefLinks = GetHrefLinks();
            // 筛选新活动
            hrefLinks = GetNewAct(hrefLinks);
            if (hrefLinks.Count == 0)
            {
                Console.WriteLine("没有新活动");
                return;
            }
            hrefLinks.ForEach(link => Console.WriteLine($"新活动：{link}"));
            string? newestAct = hrefLinks.FirstOrDefault();
            // 检查是否有充值活动
            string msg = HasRechargeAct(hrefLinks).Result;

            if (!string.IsNullOrWhiteSpace(msg))
            {
                new Notify().SendEmail(msg);
                //_ = new WxApi().SendMsgAsync(msg);
            }
            // 更新活动标记
            WriteActFlag(newestAct);

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
            HtmlNodeCollection h2Nodes1 = doc.DocumentNode.SelectNodes("//h2[text()='热门']/following-sibling::ul[1]/li");
            HtmlNodeCollection h2Nodes2 = doc.DocumentNode.SelectNodes("//h2[text()='活动']/following-sibling::ul[1]/li");

            var h2Nodes = h2Nodes1.Concat(h2Nodes2);

            List<string> hrefLinks = [];
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
            hrefLinks = hrefLinks.OrderByDescending(url =>
            {
                var tidPart = url.Split("tid=").LastOrDefault();
                return long.TryParse(tidPart, out var tid) ? tid : 0;
            }).ToList();
            return hrefLinks;
        }

        private static async Task<string> HasRechargeAct(List<string> hrefLinks)
        {
            string msg = "";
            foreach (string href in hrefLinks)
            {
                (bool isAct, string actMsg) = await IsRechargeAct(href);
                // 判断是否是充值活动
                if (isAct)
                {
                    msg += actMsg;
                }
                await Task.Delay(5000);// 延迟5秒
            }
            return msg;
        }

        private static async Task<(bool, string)> IsRechargeAct(string href)
        {
            // 获取tid
            string pattern = @"[?&]tid=(\d+)";
            Match match = Regex.Match(href, pattern);
            string tid = match.Groups[1].Value;
            // 请求活动页面
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"https://apps.game.qq.com/wmp/v3.1/public/searchNews.php?p0=18&source=web_pc&id={tid}");
            string result = await response.Content.ReadAsStringAsync();

            string jsonString = result.Replace("var searchObj=", "").TrimEnd(';');
            // 解析JSON字符串
            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject is null)
            {
                return (false, "");
            }

            // 获取html内容
            string content = jsonObject["msg"]["sContent"].ToString();

            string[] actList = ["累计充值", "每日充值", "积分夺宝", "积分暴击"];

            IEnumerable<string>? keywords = actList.Where(content.Contains);
            if (!keywords?.Any() ?? true)
            {
                return (false, "");
            }

            string msg = "";
            foreach (string? keyword in keywords)
            {
                string pattern2 = $@"(?<={keyword}[\s\S]*span[^>]*>)活动时间：[^/]*(?=</span>)";
                Match match2 = Regex.Match(content, pattern2);
                if (match2.Success)
                {
                    msg += $"<h2><strong>【{keyword}】</strong></h2>";
                    // 提取活动时间
                    string ActTime = match2.Groups[0].Value;
                    msg += $"<h2><strong>{ActTime}</strong></h2>";
                    Console.WriteLine($"【{keyword}】{ActTime}");
                }
                else
                {
                    msg += $"<h2><strong>【{keyword}】相关活动</strong></h2>";
                }
            }
            msg += content;
            return (true, msg);
        }

        private static List<string> GetNewAct(List<string> hrefLinks)
        {
            string lastAct = ReadLastAct();

            List<string> newAct = hrefLinks.TakeWhile(x => x != lastAct).ToList();
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
