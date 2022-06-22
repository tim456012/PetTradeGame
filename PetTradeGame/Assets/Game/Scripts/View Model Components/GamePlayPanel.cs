using System;
using System.Collections;
using Game.Scripts.Common.Animation;
using Game.Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.View_Model_Components
{
    public class GamePlayPanel : MonoBehaviour
    {
        public TextMeshProUGUI hourTimeText;
        public Button btnSetting, btnIpadOn, btnIpadOff, btnAnimalSearch, btnDocSearch;

        private Transform _btnPos1, _btnPos2, _originPos;
        private MonoRoutine _buttonAnimationRoutine;
        private bool _isAnimate;

        private void Awake()
        {
            _btnPos1 = GameObjFinder.FindChildGameObject(gameObject, "BtnPos1").transform;
            _btnPos2 = GameObjFinder.FindChildGameObject(gameObject, "BtnPos2").transform;
            _originPos = GameObjFinder.FindChildGameObject(gameObject, "OriginPos").transform;
            
            var position = _originPos.position;
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
            if (_isAnimate)
                return;
            
            MoveOptions(true);
        }

        public void HideIpadOptions()
        {
            if (_isAnimate)
                return;

            MoveOptions(false);
        }

        public void SetTime(string text)
        {
            hourTimeText.text = text;
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
            btnAnimalSearch.gameObject.SetActive(true);
            btnDocSearch.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            
            btnAnimalSearch.transform.MoveTo(_btnPos1.position, 0.5f, EasingEquations.EaseInQuad);
            btnDocSearch.transform.MoveTo(_btnPos2.position, 0.5f, EasingEquations.EaseInQuad);
            yield return new WaitForSeconds(0.8f);
            
            btnIpadOn.gameObject.SetActive(false);
            btnIpadOff.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            
            _buttonAnimationRoutine.Stopped += (_, _) => _isAnimate = false;
        }

        private IEnumerator DoHideButtonAnimation()
        {
            var position = _originPos.position;
            btnAnimalSearch.transform.MoveTo(position, 0.5f, EasingEquations.EaseInQuad);
            btnDocSearch.transform.MoveTo(position, 0.5f, EasingEquations.EaseInQuad);
            yield return new WaitForSeconds(0.8f);

            btnAnimalSearch.gameObject.SetActive(false);
            btnDocSearch.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            
            btnIpadOff.gameObject.SetActive(false);
            btnIpadOn.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            
            _buttonAnimationRoutine.Stopped += (_, _) => _isAnimate = false;
        }
    }
}
