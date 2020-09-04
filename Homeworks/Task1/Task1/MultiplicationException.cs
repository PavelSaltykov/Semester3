using System;

namespace Task1
{
    [Serializable]
    public class MultiplicationException : Exception
    {
        public MultiplicationException() { }
        
        public MultiplicationException(string message) : base(message) { }
        
        public MultiplicationException(string message, Exception inner) : base(message, inner) { }
        
        protected MultiplicationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
