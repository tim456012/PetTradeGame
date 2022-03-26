using UnityEngine;

namespace Game.Scripts.Tools
{
    public static class UIFinder
    {
        private static GameObject canvasObj;
    
        public static GameObject FindUIGameObject(string UIName)
        {
            if (canvasObj == null)
            {
                canvasObj = GameObjFinder.FindGameObject("Canvas");
            }

            return canvasObj == null ? null : GameObjFinder.FindChildGameObject(canvasObj, UIName);
        }

        public static T GetObjComponent<T>(GameObject container, string UIName) where T : Component
        {
            GameObject childGameObject = GameObjFinder.FindChildGameObject(container, UIName);

            T temp = childGameObject.GetComponent<T>();
            if (temp == null)
            {
                Debug.Log($"Component [{UIName}] is not a [{typeof(T)}]");
                return null;
            }
            return temp;
        }
    }

    public static class GameObjFinder
    {
        public static GameObject FindGameObject(string objName)
        {
            GameObject temp = GameObject.Find(objName);
            
            if (temp == null)
            {
                Debug.LogWarning($"GameObject[{objName}] does not exist in this scene.");
                return null;
            }
            return temp;
        }

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
                Transform[] allChildren = container.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    if (child.name == objName)
                    {
                        if (objTran == null)
                        {
                            objTran = child;
                        }
                        else
                        {
                            Debug.LogWarning($"Found same component name[{objName}] under the Container[{container.name}].");
                        }
                    }
                }
            }

            if (objTran == null)
            {
                Debug.LogError($"GameObjFinder : I can't find the component[{objName}] in container[{container.name}].");
                return null;
            }
            return objTran.gameObject;
        }
    }
}
