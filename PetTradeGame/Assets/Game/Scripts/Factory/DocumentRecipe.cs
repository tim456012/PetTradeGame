using System.Collections.Generic;
using Game.Scripts.Enum;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Document Recipe", menuName = "ScriptableObject/Document Recipe")]
    public class DocumentRecipe : ScriptableObject
    {
        public string document;
        public List<DocumentPartsData> components;
        public PaperType paperType;
    }
}
