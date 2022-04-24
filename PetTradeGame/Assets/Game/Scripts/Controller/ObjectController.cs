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
        private FactorySubController factorySubController;
        private DragAndDropSubController dragAndDropSubController;

        //private IEnumerator enqueueObject;
        private readonly List<GameObject> documents = new();
        private static readonly List<Poolable> instances = new();

        public static event EventHandler<InfoEventArgs<int>> LicenseSubmittedEvent;

        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            dragAndDropSubController = GetComponent<DragAndDropSubController>();
        }

        #region Initiation
        public void InitFactory(List<string> documentList)
        {
            var instance = gameObject.GetComponentInChildren<FactorySubController>();
            if (instance)
            {
                factorySubController = instance;
                return;
            }

            factorySubController = gameObject.AddComponent<FactorySubController>();
            ProduceDocument(documentList);
        }
        
        public void InitObjectPool(List<FunctionalObjectsData> objectList)
        {
            if (objectList == null)
                return;

            foreach (var data in objectList)
            {
                GameObjectPoolSubController.AddEntry(data.Key, data.Object, data.Amount, data.MaxAmount);
                DequeueObject(data.Key, data.SpawnPosition);
            }
        }
        #endregion

        #region Methods

        private void ProduceDocument(List<string> list)
        {
            foreach (string document in list)
            {
                var obj = FactorySubController.ProduceDocument(document);
                float x = Random.Range(-3, 3);
                float y = Random.Range(-2, 2);
                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.SetActive(true);
                documents.Add(obj);
            }
        }

        private void DequeueObject(string key, string spawnPos)
        {
            var obj = GameObjectPoolSubController.Dequeue(key);
            
            //Destroy(obj.gameObject.GetComponent<EasingControl>());
            //Destroy(obj.gameObject.GetComponent<TransformLocalPositionTweener>());

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
            
            sbyte index = InteractionSubController.ExecuteObjBehavior(original, col);
            
            if(index == 0)
                return;
            
            var p = col.GetComponent<Poolable>();
            EnqueueObject(p);
            dragAndDropSubController.TargetObj = null;
            DequeueObject("License", "LicensePos");
            LicenseSubmittedEvent?.Invoke(this, new InfoEventArgs<int>(1));
        }
        
        private static void EnqueueObject(Poolable target)
        {
            if (instances.Count <= 0 || target == null)
                return;
            
            int index = instances.IndexOf(target);
            if(index < 0)
                return;
            
            instances.RemoveAt(index);
            GameObjectPoolSubController.Enqueue(target);
            InputController.IsDragActive = false;
        }
    }
}
