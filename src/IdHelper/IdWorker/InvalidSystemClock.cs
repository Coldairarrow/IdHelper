using System;

namespace Coldairarrow.Util
{
    class InvalidSystemClock : Exception
    {      
        public InvalidSystemClock(string message) : base(message) { }
    }
}