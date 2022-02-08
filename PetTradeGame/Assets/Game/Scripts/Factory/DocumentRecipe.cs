using System.Collections.Generic;
using UnityEngine;
using Assets.Game.Scripts.View_Model_Components;

namespace Assets.Game.Scripts.Factory
{
    [CreateAssetMenu(fileName = "Document Recipe", menuName = "ScriptableObject/Document Recipe")]
    public class DocumentRecipe : ScriptableObject
    {
        public string document;
        public List<string> documentParts;
        public PaperType paperType;
    }
}
