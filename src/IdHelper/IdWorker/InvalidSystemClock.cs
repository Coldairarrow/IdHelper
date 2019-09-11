using System;

namespace Coldairarrow.Util
{
    internal class InvalidSystemClock : Exception
    {      
        public InvalidSystemClock(string message) : base(message) { }
    }
}