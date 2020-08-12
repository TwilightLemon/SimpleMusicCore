using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static SimpleMusicCore.HttpHelper;
using static SimpleMusicCore.LyricEx;

namespace SimpleMusicCore
{
    public class Music
    {
        public async static Task<string> GetVk(string mid) {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create("https://i.y.qq.com/v8/playsong.html?songmid=000edOaL1WZOWq");
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
            var o = await hwr.GetResponseAsync();
            StreamReader sr = new StreamReader(o.GetResponseStream(), Encoding.UTF8);
            var st = await sr.ReadToEndAsync();
            sr.Dispose();
            string vk = Text(st, "amobile.music.tc.qq.com/C400000By9MX0yKL2c.m4a", "&fromtag=38", 0);
            if (string.IsNullOrEmpty(vk))
            {
                Console.WriteLine("Vkey被吃掉了!!");
                await Task.Delay(500);
                Console.WriteLine("重连...");
                return await GetVk(mid);
            }
            var mids = JObject.Parse(await GetWebDatacAsync($"https://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg?songmid={mid}&platform=yqq&format=json"))["data"][0]["file"]["media_mid"].ToString();
            return $"http://musichy.tc.qq.com/amobile.music.tc.qq.com/C400{mids}.m4a" + vk + "&fromtag=98";
        }
        public static async void GetUri(Music m,bool needlyric)
        {
            string uri = await GetVk(m.MusicID);
            string name = (m.MusicName + " - " + m.Singer).Replace("\\", "-").Replace("?", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", name + ".mp3");
            HttpDownload(uri,path);
            if (needlyric)
                GetLyric(m.MusicID, name);
            Console.WriteLine("下载完成   文件于:" +path);
        }
        public static async Task<List<Music>> GetGDByIdAsync(string id) {
            if (id == "")
                id = "2591355982";
            string s =await GetWebDatacAsync($"https://c.y.qq.com/qzone/fcg-bin/fcg_ucc_getcdinfo_byids_cp.fcg?type=1&json=1&utf8=1&onlysong=0&disstid={id}&format=json&g_tk=1157737156&loginUin=2728578956&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0", Encoding.UTF8);
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
        public static async Task<List<Music>> SearchMusicAsync(string ct,int osx) {
            JObject o = JObject.Parse(await GetWebAsync($"http://59.37.96.220/soso/fcgi-bin/client_search_cp?format=json&t=0&inCharset=GB2312&outCharset=utf-8&qqmusic_ver=1302&catZhida=0&p={osx}&n=20&w={HttpUtility.UrlDecode(ct)}&flag_qc=0&remoteplace=sizer.newclient.song&new_json=1&lossless=0&aggr=1&cr=1&sem=0&force_zonghe=0"));
            List<Music> dt = new List<Music>();
            int i = 0;
            var dsl = o["data"]["song"]["list"];
            while (i < dsl.Count())
            {
                var dsli = dsl[i];
                Music m = new Music();
                string sr = dsli["lyric"].ToString();
                m.MusicName = dsli["title"].ToString()+(sr!=string.Empty? "•"+sr:"");
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
