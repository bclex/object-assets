using System;

namespace OA.Ultima.Core.Network
{
    public class NetworkException : Exception
    {
        public NetworkException(string message)
            : base(message) { }
    }
}
