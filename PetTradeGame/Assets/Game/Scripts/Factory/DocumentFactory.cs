using System.Collections.Generic;
using Game.Scripts.Enum;
using Game.Scripts.Model;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Factory
{
    public static class DocumentFactory
    {
        #region Communicate Interface
        /// <summary>
        /// Create Dealer License by loading it recipe.
        /// </summary>
        /// <param name="name">Recipe name of Dealer License</param>
        /// <returns>GameObject</returns>
        public static GameObject CreateDealerLicense(string name, string id)
        {
            var obj = Resources.Load<DealerLicenseRecipe>($"Recipes/{name}");
            if (obj != null)
                return CreateDealerLicense(obj, id);
            Debug.Log("No Document Recipe found");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject CreateHealthCertificate(string name, string id)
        {
            var obj = Resources.Load<HealthCertificateRecipe>($"Recipes/{name}");
            if (obj != null)
                return CreateHealthCertificate(obj, id);
            Debug.Log("No Document Recipe found");
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject CreatePossessionLicense(string name, string id)
        {
            var obj = Resources.Load<PossessionLicenseRecipe>($"Recipes/{name}");
            if (obj != null)
                return CreatePossessionLicense(obj, id);
            Debug.Log("No Document Recipe found");
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject CreateSpecialPermit(string name, string id)
        {
            var obj = Resources.Load<SpecialPermitRecipe>($"Recipes/{name}");
            if (obj != null)
                return CreateSpecialPermit(obj, id);
            Debug.Log("No Document Recipe found");
            return null;
        }
        #endregion
        
        #region Document Process
        private static GameObject CreateDealerLicense(DealerLicenseRecipe recipe, string id)
        {
            //Instantiate Document Base
            var obj = InstantiateGameObj($"Documents/I_DealerLicense");
            obj.name = recipe.name;
            obj.GetComponent<EntityAttribute>().paperType = DocumentType.DealerLicense;
            AddDealerLicenseContent(obj, recipe.dealerLicenseData, id);
            return obj;
        }

        private static GameObject CreateHealthCertificate(HealthCertificateRecipe recipe, string id)
        {
            var obj = InstantiateGameObj($"Documents/I_HealthCertificate");
            obj.name = recipe.name;
            obj.GetComponent<EntityAttribute>().paperType = DocumentType.HealthCertificate;
            AddHealthCertificateContent(obj, recipe.healthCertificateData, id);
            return obj;
        }

        private static GameObject CreatePossessionLicense(PossessionLicenseRecipe recipe, string id)
        {   
            var obj = InstantiateGameObj($"Documents/I_PossessionLicense");
            obj.name = recipe.name;
            obj.GetComponent<EntityAttribute>().paperType = DocumentType.PossessionLicense;
            AddPossessionLicenseContent(obj, recipe.possessionLicenseData, id);
            return obj;
        }

        private static GameObject CreateSpecialPermit(SpecialPermitRecipe recipe, string id)
        {
            var obj = InstantiateGameObj($"Documents/I_SpecialPermit");
            obj.name = recipe.name;
            obj.GetComponent<EntityAttribute>().paperType = DocumentType.SpecialPermit;
            AddSpecialPermitContent(obj, recipe.specialPermitData, id);
            return obj;
        }

        private static void AddDealerLicenseContent(GameObject obj, List<DealerLicenseData> data, string id)
        {
            //string selected = licenseData.components[index];
            //Debug.Log(selected);
            //_generatedID;

            foreach (var dealerLicenseData in data)
            {
                if (dealerLicenseData.animalId != id)
                    continue;

                int index = Random.Range(0, dealerLicenseData.businessName.Count);
                //Debug.Log(index);
                AddContentText(obj, "TM_BusinessName", dealerLicenseData.businessName[index]);
                
                index = Random.Range(0, dealerLicenseData.businessNumber.Count);
                AddContentText(obj, "TM_BusinessNumber", dealerLicenseData.businessNumber[index]);
                
                AddContentPrefab(obj, dealerLicenseData.line1Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.line2Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.line3Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.line4Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.isProcess, "TickPos_6", "I_DocumentCircle");
                AddContentPrefab(obj, dealerLicenseData.isTick, "TickPos_7", "I_DocumentCircle");
                
                index = Random.Range(0, dealerLicenseData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", dealerLicenseData.stampSign[index]);
            }
        }

        private static void AddHealthCertificateContent(GameObject obj, List<HealthCertificateData> data, string id)
        {
            foreach (var healthCertificateData in data)
            {
                if (healthCertificateData.animalId != id)
                    continue;

                AddContentPrefab(obj, healthCertificateData.isOfficial, "TickPos_1", "I_DocumentCircle");
                AddContentPrefab(obj, healthCertificateData.isLocal, "TickPos_2", "I_DocumentCircle");
                
                int index = Random.Range(0, healthCertificateData.animalName.Count);
                AddContentText(obj, "TM_AnimalName", healthCertificateData.animalName[index]);
                
                index = Random.Range(0, healthCertificateData.animalMark.Count);
                AddContentText(obj, "TM_AnimalMark", healthCertificateData.animalMark[index]);
               
                index = Random.Range(0, healthCertificateData.date.Count);
                AddContentPrefab(obj, "TM_Date", healthCertificateData.date[index]);

                index = Random.Range(0, healthCertificateData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", healthCertificateData.stampSign[index]);

            }
        }

        private static void AddPossessionLicenseContent(GameObject obj, List<PossessionLicenseData> data, string id)
        {
            foreach (var possessionLicenseData in data)
            {
                if (possessionLicenseData.animalId != id)
                    continue;
                
                int index = Random.Range(0, possessionLicenseData.licenseNumber.Count);
                AddContentText(obj, "TM_LicenseNumber", possessionLicenseData.licenseNumber[index]);
                
                index = Random.Range(0, possessionLicenseData.deadline.Count);
                AddContentText(obj, "TM_Deadline", possessionLicenseData.deadline[index]);
               
                index = Random.Range(0, possessionLicenseData.name.Count);
                AddContentText(obj, "TM_Name", possessionLicenseData.name[index]);
                
                index = Random.Range(0, possessionLicenseData.id.Count);
                AddContentText(obj, "TM_ID", possessionLicenseData.id[index]);
                
                index = Random.Range(0, possessionLicenseData.businessNumber.Count);
                AddContentText(obj, "TM_BusinessNumber", possessionLicenseData.businessNumber[index]);

                index = Random.Range(0, possessionLicenseData.animalName.Count);
                AddContentText(obj, "TM_AnimalName", possessionLicenseData.animalName[index]);
                
                index = Random.Range(0, possessionLicenseData.contract.Count);
                AddContentText(obj, "TM_Contract", possessionLicenseData.contract[index]);
                
                index = Random.Range(0, possessionLicenseData.objective.Count);
                AddContentText(obj, "TM_Contract", possessionLicenseData.objective[index]);

                index = Random.Range(0, possessionLicenseData.original.Count);
                AddContentText(obj, "TM_Contract", possessionLicenseData.original[index]);
                
                index = Random.Range(0, possessionLicenseData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", possessionLicenseData.stampSign[index]);
            }
        }

        private static void AddSpecialPermitContent(GameObject obj, List<SpecialPermitData> data, string id)
        {
            foreach (var specialPermitData in data)
            {
                if (specialPermitData.animalId != id)
                    continue;
                
                int index = Random.Range(0, specialPermitData.locationName.Count);
                AddContentText(obj, "TM_LocationName", specialPermitData.locationName[index]);
                
                AddContentPrefab(obj, specialPermitData.line1Position.ToString(), "I_DocumentCircle");
                AddContentPrefab(obj, specialPermitData.line2Position.ToString(), "I_DocumentCircle");
                
                index = Random.Range(0, specialPermitData.businessNumber.Count);
                AddContentText(obj, "TM_BusinessNumber", specialPermitData.businessNumber[index]);
                
                index = Random.Range(0, specialPermitData.deadline.Count);
                AddContentText(obj, "TM_Deadline", specialPermitData.deadline[index]);
                
                index = Random.Range(0, specialPermitData.animalName.Count);
                AddContentText(obj, "TM_AnimalName", specialPermitData.animalName[index]);
                
                index = Random.Range(0, specialPermitData.animalCount.Count);
                AddContentText(obj, "TM_AnimalCount", specialPermitData.animalCount[index]);

                index = Random.Range(0, specialPermitData.animalFeature.Count);
                AddContentText(obj, "TM_AnimalFeature", specialPermitData.animalFeature[index]);
                
                index = Random.Range(0, specialPermitData.stampSign.Count);
                AddContentPrefab(obj, "StampSignPos", specialPermitData.stampSign[index]);
            }
        }
        
        private static GameObject InstantiateGameObj(string name)
        {
            var temp = Resources.Load<GameObject>(name);
            if (temp == null)
            {
                Debug.LogError($"No prefab for name {name}");
                return new GameObject(name);
            }

            var gameObject = Object.Instantiate(temp);
            return gameObject;
        }
        
        private static void AddContentText(GameObject obj, string objName, string name)
        {
            var temp = GameObjFinder.FindChildGameObject(obj, objName);
            temp.GetComponent<TextMeshPro>().text = name;
        }

        private static void AddContentPrefab(GameObject obj, string pos, string name)
        {
            string posName = CheckPosition(pos);
            if(posName == null)
                return;
            
            var temp = GameObjFinder.FindChildGameObject(obj, posName);
            var prefab = InstantiateGameObj($"Components/{name}");
            prefab.transform.SetParent(temp.transform);
            prefab.transform.localScale = Vector3.one;
            prefab.transform.localPosition = Vector3.zero;
        }

        private static void AddContentPrefab(GameObject obj, bool isTick, string posName, string name)
        {
            if (!isTick)
                return;
            
            var temp = GameObjFinder.FindChildGameObject(obj, posName);
            var prefab = InstantiateGameObj($"Components/{name}");
            prefab.transform.SetParent(temp.transform);
            prefab.transform.localScale = Vector3.one;
            prefab.transform.localPosition = Vector3.zero;
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
                "StampSignPos" => "StampSignPos",
                _ => null
            };
        }
        #endregion
    }
}
