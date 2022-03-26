using System.Collections.Generic;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Game.Scripts.Factory
{
    public static class DocumentFactory
    {
        private static readonly Random Random = new();

        /// <summary>
        /// Create Document by loading recipe.
        /// </summary>
        /// <param name="name">Recipe name</param>
        /// <returns>GameObject</returns>
        public static GameObject CreateDocument(string name)
        {
            DocumentRecipe recipe = Resources.Load<DocumentRecipe>(name);
            if (recipe != null)
                return CreateDocument(recipe);
            Debug.Log("No Document Recipe found");
            return null;
        }


        #region Document Process
        private static GameObject CreateDocument(DocumentRecipe recipe)
        {
            //Instantiate Document Base
            GameObject obj = InstantiateGameObj($"Test/Prefabs/{recipe.document}");
            obj.name = recipe.name;
            obj.GetComponent<PaperTypes>().type = recipe.paperType;
            AddContent(obj, recipe.components);

            return obj;
        }

        private static GameObject InstantiateGameObj(string name)
        {
            GameObject temp = Resources.Load<GameObject>(name);
            if (temp == null)
            {
                Debug.LogError($"No prefab for name {name}");
                return new GameObject(name);
            }

            GameObject gameObject = GameObject.Instantiate(temp);
            return gameObject;
        }

        private static void AddContent(GameObject obj, List<DocumentPartsData> data)
        {
            //Place components to specific position by Random seed
            foreach (var partsData in data)
            {
                int index = Random.Next(partsData.components.Count);
                string selected = partsData.components[index];
                //Debug.Log(selected);

                if (partsData.componentName == "Text")
                {
                    AddContentText(obj, selected);
                    return;
                }

                GameObject gameObject = InstantiateGameObj($"Test/Prefabs/{selected}");
                index = Random.Next(partsData.positions.Count);
                GameObject temp = GameObjFinder.FindChildGameObject(obj, partsData.positions[index]);
                gameObject.transform.SetParent(temp.transform);
                gameObject.transform.localPosition = Vector3.zero;
            }
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
            GameObject temp = GameObjFinder.FindChildGameObject(obj, "TypeName");
            temp.GetComponent<TextMeshPro>().text = name;
        }
        #endregion
    }
}
