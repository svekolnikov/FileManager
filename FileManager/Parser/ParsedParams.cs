namespace FileManager.Parser
{
    public enum Commands
    {
        CreateFile,
        CreateDirectory,
        RemoveFileOrDirectory,
        CopyFile,
        CopyDirectory,
        Cd
    }

    public class ParsedParams
    {
        public Commands Command { get; set; }
        public string InitPath { get; set; }
        public string DestPath { get; set; }

        public int Page { get; set; }
    }
}
