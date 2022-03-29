using System.Collections.Generic;
using Game.Scripts.Controller.SubController;
using Game.Scripts.Enum;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controller
{
    public class ObjectController : MonoBehaviour
    {
        private FactorySubController _factorySubController;
        //private InteractionSubController _interactionSubController;
        
        //private IEnumerator enqueueObject; //?
        private readonly List<GameObject> documents = new();
        private readonly List<Poolable> instances = new();
        
        private void Awake()
        {
            enabled = false;
        }
        
        #region Initiation
        /*public void InitInteraction()
        {
            var instance = gameObject.GetComponentInChildren<InteractionSubController>();
            if (instance)
            {
                _interactionSubController = instance;
                return;
            }

            _interactionSubController = gameObject.AddComponent<InteractionSubController>();
        }*/
        
        public void InitFactory(List<string> documentList)
        {
            var instance = gameObject.GetComponentInChildren<FactorySubController>();
            if (instance)
            {
                _factorySubController = instance;
                return;
            }

            _factorySubController = gameObject.AddComponent<FactorySubController>();
            produceDocument(documentList);
        }
        
        public void InitObjectPool(List<FunctionalObjectsData> objectList)
        {
            if (objectList == null)
                return;

            foreach (var data in objectList)
            {
                GameObjectPoolSubController.AddEntry(data.Key, data.Object, data.Amount, data.MaxAmount);
                dequeueObject(data.Key, data.SpawnPosition);
            }
        }
        #endregion

        #region Methods

        private void produceDocument(List<string> list)
        {
            foreach (string document in list)
            {
                var obj = _factorySubController.ProduceDocument(document);
                float x = Random.Range(-3, 3);
                float y = Random.Range(-3, 3);
                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.SetActive(true);
                documents.Add(obj);
            }
        }

        private void dequeueObject(string key, string spawnPos)
        {
            var obj = GameObjectPoolSubController.Dequeue(key);

            if (!string.IsNullOrEmpty(spawnPos))
            {
                var temp = GameObjFinder.FindChildGameObject(gameObject, spawnPos);
                obj.transform.localPosition = temp.transform.position;
                obj.gameObject.SetActive(true);
            }
            else
            {
                float x = Random.Range(-3, 3);
                float y = Random.Range(-3, 3);
                
                obj.transform.localPosition = new Vector3(x,y,0);
                obj.gameObject.SetActive(true);
            }
           
            instances.Add(obj);
        }

        //TODO: Create enqueue object method
        
        private void ReleaseInstances()
        {
            for (int i = instances.Count - 1; i >= 0; --i)
                GameObjectPoolSubController.Enqueue(instances[i]);
            instances.Clear();
        }
        
        private void ReleaseGameObjList()
        {
            for (int i = documents.Count - 1; i >= 0; --i)
                Destroy(documents[i]);
            documents.Clear();
        }
        
        #endregion
        
        public static void ProcessCollision(GameObject original, GameObject col)
        {
            if (col == null)
                return;

            InteractionSubController.executeObjBehavior(original, col);
        }
        
        public void ClearChildren(GameObject obj)
        {
            int i = 0;
            GameObject[] allChild = new GameObject[obj.transform.childCount];

            foreach (Transform child in obj.transform)
            {
                allChild[i] = child.gameObject;
                i++;
            }

            foreach (GameObject child in allChild)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
