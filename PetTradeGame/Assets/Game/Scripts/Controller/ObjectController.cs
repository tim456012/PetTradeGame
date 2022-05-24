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
        private DragAndDropSubController _dragAndDropSubController;

        //private IEnumerator enqueueObject;
        private readonly List<GameObject> _documents = new List<GameObject>();
        private static readonly List<Poolable> Instances = new List<Poolable>();

        public static event EventHandler<InfoEventArgs<int>> LicenseSubmittedEvent;

        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            _dragAndDropSubController = GetComponent<DragAndDropSubController>();
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
                _documents.Add(obj);
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

                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.gameObject.SetActive(true);
            }

            Instances.Add(obj);
        }

        private void ReleaseInstances()
        {
            for (int i = Instances.Count - 1; i >= 0; --i)
                GameObjectPoolSubController.Enqueue(Instances[i]);
            Instances.Clear();
        }

        private void ReleaseGameObjList()
        {
            for (int i = _documents.Count - 1; i >= 0; --i)
                Destroy(_documents[i]);
            _documents.Clear();
        }
        #endregion

        public void ProcessCollision(GameObject original, GameObject col)
        {
            if (col == null)
                return;

            if (original.GetComponent<EasingControl>() || col.GetComponent<EasingControl>())
                return;

            sbyte index = InteractionSubController.ExecuteObjBehavior(original, col);

            if (index == 0)
                return;

            var p = col.GetComponent<Poolable>();
            EnqueueObject(p);
            _dragAndDropSubController.TargetObj = null;
            DequeueObject("License", "LicensePos");
            LicenseSubmittedEvent?.Invoke(this, new InfoEventArgs<int>(1));
        }

        private static void EnqueueObject(Poolable target)
        {
            if (Instances.Count <= 0 || target == null)
                return;

            int index = Instances.IndexOf(target);
            if (index < 0)
                return;

            Instances.RemoveAt(index);
            GameObjectPoolSubController.Enqueue(target);
            InputController.IsDragActive = false;
        }
    }
}
