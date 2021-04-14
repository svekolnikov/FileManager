using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using FileManager.Content;
using FileManager.Exceptions;
using FileManager.Parser;

namespace FileManager
{
    public class App
    {
        private readonly string configPath = "config.json";
        private readonly string loggerDefaultPath = "logger.txt";
        private readonly string defaultPath = "C:\\";

        private ConfigModel _config;
        private Table _table;
        private AppUi _appUi;

        public App()
        {
            _config = GetConfigOrDefault();

            Column[] columns =
            {
                new Column {Name = "Name", Width = 80},
                new Column {Name = "Type", Width = 10},
                new Column {Name = "Ext",  Width = 10},
                new Column {Name = "Size", Width = 15},
                new Column {Name = "DateCreated created", Width = 25}
            };

            _table = new Table(columns, 1, _config.RowsPerPage) { Path = _config.Path };
            _appUi = new AppUi(_config.UiLeft, _config.UiTop, _table);

            if (string.IsNullOrWhiteSpace(_table.Path))
            {
                _table.Path = defaultPath;
            }

            if (string.IsNullOrWhiteSpace(_config.LoggerPath))
                _config.LoggerPath = loggerDefaultPath; // default logger path is local

            if (!File.Exists(_config.LoggerPath))
            {
                //create new logger file
                try
                {
                    var stream = File.Create(_config.LoggerPath);
                    stream.Close();
                }
                catch (DirectoryNotFoundException e)
                {
                    _config.LoggerPath = loggerDefaultPath;
                    var stream = File.Create(_config.LoggerPath);
                    stream.Close();
                }
            }

            _config = SaveConfig(_config);
        }

        public void Run()
        {
            AddToLogger("Application run.");

            _table.ClearContent();
            _table.AddRange(GetSubDirectories(_config.Path));
            _table.AddRange(GetFiles(_config.Path));
            _table.SetPage(_config.Page);
            _appUi.Update();

            CommandParser comparser = new CommandParser(_config);
            ParsedParams parsedParams = new ParsedParams();

            while (true)
            {
                try
                {
                    parsedParams = comparser.Parse(Console.ReadLine());
                    _appUi.PrintInfo("");

                    switch (parsedParams.Command)
                    {
                        case Commands.CreateFile:
                            var fs = File.Create(parsedParams.InitPath);
                            fs.Close();
                            break;
                        case Commands.CreateDirectory:
                            Directory.CreateDirectory(parsedParams.InitPath);
                            break;
                        case Commands.RemoveFileOrDirectory:
                            RemoveFileOrDirectory(parsedParams.InitPath);
                            break;
                        case Commands.Cd:
                            GoToPath(AddSlash(parsedParams.DestPath), parsedParams.Page);
                            break;
                        case Commands.CopyFile:
                            File.Copy(parsedParams.InitPath, parsedParams.DestPath, true);
                            break;
                        case Commands.CopyDirectory:
                            CopyDirectory(parsedParams.InitPath, parsedParams.DestPath);
                            break;
                    }

                }
                catch (DirNotFoundException e)
                {
                    _appUi.PrintInfo(e.Message);
                    AddToLogger(e.Message);
                }
                catch (InvalidCmdParameterException e)
                {
                    _appUi.PrintInfo(e.Message);
                    AddToLogger(e.Message);
                }
                catch (WrongCmdException e)
                {
                    _appUi.PrintInfo(e.Message);
                    AddToLogger(e.Message);
                }
                catch (IOException e)
                {
                    _appUi.PrintInfo("Ошибка создания файла.");
                    AddToLogger(e.Message);
                }
                catch (Exception e)
                {
                    _appUi.PrintInfo("Неизвестная ошибка.");
                    AddToLogger(e.Message);
                }

                GoToPath(AddSlash(parsedParams.DestPath), parsedParams.Page);
                _appUi.Update();
                _appUi.FocusOnCommandLine();
                AddToLogger($"Path: {_table.Path} ");
                _config = SaveConfig(_config);
            }
        }


