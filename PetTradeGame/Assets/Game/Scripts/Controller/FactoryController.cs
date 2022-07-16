using System.Collections.Generic;
using Game.Scripts.Factory;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller
{
    public class FactoryController : MonoBehaviour
    {

        public string generatedID;
        private readonly List<GameObject> _documents = new List<GameObject>();

        private List<RecipeData> _recipeDataList;
        private List<ScoreData> _scoreData;

        private void ProduceDocument(List<RecipeData> list, IReadOnlyList<ScoreData> scoreData)
        {
            DrawID:
            int index = Random.Range(0, scoreData.Count);
            string id = scoreData[index].id;
            Debug.Log($"ID of this documents: {id}");

            if (id == generatedID)
            {
                //Debug.Log($"Same ID result: {id}, Previous ID: {_factorySubController.generatedID}. Redraw.");
                goto DrawID;
            }

            foreach (RecipeData recipeData in list)
            {
                GameObject obj = ProduceDocument(recipeData.documentRecipeType, recipeData.documentRecipeName, id);
                if (obj == null)
                    continue;

                //TODO: Make animation
                float x = Random.Range(-3, 3);
                float y = Random.Range(-2, 2);
                obj.transform.localPosition = new Vector3(x, y, 0);
                obj.SetActive(true);
                _documents.Add(obj);
            }
        }

        private GameObject ProduceDocument(string documentType, string documentName, string id)
        {
            generatedID = id;

            GameObject obj = documentType switch
            {
                "DealerLicenseRecipe" => DocumentFactory.CreateDealerLicense(documentName, id),
                "HealthCertificateRecipe" => DocumentFactory.CreateHealthCertificate(documentName, id),
                "PossessionLicenseRecipe" => DocumentFactory.CreatePossessionLicense(documentName, id),
                "SpecialPermitRecipe" => DocumentFactory.CreateSpecialPermit(documentName, id),
                _ => null
            };

            return obj;
        }

        public void InitFactory(List<RecipeData> documentList, List<ScoreData> scoreData)
        {
            _recipeDataList = new List<RecipeData>(documentList);
            _scoreData = new List<ScoreData>(scoreData);

            ProduceDocument(_recipeDataList, _scoreData);
        }

        public void ReGenerateDocument()
        {
            foreach (GameObject document in _documents)
            {
                document.SetActive(false);
            }

            _documents.Clear();
            ProduceDocument(_recipeDataList, _scoreData);
        }

        public void Release()
        {
            for (int i = _documents.Count - 1; i >= 0; --i)
                Destroy(_documents[i]);
            _documents.Clear();
        }
    }
}