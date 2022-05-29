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

        private List<RecipeData> _documents;
        private List<FunctionalObjectsData> _functionalObjects;
        private ScoreData _scoreData;
        
        protected override void Awake()
        {
            base.Awake();
            _gamePlayController = Owner.GetComponentInChildren<GamePlayController>();
            _objectController = Owner.GetComponentInChildren<ObjectController>();
            _uiController = Owner.GetComponentInChildren<UIController>();
        }

        public override void Enter()
        {
            base.Enter();
            if(Owner.debugMode)
                _uiController.SetDebugMode();
            
            _gamePlayController.enabled = true;
            
            _objectController.gameObject.AddComponent<DragAndDropSubController>();
            _objectController.enabled = true;

            _documents = Owner.levelData.documentRecipeData;
            _functionalObjects = Owner.levelData.functionalObjectsData;
            _scoreData = Owner.levelData.scoreData;
                
            _objectController.InitFactory(_documents, _scoreData);
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

        private void OnSubmitted(object sender, InfoEventArgs<sbyte> e)
        {
            string id = _objectController.GetGeneratedID();
            var content = _scoreData.scoreContents;
            foreach (var scoreContent in content)
            {
                if (!id.Equals(scoreContent.id))
                    continue;
                int score = _gamePlayController.CalculateScore(e.info, scoreContent.score);
                _uiController.SetScore(score);
            }
        }
    }
}
