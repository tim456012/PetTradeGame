using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.TempCode;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controller
{
    //TODO: Need to rewrite FactoryController
    public class ObjectController : MonoBehaviour
    {
        private FactoryController factoryController;
        private InteractionController interactionController;
        
        private IEnumerator enqueueObject;
        private readonly List<Poolable> instances = new();
        
        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            interactionController = GetComponentInChildren<InteractionController>();
        }

        #region Object Initiation
        public void InitFactory(List<string> documentList)
        {
            var instance = gameObject.GetComponentInChildren<FactoryController>();
            if (instance)
            {
                factoryController = instance;
                return;
            }

            factoryController = gameObject.AddComponent<FactoryController>();
            factoryController.ProduceDocument(documentList);
        }

        public void InitObjectPool(List<FunctionalObjectsData> objectList)
        {
            if (objectList == null)
                return;

            foreach (var data in objectList)
            {
                GameObjectPoolController.AddEntry(data.Key, data.Object, data.Amount, data.MaxAmount);
                dequeueObject(data.Key, data.SpawnPosition);
            }
        }

        private void dequeueObject(string key, string spawnPos)
        {
            var obj = GameObjectPoolController.Dequeue(key);

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
                GameObjectPoolController.Enqueue(instances[i]);
            instances.Clear();
        }
        #endregion

        //TODO: Need to modify
        private void ProcessCollision(ObjectType objectType, Collider2D col)
        {
            if (col == null)
                return;

            var collidedObjectType = col.GetComponent<EntityAttribute>().objectType;


            /*if (collidedObjectType.ObjectType == ObjectType.Test && selfObjectType.ObjectType == ObjectType.GreenStamp)
            {
                GameObject stamp = obj.GetComponent<AvailableParts>().parts.Find(part => part.name == "Approved");
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");

                ClearChildren(pos);
                Instantiate(stamp, pos.transform);
            }

            if (collidedObjectType.ObjectType == ObjectType.Test && selfObjectType.ObjectType == ObjectType.RedStamp)
            {
                GameObject stamp = obj.GetComponent<AvailableParts>().parts.Find(part => part.name == "Rejected");
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");
                ClearChildren(pos);

                Instantiate(stamp, pos.transform);
            }*/

            /*if (collidedObjectType.ObjectType == ObjectType.Bin && selfObjectType.ObjectType == ObjectType.Test)
            {
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");
                ClearChildren(pos);

                e.info.gameObject.transform.SetParent(collidedObjectType.transform);
                e.info.gameObject.transform.localPosition = Vector3.zero;

                //GameObjectPoolController.Enqueue(pos.GetComponentInParent<Poolable>());
            }*/
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
