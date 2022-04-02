using System;
using System.Collections.Generic;
using Game.Scripts.Common.Animation;
using Game.Scripts.Controller.SubController;
using Game.Scripts.EventArguments;
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
        
        //private IEnumerator enqueueObject; //?
        private readonly List<GameObject> documents = new();
        private static readonly List<Poolable> instances = new();

        public static event EventHandler<InfoEventArgs<int>> LicenseSubmitedEvent; 

        private void Awake()
        {
            enabled = false;
        }
        
        #region Initiation
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
                float y = Random.Range(-2, 2);
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
        
        public void ProcessCollision(GameObject original, GameObject col)
        {
            if (col == null)
                return;

            if(original.GetComponent<EasingControl>() || col.GetComponent<EasingControl>())
                return;
            
            sbyte index = InteractionSubController.executeObjBehavior(original, col);
            
            if(index == 0)
                return;
            
            var p = col.GetComponent<Poolable>();
            EnqueueObject(p);
            LicenseSubmitedEvent?.Invoke(this, new InfoEventArgs<int>(1));
        }
        
        private void EnqueueObject(Poolable target)
        {
            if (instances.Count <= 0 || target == null)
                return;
            
            int index = instances.IndexOf(target);
            if(index < 0)
                return;
                
            instances.RemoveAt(index);
            GameObjectPoolSubController.Enqueue(target);
        }
    }
}
