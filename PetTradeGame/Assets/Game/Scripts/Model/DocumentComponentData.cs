using System.Collections.Generic;
using Game.Scripts.Enum;
using Game.Scripts.Tools;

namespace Game.Scripts.Model
{
    [System.Serializable]
    public class DocumentComponentData
    {
        public bool isText;
        public List<ComponentInformation> componentInformation;
    }

    [System.Serializable]
    public class ComponentInformation
    {
        public List<string> componentsName;
        public DocumentComponentPositions componentPositions;
    }
}
