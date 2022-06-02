using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Common.Animation;
using Game.Scripts.Controller.SubController;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.TempCode;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controller
{
    //TODO: Store correct and wrong number of document
    public class ObjectController : MonoBehaviour
    {
        [SerializeField] private Button scalerButton;

        private FactorySubController _factorySubController;
        private DragAndDropSubController _dragAndDropSubController;
        private GameObject _lastObj;
        private IEnumerator _reGenerateDocument;
        private readonly List<GameObject> _documents = new List<GameObject>();
        private static readonly List<Poolable> Instances = new List<Poolable>();
        private List<RecipeData> _recipeDataList;
        private ScoreData _scoreData;
        private bool _isScaled, _isNotReady;

        public static event EventHandler<InfoEventArgs<sbyte>> LicenseSubmittedEvent;

        private void Awake()
        {
            enabled = false;
        }

        private void Start()
        {
            _dragAndDropSubController = GetComponent<DragAndDropSubController>();
        }

        #region Initiation
        //TODO: Load next level data.
        public void InitFactory(List<RecipeData> documentList, ScoreData scoreData)
        {
            var instance = gameObject.GetComponentInChildren<FactorySubController>();
            if (instance)
            {
                _factorySubController = instance;
                return;
            }

            _factorySubController = gameObject.AddComponent<FactorySubController>();
            _recipeDataList = new List<RecipeData>(documentList);
            _scoreData = scoreData;
            ProduceDocument(_recipeDataList, _scoreData.scoreContents);
        }

        public void InitObjectPool(List<FunctionalObjectsData> objectList)
        {
            if (objectList == null)
                return;

            foreach (var data in objectList)
            {
                GameObjectPoolSubController.AddEntry(data.key, data.prefab, data.amount, data.maxAmount);
                DequeueObject(data.key, data.spawnPosition);
            }
        }
        #endregion

        #region Factory Methods
        private void ProduceDocument(List<RecipeData> list, IReadOnlyList<ScoreContent> scoreData)
        {
            DrawID:
            int index = Random.Range(0, scoreData.Count);
            string id = scoreData[index].id;
            //Debug.Log(id);

            if (id == _factorySubController.generatedID)
            {
                Debug.Log($"Same ID result: {id}, Previous ID: {_factorySubController.generatedID}. Redraw.");
                goto DrawID;
            }

            foreach (var recipeData in list)
            {
                var obj = _factorySubController.ProduceDocument(recipeData.documentRecipeType, recipeData.documentRecipeName, id);
                //TODO: Make animation
                float x = Random.Range(-3, 3);
                float y = Random.Range(-2, 2);
                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.SetActive(true);
                _documents.Add(obj);
            }
        }

        public void ReleaseDocuments()
        {
            for (int i = _documents.Count - 1; i >= 0; --i)
                Destroy(_documents[i]);
            _documents.Clear();
        }

        public void Release()
        {
            _recipeDataList.Clear();
            _scoreData = null;
        }
        #endregion

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

        public void ReleaseInstances()
        {
            for (int i = Instances.Count - 1; i >= 0; --i)
                GameObjectPoolSubController.Enqueue(Instances[i]);
            Instances.Clear();
        }
        #endregion

        #region Functions
        private IEnumerator GenerateDocuments()
        {
            _isNotReady = true;
            yield return new WaitForSeconds(2);
            ProduceDocument(_recipeDataList, _scoreData.scoreContents);
            _isNotReady = false;
        }
        
        public void ProcessCollision(GameObject original, GameObject col)
        {
            if (col == null)
                return;

            if (original.GetComponent<EasingControl>() || col.GetComponent<EasingControl>())
                return;

            sbyte index = InteractionSubController.ExecuteObjBehavior(original, col);

            if (index == 0 || _isNotReady)
                return;

            var p = col.GetComponent<Poolable>();
            EnqueueObject(p);
            _dragAndDropSubController.TargetObj = null;
            DequeueObject("License", "LicensePos");
            LicenseSubmittedEvent?.Invoke(this, p.GetComponent<LicenseInfo>().isApproved ? new InfoEventArgs<sbyte>(0) : new InfoEventArgs<sbyte>(1));
        }

        public void ScaleDocument()
        {
            var obj = _dragAndDropSubController.LastObj;
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
                scalerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Put down";
                _lastObj.transform.localScale = new Vector3(0.5f, 0.5f, 0);
                sg.sortingLayerName = "SelectedObjects";
            }
            else
            {
                scalerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pick up";
                _lastObj.transform.localScale = new Vector3(0.3f, 0.3f, 0);
                sg.sortingLayerName = "Default";
            }
        }
        
        public string GetGeneratedID()
        {
            return _factorySubController.generatedID;
        }

        public void ReGenerateDocument()
        {
            _reGenerateDocument = GenerateDocuments();
            StartCoroutine(_reGenerateDocument);
            _reGenerateDocument = null;
        }

        public void StopProcess()
        {
            if (_reGenerateDocument == null)
                return;
            StopCoroutine(_reGenerateDocument);
            _reGenerateDocument = null;
        }
        #endregion
    }
}
