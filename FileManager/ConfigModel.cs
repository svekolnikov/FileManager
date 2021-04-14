using System;

namespace FileManager
{
    [Serializable]
    public class ConfigModel
    {
        public ConfigModel()
        {}
        /// <summary>
        /// Logger location.
        /// </summary>
        public string LoggerPath { get; set; }
        /// <summary>
        /// Last browsed directory.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Last selected page.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Row numbers in table.
        /// </summary>
        public int RowsPerPage { get; set; }
        /// <summary>
        /// UI offset from top.
        /// </summary>
        public int UiTop { get; set; }
        /// <summary>
        /// UI offset from left.
        /// </summary>
        public int UiLeft { get; set; }
    }
}
