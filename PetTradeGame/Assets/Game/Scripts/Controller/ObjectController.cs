using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Common.Animation;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.TempCode;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controller
{
    public class ObjectController : MonoBehaviour
    {
        #region Field
        private readonly static List<Poolable> Instances = new List<Poolable>();

        private FactoryController _factoryController;
        private DragAndDropController _dragAndDropController;
        private GameObject _lastObj;
        private IEnumerator _reGenerateDocument;
        private bool _isScaled, _isNotReady, _isEnd;
        private int _index;
        
        public static event EventHandler<InfoEventArgs<int>> LicenseSubmittedEvent;
        #endregion

        #region MonoBehavior
        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            _factoryController = GetComponent<FactoryController>();
            _dragAndDropController = GetComponent<DragAndDropController>();
        }
        
        #endregion
        
        public void InitObjectPool(List<FunctionalObjectsData> objectList)
        {
            if (objectList == null)
                return;
            
            foreach (var data in objectList)
            {
                GameObject prefab = null;
                Addressables.LoadAssetAsync<GameObject>(data.prefab).Completed += obj =>
                {
                    if(obj.Status == AsyncOperationStatus.Succeeded)
                        prefab = obj.Result;
                };
                GameObjectPoolSubController.AddEntry(data.key, prefab, data.amount, data.maxAmount);
                DequeueObject(data.key, data.spawnPosition);
            }
        }
        
        #region Object Pool Method
        private void DequeueObject(string key, string spawnPos)
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

                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.gameObject.SetActive(true);
            }

            Instances.Add(obj);
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
        #endregion

        #region Functions
        private IEnumerator GenerateDocuments()
        {
            _isNotReady = true;
            yield return new WaitForSeconds(2);
            if (_isEnd)
            {
                _isEnd = false;
                _isNotReady = false;
                yield break;
            }
            
            _factoryController.ReGenerateDocument();
            _isNotReady = false;
        }
        
        private static void ExecuteObjBehavior(GameObject original, GameObject target, out int i)
        {
            var oType = original.GetComponent<EntityAttribute>().objectType;
            var tType = target.GetComponent<EntityAttribute>().objectType;
            i = -1;

            int index = CheckObjectType(oType, tType);
            if (index == 0)
                return;

            GameObject stamp, pos;
            switch (index)
            {
                //Green Stamp
                case 1:
                    stamp = target.GetComponent<LicenseInfo>().parts.Find(part => part.name == "I_Approved");
                    pos = GameObjFinder.FindChildGameObject(target, "Pos");
                    target.GetComponent<LicenseInfo>().isApproved = true;
                    target.GetComponent<LicenseInfo>().isStamped = true;
                    ClearChildren(pos);
                    Instantiate(stamp, pos.transform);
                    return;
                //Red Stamp
                case 2:
                    stamp = target.GetComponent<LicenseInfo>().parts.Find(part => part.name == "I_Rejected");
                    pos = GameObjFinder.FindChildGameObject(target, "Pos");
                    target.GetComponent<LicenseInfo>().isApproved = false;
                    target.GetComponent<LicenseInfo>().isStamped = true;
                    ClearChildren(pos);
                    Instantiate(stamp, pos.transform);
                    return;
                //CollectBox
                case 3:
                    if(!target.GetComponent<LicenseInfo>().isStamped)
                        break;

                    i = target.GetComponent<LicenseInfo>().isApproved ? 0 : 1;
                    target.GetComponent<LicenseInfo>().isStamped = false;
                    target.GetComponent<LicenseInfo>().isApproved = false;
                    pos = GameObjFinder.FindChildGameObject(target, "Pos");
                    ClearChildren(pos);
                    break;
            }
        }
        
        private static int CheckObjectType(ObjectType original, ObjectType target)
        {
            switch (original)
            {
                case ObjectType.GreenStamp:
                    if (target == ObjectType.License)
                        return 1;
                    break;
                case ObjectType.RedStamp:
                    if (target == ObjectType.License)
                        return 2;
                    break;
                case ObjectType.CollectBox:
                    if (target == ObjectType.License)
                        return 3;
                    break;
                case ObjectType.None:
                case ObjectType.License:
                default:
                    return 0;
            }

            return 0;
        }

        private static void ClearChildren(GameObject obj)
        {
            int i = 0;
            var allChild = new GameObject[obj.transform.childCount];

            foreach (Transform child in obj.transform)
            {
                allChild[i] = child.gameObject;
                i++;
            }

            foreach (var child in allChild)
            {
                Destroy(child.gameObject);
            }
        }
        
        public void ProcessCollision(GameObject original, GameObject col)
        {
            if (col == null)
                return;

            if (original.GetComponent<EasingControl>() || col.GetComponent<EasingControl>())
                return;

            ExecuteObjBehavior(original, col, out _index);
            if (_index == -1 || _isNotReady)
                return;

            var p = col.GetComponent<Poolable>();
            EnqueueObject(p);
            _dragAndDropController.TargetObj = null;
            DequeueObject("License", "LicensePos");
            LicenseSubmittedEvent?.Invoke(this, new InfoEventArgs<int>(_index));
        }

        public void ScaleDocument()
        {
            var obj = _dragAndDropController.LastObj;
            if (obj == null)
                return;
            if (!obj.GetComponent<EntityAttribute>().isDocument)
                return;
            if (_lastObj == null)
                _lastObj = obj;

            _isScaled = !_isScaled;
            if (_lastObj != obj)
            {
                _lastObj.transform.localScale = new Vector3(0.3f, 0.3f, 0);
                _lastObj.GetComponent<SortingGroup>().sortingLayerName = "Default";
                _lastObj = obj;
            }

            var sg = _lastObj.GetComponent<SortingGroup>();
            if (_isScaled)
            {
                _lastObj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
                sg.sortingLayerName = "SelectedObjects";
            }
            else
            {
                _lastObj.transform.localScale = new Vector3(0.3f, 0.3f, 0);
                sg.sortingLayerName = "Default";
            }
        }
        
        public string GetGeneratedID()
        {
            return _factoryController.generatedID;
        }

        public void ReGenerateDocument()
        {
            _reGenerateDocument = GenerateDocuments();
            StartCoroutine(_reGenerateDocument);
            _reGenerateDocument = null;
        }

        public void StopProcess()
        {
            _isEnd = true;
            if (_reGenerateDocument == null)
                return;
            StopCoroutine(_reGenerateDocument);
            _reGenerateDocument = null;
        }
        #endregion

        #region Release Methods
        public void Release()
        {
            for (int i = Instances.Count - 1; i >= 0; --i)
                GameObjectPoolSubController.Enqueue(Instances[i]);
            Instances.Clear();
        }
        #endregion
    }
}
