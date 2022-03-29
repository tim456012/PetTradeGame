using System;
using Game.Scripts.Enum;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller.SubController
{
    //TODO: Create Object interaction behavior process
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
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        #endregion
        
        #region Methods

        public static void executeObjBehavior(GameObject original, GameObject target)
        {
            var oType = original.GetComponent<EntityAttribute>().objectType;
            var tType = target.GetComponent<EntityAttribute>().objectType;

            CheckObjectType(oType, tType);
        }
        
        private static void CheckObjectType(ObjectType original, ObjectType target)
        {
            Debug.Log("Testing");
            /*switch (original, target)
            {
                
            }*/


            /*if (original == ObjectType.GreenStamp && target == ObjectType.License)
            {
                GameObject stamp = obj.GetComponent<AvailableParts>().parts.Find(part => part.name == "Approved");
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");

                ClearChildren(pos);
                Instantiate(stamp, pos.transform);
            }*/

            /*if (collidedObjectType.ObjectType == ObjectType.Test && selfObjectType.ObjectType == ObjectType.RedStamp)
            {
                GameObject stamp = obj.GetComponent<AvailableParts>().parts.Find(part => part.name == "Rejected");
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");
                ClearChildren(pos);

                Instantiate(stamp, pos.transform);
            }*/

            /*if (collidedObjectType.ObjectType == ObjectType.Bin && selfObjectType.ObjectType == ObjectType.Test)
            {
                GameObject pos = GameObjFinder.FindChildGameObject(obj, "Pos");
                ClearChildren(pos);

                e.info.gameObject.transform.SetParent(collidedObjectType.transform);
                e.info.gameObject.transform.localPosition = Vector3.zero;

                //GameObjectPoolController.Enqueue(pos.GetComponentInParent<Poolable>());
            }*/
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
