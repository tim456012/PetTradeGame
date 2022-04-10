﻿using System.Collections.Generic;
using Game.Scripts.Controller.SubController;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class MainGameState : GameLoopState
    {
        private GamePlayController gamePlayController;
        private ObjectController objectController;
        private UIController uiController;
        
        private List<string> documents;
        private List<FunctionalObjectsData> functionalObjects;

        protected override void Awake()
        {
            base.Awake();
            gamePlayController = owner.GetComponentInChildren<GamePlayController>();
            objectController = owner.GetComponentInChildren<ObjectController>();
            uiController = owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
            gamePlayController.enabled = true;

            objectController.gameObject.AddComponent<DragAndDropSubController>();
            objectController.enabled = true;

            documents = owner.LevelData.DocumentsNeeded;
            functionalObjects = owner.LevelData.FunctionalObjectsData;
            
            objectController.InitFactory(documents);
            objectController.InitObjectPool(functionalObjects);

            Debug.Log("Entering playing state");
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            EntityAttribute.FunctionalObjCollisionEvent += OnFunctObjCollision;
            ObjectController.LicenseSubmittedEvent += OnSubmitted;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            EntityAttribute.FunctionalObjCollisionEvent -= OnFunctObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EntityAttribute.FunctionalObjCollisionEvent -= OnFunctObjCollision;
            ObjectController.LicenseSubmittedEvent -= OnSubmitted;
        }

        private void OnFunctObjCollision(object sender, InfoEventArgs<GameObject> col)
        {
            var original = sender as GameObject;
            if(original == null)
                return;

            objectController.ProcessCollision(original, col.info);
        }

        private void OnSubmitted(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 1)
            {
                uiController.setScore(++gamePlayController.Score);
            }
        }
    }
}
