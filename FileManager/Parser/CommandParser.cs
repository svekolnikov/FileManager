using System.IO;
using System.Text.RegularExpressions;
using FileManager.Exceptions;

namespace FileManager.Parser
{
    public class CommandParser
    {
        private readonly ConfigModel _config;
        private readonly ParsedParams _parsedParams;

        public CommandParser(ConfigModel config)
        {
            this._config = config;
            _parsedParams = new ParsedParams();
            Reset();
        }

        /// <summary>
        /// Resets properties of parsed parameters entity.
        /// </summary>
        private void Reset()
        {
            _parsedParams.Page = 1;
            _parsedParams.DestPath = string.Empty;
            _parsedParams.InitPath = string.Empty;
        }

        /// <summary>
        /// Do parse input string.
        /// </summary>
        /// <param name="cmd">Inpunt string.</param>
        /// <returns>Instance of ParsedParams.</returns>
        public ParsedParams Parse(string cmd)
        {
            Reset();
            if (string.IsNullOrWhiteSpace(cmd))
                throw new WrongCmdException("");

            cmd = cmd.Trim().ToLower();
            string[] cmdSplit = cmd.Split(" ");
            string data = string.Empty;

            if (cmdSplit[0] == "..")
            {
                DirectoryInfo di = new DirectoryInfo(_config.Path).Parent;
                if (di != null)
                {
                    _parsedParams.Command = Commands.Cd;
                    _parsedParams.DestPath = di.FullName;
                }
            }
            else if(cmdSplit.Length < 2 )
            {
                throw new WrongCmdException("");
            }
            else switch (cmdSplit[0])
            {
                case "cd":
                {
                    data = cmd.Remove(0, 3); // remove " cd"
                    if (Directory.Exists(data))
                    {
                        _parsedParams.DestPath = data;
                        _parsedParams.Command = Commands.Cd;
                    }
                    else if (Directory.Exists(_config.Path + data)) // local directory
                    {
                        _parsedParams.DestPath = _config.Path + data;
                        _parsedParams.Command = Commands.Cd;
                    }
                    else
                    {
                        throw new DirNotFoundException(data);
                    }
                    break;
                }
                case "ls":
                {
                    data = cmd.Remove(0, 3); // remove " cd"

                    // path -p 5
                    // group1: path , group2: -p, group3: page, group6: page if cmd "ls -p 5"
                    Regex rgLs = new Regex(@"^(.+?)(\s-p\s)(\d+$)$|((-p\s)(\d+$))", RegexOptions.IgnoreCase); 
                    Match match = rgLs.Match(data);
                    if (match.Success)
                    {
                        if (!string.IsNullOrEmpty(match.Groups[6].Value)) // ls -p 5
                        {
                            _parsedParams.DestPath = "";
                            _parsedParams.Page = int.Parse(match.Groups[6].Value);
                            _parsedParams.Command = Commands.Cd;
                        }
                        else if (!Directory.Exists(match.Groups[1].Value))
                        {
                            throw new DirNotFoundException(match.Groups[1].Value);
                        }
                        else
                        {
                            _parsedParams.DestPath = match.Groups[1].Value;
                            _parsedParams.Page = int.Parse(match.Groups[3].Value);
                            _parsedParams.Command = Commands.Cd;
                        }
                    }
                    else
                    {
                        throw new InvalidCmdParameterException("ls");
                    }
                    break;
                }
                case "cp": // cp d:\exampledir\folder1 d:\exampledir\folder2
                    data = cmd.Remove(0, 3);
                    string diskInit, diskDest, initPath, destPath;
                    string pattern = @"[a-zA-Z]:"; // Regex pattern: " D:"

                    MatchCollection mc = new Regex(pattern).Matches(data);
                    if (mc.Count == 2)
                    {
                        diskInit = mc[0].Value.Trim();
                        diskDest = mc[1].Value.Trim();
                    }
                    else
                    {
                        throw new InvalidCmdParameterException("cp");
                    }

                    //Split two paths
                    string[] splitData = Regex.Split(data, pattern, RegexOptions.IgnoreCase);
                    if (splitData.Length == 3)
                    {
                        initPath = diskInit + splitData[1].Trim();
                        destPath = diskDest + splitData[2].Trim();
                    }
                    else
                    {
                        throw new InvalidCmdParameterException("cp");
                    }

                    //Combine
                    if (Directory.Exists(initPath) && Directory.Exists(destPath))
                    {
                        _parsedParams.Command = Commands.CopyDirectory;
                    }
                    else if (File.Exists(initPath) && File.Exists(destPath))
                    {
                        _parsedParams.Command = Commands.CopyFile;
                    }
                    else
                    {
                        throw new InvalidCmdParameterException("cp");
                    }

                    _parsedParams.InitPath = initPath;
                    _parsedParams.DestPath = destPath;

                    break;
                case "rm":
                    data = cmd.Remove(0, 3);
                    if (Directory.Exists(data))
                    {
                        _parsedParams.InitPath = data;
                        _parsedParams.Command = Commands.RemoveFileOrDirectory;
                    }
                    else if (Directory.Exists(_config.Path + data)) // local directory
                    {
                        _parsedParams.InitPath = _config.Path + data;
                        _parsedParams.Command = Commands.RemoveFileOrDirectory;
                    }
                    else
                    {
                        throw new DirNotFoundException(data);
                    }
                    break;
                case "nf":
                    data = cmd.Remove(0, 3);
                    _parsedParams.InitPath = _config.Path + data;
                    _parsedParams.Command = Commands.CreateFile;
                    break;
                case "nd":
                    data = cmd.Remove(0, 3);
                    _parsedParams.InitPath = _config.Path + data;
                    _parsedParams.Command = Commands.CreateDirectory;
                    break;
            }

            return _parsedParams;
        }
    }
}
