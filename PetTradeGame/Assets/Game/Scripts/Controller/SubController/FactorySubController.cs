using System.Collections.Generic;
using Game.Scripts.Factory;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.SubController
{
    public class FactorySubController : MonoBehaviour
    {
        public string generatedID;

        public GameObject ProduceDocument(string documentType, string documentName, string id)
        {
            generatedID = id;
            
            var obj = documentType switch
            {
                "DealerLicenseRecipe" => DocumentFactory.CreateDealerLicense(documentName, id),
                "HealthCertificateRecipe" => DocumentFactory.CreateHealthCertificate(documentName, id),
                "PossessionLicenseRecipe" => DocumentFactory.CreatePossessionLicense(documentName, id),
                "SpecialPermitRecipe" => DocumentFactory.CreateSpecialPermit(documentName, id),
                _ => null
            };
            
            return obj;
        }
    }
}
