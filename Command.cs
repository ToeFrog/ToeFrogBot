using System;
using System.Collections.Generic;
using System.Text;

namespace ToeFrogBot
{
    public abstract class Command
    {
        public abstract void Execute();

        public abstract bool Exists(string name);
    }
}