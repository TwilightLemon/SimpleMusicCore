using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SimpleMusicCore
{
    /// <summary>
    /// 歌词获取/处理类
    /// </summary>
    public class LyricEx
    {
        public static string PushLyric(string t, string x, string file)
        {
            List<string> datatime = new List<string>();
            List<string> datatext = new List<string>();
            Dictionary<string, string> gcdata = new Dictionary<string, string>();
            string[] dta = t.Split('\n');
            foreach (var dt in dta)
                parserLine(dt, datatime, datatext, gcdata);
            List<string> dataatimes = new List<string>();
            List<string> dataatexs = new List<string>();
            Dictionary<string, string> fydata = new Dictionary<string, string>();
            string[] dtaa = x.Split('\n');
            foreach (var dt in dtaa)
                parserLine(dt, dataatimes, dataatexs, fydata);
            List<string> KEY = new List<string>();
            Dictionary<string, string> gcfydata = new Dictionary<string, string>();
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (var dt in datatime)
            {
                KEY.Add(dt);
                gcfydata.Add(dt, "");
            }
            for (int i = 0; i != gcfydata.Count; i++)
            {
                if (fydata.ContainsKey(KEY[i]))
                    gcfydata[KEY[i]] = (gcdata[KEY[i]] + "^" + fydata[KEY[i]]).Replace("\n", "").Replace("\r", "");
                else
                {
                    string dt = YwY(KEY[i], 1);
                    if (fydata.ContainsKey(dt))
                        gcfydata[KEY[i]] = (gcdata[KEY[i]] + "^" + fydata[dt]).Replace("\n", "").Replace("\r", "");
                    else gcfydata[KEY[i]] = (gcdata[KEY[i]] + "^").Replace("\n", "").Replace("\r", "");
                }
            }
            string LyricData = "";
            for (int i = 0; i != KEY.Count; i++)
            {
                string value = gcfydata[KEY[i]].Replace("[", "").Replace("]", "");
                string key = KEY[i];
                LyricData += $"[{key}]{value}\r\n";
            }
            File.WriteAllText(file, LyricData);
            return LyricData;
        }
        public static string GetLyric(string McMind, string name)
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "Download",name+".lrc");
            if (!File.Exists(file))
            {
                WebClient c = new WebClient();
                c.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36");
                c.Headers.Add("Accept", "*/*");
                c.Headers.Add("Referer", "https://y.qq.com/portal/player.html");
                c.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                c.Headers.Add("Cookie", "tvfe_boss_uuid=c3db0dcc4d677c60; pac_uid=1_2728578956; qq_slist_autoplay=on; ts_refer=ADTAGh5_playsong; RK=pKOOKi2f1O; pgv_pvi=8927113216; o_cookie=2728578956; pgv_pvid=5107924810; ptui_loginuin=2728578956; ptcz=897c17d7e17ae9009e018ebf3f818355147a3a26c6c67a63ae949e24758baa2d; pt2gguin=o2728578956; pgv_si=s5715204096; qqmusic_fromtag=66; yplayer_open=1; ts_last=y.qq.com/portal/player.html; ts_uid=996779984; yq_index=0");
                c.Headers.Add("Host", "c.y.qq.com");
                string s = Text(c.DownloadString($"https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg?callback=MusicJsonCallback_lrc&pcachetime=1494070301711&songmid={McMind}&g_tk=5381&jsonpCallback=MusicJsonCallback_lrc&loginUin=0&hostUin=0&format=jsonp&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0"), "MusicJsonCallback_lrc(", ")", 0);
                JObject o = JObject.Parse(s);
                string t = Encoding.UTF8.GetString(Convert.FromBase64String(o["lyric"].ToString())).Replace("&apos;", "\'");
                if (o["trans"].ToString() == "") { File.WriteAllText(file, t); return t; }
                else
                {
                    string x = Encoding.UTF8.GetString(Convert.FromBase64String(o["trans"].ToString())).Replace("&apos;", "\'");
                    return PushLyric(t, x, file);
                }
            }
            else
                return File.ReadAllText(file);
        }
        public static string[] parserLine(string str, List<string> times, List<string> texs, Dictionary<string, string> data, bool doesAdd = true)
        {
            if (!str.StartsWith("[ti:") && !str.StartsWith("[ar:") && !str.StartsWith("[al:") && !str.StartsWith("[by:") && !str.StartsWith("[offset:") && !str.StartsWith("[kana") && str.Length != 0)
            {
                string TimeData = Text(str, "[", "]", 0);
                string io = "[" + TimeData + "]";
                string TexsData = str.Replace(io, "");
                //string unTimeData = TimeData.Substring(0, TimeData.Length - 1);
                if (doesAdd)
                {
                    if (data.ContainsKey(TimeData))
                    {
                        texs.Add(TexsData);
                        data[TimeData] += "^" + TexsData;
                    }
                    else
                    {
                        times.Add(TimeData);
                        texs.Add(TexsData);
                        data.Add(TimeData, TexsData);
                    }
                }
                return new string[2] { TimeData, TexsData };
            }
            else return null;
        }
        public static string YwY(string str, int i)
        {//00:02.06 => 00:02.07
            string lstr = Text(str + "]", ".", "]", 0);//06
            string LastTime = (int.Parse(lstr) + i).ToString();//06+i
            if (LastTime.Length == 1)
                LastTime = "0" + LastTime;
            return str.Replace(lstr, LastTime.ToString());
        }
        public static string Text(string all, string r, string l, int t)
        {

            int A = all.IndexOf(r, t);
            int B = all.IndexOf(l, A + 1);
            if (A < 0 || B < 0)
            {
                return null;
            }
            else
            {
                A = A + r.Length;
                B = B - A;
                if (A < 0 || B < 0)
                {
                    return null;
                }
                return all.Substring(A, B);
            }

        }
    }
}
