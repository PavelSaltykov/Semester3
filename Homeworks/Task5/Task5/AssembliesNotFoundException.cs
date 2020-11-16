using System;

namespace Task5
{
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
