using System;
using Game.Scripts.Enum;
using Game.Scripts.EventArguments;
using Game.Scripts.TempCode;
using Game.Scripts.Tools;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller.SubController
{
    /// <summary>
    /// This sub-controller will responsible for all gameObject interaction behavior.
    /// </summary>
    public class InteractionSubController : MonoBehaviour
    {
        #region Field / Properties
        private static InteractionSubController Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateSharedInstance();
                }

                return instance;
            }
        }

        private static InteractionSubController instance;
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }
        #endregion

        #region Methods
        public static sbyte ExecuteObjBehavior(GameObject original, GameObject target)
        {
            var oType = original.GetComponent<EntityAttribute>().objectType;
            var tType = target.GetComponent<EntityAttribute>().objectType;

            sbyte index = CheckObjectType(oType, tType);

            if (index == 0)
                return 0;

            GameObject stamp, pos;

            switch (index)
            {
                case 1:
                    stamp = target.GetComponent<LicenseInfo>().parts.Find(part => part.name == "I_Approved");
                    pos = GameObjFinder.FindChildGameObject(target, "Pos");
                    target.GetComponent<LicenseInfo>().isApproved = true;
                    target.GetComponent<LicenseInfo>().isStamped = true;
                    ClearChildren(pos);
                    Instantiate(stamp, pos.transform);
                    break;
                case 2:
                    stamp = target.GetComponent<LicenseInfo>().parts.Find(part => part.name == "I_Rejected");
                    pos = GameObjFinder.FindChildGameObject(target, "Pos");
                    target.GetComponent<LicenseInfo>().isApproved = false;
                    target.GetComponent<LicenseInfo>().isStamped = true;
                    ClearChildren(pos);
                    Instantiate(stamp, pos.transform);
                    break;
                case 3:
                    pos = GameObjFinder.FindChildGameObject(target, "Pos");
                    ClearChildren(pos);
                    return 1;
            }
            
            return 0;
        }

        private static sbyte CheckObjectType(ObjectType original, ObjectType target)
        {
            //Debug.Log("Testing");
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
        #endregion
        
        private static void CreateSharedInstance()
        {
            var obj = new GameObject("Interaction Controller");
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<InteractionSubController>();
        }
    }
}
