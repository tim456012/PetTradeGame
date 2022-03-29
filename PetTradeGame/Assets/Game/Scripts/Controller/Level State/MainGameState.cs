using System.Collections.Generic;
using Game.Scripts.Controller.SubController;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class MainGameState : GameLoopState
    {
        private GamePlayController gamePlayController;
        private ObjectController objectController;
        
        private List<string> documents;
        private List<FunctionalObjectsData> functionalObjects;

        protected override void Awake()
        {
            base.Awake();
            gamePlayController = owner.GetComponentInChildren<GamePlayController>();
            objectController = owner.GetComponentInChildren<ObjectController>();
        }

        public override void Enter()
        {
            base.Enter();
            gamePlayController.enabled = true;

            //objectController.gameObject.AddComponent<InteractionSubController>();
            objectController.gameObject.AddComponent<DragAndDropSubController>();
            objectController.enabled = true;

            documents = owner.LevelData.DocumentsNeeded;
            functionalObjects = owner.LevelData.FunctionalObjectsData;

            //objectController.InitInteraction();
            
            objectController.InitFactory(documents);
            objectController.InitObjectPool(functionalObjects);

            Debug.Log("Entering playing state");
        }

        protected override void AddListeners()
        {
            base.AddListeners();
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
