using System.Collections.Generic;
using UnityEngine;
using Assets.Game.Scripts.Enum;
using Assets.Game.Scripts.Model;

namespace Assets.Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Document Recipe", menuName = "ScriptableObject/Document Recipe")]
    public class DocumentRecipe : ScriptableObject
    {
        public string document;
        public List<DocumentPartsData> components;
        public PaperType paperType;
    }
}
