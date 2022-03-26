namespace Game.Scripts.EventArguments
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
