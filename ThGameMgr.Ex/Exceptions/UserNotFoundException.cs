using System.Runtime.Serialization;

namespace ThGameMgr.Ex.Exceptions
{
    class UserNotFoundException : Exception
    {
        public string UserName { get; }
        public UserNotFoundException(string userName) 
            : base($"'{userName}' does not exist.") 
        {
            this.UserName = userName;
        }

        public UserNotFoundException(string userName, string message) : base(message) 
        {
            this.UserName = userName;
        }
        public UserNotFoundException(string userName, string message, Exception inner) : base(message, inner) 
        { 
            this.UserName = userName;
        }

        protected UserNotFoundException(string userName, SerializationInfo info, StreamingContext context) 
        {
            this.UserName = userName;
        }
    }
}
