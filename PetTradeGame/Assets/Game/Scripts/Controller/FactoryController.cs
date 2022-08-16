using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.EventArguments;
using Game.Scripts.Factory;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controller
{
    public class FactoryController : MonoBehaviour
    {
        [SerializeField] private DocumentFactory documentFactory;

        [HideInInspector] public string generatedID;
        private readonly List<GameObject> _documents = new List<GameObject>();

        private List<RecipeData> _recipeDataList;
        private List<ScoreData> _scoreData;

        private void Start()
        {
            DocumentFactory.OnDocumentCreated += OnDocumentCreated;
        }

        private void OnDisable()
        {
            DocumentFactory.OnDocumentCreated -= OnDocumentCreated;
        }

        //Draw a random number from the scoreData list. If the id is same as the last one, draw again until it is different.
        //Then load all the recipe data from the list and call the factory to create the document
        private IEnumerator ProduceDocument()
        {
            var random = Random.Range(0, _scoreData.Count);
            var id = _scoreData[random].id;
            while (id == generatedID)
            {
                Debug.Log($"Same ID result: {id}. Redraw.");
                random = Random.Range(0, _scoreData.Count);
                id = _scoreData[random].id;
            }
            yield return null;

            generatedID = id;
            Debug.Log(id);
            foreach (RecipeData recipeData in _recipeDataList)
            {
                documentFactory.CreateDocument(recipeData, id);
                yield return null;
            }
        }

        private void OnDocumentCreated(object sender, InfoEventArgs<GameObject> e)
        {
            GameObject obj = e.info;
            if (obj == null) 
                return;

            //Set up documents' transform and add it to the list
            //TODO: Make animation
            float x = Random.Range(-3, 3);
            float y = Random.Range(-2, 2);
            obj.SetActive(true);
            obj.transform.localPosition = new Vector3(x, y, 0);
            _documents.Add(obj);
            //Debug.Log(_documents.Count);
        }

        public void InitFactory(IEnumerable<RecipeData> documentList, IEnumerable<ScoreData> scoreData)
        {
            _recipeDataList = new List<RecipeData>(documentList);
            _scoreData = new List<ScoreData>(scoreData);

            StartCoroutine(ProduceDocument());
        }

        public void ReGenerateDocument()
        {
            foreach (GameObject document in _documents)
            {
                document.SetActive(false);
                Destroy(document);
            }

            _documents.Clear();
            StartCoroutine(ProduceDocument());
        }

        public void Release()
        {
            StopCoroutine(ProduceDocument());
            for (var i = _documents.Count - 1; i >= 0; --i)
                Destroy(_documents[i]);
            _documents.Clear();
            documentFactory.ReleaseRecipe();
        }

        public void ReleaseDocument()
        {
            StopCoroutine(ProduceDocument());
            for (var i = _documents.Count - 1; i >= 0; --i)
                Destroy(_documents[i]);
            _documents.Clear();
            documentFactory.ReleaseObject();
        }
    }
}