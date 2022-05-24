using System.Collections.Generic;
using Game.Scripts.Controller.SubController;
using Game.Scripts.EventArguments;
using Game.Scripts.Model;
using Game.Scripts.View_Model_Components;
using UnityEngine;

namespace Game.Scripts.Controller.Level_State
{
    public class MainGameState : GameLoopState
    {
        private GamePlayController _gamePlayController;
        private ObjectController _objectController;
        private UIController _uiController;

        private List<string> _documents;
        private List<FunctionalObjectsData> _functionalObjects;

        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = owner.GetComponentInChildren<GamePlayController>();
            _objectController = owner.GetComponentInChildren<ObjectController>();
            _uiController = owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
            if(owner.debugMode)
                _uiController.SetDebugMode();
            
            _gamePlayController.enabled = true;
            
            _objectController.gameObject.AddComponent<DragAndDropSubController>();
            _objectController.enabled = true;

            _documents = owner.levelData.DocumentsNeeded;
            _functionalObjects = owner.levelData.FunctionalObjectsData;

            _objectController.InitFactory(_documents);
            _objectController.InitObjectPool(_functionalObjects);

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
            if (original == null)
                return;

            _objectController.ProcessCollision(original, col.info);
        }

        private void OnSubmitted(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 1)
                _uiController.SetScore(++_gamePlayController.Score);
        }
    }
}
