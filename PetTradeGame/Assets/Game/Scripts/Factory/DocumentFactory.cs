using System.Collections.Generic;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Game.Scripts.Factory
{
    //TODO: Modify document generate algorithm
    public static class DocumentFactory
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Create Document by loading recipe.
        /// </summary>
        /// <param name="name">Recipe name</param>
        /// <returns>GameObject</returns>
        public static GameObject CreateDocument(string name)
        {
            var recipe = Resources.Load<DealerLicenseRecipe>($"Recipes/{name}");
            if (recipe != null)
                return CreateDocument(recipe);
            Debug.Log("No Document Recipe found");
            return null;
        }

        #region Document Process
        private static GameObject CreateDocument(DealerLicenseRecipe recipe)
        {
            /*//Instantiate Document Base
            var obj = InstantiateGameObj($"Documents/{recipe.documentType}");
            obj.name = recipe.name;
            obj.GetComponent<EntityAttribute>().paperType = recipe.documentType;
            AddContent(obj, recipe.components);
            return obj;*/
            return null;
        }

        private static GameObject InstantiateGameObj(string name)
        {
            var temp = Resources.Load<GameObject>(name);
            if (temp == null)
            {
                Debug.LogError($"No prefab for name {name}");
                return new GameObject(name);
            }

            var gameObject = Object.Instantiate(temp);
            return gameObject;
        }

        private static void AddContent(GameObject obj, List<DocumentComponentData> data)
        {
            /*//Place components to specific position by Random seed
            foreach (var partsData in data)
            {
                int index = Random.Next(partsData.components.Count);
                string selected = partsData.components[index];
                Debug.Log(selected);

                /*if (partsData.componentType == "Text")
                {
                    AddContentText(obj, selected);
                    return;
                }#1#

                var gameObject = InstantiateGameObj($"Test/Prefabs/{selected}");
                //index = Random.Next(partsData.positions.Count);
                //var temp = GameObjFinder.FindChildGameObject(obj, partsData.positions[index]);
                //gameObject.transform.SetParent(temp.transform);
                gameObject.transform.localPosition = Vector3.zero;
            }*/
        }

        /*private static void RemoveContent(GameObject obj, string parts)
        {
            foreach (DocumentPartsData part in parts)
            {
                GameObject.Destroy(GameObjFinder.FindChildGameObject(obj, part.componentsName));
            }
        }*/

        private static void AddContentText(GameObject obj, string name)
        {
            var temp = GameObjFinder.FindChildGameObject(obj, "TypeName");
            temp.GetComponent<TextMeshPro>().text = name;
        }
        #endregion
    }
}
