using System.Text;
using WeirdTool;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
//builder.Services.AddMemoryCache();
//builder.Services.AddSenparcWeixinServices(builder.Configuration);

WebApplication app = builder.Build();

////启用微信配置（必须）
//var registerService = app.UseSenparcWeixin(app.Environment,
//    null /* 不为 null 则覆盖 appsettings  中的 SenpacSetting 配置*/,
//    null /* 不为 null 则覆盖 appsettings  中的 SenpacWeixinSetting 配置*/,
//    register => { /* CO2NET 全局配置 */ },
//    (register, weixinSetting) =>
//    {
//        //注册公众号信息（可以执行多次，注册多个公众号）
//        register.RegisterMpAccount(weixinSetting, "【奇葩工具】");
//    });

// 注入Configuration
AppSettings.SetConfiguration(app.Configuration);
Scheduler.Initialize();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseMessageHandlerForMp("/wxmp", CustomMessageHandler.GenerateMessageHandler, options =>
//{
//    options.AccountSettingFunc = context => Config.SenparcWeixinSetting;
//});
//app.MapGet("/refresh-menu", () =>
//{
//    return new WxApi().RefreshMenu();
//});
//app.MapGet("/wxmp/test", async () =>
//{
//   await new WxApi().Test();
//});

app.Run();
