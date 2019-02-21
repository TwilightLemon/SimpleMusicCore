using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static SimpleMusicCore.HttpHelper;
using static SimpleMusicCore.LyricEx;

namespace SimpleMusicCore
{
    public class Music
    {
        public static void GetUri(Music m)
        {
            List<string[]> MData = new List<string[]>();
            MData.Add(new string[] { "M800", "mp3" });
            MData.Add(new string[] { "C600", "m4a" });
            MData.Add(new string[] { "M500", "mp3" });
            MData.Add(new string[] { "C400", "m4a" });
            MData.Add(new string[] { "M200", "mp3" });
            MData.Add(new string[] { "M100", "mp3" });
            string uri = "";
            var guid = "365305415";
            var mid = JObject.Parse(GetWebDatac($"https://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg?songmid={m.MusicID}&platform=yqq&format=json"))["data"][0]["file"]["media_mid"].ToString();
            for (int i = 0; i < MData.Count; i++)
            {
                string[] datakey = MData[i];
                var key = JObject.Parse(GetWebDatac($"https://c.y.qq.com/base/fcgi-bin/fcg_musicexpress.fcg?json=3&guid={guid}&format=json"))["key"].ToString();
                uri = $"https://dl.stream.qqmusic.qq.com/{datakey[0]}{mid}.{datakey[1]}?vkey={key}&guid={guid}&uid=0&fromtag=30";
                if (GetWebCode(uri) == 200)
                    break;
            }
            Console.WriteLine(uri);
            string name = (m.MusicName + " - " + m.Singer).Replace("\\", "-").Replace("?", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            HttpDownload(uri, Directory.GetCurrentDirectory() + "/Download/" + name + ".mp3");
            GetLyric(m.MusicID, name);
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
