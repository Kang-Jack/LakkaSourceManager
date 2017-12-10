using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Console
{
    class LakkaSourceManagerConsole
    {
        static void Main(string[] args)
        {
            //Thread.Sleep(15000);
            var list = ReadlplFile(@"FB Alpha - Arcade Games.lpl");
            var invalidList = getInvalidateGames(list);
            if (invalidList.Count == 0)
            {
                System.Console.WriteLine("list has games :" + list.Count.ToString());
                var rs = cleanRomsByList(list, @".\\roms\\FBA");
                System.Console.WriteLine("delete files :" + rs.ToString());
            }
            else
            {
                System.Console.WriteLine("Not invalide games have :" + invalidList.Count.ToString());
                foreach (var invalid in invalidList)
                {
                    System.Console.WriteLine("error in Line:" + invalid.line_No.ToString());
                }
            }
        }

        public static IList<lplGameContent> ReadlplFile(string lplfilePathName)
        {
            if (!lplfilePathName.EndsWith(".lpl", StringComparison.OrdinalIgnoreCase))
            {
                System.Console.WriteLine("File_type_does_not_support");
                return null;
            }
            if (!File.Exists(lplfilePathName))
            {
                System.Console.WriteLine("Resources.Please_refresh_the_list");
                return null;
            }
            var gameList = new List<lplGameContent>();
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(lplfilePathName);
            while ((line = file.ReadLine()) != null)
            {
                var game = new lplGameContent();
                counter++;
                game.line_No = counter;
                game.bin_path = line;
                try
                {
                    game.game_name = file.ReadLine();
                    counter++;
                    game.DETECT1 = file.ReadLine();
                    counter++;
                    game.DETECT2 = file.ReadLine();
                    counter++;
                    game.guid_string = file.ReadLine();
                    counter++;
                    game.list_name = file.ReadLine();
                    counter++;
                }
                catch (Exception)
                {
                    throw;
                }
                gameList.Add(game);
            }

            file.Close();
            return gameList;
        }

        public static IList<lplGameContent> getInvalidateGames(IList<lplGameContent> games)
        {
            var invalidGames = new List<lplGameContent>();
            if (games == null)
            {
                System.Console.WriteLine("File_type_does_not_support");
                return null;
            }
            foreach (var game in games)
            {
                var isValid = game.bin_path.Contains("/storage/roms/") && game.list_name.Contains(".lpl");
                if (!isValid)
                    invalidGames.Add(game);
            }
            return invalidGames;
        }

        public static int cleanRomsByList(IList<lplGameContent> games, string rom_path)
        {
            if (string.IsNullOrEmpty(rom_path))
                return 0;

            var removeList = new List<string>();
            string destPath = rom_path + "\\deleted\\";
            CheckDeleteFolder(destPath);
            int totalRemove = 0;

            var files = Directory.GetFiles(rom_path);
            if (files != null)
                foreach (var f in files)
                {
                    bool isFound = false;
                    foreach (var g in games)
                    {
                        var filestrings = g.bin_path.Split('/');
                        if (filestrings.Length > 1)
                            if (f.Contains(filestrings[filestrings.Length - 1]))
                            {
                                isFound = true;
                                break;
                            }
                    }
                    if (isFound)
                        continue;
                    removeList.Add(f);
                }
            int notDeletFile = 0;
            foreach (string file in removeList)
            {
                if (!File.Exists(file)) continue;
                var fInfo = new FileInfo(file);
                try
                {
                    File.Move(file, destPath + fInfo.Name);
                }
                catch (Exception exception)
                {
                    notDeletFile++;
                }
            }
            return removeList.Count - notDeletFile;
        }


        public static void CheckDeleteFolder(string destPath)
        {
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
        }
    }

    public class lplGameContent
    {
        public string bin_path;
        public string game_name;
        public string DETECT1;
        public string DETECT2;
        public string guid_string;
        public string list_name;
        public int line_No;
        //Sample
        //storage/roms/FBA/1941.zip
        //1941反击战（世界版900227）
        //DETECT
        //DETECT
        //64e58dc3|crc
        //FB Alpha - Arcade Games.lpl
//第一行：/storage/roms/FBA/1941.zip 代表着这个ROM位于 /storage/roms/FBA/ 文件夹内，且ROM名字是1941.zip
//第二行：1941反击战（世界版900227） 代表着这个游戏在主界面列表显示出来的内容，注意别用怪异的符号，尽量是中文、英语+数字
//第三行：DETECT 是高级应用选填，意为指向特定路径的特定模拟器，大家默认DETECT这样子最好（某些游戏需要特定模拟器，就在这里指定模拟器so文件路径）
//第四行：DETECT 也是高级应用选填，意为分类注释，建议默认为DETECT就是这样子就OK
//第五行：2ad6ca92|crc 在|crc之前，就是对应的这个ROM的CRC32值，运行之前下载包里面的CRC32工具，把ROM文件拖进去就看得到
//第六行：FB Alpha - Arcade Games.lpl 这是FBA模拟器的标准列表文件名，请别乱改，默认就是这样子
    }

}
