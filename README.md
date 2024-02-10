# WeirdTool
王者荣耀每日充值、累计充值提示工具。
## 工作原理
每天中午12点访问王者荣耀官网活动页面，检查到如果有新的荣耀积分活动，就用你的一个邮箱账号发送邮件到你的另一个邮箱账号进行提示。
## 准备工作
- 2个邮箱账号：
  - 1个作发件箱（推荐新浪邮箱）
  - 1个作收件箱（你的常用邮箱，推荐QQ邮箱，微信QQ都能实时收到提醒）
- 1台能访问外网的服务器，并开放出站端口：587（用来发邮件，一般默认出站端口都是开放的）


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

前3个环境变量给程序用发件箱发邮件授权用的，在发件箱页面能找到对应参数，<br> 
以[新浪邮箱](https://mail.sina.com.cn/)为例：

![](/sina_email.png)
