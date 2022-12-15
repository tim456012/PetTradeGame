using System;
using System.Collections;
using Game.Scripts.Common.Animation;
using Game.Scripts.Controller;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class GamePlayPanel : MonoBehaviour
    {
        public Button btnIpadOn, btnIpadOff, btnAnimalSearch, btnDocSearch, btnScaleUp, btnScaleDown;

        private Transform _btnPos1, _btnPos2, _originPos;
        private MonoRoutine _buttonAnimationRoutine;
        private bool _isAnimate, _isDisable, _isShowingIpad;

        public static event EventHandler ShowIpadView, HideIpadView, ScaleUpDoc, ScaleDownDoc;
        public static event EventHandler GamePause, GameResume, ClearData, TutorialIpadEvent, HideTutorialIpadEvent;
        public static event EventHandler BackToMenuEvent;
        
        private void Awake()
        {
            _btnPos1 = GameObjFinder.FindChildGameObject(gameObject, "BtnPos1").transform;
            _btnPos2 = GameObjFinder.FindChildGameObject(gameObject, "BtnPos2").transform;
            _originPos = GameObjFinder.FindChildGameObject(gameObject, "OriginPos").transform;

            Vector3 position = _originPos.position;
            btnAnimalSearch.transform.position = position;
            btnDocSearch.transform.position = position;
        }

        private void Start()
        {
            btnIpadOn.gameObject.SetActive(true);
            btnIpadOff.gameObject.SetActive(false);
            btnAnimalSearch.gameObject.SetActive(false);
            btnDocSearch.gameObject.SetActive(false);
        }

        public void ShowIpadOptions()
        {
            if (_isAnimate || _isDisable)
                return;

            MoveOptions(true);
        }

        public void HideIpadOptions()
        {
            if (_isAnimate || _isDisable)
                return;

            MoveOptions(false);
        }

        public void ScaleUpDocument()
        {
            if(_isShowingIpad)
                return;
            
            btnScaleUp.gameObject.SetActive(false);
            btnScaleDown.gameObject.SetActive(true);
            ScaleUpDoc?.Invoke(this, EventArgs.Empty);
        }

        public void ScaleDownDocument()
        {
            if(_isShowingIpad)
                return;
            
            btnScaleDown.gameObject.SetActive(false);
            btnScaleUp.gameObject.SetActive(true);
            ScaleDownDoc?.Invoke(this, EventArgs.Empty);
        }

        public void ShowSettingPanel()
        {
            InputController.IsPause = true;
            GamePause?.Invoke(this, EventArgs.Empty);
        }
        
        public void HideSettingPanel()
        {
            GameResume?.Invoke(this, EventArgs.Empty);
        }

        public void ClearGameData()
        {
            ClearData?.Invoke(this, EventArgs.Empty);
        }

        private void MoveOptions(bool isShow)
        {
            _isAnimate = true;
            if (isShow)
            {
                _buttonAnimationRoutine = new MonoRoutine(DoShowButtonAnimation, this);
                _buttonAnimationRoutine.Start();
            }
            else
            {
                _buttonAnimationRoutine = new MonoRoutine(DoHideButtonAnimation, this);
                _buttonAnimationRoutine.Start();
            }
        }

        private IEnumerator DoShowButtonAnimation()
        {
            InputController.IsPause = true;
            _isShowingIpad = true;

            ShowIpadView?.Invoke(this, EventArgs.Empty);

            btnAnimalSearch.gameObject.SetActive(true);
            btnDocSearch.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            btnAnimalSearch.transform.MoveTo(_btnPos1.position, 0.5f, EasingEquations.EaseInQuad);
            btnDocSearch.transform.MoveTo(_btnPos2.position, 0.5f, EasingEquations.EaseInQuad);
            yield return new WaitForSeconds(0.8f);

            btnIpadOn.gameObject.SetActive(false);
            btnIpadOff.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            _buttonAnimationRoutine.Stopped += (_, _) =>
            {
                _isAnimate = false;
                TutorialIpadEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        private IEnumerator DoHideButtonAnimation()
        {
            Vector3 position = _originPos.position;
            btnAnimalSearch.transform.MoveTo(position, 0.5f, EasingEquations.EaseInQuad);
            btnDocSearch.transform.MoveTo(position, 0.5f, EasingEquations.EaseInQuad);
            yield return new WaitForSeconds(0.8f);

            btnAnimalSearch.gameObject.SetActive(false);
            btnDocSearch.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);

            btnIpadOff.gameObject.SetActive(false);
            btnIpadOn.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            _buttonAnimationRoutine.Stopped += (_, _) =>
            {
                _isAnimate = false; 
                _isShowingIpad = false;
                HideIpadView?.Invoke(this, EventArgs.Empty);
                HideTutorialIpadEvent?.Invoke(this, EventArgs.Empty);
                InputController.IsPause = false;
            };
        }
        
        public void BackToMenu()
        {
            Vector3 position = _originPos.position;
            btnDocSearch.transform.position = position;
            btnAnimalSearch.transform.position = position;
            btnIpadOff.gameObject.SetActive(false);
            btnIpadOn.gameObject.SetActive(true);
            btnAnimalSearch.gameObject.SetActive(false);
            btnDocSearch.gameObject.SetActive(false);
            HideIpadView?.Invoke(this, EventArgs.Empty);
            

            BackToMenuEvent?.Invoke(this, EventArgs.Empty);
        }

        public void IpadBtnSwitch(bool isDisable)
        {
            _isDisable = isDisable;
        }
    }
}