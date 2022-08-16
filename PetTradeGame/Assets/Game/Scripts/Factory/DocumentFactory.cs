using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Game.Scripts.Factory
{
    public class DocumentFactory : MonoBehaviour
    {
        public static event EventHandler<InfoEventArgs<GameObject>> OnDocumentCreated; 

        private GameObject _prefabResult;
        private readonly List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();
        private readonly List<AsyncOperationHandle> _objectHandles = new List<AsyncOperationHandle>();

        #region Communicate Interface
        public void CreateDocument(RecipeData recipeData, string id)
        {
            StartCoroutine(CreatDocumentObject(recipeData.documentRecipe, id, recipeData.documentRecipeType));
        }
        
        private IEnumerator CreatDocumentObject(AssetReference recipe, string id, DocumentType documentType = DocumentType.None)
        {
            AsyncOperationHandle recipeHandle;
            string key;
            switch (documentType)
            {
                case DocumentType.PossessionLicense:
                    recipeHandle = recipe.Get<PossessionLicenseRecipe>();
                    key = "Documents/I_PossessionLicense";
                    break;
                case DocumentType.HealthCertificate:
                    recipeHandle = recipe.Get<HealthCertificateRecipe>();
                    key = "Documents/I_HealthCertificate";
                    break;
                case DocumentType.SpecialPermit:
                    recipeHandle = recipe.Get<SpecialPermitRecipe>();
                    key = "Documents/I_SpecialPermit";
                    break;
                case DocumentType.DealerLicense:
                    recipeHandle = recipe.Get<DealerLicenseRecipe>();
                    key = "Documents/I_DealerLicense";
                    break;
                case DocumentType.None:
                default:
                    yield break;
            }
            
            if(!recipeHandle.IsDone)
                yield return recipeHandle;

            if (recipeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Recipe is not valid");
                Addressables.Release(recipeHandle);
                yield break;
            }

            _handles.Add(recipeHandle);
            var obj = key.Get<GameObject>();
            if(!obj.IsDone)
                yield return obj;

            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Object is not valid");
                Addressables.Release(obj);
                yield break;
            }

            _objectHandles.Add(obj);
            GameObject doc = Instantiate(obj.Result);
            doc.name = documentType.ToString();
            doc.GetComponent<EntityAttribute>().paperType = documentType;
            yield return doc;
            
            Coroutine t = StartCoroutine(CreateDocument(doc, documentType, recipeHandle.Result, id));
            yield return t;
        }

        private IEnumerator CreateDocument(GameObject obj, DocumentType documentType, object data, string id)
        {
            if (data == null)
            {
                yield break;
            }
            
            switch (documentType)
            {
                case DocumentType.PossessionLicense:
                    var pl = data as PossessionLicenseRecipe;
                    if (pl!.possessionLicenseData.All(x => x.animalId != id))
                    {
                        Debug.Log("No possession license for this id found");
                        break;
                    }
                    AddPossessionLicenseContent(obj, pl!.possessionLicenseData, id);
                    break;
                case DocumentType.HealthCertificate:
                    var hc = data as HealthCertificateRecipe;
                    if (hc!.healthCertificateData.All(x => x.animalId != id))
                    {
                        Debug.Log("No health certificate for this id found");
                        break;
                    }
                    AddHealthCertificateContent(obj, hc!.healthCertificateData, id);
                    break;
                case DocumentType.SpecialPermit:
                    var sp = data as SpecialPermitRecipe;
                    if (sp!.specialPermitData.All(x => x.animalId != id))
                    {
                        Debug.Log("No special permit for this id found");
                        break;
                    }
                    AddSpecialPermitContent(obj, sp!.specialPermitData, id);
                    break;
                case DocumentType.DealerLicense:
                    var dl = data as DealerLicenseRecipe;
                    if (dl!.dealerLicenseData.All(x => x.animalId != id))
                    {
                        Debug.Log("No dealer license for this id found");
                        break;
                    }
                    AddDealerLicenseContent(obj, dl!.dealerLicenseData, id);
                    break;
                case DocumentType.None:
                default:
                    yield break;
            }

            yield return null;
            OnDocumentCreated?.Invoke(this, new InfoEventArgs<GameObject>(obj));
        }
        
        private IEnumerator CreateGameObject(string key, GameObject parent, string posName)
        {
            var handle = key.Get<GameObject>();
            if(!handle.IsDone)
                yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Object is not valid");
                Addressables.Release(handle);
                yield break;
            }
            
            _objectHandles.Add(handle);
            Transform parentTran = GameObjFinder.FindChildGameObject(parent, posName).transform;
            GameObject prefab = Instantiate(handle.Result, parentTran, true);
            //prefab.transform.localScale = Vector3.one;
            prefab.transform.localPosition = Vector3.zero;
        }
        
        #endregion

        public void ReleaseRecipe()
        {
            if (_handles.Count > 0)
            {
                for(var i = _handles.Count - 1; i >= 0; --i)
                {
                    AsyncOperationHandle temp = _handles[i];
                    Addressables.Release(temp);
                    _handles.RemoveAt(i);
                }
                _handles.Clear();
            }
            else
                Debug.Log("Nothing to release");
        }

        public void ReleaseObject()
        {
            if (_objectHandles.Count > 0)
            {
                for(var i = _objectHandles.Count - 1; i >= 0; --i)
                {
                    AsyncOperationHandle temp = _objectHandles[i];
                    Addressables.Release(temp);
                    _objectHandles.RemoveAt(i);
                }
                _objectHandles.Clear();
            }
            else
                Debug.Log("Nothing to release");
        }

        #region Content

        private void AddDealerLicenseContent(GameObject obj, List<DealerLicenseData> data, string id)
        {
            foreach (DealerLicenseData dealerLicenseData in data)
            {
                if (dealerLicenseData.animalId != id)
                    continue;

                var businessName = dealerLicenseData.businessName;
                var businessNumber = dealerLicenseData.businessNumber;
                var stampSign = dealerLicenseData.stampSign;

                var index = Random.Range(0, businessName.Count);
                AddContentText(obj, "TM_BusinessName", businessName.Count == 0 ? businessName[0] : businessName[index]);

                index = Random.Range(0, businessNumber.Count);
                AddContentText(obj, "TM_BusinessNumber", businessNumber.Count == 0 ? businessNumber[0] : businessNumber[index]);

                AddContentPrefab(obj, dealerLicenseData.line1Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.line2Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.line3Position.ToString(), "I_Tick");
                AddContentPrefab(obj, dealerLicenseData.line4Position.ToString(), "I_Tick");
                AddContentPrefabChoice(obj, "TickPos7", "I_Tick", dealerLicenseData.isProcess);
                AddContentPrefabChoice(obj, "TickPos8", "I_Tick", dealerLicenseData.isTick);

                index = Random.Range(0, dealerLicenseData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", stampSign.Count == 0 ? stampSign[0] : stampSign[index]);
            }
        }

        private void AddHealthCertificateContent(GameObject obj, List<HealthCertificateData> data, string id)
        {
            foreach (HealthCertificateData healthCertificateData in data)
            {
                if (healthCertificateData.animalId != id)
                    continue;

                var animalName = healthCertificateData.animalName;
                var animalMark = healthCertificateData.animalMark;
                var date = healthCertificateData.date;
                var stampSign = healthCertificateData.stampSign;

                AddContentPrefabChoice(obj, "TickPos1", "I_Tick", healthCertificateData.isOfficial);
                AddContentPrefabChoice(obj, "TickPos2", "I_Tick", healthCertificateData.isLocal);

                var index = Random.Range(0, healthCertificateData.animalName.Count);
                AddContentText(obj, "TM_AnimalName", animalName.Count == 0 ? animalName[0] : animalName[index]);

                index = Random.Range(0, healthCertificateData.animalMark.Count);
                AddContentText(obj, "TM_AnimalMark", animalMark.Count == 0 ? animalMark[0] : animalMark[index]);

                index = Random.Range(0, healthCertificateData.date.Count);
                AddContentText(obj, "TM_Date", date.Count == 0 ? date[0] : date[index]);

                index = Random.Range(0, healthCertificateData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", stampSign.Count == 0 ? stampSign[0] : stampSign[index]);
            }
        }

        private void AddPossessionLicenseContent(GameObject obj, List<PossessionLicenseData> data, string id)
        {
            foreach (PossessionLicenseData possessionLicenseData in data)
            {
                if (possessionLicenseData.animalId != id)
                    continue;

                var licenseNumber = possessionLicenseData.licenseNumber;
                var deadline = possessionLicenseData.deadline;
                var applicantName = possessionLicenseData.name;
                var bId = possessionLicenseData.id;
                var businessNumber = possessionLicenseData.businessNumber;
                var animalName = possessionLicenseData.animalName;
                var contract = possessionLicenseData.contract;
                var objective = possessionLicenseData.objective;
                var original = possessionLicenseData.original;
                var stampSign = possessionLicenseData.stampSign;

                var index = Random.Range(0, possessionLicenseData.licenseNumber.Count);
                AddContentText(obj, "TM_LicenseNumber", licenseNumber.Count == 0 ? licenseNumber[0] : licenseNumber[index]);

                index = Random.Range(0, possessionLicenseData.deadline.Count);
                AddContentText(obj, "TM_Deadline", deadline.Count == 0 ? deadline[0] : deadline[index]);

                index = Random.Range(0, possessionLicenseData.name.Count);
                AddContentText(obj, "TM_Name", applicantName.Count == 0 ? applicantName[0] : applicantName[index]);

                index = Random.Range(0, possessionLicenseData.id.Count);
                AddContentText(obj, "TM_ID", bId.Count == 0 ? bId[0] : bId[index]);

                index = Random.Range(0, possessionLicenseData.businessNumber.Count);
                AddContentText(obj, "TM_BusinessNumber", businessNumber.Count == 0 ? businessNumber[0] : businessNumber[index]);

                index = Random.Range(0, possessionLicenseData.animalName.Count);
                AddContentText(obj, "TM_AnimalName", animalName.Count == 0 ? animalName[0] : animalName[index]);

                index = Random.Range(0, possessionLicenseData.contract.Count);
                AddContentText(obj, "TM_Contract", contract.Count == 0 ? contract[0] : contract[index]);

                index = Random.Range(0, possessionLicenseData.objective.Count);
                AddContentText(obj, "TM_Objective", objective.Count == 0 ? objective[0] : objective[index]);

                index = Random.Range(0, possessionLicenseData.original.Count);
                AddContentText(obj, "TM_Original", original.Count == 0 ? original[0] : original[index]);

                index = Random.Range(0, possessionLicenseData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", stampSign.Count == 0 ? stampSign[0] : stampSign[index]);
            }
        }

        private void AddSpecialPermitContent(GameObject obj, List<SpecialPermitData> data, string id)
        {
            foreach (SpecialPermitData specialPermitData in data)
            {
                if (specialPermitData.animalId != id)
                    continue;

                var locationName = specialPermitData.locationName;
                var objective = specialPermitData.objective;
                var businessNumber = specialPermitData.businessNumber;
                var deadline = specialPermitData.deadline;
                var animalName = specialPermitData.animalName;
                var animalCount = specialPermitData.animalCount;
                var animalFeature = specialPermitData.animalFeature;
                var stampSign = specialPermitData.stampSign;

                var index = Random.Range(0, specialPermitData.locationName.Count);
                AddContentText(obj, "TM_LocationName", locationName.Count == 0 ? locationName[0] : locationName[index]);

                AddContentPrefab(obj, specialPermitData.line1Position.ToString(), "I_DocumentCircle");

                index = Random.Range(0, specialPermitData.objective.Count);
                AddContentText(obj, "TM_Objective", objective.Count == 0 ? objective[0] : objective[index]);

                index = Random.Range(0, specialPermitData.businessNumber.Count);
                AddContentText(obj, "TM_BusinessNumber", businessNumber.Count == 0 ? businessNumber[0] : businessNumber[index]);

                index = Random.Range(0, specialPermitData.deadline.Count);
                AddContentText(obj, "TM_Deadline", deadline.Count == 0 ? deadline[0] : deadline[index]);

                index = Random.Range(0, specialPermitData.animalName.Count);
                AddContentText(obj, "TM_AnimalName", animalName.Count == 0 ? animalName[0] : animalName[index]);

                index = Random.Range(0, specialPermitData.animalCount.Count);
                AddContentText(obj, "TM_AnimalCount", animalCount.Count == 0 ? animalCount[0] : animalCount[index]);

                index = Random.Range(0, specialPermitData.animalFeature.Count);
                AddContentText(obj, "TM_AnimalFeature", animalFeature.Count == 0 ? animalFeature[0] : animalFeature[index]);

                index = Random.Range(0, specialPermitData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", stampSign.Count == 0 ? stampSign[0] : stampSign[index]);
            }
        }

        private void AddContentText(GameObject obj, string objName, string name)
        {
            GameObject temp = GameObjFinder.FindChildGameObject(obj, objName);
            temp.GetComponent<TextMeshPro>().text = name;
        }

        private void AddContentPrefab(GameObject obj, string pos, string name)
        {
            if (name == " ")
                return;

            var posName = CheckPosition(pos);
            if (posName == null)
                return;
            
            StartCoroutine(CreateGameObject($"Components/{name}", obj, posName));
        }

        private void AddContentPrefabChoice(GameObject obj, string posName, string objName, bool isTick)
        {
            if(!isTick)
                return;
            
            AddContentPrefab(obj, posName, objName);
        }
        
        private string CheckPosition(string pos)
        {
            return pos switch
            {
                "None" => null,
                "CirclePos1" => "CirclePos_1",
                "CirclePos2" => "CirclePos_2",
                "CirclePos3" => "CirclePos_3",
                "CirclePos4" => "CirclePos_4",
                "CirclePos5" => "CirclePos_5",
                "TickPos1" => "TickPos_1",
                "TickPos2" => "TickPos_2",
                "TickPos3" => "TickPos_3",
                "TickPos4" => "TickPos_4",
                "TickPos5" => "TickPos_5",
                "TickPos6" => "TickPos_6",
                "TickPos7" => "TickPos_7",
                "TickPos8" => "TickPos_8",
                "StampSignPos" => "StampSignPos",
                _ => null
            };
        }

        #endregion
    }
}