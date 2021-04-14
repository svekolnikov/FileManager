using System;
namespace FileManager.Exceptions
{
    [Serializable]
    public class DirNotFoundException : Exception
    {
        public DirNotFoundException()
        {
                
        }

        public DirNotFoundException(string directory)
        :base($"Директория [{directory}] не найдена.")
        {
                
        }
    }
}
