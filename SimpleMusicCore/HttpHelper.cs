using System.IO;
using System.Net;
using System.Text;

namespace SimpleMusicCore
{
    /// <summary>
    /// 发送Http请求类
    /// </summary>
    public class HttpHelper
    {
                public static bool HttpDownload(string url, string path)
        {
            string tempPath = Path.GetDirectoryName(path) + @"\temp";
            Directory.CreateDirectory(tempPath);
            string tempFile = tempPath + @"\" + Path.GetFileName(path) + ".temp";
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.81 Safari/537.36";
                request.Host = "dl.stream.qqmusic.qq.com";
                request.Headers[HttpRequestHeader.Cookie] = "pgv_pvi=4037556224; pgv_pvid=237001681; RK=JKLEPi2c3M; ptcz=2a93b1e1d777f6bd5142e3502f10201341cff5e638c8446bef30e7bee76ba7fc; ptui_loginuin=3474980436; luin=o2728578956; lskey=000100005f647ba07620e804cb89a4c780562386197ba53857beba2223acf5567fd12e2b7b1eb4de131b2f5a";
                request.KeepAlive = true;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                HttpWebResponse response = request.GetResponseAsync().GetAwaiter().GetResult() as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                fs.Dispose();
                responseStream.Dispose();
                System.IO.File.Move(tempFile, path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string GetWebAsync(string url)
        {
            try
            {
                HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
                var o = hwr.GetResponseAsync().GetAwaiter().GetResult();
                StreamReader sr = new StreamReader(o.GetResponseStream(), Encoding.UTF8);
                var st = sr.ReadToEnd();
                sr.Dispose();
                return st;
            }
            catch { return ""; }
        }
        public static string GetWebDatac(string url, Encoding c = null)
        {
            if (c == null) c = Encoding.UTF8;
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.Headers[HttpRequestHeader.Referer] = "https://y.qq.com/portal/player.html";
            hwr.Headers[HttpRequestHeader.Host] = "c.y.qq.com";
            hwr.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36";
            hwr.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh;q=0.8";
            hwr.Headers[HttpRequestHeader.Cookie] = "pgv_pvi=1693112320; RK=DKOGai2+wu; pgv_pvid=1804673584; ptcz=3a23e0a915ddf05c5addbede97812033b60be2a192f7c3ecb41aa0d60912ff26; pgv_si=s4366031872; _qpsvr_localtk=0.3782697029073365; ptisp=ctc; luin=o2728578956; lskey=00010000863c7a430b79e2cf0263ff24a1e97b0694ad14fcee720a1dc16ccba0717d728d32fcadda6c1109ff; pt2gguin=o2728578956; uin=o2728578956; skey=@PjlklcXgw; p_uin=o2728578956; p_skey=ROnI4JEkWgKYtgppi3CnVTETY3aHAIes-2eDPfGQcVg_; pt4_token=wC-2b7WFwI*8aKZBjbBb7f4Am4rskj11MmN7bvuacJQ_; p_luin=o2728578956; p_lskey=00040000e56d131f47948fb5a2bec49de6174d7938c2eb45cb224af316b053543412fd9393f83ee26a451e15; ts_refer=ui.ptlogin2.qq.com/cgi-bin/login; ts_last=y.qq.com/n/yqq/playlist/2591355982.html; ts_uid=1420532256; yqq_stat=0";
            var o = hwr.GetResponse();
            StreamReader sr = new StreamReader(o.GetResponseStream(), c);
            var st = sr.ReadToEnd();
            sr.Dispose();
            return st;
        }
        public static int GetWebCode(string url)
        {
            try
            {
                HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
                var o = hwr.GetResponse() as HttpWebResponse;
                return (int)o.StatusCode;
            }
            catch { return 404; }
        }
    }
}
