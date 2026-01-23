using System;

namespace ZooArchitect.Architecture.Exceptions
{
    public sealed class BrokenGameRuleException : Exception
    {
        public BrokenGameRuleException(string message) : base(message)
        {
        }
    }
}
