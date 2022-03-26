using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "Document Position", menuName = "ScriptableObject/Document Position")]
    public class DocumentPositionData : ScriptableObject
    {
        public List<string> positions;
    }
}
