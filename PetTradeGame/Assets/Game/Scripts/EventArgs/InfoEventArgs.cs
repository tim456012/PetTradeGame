using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Scripts.EventArgs
{
    public class InfoEventArgs<T> : System.EventArgs
    {
        public T info;

        public InfoEventArgs()
        {
            info = default(T);
        }

        public InfoEventArgs(T info)
        {
            this.info = info;
        }
    }
}
