using System;
using System.Collections.Generic;
using System.Text;

namespace FrogBot
{
    public abstract class Command
    {
        public abstract void Execute();

        public abstract void Stop();
    }
}
