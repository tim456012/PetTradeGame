using System;

namespace Game.Scripts.Enum
{
    [Flags]
    public enum ObjectType
    {
        None = 0,
        GreenStamp = 1,
        RedStamp = 2,
        License= 3,
        CollectBox = 4
    }
}
