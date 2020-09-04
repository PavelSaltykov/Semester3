using System;

namespace Task1
{
    /// <summary>
    /// The exception that is thrown when the number of columns in the first matrix is not equal
    /// to the number of rows in the second matrix.
    /// </summary>
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
