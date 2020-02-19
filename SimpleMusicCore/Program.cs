using System;
using System.Collections.Generic;
using System.IO;
using static SimpleMusicCore.Music;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMusicCore
{
    /// <summary>
    /// 程序主逻辑  By:Twilight./Lemon(2728578956)
    /// </summary>
    class Program
    {
        static async Task Main()
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + "/Download") == false)
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Download");
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "SimpleMusicCore";
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Hello World!");
                Console.WriteLine("Welcome to SimpleMusicCore");
                Console.WriteLine("©Twilight./Lemon 2017-2020    https://github.com/TwilightLemon");
                Console.WriteLine("选择模式:s(搜索模式)、z(歌单模式)");
                var si = Console.ReadLine();
                if (si == "s")
                {
                    int osx = 1;
                    Console.WriteLine("已选择搜索模式，请键入搜索关键词:");
                    string st = Console.ReadLine();
                    Console.WriteLine("加载中...");
                    List<Music> v = await SearchMusicAsync(st, osx);
                    Download(v);
                }
                else if (si == "z")
                {
                    Console.WriteLine("已选择歌单模式，请键入歌单ID:(留空为默认歌单TwilightMusicWorld)");
                    string id = Console.ReadLine();
                    var v =await GetGDByIdAsync(id);
                    Download(v);
                }
                Console.ReadLine();
            }
        }

        public static void Download(List<Music> v)
        {
            Console.WriteLine("加载完成，键入序号进行下载(输入all下载全部,多个使用;分开)");
            var dc = Console.ReadLine();
            if (dc.Contains(";"))
            {
                Console.WriteLine("需要下载歌词吗? [是/否]");
                bool needlrc = Console.ReadLine() == "是" ? true : false;
                string[] dt = dc.Split(';');
                List<Music> sc = new List<Music>();
                foreach (var c in dt)
                    sc.Add(v[int.Parse(c)]);
                foreach (var mn in sc)
                {
                    Console.WriteLine("正在下载:" + mn.MusicName);
                    GetUri(mn, needlrc);
                }
                Console.WriteLine("下载中...");
            }
            else
            {
                if (dc != "all")
                {
                    Console.WriteLine("需要下载歌词吗? [是/否]");
                    bool needlrc = Console.ReadLine() == "是" ? true : false;
                    var d = v[int.Parse(dc)];
                    GetUri(d,needlrc);
                    Console.WriteLine("下载中...");
                }
                else
                {
                    Console.WriteLine("需要下载歌词吗? [是/否]");
                    bool needlrc = Console.ReadLine() == "是" ? true : false;
                    foreach (var mn in v)
                    {
                        Console.WriteLine("正在下载:" + mn.MusicName);
                        GetUri(mn,needlrc);
                    }
                    Console.WriteLine("下载中...");
                }
            }
        }
    }
}
