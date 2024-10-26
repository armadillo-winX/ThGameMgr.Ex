using System.Runtime.Serialization;

namespace ThGameMgr.Ex.Exceptions
{
    class InvalidUserException : Exception
    {
        public InvalidUserException() : base() { }
        public InvalidUserException(string message) : base(message) { }
        public InvalidUserException(string message, Exception inner) : base(message, inner) { }
        protected InvalidUserException(SerializationInfo info, StreamingContext context) { }
    }
}
