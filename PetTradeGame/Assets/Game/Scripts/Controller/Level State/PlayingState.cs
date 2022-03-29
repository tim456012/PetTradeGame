﻿using System.Collections.Generic;
using Game.Scripts.Model;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class PlayingState : GameLoopState
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
            gamePlayController.gameObject.AddComponent<DragAndDropController>();
            gamePlayController.enabled = true;

            objectController.gameObject.AddComponent<InteractionController>();
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
