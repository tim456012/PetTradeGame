using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.Scripts.Factory
{
    public static class DocumentFactory
    {
        public static event EventHandler<InfoEventArgs<GameObject>> OnDocumentCreated; 

        private static GameObject _prefabResult;
        
        #region Communicate Interface
        public static void CreateDocument(DocumentType documentType, AssetReference recipe, string id)
        {
            switch (documentType)
            {
                case DocumentType.PossessionLicense:
                    LoadPossessionLicense(recipe, id);
                    break;
                case DocumentType.HealthCertificate:
                    LoadHealthCertificate(recipe, id);
                    break;
                case DocumentType.SpecialPermit:
                    LoadSpecialPermit(recipe, id);
                    break;
                case DocumentType.DealerLicense:
                    LoadDealerLicense(recipe, id);
                    break;
                case DocumentType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(documentType), documentType, null);
            }
        }
        
        private static void CreateDealerLicense(DealerLicenseRecipe dl, GameObject obj, string id)
        {
            if (dl.dealerLicenseData.All(data => data.animalId != id))
            {
                Debug.Log("No dealer license for this id found.");
                return;
            }

            Debug.Log("Start creating dealer license");
            GameObject doc = Object.Instantiate(obj);

            doc.name = dl.name;
            doc.GetComponent<EntityAttribute>().paperType = DocumentType.DealerLicense;
            
            AddDealerLicenseContent(doc, dl.dealerLicenseData, id);
            OnDocumentCreated?.Invoke(null, new InfoEventArgs<GameObject>(doc));
        }
        
        private static void CreateHealthCertificate(HealthCertificateRecipe hc, GameObject obj, string id)
        {
            if (hc.healthCertificateData.All(data => data.animalId != id))
            {
                Debug.Log("No health certificate for this id found");
                return;
            }
            
            Debug.Log("Start creating health certificate");
            GameObject doc = Object.Instantiate(obj);

            doc.name = hc.name;
            doc.GetComponent<EntityAttribute>().paperType = DocumentType.HealthCertificate;
            AddHealthCertificateContent(doc, hc.healthCertificateData, id);
            OnDocumentCreated?.Invoke(null, new InfoEventArgs<GameObject>(doc));
        }

        private static void CreatePossessionLicense(PossessionLicenseRecipe pl, GameObject obj, string id)
        {
            if (pl.possessionLicenseData.All(data => data.animalId != id))
            {
                Debug.Log("No possession license for this id found");
                return;
            }

            Debug.Log("Start creating possession license");
            GameObject doc = Object.Instantiate(obj);

            doc.name = pl.name;
            doc.GetComponent<EntityAttribute>().paperType = DocumentType.PossessionLicense;
            AddPossessionLicenseContent(doc, pl.possessionLicenseData, id);
            OnDocumentCreated?.Invoke(null, new InfoEventArgs<GameObject>(doc));
        }

        private static void CreateSpecialPermit(SpecialPermitRecipe sp, GameObject obj, string id)
        {
            if (sp.specialPermitData.All(data => data.animalId != id))
            {
                Debug.Log("No special permit for this id found");
                return;
            }

            Debug.Log("Start creating special permit");
            GameObject doc = Object.Instantiate(obj);

            doc.name = sp.name;
            doc.GetComponent<EntityAttribute>().paperType = DocumentType.SpecialPermit;
            AddSpecialPermitContent(doc, sp.specialPermitData, id);
            OnDocumentCreated?.Invoke(null, new InfoEventArgs<GameObject>(doc));
        }

        private static void CreateGameObject(GameObject obj, GameObject parent, string posName)
        {
            Transform parentTran = GameObjFinder.FindChildGameObject(parent, posName).transform;

            GameObject prefab = Object.Instantiate(obj, parentTran, true);
            //prefab.transform.localScale = Vector3.one;
            prefab.transform.localPosition = Vector3.zero;
        }

        #endregion

        #region Tasks

        private static async void LoadPossessionLicense(AssetReference recipe, string id)
        {
            PossessionLicenseRecipe pl = await recipe.Get<PossessionLicenseRecipe>().Task;
            GameObject obj = await "Documents/I_SpecialPermit".Get<GameObject>().Task;
            
            CreatePossessionLicense(pl, obj, id);
            Addressables.Release(pl);
            Addressables.Release(obj);
        }

        private static async void LoadSpecialPermit(AssetReference recipe, string id)
        {
            SpecialPermitRecipe sp = await recipe.Get<SpecialPermitRecipe>().Task;
            GameObject obj = await "Documents/I_SpecialPermit".Get<GameObject>().Task;
            CreateSpecialPermit(sp, obj ,id);
            Addressables.Release(sp);
            Addressables.Release(obj);
        }
        
        private static async void LoadHealthCertificate(AssetReference recipe, string id)
        {
            HealthCertificateRecipe hc = await recipe.Get<HealthCertificateRecipe>().Task;
            GameObject obj = await "Documents/I_HealthCertificate".Get<GameObject>().Task;
            CreateHealthCertificate(hc, obj, id);
            Addressables.Release(hc);
            Addressables.Release(obj);
        }
        
        private static async void LoadDealerLicense(AssetReference recipe, string id)
        {
            DealerLicenseRecipe dl = await recipe.Get<DealerLicenseRecipe>().Task;
            GameObject obj = await "Documents/I_DealerLicense".Get<GameObject>().Task;
            CreateDealerLicense(dl, obj, id);
            Addressables.Release(dl);
            Addressables.Release(obj);
        }
        
        private static async void LoadGameObject(string name, GameObject parent, string posName)
        {
            GameObject obj = await name.Get<GameObject>().Task;
            CreateGameObject(obj, parent, posName);
            Addressables.Release(obj);
        }

        #endregion

        #region Content

        private static void AddDealerLicenseContent(GameObject obj, List<DealerLicenseData> data, string id)
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

        private static void AddHealthCertificateContent(GameObject obj, List<HealthCertificateData> data, string id)
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

        private static void AddPossessionLicenseContent(GameObject obj, List<PossessionLicenseData> data, string id)
        {
            foreach (PossessionLicenseData possessionLicenseData in data)
            {
                if (possessionLicenseData.animalId != id)
                    continue;

                var licenseNumber = possessionLicenseData.licenseNumber;
                var deadline = possessionLicenseData.deadline;
                var name = possessionLicenseData.name;
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
                AddContentText(obj, "TM_Name", name.Count == 0 ? name[0] : name[index]);

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

        private static void AddSpecialPermitContent(GameObject obj, List<SpecialPermitData> data, string id)
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

        private static void AddContentText(GameObject obj, string objName, string name)
        {
            GameObject temp = GameObjFinder.FindChildGameObject(obj, objName);
            temp.GetComponent<TextMeshPro>().text = name;
        }

        private static void AddContentPrefab(GameObject obj, string pos, string name)
        {
            if (name == " ")
                return;

            var posName = CheckPosition(pos);
            if (posName == null)
                return;
            
            LoadGameObject($"Components/{name}", obj, posName);
        }

        private static void AddContentPrefabChoice(GameObject obj, string posName, string objName, bool isTick)
        {
            if(!isTick)
                return;
            
            AddContentPrefab(obj, posName, objName);
        }
        
        private static string CheckPosition(string pos)
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