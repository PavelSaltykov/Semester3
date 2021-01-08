using System;

namespace MyNUnit
{
    /// <summary>
    /// The exception that is thrown when assembly files were not found.
    /// </summary>
    [Serializable]
    public class AssembliesNotFoundException : Exception
    {
        public AssembliesNotFoundException() { }

        public AssembliesNotFoundException(string message) : base(message) { }

        public AssembliesNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected AssembliesNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
