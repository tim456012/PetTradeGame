using UnityEngine;

namespace Game.Scripts.Tools
{
    public static class UIFinder
    {
        private static GameObject canvasObj;
    
        /// <summary>
        /// Find UI GameObject under the canvas.
        /// </summary>
        /// <param name="UIName">GameObject name that you want to search.</param>
        /// <returns>GameObject</returns>
        public static GameObject FindUIGameObject(string UIName)
        {
            if (canvasObj == null)
                canvasObj = GameObjFinder.FindGameObject("Canvas");

            return canvasObj == null ? null : GameObjFinder.FindChildGameObject(canvasObj, UIName);
        }

        /// <summary>
        /// Find GameObject's component.
        /// </summary>
        /// <param name="container">Target GameObject</param>
        /// <param name="componentName">Component that you want to find.</param>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Component type</returns>
        public static T GetObjComponent<T>(GameObject container, string componentName) where T : Component
        {
            var childGameObject = GameObjFinder.FindChildGameObject(container, componentName);

            var temp = childGameObject.GetComponent<T>();
            if (temp != null)
                return temp;
            Debug.Log($"Component [{componentName}] is not a [{typeof(T)}]");
            return null;
        }
    }
    
    public static class GameObjFinder
    {
        /// <summary>
        /// Find the GameObject.
        /// </summary>
        /// <param name="objName">Target GameObject</param>
        /// <returns>GameObject</returns>
        public static GameObject FindGameObject(string objName)
        {
            var temp = GameObject.Find(objName);

            if (temp != null)
                return temp;
            
            Debug.LogWarning($"GameObject[{objName}] does not exist in this scene.");
            return null;
        }

        /// <summary>
        /// Find the child GameObject under the parent
        /// </summary>
        /// <param name="container">Parent GameObject</param>
        /// <param name="objName">Target GameObject</param>
        /// <returns>GameObject</returns>
        public static GameObject FindChildGameObject(GameObject container, string objName)
        {
            if (container == null)
            {
                Debug.LogError($"GameObjFinder : I can't find the child's container. (Null)");
                return null;
            }
            
            Transform objTran = null;

            if (container.name == objName)
            {
                objTran = container.transform;
            }
            else
            {
                var allChildren = container.transform.GetComponentsInChildren<Transform>();
                foreach (var child in allChildren)
                {
                    if (child.name != objName)
                        continue;
                    
                    if (objTran == null)
                        objTran = child;
                    else
                        Debug.LogWarning($"Found same component name[{objName}] under the Container[{container.name}].");
                }
            }

            if (objTran != null)
                return objTran.gameObject;
            
            Debug.LogError($"GameObjFinder : I can't find the component[{objName}] in container[{container.name}].");
            return null;
        }
    }
}
