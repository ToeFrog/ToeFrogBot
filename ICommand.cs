using System;
using System.Collections.Generic;
using System.Text;

namespace ToeFrogBot
{
    public interface ICommand
    {
        public abstract void Execute();

        public static bool Exists(string name, out ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}