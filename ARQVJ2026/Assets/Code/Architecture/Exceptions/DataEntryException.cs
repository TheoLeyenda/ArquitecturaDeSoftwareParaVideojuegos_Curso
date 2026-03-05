using System;

namespace ZooArchitect.Architecture.Exceptions
{
    public sealed class DataEntryException : Exception
    {
        public DataEntryException(string message) : base(message)
        {
        }
    }

    
}
