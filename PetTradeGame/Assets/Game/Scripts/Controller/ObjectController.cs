using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Common.Animation;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace Game.Scripts.Controller
{
    public class ObjectController : MonoBehaviour
    {
        #region Field

        private readonly List<GameObject> _instances = new List<GameObject>();
        
        private List<FunctionalObjectsData> _objectList = new List<FunctionalObjectsData>();
        private readonly Dictionary<string, Sprite> _animalGuideList = new Dictionary<string, Sprite>();
        private FactoryController _factoryController;
        private DragAndDropController _dragAndDropController;
        private GameObject _lastObj;
        private IEnumerator _reGenerateDocument, _reGenerateLicense;
        private bool _isScaled, _isNotReady, _isEnd;
        private int _index;

        public static event EventHandler<InfoEventArgs<int>> LicenseSubmittedEvent;
        public static event EventHandler<InfoEventArgs<Sprite>> SetAnimalGuideEvent; 

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
            if (_isEnd)
                _isEnd = false;
            
            if (objectList == null)
                return;

            _objectList = objectList;
            foreach (FunctionalObjectsData data in objectList)
            {
                GameObject prefabAsset = await data.prefab.LoadAssetAsync<GameObject>().Task;
                Transform p = GameObjFinder.FindChildGameObject(gameObject, data.spawnPosition).transform;

                Vector3 position = p.position;
                prefabAsset.transform.localPosition = position;
                
                GameObject obj = Instantiate(prefabAsset, position, Quaternion.identity);
                _instances.Add(obj);
                obj.SetActive(true);
                Addressables.Release(prefabAsset);
            }
        }

        public void Init()
        {
            enabled = true;
            _factoryController.enabled = true;
            _dragAndDropController.enabled = true;
        }
        
        public void Release()
        {
            _index = 0;
            _isNotReady = false;
            
            for (var i = _instances.Count - 1; i >= 0; --i)
            {
                Destroy(_instances[i].gameObject);
                _instances.RemoveAt(i);
            }
            
            _instances.Clear();
            _animalGuideList.Clear();
        }
        
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

                Vector3 position = p.position;
                prefabAsset.transform.localPosition = position;
                
                GameObject obj = Instantiate(prefabAsset, position, Quaternion.identity);
                _instances.Add(obj);
                obj.gameObject.SetActive(true);
                Addressables.Release(prefabAsset);
            }
        }

        private IEnumerator GenerateDocuments()
        {
            _isNotReady = true;
            yield return new WaitForSeconds(1f);
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

            _instances.Remove(target);
            target.SetActive(false);
            Destroy(target);
        }
        
        #region Functions
        
        private void ExecuteObjBehavior(GameObject original, GameObject target, out int i)
        {
            ObjectType oType = original.GetComponent<EntityAttribute>().objectType;
            ObjectType tType = target.GetComponent<EntityAttribute>().objectType;
            i = -1;
            if (_isEnd)
            {
                i = 0;
                return;
            }
            
            var index = CheckObjectType(oType, tType);
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
            if (_index == -1 || _isNotReady || _isEnd)
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
        
        public void StopProcess()
        {
            _isEnd = true;
            if (_reGenerateDocument == null)
                return;
            StopCoroutine(_reGenerateDocument);
            _reGenerateDocument = null;
        }

        public void InitAnimalGuide(List<Sprite> guide)
        {
            foreach (Sprite sprite in guide)
            {
                var spriteName = sprite.name;
                _animalGuideList.Add(spriteName, sprite);
            }
        }
        
        public void GetAnimalGuide()
        { 
            var id = _factoryController.generatedID[..2];
            if (!_animalGuideList.TryGetValue(id, out Sprite sprite))
            {
                Debug.Log($"No animal guide found: {id}");
                return;
            }
            
            SetAnimalGuideEvent?.Invoke(this, new InfoEventArgs<Sprite>(sprite));
        }

        #endregion
    }
}