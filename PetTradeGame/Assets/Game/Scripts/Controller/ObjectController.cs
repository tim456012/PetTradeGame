using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.TempCode;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class ObjectController : MonoBehaviour
    {
        private FactoryController factoryController;
        
        private IEnumerator enqueueObject;
        private List<Poolable> instances = new();

        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            DragAndDropController.TargetObjSelectedEvent += OnSelected;
        }

        public void InitFactory(List<string> documentList)
        {
            FactoryController instance = gameObject.GetComponentInChildren<FactoryController>();
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

            foreach (FunctionalObjectsData data in objectList)
            {
                GameObjectPoolController.AddEntry(data.Key, data.Object, data.Amount, data.MaxAmount);
                dequeueObject(data.Key, data.SpawnPosition);
            }
        }

        private void dequeueObject(string key, string spawnPos)
        {
            Poolable obj = GameObjectPoolController.Dequeue(key);

            if (!string.IsNullOrEmpty(spawnPos))
            {
                GameObject temp = GameObjFinder.FindChildGameObject(gameObject, spawnPos);
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

        //TODO: Need to modify
        private void OnSelected(object sender, InfoEventArgs<GameObject> e)
        {
            if (!DetectCollision(e.info, out GameObject obj) || !e.info.GetComponent<EntityAttribute>().IsFunctionalObject)
                return;

            if (obj == null)
                return;

            ObjectTypes selfObjectType = e.info.GetComponent<ObjectTypes>();
            ObjectTypes collidedObjectType = obj.GetComponent<ObjectTypes>();

            if (collidedObjectType.ObjectType == ObjectType.Test && selfObjectType.ObjectType == ObjectType.GreenStamp)
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
            }

            /*if (collidedObjectType.ObjectType == ObjectType.Bin && selfObjectType.ObjectType == ObjectType.Test)
            {
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");
                ClearChildren(pos);

                e.info.gameObject.transform.SetParent(collidedObjectType.transform);
                e.info.gameObject.transform.localPosition = Vector3.zero;

                //GameObjectPoolController.Enqueue(pos.GetComponentInParent<Poolable>());
            }*/
        }

        public bool DetectCollision(GameObject obj, out GameObject collided)
        {
            collided = null;
            Collider2D self = obj.GetComponent<Collider2D>();

            foreach (Poolable other in instances)
            {
                if (other.gameObject == obj)
                    continue;

                Collider2D target = other.GetComponent<Collider2D>();

                if (!target.bounds.Intersects(self.bounds)) 
                    continue;

                collided = target.gameObject;
                return true;
            }
            return false;
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

        private void OnDisable()
        {
            DragAndDropController.TargetObjSelectedEvent -= OnSelected;
        }

        private void OnEnable()
        {
            DragAndDropController.TargetObjSelectedEvent += OnSelected;
        }
    }
}
