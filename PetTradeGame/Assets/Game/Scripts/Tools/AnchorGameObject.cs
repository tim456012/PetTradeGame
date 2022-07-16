using System.Collections;
using UnityEngine;

namespace Game.Scripts.Tools
{
    [ExecuteInEditMode]
    public class AnchorGameObject : MonoBehaviour
    {
        public enum AnchorType
        {
            BottomLeft,
            BottomCenter,
            BottomRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            TopLeft,
            TopCenter,
            TopRight
        }

        public bool executeInUpdate;

        public AnchorType anchorType;
        public Vector3 anchorOffset;

        private IEnumerator updateAnchorRoutine; //Coroutine handle so we don't start it if it's already running

        // Use this for initialization
        private void Start()
        {
            updateAnchorRoutine = UpdateAnchorAsync();
            StartCoroutine(updateAnchorRoutine);
        }

#if UNITY_EDITOR
        // Update is called once per frame
        private void Update()
        {
            if (updateAnchorRoutine == null && executeInUpdate)
            {
                updateAnchorRoutine = UpdateAnchorAsync();
                StartCoroutine(updateAnchorRoutine);
            }
        }
#endif

        /// <summary>
        ///     Coroutine to update the anchor only once CameraFit.Instance is not null.
        /// </summary>
        private IEnumerator UpdateAnchorAsync()
        {

            uint cameraWaitCycles = 0;

            while (MobileCameraViewHandler.Instance == null)
            {
                ++cameraWaitCycles;
                yield return new WaitForEndOfFrame();
            }

            if (cameraWaitCycles > 0)
            {
                print(string.Format("CameraAnchor found CameraFit instance after waiting {0} frame(s). " +
                                    "You might want to check that CameraFit has an earlie execution order.", cameraWaitCycles));
            }

            UpdateAnchor();
            updateAnchorRoutine = null;

        }

        private void UpdateAnchor()
        {
            switch (anchorType)
            {
                case AnchorType.BottomLeft:
                    SetAnchor(MobileCameraViewHandler.Instance.BottomLeft);
                    break;
                case AnchorType.BottomCenter:
                    SetAnchor(MobileCameraViewHandler.Instance.BottomCenter);
                    break;
                case AnchorType.BottomRight:
                    SetAnchor(MobileCameraViewHandler.Instance.BottomRight);
                    break;
                case AnchorType.MiddleLeft:
                    SetAnchor(MobileCameraViewHandler.Instance.MiddleLeft);
                    break;
                case AnchorType.MiddleCenter:
                    SetAnchor(MobileCameraViewHandler.Instance.MiddleCenter);
                    break;
                case AnchorType.MiddleRight:
                    SetAnchor(MobileCameraViewHandler.Instance.MiddleRight);
                    break;
                case AnchorType.TopLeft:
                    SetAnchor(MobileCameraViewHandler.Instance.TopLeft);
                    break;
                case AnchorType.TopCenter:
                    SetAnchor(MobileCameraViewHandler.Instance.TopCenter);
                    break;
                case AnchorType.TopRight:
                    SetAnchor(MobileCameraViewHandler.Instance.TopRight);
                    break;
            }
        }

        private void SetAnchor(Vector3 anchor)
        {
            Vector3 newPos = anchor + anchorOffset;
            if (!transform.position.Equals(newPos))
            {
                transform.position = newPos;
            }
        }
    }
}