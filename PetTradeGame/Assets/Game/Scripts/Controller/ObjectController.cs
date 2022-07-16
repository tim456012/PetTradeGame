using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

namespace Game.Scripts.Controller
{
    public class ObjectController : MonoBehaviour
    {
        #region Field

        private static readonly Dictionary<string, GameObject> Instances = new Dictionary<string, GameObject>();
        
        private List<FunctionalObjectsData> _objectList = new List<FunctionalObjectsData>();
        private FactoryController _factoryController;
        private DragAndDropController _dragAndDropController;
        private GameObject _lastObj;
        private IEnumerator _reGenerateDocument, _reGenerateLicense;
        private bool _isScaled, _isNotReady, _isEnd;
        private int _index;

        public static event EventHandler<InfoEventArgs<int>> LicenseSubmittedEvent;

        #endregion

        private void Awake()
        {
            _factoryController = GetComponent<FactoryController>();
            _dragAndDropController = GetComponent<DragAndDropController>();

            _factoryController.enabled = false;
            _dragAndDropController.enabled = false;
            enabled = false;
        }

        public async void InitFunctionalObject(List<FunctionalObjectsData> objectList)
        {
            if (objectList == null)
                return;

            _objectList = objectList;
            foreach (FunctionalObjectsData data in objectList)
            {
                GameObject prefabAsset = await data.prefab.LoadAssetAsync<GameObject>().Task;
                Transform p = GameObjFinder.FindChildGameObject(gameObject, data.spawnPosition).transform;
                
                prefabAsset.transform.localPosition = p.position;
                Instantiate(prefabAsset, p.position, Quaternion.identity);
                Instances.Add(data.key, prefabAsset);
                prefabAsset.SetActive(true);
                Addressables.Release(prefabAsset);
            }
        }

        public void Init()
        {
            enabled = true;
            _factoryController.enabled = true;
            _dragAndDropController.enabled = true;
        }

        #region Release Methods

        public void Release()
        {
            foreach (var instance in Instances)
            {
                instance.Value.SetActive(false);
                Destroy(instance.Value);
                Instances.Remove(instance.Key);
            }
            Instances.Clear();
        }

        #endregion

        public void ReGenerateDocument()
        {
            _reGenerateDocument = GenerateDocuments();
            StartCoroutine(_reGenerateDocument);
            _reGenerateDocument = null;
        }

        public async void ReGenerateLicense()
        {
            foreach (FunctionalObjectsData data in _objectList)
            {
                if (data.key != "License")
                    continue;

                GameObject prefabAsset = await data.prefab.LoadAssetAsync<GameObject>().Task;
                Transform p = GameObjFinder.FindChildGameObject(gameObject, data.spawnPosition).transform;

                prefabAsset.transform.localPosition = p.position;
                Instantiate(prefabAsset, p.position, Quaternion.identity);
                Instances.Add(data.key, prefabAsset);
                prefabAsset.gameObject.SetActive(true);
                Addressables.Release(prefabAsset);
            }
        }

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
        
        private void DestroyLicense(GameObject target)
        {
            InputController.IsDragActive = false;
            if (target == null)
                return;

            Instances.Remove("License");
            target.SetActive(false);
            Debug.Log("Disable");
            Destroy(target);
        }
        
        #region Functions
        
        private static void ExecuteObjBehavior(GameObject original, GameObject target, out int i)
        {
            ObjectType oType = original.GetComponent<EntityAttribute>().objectType;
            ObjectType tType = target.GetComponent<EntityAttribute>().objectType;
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
                    if (!target.GetComponent<LicenseInfo>().isStamped)
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

        public void ProcessCollision(GameObject original, GameObject col)
        {
            if (col == null)
                return;

            if (original.GetComponent<EasingControl>() || col.GetComponent<EasingControl>())
                return;

            ExecuteObjBehavior(original, col, out _index);
            if (_index == -1 || _isNotReady)
                return;

            DestroyLicense(col);
            _dragAndDropController.TargetObj = null;
            LicenseSubmittedEvent?.Invoke(this, new InfoEventArgs<int>(_index));
        }

        public void ScaleDocument()
        {
            GameObject obj = _dragAndDropController.LastObj;
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

        //Need Optimize
        public string GetGeneratedID()
        {
            return _factoryController.generatedID;
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
    }
}