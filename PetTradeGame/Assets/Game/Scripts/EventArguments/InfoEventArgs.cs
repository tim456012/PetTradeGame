using System;

namespace Game.Scripts.EventArguments
{
    public class InfoEventArgs<T> : EventArgs
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