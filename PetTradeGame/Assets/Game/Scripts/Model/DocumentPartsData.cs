using System.Collections.Generic;
using UnityEngine; 

namespace Assets.Game.Scripts.Model
{
    [System.Serializable]
    public class DocumentPartsData
    {
        public string componentName;
        public List<string> components;
        public List<string> positions;
    }
}
