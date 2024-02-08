using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageContexts;
using Senparc.Weixin.MP.MessageHandlers;

namespace WeChatMP
{
    public class CustomMessageHandler : MessageHandler<DefaultMpMessageContext>
    {
        readonly static string _appId = Senparc.Weixin.Config.SenparcWeixinSetting.MpSetting.WeixinAppId;
        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false, IServiceProvider serviceProvider = null)
            : base(inputStream, postModel, maxRecordCount, onlyAllowEncryptMessage, serviceProvider: serviceProvider)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalGlobalMessageContext.ExpireMinutes = 3。
            GlobalMessageContext.ExpireMinutes = 3;

            OnlyAllowEncryptMessage = true;//是否只允许接收加密消息，默认为 false
        }
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var reponseMessage = CreateResponseMessage<ResponseMessageText>();
            reponseMessage.Content = "功能未实现";
            return reponseMessage;
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="requestMessage">请求消息</param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEvent_ClickRequestAsync(RequestMessageEvent_Click requestMessage)
        {
            string openID = requestMessage.FromUserName;

            var reponseMessage = CreateResponseMessage<ResponseMessageText>();

            if (requestMessage.EventKey == "SubGlory")
            {
                string tagName = "订阅农药";
                var tags = await UserTagApi.GetAsync(_appId);
                var tag = tags.tags.FirstOrDefault(x => x.name == tagName);
                if (tag == null)
                {
                    await UserTagApi.CreateAsync(_appId, tagName);

                    tags = await UserTagApi.GetAsync(_appId);
                    tag = tags.tags.FirstOrDefault(x => x.name == tagName);
                }
                int tagID = tag?.id ?? 0;
                await UserTagApi.BatchTaggingAsync(_appId, tagID, new List<string> { openID });

                reponseMessage.Content = "您已成功订阅王者荣耀【累计充值】【每日充值】【积分夺宝打折】活动信息";
            }
            else
            {
                reponseMessage.Content = "您点击了其他事件按钮";
            }

            return reponseMessage;
        }

        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, IServiceProvider, CustomMessageHandler> GenerateMessageHandler = (stream, postModel, maxRecordCount, serviceProvider)
                         => new CustomMessageHandler(stream, postModel, maxRecordCount, false /* 是否只允许处理加密消息，以提高安全性 */, serviceProvider: serviceProvider);
    }
}
