using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Model
{
    [CreateAssetMenu(fileName = "Document Position", menuName = "ScriptableObject/Document Position")]
    public class DocumentPositionData : ScriptableObject
    {
        public List<string> positions;
    }
}
