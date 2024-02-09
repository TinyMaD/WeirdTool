# WeirdTool
王者荣耀每日充值、累计充值提示工具。每天中午12点检查到荣耀积分活动就发送邮件进行提示

## Docker 一键部署
```Shell
docker run -d -e smtp_host=发件箱smtp服务器地址 -e uid=发件箱地址 -e pwd=SMTP授权码 -e email=收件箱地址 -e TZ=Asia/Shanghai --name weird-tool --restart=always duckergoder/weird-tool:1.0.0
```
| 环境变量 | 说明 |
| :----: | :---- |
| smtp_host | 发件箱smtp服务器地址，例：smtp.sina.com |
| uid | 发件箱地址，例：youremail@email.com |
| pwd | SMTP授权码，例：bf84dg1fd5sg4 |
| email | 收件箱地址，例：youremail@email.com，<br>可同时向多个邮箱发送邮件，多个收件箱地址用***英文逗号***隔开 |

以[新浪邮箱](https://mail.sina.com.cn/)为例：

![](/sina_email.png)
