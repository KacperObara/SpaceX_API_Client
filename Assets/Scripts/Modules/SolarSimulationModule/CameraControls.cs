using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Modules.SolarSimulationModule
{
    public class CameraControls : MonoBehaviour
    {
        [FormerlySerializedAs("ZoomSpeed")]
        [Header("Zoom Settings")]
        [SerializeField] private float EditorZoomSpeed = 8.0f;
        [SerializeField] private float MobileZoomSpeed = 3.0f;
        [SerializeField] private float MinDistance = -2000f;
        [SerializeField] private float MaxDistance = -150f;
        [SerializeField] private float Distance = -700f;  
    
        [SerializeField] private Transform Target;
        [Inject] private readonly Camera _camera;
    
        private float _lastPinchDistance;
    
        private void Start()
        {
            UpdateCamera();
        }

        public void TickCamera(float deltaTime)
        {
            HandleZoom();
        }

        private void UpdateCamera()
        {
            _camera.transform.position = Target.position + new Vector3(0, 0, Distance);
        }
    
        private void HandleZoom()
        {
#if UNITY_EDITOR 
            float scroll = Input.GetAxis( "Mouse ScrollWheel" );
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Distance += scroll * EditorZoomSpeed * 50f;
                Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
                UpdateCamera();
            }
#endif
        
            if (Input.touches.Length >= 2)
            {
                Touch t0 = Input.touches[0];
                Touch t1 = Input.touches[1];

                float currDist = Vector2.Distance(t0.position, t1.position);

                if (t1.phase == TouchPhase.Began)
                {
                    _lastPinchDistance = currDist;
                }
                else
                {
                    float delta = currDist - _lastPinchDistance;
                    _lastPinchDistance = currDist;

                    Distance += delta * MobileZoomSpeed;
                    Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
                
                    UpdateCamera();
                }
            }
        }
    }
}
