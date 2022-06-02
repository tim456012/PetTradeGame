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
        
        private float _width, _height;
        //Bottom Screen
        private Vector3 _bl, _bc, _br;
        //Middle Screen
        private Vector3 _ml, _mc, _mr;
        //Top Screen
        private Vector3 _tl, _tc, _tr;
        #endregion

        #region Properties
        public float Width => _width;
        public float height => _height;
        
        //Helper Points
        public Vector3 BottomLeft => _bl;
        public Vector3 BottomCenter => _bc;
        public Vector3 BottomRight => _br;
        public Vector3 MiddleLeft => _ml;
        public Vector3 MiddleCenter => _mc;
        public Vector3 MiddleRight => _mr;
        public Vector3 TopLeft => _tl;
        public Vector3 TopCenter => _tc;
        public Vector3 TopRight => _tr;
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

            _height = 2f * camera.orthographicSize;
            _width = _height * camera.aspect;

            var position = camera.transform.position;
            float cameraX = position.x;
            float cameraY = position.y;

            float leftX = cameraX - _width / 2;
            float rightX = cameraX + _width / 2;
            float topY = cameraY + _height / 2;
            float bottomY = cameraY - _height / 2;
            
            //Bottom
            _bl = new Vector3(leftX, bottomY, 0);
            _bc = new Vector3(cameraX, bottomY, 0);
            _br = new Vector3(rightX, bottomY, 0);
            
            //Middle
            _ml = new Vector3(leftX, cameraY, 0);
            _mc = new Vector3(cameraX, cameraY, 0);
            _mr = new Vector3(rightX, cameraY, 0);
            
            //Top
            _tl = new Vector3(leftX, topY, 0);
            _tc = new Vector3(cameraX, topY, 0);
            _tr = new Vector3(rightX, topY, 0);
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

            var temp = Gizmos.matrix;
            var transform1 = transform;
            Gizmos.matrix = Matrix4x4.TRS(transform1.position, transform1.rotation, Vector3.zero);
            if (camera.orthographic)
            {
                float spread = camera.farClipPlane - camera.nearClipPlane;
                float center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
                Gizmos.DrawWireCube(new Vector3(0,0,center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2 , spread));
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
