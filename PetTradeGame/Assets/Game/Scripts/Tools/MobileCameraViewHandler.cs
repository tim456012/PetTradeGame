using UnityEngine;

namespace Game.Scripts.Tools
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class MobileCameraViewHandler : MonoBehaviour
    {
        public enum Constraint { Landscape, Portrait }

        #region Fields

        public Color wireColor = Color.white;
        public float unitsSize = 1;
        public Constraint constraint = Constraint.Landscape;
        public static MobileCameraViewHandler Instance;
        public new Camera camera;

        public bool executeInUpdate;

        //Bottom Screen
        //Middle Screen
        //Top Screen

        #endregion

        #region Properties

        public float Width { get; private set; }

        public float height { get; private set; }

        //Helper Points
        public Vector3 BottomLeft { get; private set; }

        public Vector3 BottomCenter { get; private set; }

        public Vector3 BottomRight { get; private set; }

        public Vector3 MiddleLeft { get; private set; }

        public Vector3 MiddleCenter { get; private set; }

        public Vector3 MiddleRight { get; private set; }

        public Vector3 TopLeft { get; private set; }

        public Vector3 TopCenter { get; private set; }

        public Vector3 TopRight { get; private set; }

        #endregion

        #region Methods

        private void Awake()
        {
            camera = GetComponent<Camera>();
            Instance = this;
            ComputeResolution();
        }

        private void ComputeResolution()
        {
            if (constraint == Constraint.Landscape)
                camera.orthographicSize = 1f / camera.aspect * unitsSize / 2f;
            else
                camera.orthographicSize = unitsSize / 2f;

            height = 2f * camera.orthographicSize;
            Width = height * camera.aspect;

            Vector3 position = camera.transform.position;
            float cameraX = position.x;
            float cameraY = position.y;

            float leftX = cameraX - Width / 2;
            float rightX = cameraX + Width / 2;
            float topY = cameraY + height / 2;
            float bottomY = cameraY - height / 2;

            //Bottom
            BottomLeft = new Vector3(leftX, bottomY, 0);
            BottomCenter = new Vector3(cameraX, bottomY, 0);
            BottomRight = new Vector3(rightX, bottomY, 0);

            //Middle
            MiddleLeft = new Vector3(leftX, cameraY, 0);
            MiddleCenter = new Vector3(cameraX, cameraY, 0);
            MiddleRight = new Vector3(rightX, cameraY, 0);

            //Top
            TopLeft = new Vector3(leftX, topY, 0);
            TopCenter = new Vector3(cameraX, topY, 0);
            TopRight = new Vector3(rightX, topY, 0);
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (executeInUpdate)
                ComputeResolution();
            #endif
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = wireColor;

            Matrix4x4 temp = Gizmos.matrix;
            Transform transform1 = transform;
            Gizmos.matrix = Matrix4x4.TRS(transform1.position, transform1.rotation, Vector3.zero);
            if (camera.orthographic)
            {
                float spread = camera.farClipPlane - camera.nearClipPlane;
                float center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
                Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
            }
            Gizmos.matrix = temp;
        }

        #endregion
    }
}