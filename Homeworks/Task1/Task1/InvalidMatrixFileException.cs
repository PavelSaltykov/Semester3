using System;

namespace Task1
{
    /// <summary>
    /// The exception that is thrown when the file contains an invalid matrix.
    /// </summary>
    [Serializable]
    public class InvalidMatrixFileException : Exception
    {
        public InvalidMatrixFileException() { }

        public InvalidMatrixFileException(string message) : base(message) { }
        
        public InvalidMatrixFileException(string message, Exception inner) : base(message, inner) { }
        
        protected InvalidMatrixFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
