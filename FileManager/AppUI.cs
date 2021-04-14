using System;
using FileManager.Content;

namespace FileManager
{
    public class AppUi
    {
        private readonly int _x0;
        private readonly int _y0;
        private readonly Table _table;
        
        private int _yPath;
        private int _yCont;
        private int _yInfo;
        private int _yCmmd;

        /// <summary>
        /// Draws the user interface according to the specified coordinates.
        /// </summary>
        /// <param name="xPos">Offset from the left side.</param>
        /// <param name="yPos">Offset from the top side.</param>
        /// <param name="t">Шnstance of table </param>
        public AppUi(int xPos, int yPos, Table t)
        {
            _x0 = xPos;
            _y0 = yPos;
            _table = t;

            CreateUi();
        }

        /// <summary>
        /// Clear and print new tstring of info in the "System info" block.
        /// </summary>
        /// <param name="info"></param>
        public void PrintInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(_x0 + 2, _yInfo + 1);
            Console.Write(new string(' ', _table.Width - 2));
            Console.SetCursorPosition(_x0 + 2, _yInfo + 1);
            Console.Write(info);
        }

        /// <summary>
        /// Updates UI table.
        /// </summary>
        public void Update()
        {
            ClearTable();
            PrintPath(_table.Path, ConsoleColor.Yellow);

            PrintBlockHeader(_x0, _yCont, $"Content - Page {_table.CurrentPage} of {_table.PagesCount}", ConsoleColor.DarkYellow);

            for (var i = 0; i < _table.Page.Count; i++)
            {
                var entry = _table.Page[i];

                if      (entry.Type == TableEntityType.DIR)  Console.ForegroundColor = ConsoleColor.Green;
                else if (entry.Type == TableEntityType.File) Console.ForegroundColor = ConsoleColor.Yellow;
                
                Console.SetCursorPosition(_x0 + _table.Columns[0].Left + 2, _yCont + i + 3); Console.Write(entry.Name);
                Console.SetCursorPosition(_x0 + _table.Columns[1].Left + 2, _yCont + i + 3); Console.Write(entry.Type);
                Console.SetCursorPosition(_x0 + _table.Columns[2].Left + 2, _yCont + i + 3); Console.Write(entry.Extension);
                Console.SetCursorPosition(_x0 + _table.Columns[3].Left + 2, _yCont + i + 3); Console.Write(entry.Size);
                Console.SetCursorPosition(_x0 + _table.Columns[4].Left + 2, _yCont + i + 3); Console.Write(entry.DateCreated);
            }

            FocusOnCommandLine();
        }

        /// <summary>
        /// Clear command line and set cursor in start of line.
        /// </summary>
        public void FocusOnCommandLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(_x0 + 2, _yCmmd + 1);
            Console.Write(new string(' ', _table.Width - 2));
            Console.SetCursorPosition(_x0 + 2, _yCmmd + 1);
            Console.Write($"Path>");
        }


        private void CreateUi()
        {
            _yPath = _y0;
            _yCont = _yPath + 3;
            int linesCont = _table.RowsPerPage + 2;
            _yInfo = _yCont + linesCont + 2;
            int linesInfo = 1;
            _yCmmd = _yInfo + linesInfo + 2;
            int linesCmmd = 1;
            int linesPath = 1;

            Console.Clear();

            DrawBlock(_x0, _yPath, linesPath, _table.Width, ConsoleColor.Blue);
            PrintBlockHeader(_x0, _yPath, "Path", ConsoleColor.DarkYellow);

            DrawBlock(_x0, _yCont, linesCont, _table.Width, ConsoleColor.Blue);
            PrintBlockHeader(_x0, _yCont, "Content", ConsoleColor.DarkYellow);

            DrawBlock(_x0, _yInfo, linesInfo, _table.Width, ConsoleColor.Blue);
            PrintBlockHeader(_x0, _yInfo, "System info", ConsoleColor.DarkYellow);

            DrawBlock(_x0, _yCmmd, linesCmmd, _table.Width, ConsoleColor.Blue);
            PrintBlockHeader(_x0, _yCmmd, "Command line", ConsoleColor.DarkYellow);

            DrawTable(_x0 + 1, _yCont + 1, linesCont, ConsoleColor.Blue, ConsoleColor.DarkRed);

            FocusOnCommandLine();
        }

        private void ClearTable()
        {
            PrintPath(new string(' ', _table.Width - 2), ConsoleColor.Black);
            for (int i = 0; i < _table.RowsPerPage; i++)
            {
                Console.SetCursorPosition(_x0 + _table.Columns[0].Left + 2, _yCont + i + 3); Console.Write(new string(' ', _table.Columns[0].Width - 1));
                Console.SetCursorPosition(_x0 + _table.Columns[1].Left + 2, _yCont + i + 3); Console.Write(new string(' ', _table.Columns[1].Width - 1));
                Console.SetCursorPosition(_x0 + _table.Columns[2].Left + 2, _yCont + i + 3); Console.Write(new string(' ', _table.Columns[2].Width - 1));
                Console.SetCursorPosition(_x0 + _table.Columns[3].Left + 2, _yCont + i + 3); Console.Write(new string(' ', _table.Columns[3].Width - 1));
                Console.SetCursorPosition(_x0 + _table.Columns[4].Left + 2, _yCont + i + 3); Console.Write(new string(' ', _table.Columns[4].Width - 1));
            }
        }

        private void PrintPath(string path, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(_x0 + 2, _yPath + 1);
            Console.Write(path);
        }
        
        private void DrawBlock(int x, int y, int numLines, int width,ConsoleColor borderColor)
        {
            Console.SetCursorPosition(x, y);

            Console.ForegroundColor = borderColor;
            Console.Write("╔"); Console.Write(new string('═', width)); Console.Write("╗");
            for (int i = 0; i < numLines; i++)
            {
                Console.SetCursorPosition(x, y + i + 1); Console.Write("║"); Console.SetCursorPosition(x + width + 1, y + i + 1); Console.Write("║");
            }
            Console.SetCursorPosition(x, y + numLines + 1); Console.Write("╚"); Console.Write(new string('═', width)); Console.Write("╝");
        }

        private void PrintBlockHeader(int x, int y, string header, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(_x0 + 2, y);
            Console.WriteLine($" {header} ");
        }

        private void DrawTable(int x, int y, int numLines, ConsoleColor lineColor, ConsoleColor labelColor)
        {
            for (int i = 0; i < _table.Columns.Length; i++)
            {
                //Header names
                Console.ForegroundColor = labelColor;
                Console.SetCursorPosition(x + _table.Columns[i].Left + _table.Tab, y); Console.Write(_table.Columns[i].Name);

                //Borders
                Console.ForegroundColor = lineColor;
                if (i == _table.Columns.Length - 1)
                {
                    Console.SetCursorPosition(x + _table.Columns[i].Left, y + 1); Console.Write(new string('─', _table.Columns[i].Width + 2));
                }
                else
                {
                    Console.SetCursorPosition(x + _table.Columns[i].Left, y + 1); Console.Write(new string('─', _table.Columns[i].Width + 1));
                }

                if (i > 0 && i < _table.Columns.Length)
                {
                    Console.SetCursorPosition(x + _table.Columns[i].Left - 1, y );    Console.Write("│");
                    Console.SetCursorPosition(x + _table.Columns[i].Left - 1, y + 1); Console.Write("┼");

                    for (int line = 2; line < numLines; line++)
                    {
                        Console.SetCursorPosition(x + _table.Columns[i].Left - 1, y + line); Console.Write("│");
                    }
                }

            }
            Console.SetCursorPosition(x, y - 1);
        }
    }
}
