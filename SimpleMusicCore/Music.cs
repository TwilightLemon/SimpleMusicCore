using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using static SimpleMusicCore.HttpHelper;
using static SimpleMusicCore.LyricEx;

namespace SimpleMusicCore
{
    public class Music
    {
        public static string vk = "";
        public async static void GetVk() {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create("https://i.y.qq.com/v8/playsong.html?songmid=0013KcQ72u8FY7,0011jIhY1wP6wB");
            hwr.Timeout = 20000;
            hwr.KeepAlive = true;
            hwr.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
            hwr.Headers.Add(HttpRequestHeader.Upgrade, "1");
            hwr.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3854.3 Mobile Safari/537.36";
            hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            hwr.Referer = "https://i.y.qq.com/n2/m/share/details/album.html?albummid=003bBofB3UzHxS&ADTAG=myqq&from=myqq&channel=10007100";
            hwr.Host = "i.y.qq.com";
            hwr.Headers.Add("sec-fetch-mode", "navigate");
            hwr.Headers.Add("sec-fetch-site", "same - origin");
            hwr.Headers.Add("sec-fetch-user", "?1");
            hwr.Headers.Add("upgrade-insecure-requests", "1");
            hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.9");
            hwr.Headers.Add(HttpRequestHeader.Cookie, "pgv_pvid=3531479395; euin_cookie=7F96147BC631FEA7AA0F17E75A04AA6D1319C724DF815031; ptcz=b6a78a1389245b1d160bd02b1bd65a22d62fe28d6c0914e7264b6c74f1216b1f; pgv_pvi=4809115648; uin_cookie=2728578956; sensorsdata2015jssdkcross=%7B%22distinct_id%22%3A%22168c5952d117ea-0a44be39a4fa52-4f7b614a-1049088-168c5952d1218a%22%2C%22%24device_id%22%3A%22168c5952d117ea-0a44be39a4fa52-4f7b614a-1049088-168c5952d1218a%22%2C%22props%22%3A%7B%22%24latest_traffic_source_type%22%3A%22%E7%9B%B4%E6%8E%A5%E6%B5%81%E9%87%8F%22%2C%22%24latest_referrer%22%3A%22%22%2C%22%24latest_referrer_host%22%3A%22%22%2C%22%24latest_search_keyword%22%3A%22%E6%9C%AA%E5%8F%96%E5%88%B0%E5%80%BC_%E7%9B%B4%E6%8E%A5%E6%89%93%E5%BC%80%22%7D%7D; luin=o2728578956; RK=sKKMfg2M0M; ptui_loginuin=3474980436; lskey=00010000576a974eff7a7aa945b8d7ed8cd76e2bbb6c78740bb90e8bf6dac501f3694ee7376dc6dd1f584e57; pgv_si=s5282453504; _qpsvr_localtk=0.10894953139375662; ptisp=cm; uin=o2728578956; skey=@k47rNf4TE; p_lskey=00040000ca1d04a223083024bcc92a8a7f5f44e29d41aaa5745c25ccf41622db369d1d8cfb71c2e4240a6e16; ts_refer=xui.ptlogin2.qq.com/cgi-bin/xlogin; ts_uid=3700488506; userAction=1; p_luin=o2728578956; p_uin=o2728578956; pt4_token=rBFs0zEcx7GN*Z3Ntq9gW8L6it4Yt*tpNXZEgH4k63I_; p_skey=9lXTqQ7Y-yzxr8FE164qwCI-wHfMx0eTTBqswzKUWXc_; yqq_stat=0");
            var o = await hwr.GetResponseAsync();
            StreamReader sr = new StreamReader(o.GetResponseStream(), Encoding.UTF8);
            var st = await sr.ReadToEndAsync();
            sr.Dispose();
            vk = Text(st, "http://aqqmusic.tc.qq.com/amobile.music.tc.qq.com/C4000013KcQ72u8FY7.m4a", "&fromtag=38", 0);

        }
        public static void GetUri(Music m)
        {
            string uri= $"http://aqqmusic.tc.qq.com/amobile.music.tc.qq.com/C400{m.MusicID}.m4a" + vk + "&fromtag=38";
            Console.WriteLine(uri);
            string name = (m.MusicName + " - " + m.Singer).Replace("\\", "-").Replace("?", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            HttpDownload(uri, Directory.GetCurrentDirectory() + "/Download/" + name + ".mp3");
            //GetLyric(m.MusicID, name);
            Console.WriteLine("下载完成   文件于:" + Directory.GetCurrentDirectory() + "/Download/" + name + ".mp3");
        }
        public static List<Music> GetGDById(string id) {
            if (id == "")
                id = "2591355982";
            string s = GetWebDatac($"https://c.y.qq.com/qzone/fcg-bin/fcg_ucc_getcdinfo_byids_cp.fcg?type=1&json=1&utf8=1&onlysong=0&disstid={id}&format=json&g_tk=1157737156&loginUin=2728578956&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0", Encoding.UTF8);
            JObject o = JObject.Parse(s);
            List<Music> dt = new List<Music>();
            int i = 0;
            JToken c0s = o["cdlist"][0]["songlist"];
            while (i != c0s.Count())
            {
                JToken c0si = c0s[i];
                Music m = new Music()
                {
                    MusicName = c0si["songname"].ToString(),
                    Singer = c0si["singer"][0]["name"].ToString(),
                    MusicID = c0si["songmid"].ToString(),
                };
                dt.Add(m);
                Console.WriteLine($"[{i}]  {m.MusicName} - {m.Singer}");
                i++;
            }
            return dt;
        }
        public static List<Music> SearchMusic(string ct,int osx) {
            JObject o = JObject.Parse(GetWebAsync($"http://59.37.96.220/soso/fcgi-bin/client_search_cp?format=json&t=0&inCharset=GB2312&outCharset=utf-8&qqmusic_ver=1302&catZhida=0&p={osx}&n=20&w={HttpUtility.UrlDecode(ct)}&flag_qc=0&remoteplace=sizer.newclient.song&new_json=1&lossless=0&aggr=1&cr=1&sem=0&force_zonghe=0"));
            List<Music> dt = new List<Music>();
            int i = 0;
            var dsl = o["data"]["song"]["list"];
            while (i < dsl.Count())
            {
                var dsli = dsl[i];
                Music m = new Music();
                m.MusicName = dsli["title"].ToString()+"•"+ dsli["lyric"].ToString();
                string Singer = "";
                for (int osxc = 0; osxc != dsli["singer"].Count(); osxc++)
                { Singer += dsli["singer"][osxc]["name"] + "&"; }
                m.Singer = Singer.Substring(0, Singer.LastIndexOf("&"));
                m.MusicID = dsli["mid"].ToString();
                dt.Add(m);
                Console.WriteLine($"[{i}]  {m.MusicName} - {m.Singer}");
                i++;
            }
            return dt;
        }
        public Music() { }
        public string MusicName { set; get; }
        public string Singer { set; get; }
        public string MusicID { set; get; }
    }
}
