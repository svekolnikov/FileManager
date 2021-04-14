using System;
using System.Collections.Generic;

namespace FileManager.Content
{
    public class Table
    {
        /// <summary>
        /// Creates table.
        /// </summary>
        /// <param name="columns">Instancies of table columns.</param>
        /// <param name="tab">Tabulation inside the table.</param>
        /// <param name="rowsPerPage">Number of displayed items tables</param>
        public Table(Column[] columns, int tab, int rowsPerPage)
        {
            RowsPerPage = rowsPerPage;
            Page = new List<Row>();
            Content = new List<Row>();

            Tab = tab;
            var left = 0;
            foreach (var c in columns)
            {
                c.Left = left;
                left += tab + c.Width + 1;
            }
            Width = left; //width in total with borders and tabs
            Columns = columns;
        }

        public int RowsPerPage { get; }
        public int CurrentPage { get; private set; }
        public int PagesCount { get; private set; }

        public int Tab { get; }
        public int Width { get; }
        public string Path { get; set; }
        public Column[] Columns { get; }
        
        /// <summary>
        /// Consist conent for current page.
        /// </summary>
        public List<Row> Page { get; private set; }

        /// <summary>
        /// Set current page.
        /// </summary>
        /// <param name="page">Page number</param>
        public void SetPage(int page = 1)
        {
            if (page > PagesCount)
            {
                CurrentPage = PagesCount;
                SetPage(CurrentPage);
            }
            else if (page < 1)
            {
                CurrentPage = 1;
                SetPage(CurrentPage);
            }
            else
            {
                CurrentPage = page;
                int start = (CurrentPage - 1) * RowsPerPage;
                int end = start + RowsPerPage - 1;
                if (end > Content.Count - 1) end = Content.Count - 1;
                int count = end - start + 1;

                Page.AddRange(Content.GetRange(start, count));
            }
        }

        /// <summary>
        /// Clear table content.
        /// </summary>
        public void ClearContent()
        {
            CurrentPage = 0;
            PagesCount = 0;
            Page.Clear();
            Content.Clear();
        }

        /// <summary>
        /// Add range of content to table.
        /// </summary>
        /// <param name="content">instance of List&lt;Row&gt;</param>
        public void AddRange(List<Row> content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            Content.AddRange(content);
            PagesCount = (int)Math.Ceiling((float)Content.Count / RowsPerPage);
        }

        /// <summary>
        /// Consist all table content
        /// </summary>
        private List<Row> Content { get;}
    }
}
