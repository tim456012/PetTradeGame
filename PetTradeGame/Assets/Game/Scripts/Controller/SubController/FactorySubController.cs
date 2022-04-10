using System.Collections.Generic;
using Game.Scripts.Factory;
using UnityEngine;

namespace Game.Scripts.Controller.SubController
{
    public class FactorySubController : MonoBehaviour
    {
        public static GameObject ProduceDocument(string document)
        {
            var obj = DocumentFactory.CreateDocument(document);
            return obj;
        }
    }
}
