namespace FileManager.Content
{
    public enum TableEntityType
    {
        Directory, 
        File
    }

    public class Row
    {
        /// <summary>
        /// Directory or file name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type of table entity.
        /// </summary>
        public TableEntityType Type { get; set; }
        /// <summary>
        /// Extension of table entity.
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Size of table entity.
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// DateCreated created.
        /// </summary>
        public string DateCreated { get; set; }
    }
}
