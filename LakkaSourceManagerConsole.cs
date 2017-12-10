using System;
using System.Collections.Generic;
using System.IO;

namespace Console
{
    class LakkaSourceManagerConsole
    {
        static void Main(string[] args)
        {
            var list = ReadlplFile(@"FB Alpha - Arcade Games.lpl");
            var invalidList = getInvalidateGames(list);
            if (invalidList.Count == 0)
            {
                var rs = cleanRomsByList(list, @".\\roms\\FBA");
                System.Console.WriteLine("delete files :" + rs.ToString());
            }
            else
            {
                System.Console.WriteLine("Not invalide games has :" + invalidList.Count.ToString());
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
    }

}
