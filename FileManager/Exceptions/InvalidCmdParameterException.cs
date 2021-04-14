using System;

namespace FileManager.Exceptions
{
    [Serializable]
    public class InvalidCmdParameterException : Exception
    {
        public InvalidCmdParameterException()
        {

        }

        public InvalidCmdParameterException(string param)
        : base($"Неверные параметры команды {param}.")
        {
                
        }
        
    }
}
