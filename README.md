# Reception
C#/Python remote access tool.\
No release currently.

# Preview

![](https://github.com/iss4cf0ng/Reception/blob/master/1.png)
![](https://github.com/iss4cf0ng/Reception/blob/master/2.png)
![](https://github.com/iss4cf0ng/Reception/blob/master/3.png)

# 原理
1. Python檔(Client)運行後就會連接Server，然後進行Key Exchange，流程大概就是Server產生RSA key pair，把public key傳到Client，Client產生AES-128的key和IV，
Client用RSA public key加密AES key傳回Server然後用private key解密，之後Server和Client用AES CBC通訊。
2. Server會把Python的payload加密後傳到Client，Client解密後用exec()函數執行惡意Payload，把結果加密後傳回Server。
3. Server可以把預先做好的第三方庫的zip檔加密後傳到Client，Client解密後會把zip解壓在某個temporary folder，再由temporary folder import，這樣一樣就算Client沒有該Libaray，也可以在Client安裝
