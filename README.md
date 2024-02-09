# WeirdTool
王者荣耀每日充值、累计充值提示工具

## Docker 一键部署
```Shell
docker run -d -e smtp_host=发件箱smtp服务器地址 -e uid=发件箱地址 -e pwd=SMTP授权码 -e email=收件箱地址 -e TZ=Asia/Shanghai --name weird-tool --restart=always duckergoder/weird-tool:1.0.0
```
以[新浪邮箱](https://mail.sina.com.cn/)为例：

![](/sina_email.png)
