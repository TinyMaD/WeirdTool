using Newtonsoft.Json;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;

namespace WeirdTool.Services
{
    public class WxApi
    {
        readonly static string _appId = Senparc.Weixin.Config.SenparcWeixinSetting.MpSetting.WeixinAppId;
        public string RefreshMenu()
        {
            ButtonGroup bg = new ButtonGroup();

            //定义一级菜单
            var subButton = new SubButton()
            {
                name = "王者荣耀"
            };
            bg.button.Add(subButton);

            //下属二级菜单
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubGlory",
                name = "订阅特殊活动信息"
            });
            //subButton.sub_button.Add(new SingleViewButton()
            //{
            //    url = "https://weixin.senparc.com/",
            //    name = "Url跳转"
            //});

            //最多可添加 3 个一级自定义菜单，每个菜单下最多 5 个子菜单
            var result = "已刷新公众号菜单；";
            try
            {
                var a = CommonApi.CreateMenu(_appId, bg);
            }
            catch (Exception ex)
            {
                result += JsonConvert.SerializeObject(ex);
            }
            return result;
        }

        public async Task SendMsgAsync(string msg)
        {
            string tagName = "订阅农药";
            var tags = await UserTagApi.GetAsync(_appId);
            var tag = tags.tags.FirstOrDefault(x => x.name == tagName);
            if (tag == null)
            {
                return;
            }
            int tagID = tag?.id ?? 0;
            var result = await GroupMessageApi.SendGroupMessageByTagIdAsync(_appId, tagID.ToString(), msg, GroupMessageType.text);
        }

        public async Task Test()
        {
            var result = await GroupMessageApi.SendGroupMessageByTagIdAsync(_appId, "100", "【每日充值】\r\n 活动时间：9月8日0:00-9月11日23:59 \r\n 活动链接：https://pvp.qq.com/web201706/newsdetail.shtml?tid=606470", GroupMessageType.text);
        }
    }
}