        private ConfigModel CreateDefaultConfig()
        {
            return new ConfigModel
            {
                Path = defaultPath,
                LoggerPath = loggerDefaultPath,
                Page = 0,
                RowsPerPage = 10,
                UiLeft = 0,
                UiTop = 0
            };
        }

        private ConfigModel SaveConfig(ConfigModel config)
        {
            if (config == null)
                config = CreateDefaultConfig();

            string json = JsonSerializer.Serialize(config);
            try
            {
                File.WriteAllText(configPath, json);
            }
            catch (Exception e)
            {
                _appUi.PrintInfo("Ошибак записи в кофиг файл.");
                AddToLogger("Ошибак записи в кофиг файл.");
            }

            return config;
        }

        private ConfigModel GetConfigOrDefault()
        {
            ConfigModel conf;
            try
            {
                string json = File.ReadAllText(configPath);
                conf = JsonSerializer.Deserialize<ConfigModel>(json);
            }
            catch
            {
                conf = CreateDefaultConfig();
            }
            return conf;
        }

        private void AddToLogger(string measage)
        {
            try
            {
                using var sw = File.AppendText(_config.LoggerPath);
                sw.WriteLine($"[{DateTime.Now.ToUniversalTime()}] {measage}");
                sw.Close();
            }
            catch(Exception e)
            {
                _appUi.PrintInfo("Ошибка записи в логгер.");
            }
        }

        private void GoToPath(string path, int page = 0)
        {
            _config.Path = !string.IsNullOrEmpty(path) ? path : _config.Path;
            _config.Page = page;

            _table.Path = _config.Path;
            _table.ClearContent();
            _table.AddRange(GetSubDirectories(_config.Path));
            _table.AddRange(GetFiles(_config.Path));
            _table.SetPage(_config.Page);

            string json = JsonSerializer.Serialize(_config);
            File.WriteAllText("config.json", json);
        }

        private string AddSlash(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var pathSplit = path.Split("\\");
                if (pathSplit[^1] != "")
                    path += "\\";
            }
            return path;
        }
        
        private List<Row> GetSubDirectories(string path)
        {
            string[] subDirs = {};
            try
            {
                subDirs = Directory.GetDirectories(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //subdirectories
            List<Row> rows = new List<Row>();
            foreach (string dir in subDirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                rows.Add(new Row
                {
                    Size = 0,
                    Name = di.Name,
                    Extension = "<DIR>",
                    Type = TableEntityType.DIR,
                    DateCreated = di.CreationTime.ToString(CultureInfo.CurrentCulture)
                });
            }

            return rows;
        }

        private List<Row> GetFiles(string path)
        {
            string[] files = {};
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            //files
            List<Row> rows = new List<Row>();
            foreach (string f in files)
            {
                FileInfo fi = new FileInfo(f);
                rows.Add(new Row
                {
                    Size = fi.Length,
                    Name = Path.GetFileNameWithoutExtension(fi.FullName),
                    Extension = fi.Extension.Replace(".", ""),
                    Type = TableEntityType.File,
                    DateCreated = fi.CreationTime.ToString(CultureInfo.CurrentCulture)
                });
            }

            return rows;
        }
        
        private void RemoveFileOrDirectory(string path)
        {
            if (Path.HasExtension(path)) //file
            {
                File.Delete(path);
            }
            else //folder
            {
                Directory.Delete(path, true);
            }
        }
        
        private void CopyDirectory(string initPath, string destPath)
        {
            var initDi = new DirectoryInfo(initPath);
            var destDi = new DirectoryInfo(destPath);

            CopyDirectoryReqursive(initDi, destDi);
        }

        private void CopyDirectoryReqursive(DirectoryInfo initDi, DirectoryInfo destDi)
        {
            Directory.CreateDirectory(destDi.FullName);

            //Copy each file to new directory
            foreach (FileInfo fileInfo in initDi.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(destDi.FullName, fileInfo.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo initDirectoryInfo in initDi.GetDirectories())
            {
                DirectoryInfo nextDestSubDir = destDi.CreateSubdirectory(initDirectoryInfo.Name);
                CopyDirectoryReqursive(initDirectoryInfo, nextDestSubDir);
            }
        }
    }
}
