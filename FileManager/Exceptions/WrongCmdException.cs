using System;

namespace FileManager.Exceptions
{
    [Serializable]
    public class WrongCmdException : Exception
    {
        public WrongCmdException()
        {}

        public WrongCmdException(string message)
        : base("Неверная команда.")
        {
                
        }
    }
}
