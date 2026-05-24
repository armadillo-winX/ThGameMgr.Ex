using System.Runtime.Serialization;

namespace ThGameMgr.Ex.Exceptions
{
    class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base() { }
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected UserNotFoundException(SerializationInfo info, StreamingContext context) { }
    }
}
