using UnityEngine;

namespace Tools
{
    public class AspectRatioScaler : MonoBehaviour
    {
        [SerializeField] private Vector3 _scaleDefault = new Vector3(1f, 1f, 1f);
        [SerializeField] private Vector3 _scaleWide = new Vector3(1f, 1f, 1f);
    
        void Start()
        {
            Scale();
        }
    
        public void Scale()
        {
            ScreenUtils.AspectRatio ratio = ScreenUtils.GetAspectRatio();
            switch (ratio)
            {
                case ScreenUtils.AspectRatio.Wide:
                    this.transform.localScale = _scaleWide;
                    break;
                default:
                case ScreenUtils.AspectRatio.Tall:
                    this.transform.localScale = _scaleDefault;
                    break;
            }
        }
    }
}