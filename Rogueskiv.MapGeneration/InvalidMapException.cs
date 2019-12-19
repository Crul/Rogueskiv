using System;

namespace Rogueskiv.MapGeneration
{
    class InvalidMapException : Exception
    {
        public InvalidMapException(string message)
            : base(GetMessage(message)) { }

        public InvalidMapException(string message, Exception innerException)
            : base(GetMessage(message), innerException) { }

        public InvalidMapException()
        { }

        private static string GetMessage(string message) =>
            $"InvalidMap: {message}";
    }
}
