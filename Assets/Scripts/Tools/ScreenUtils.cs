using UnityEditor;
using UnityEngine;

namespace Tools
{
    [DefaultExecutionOrder(-9900)]
    public class ScreenUtils : MonoBehaviour
    {
        public enum AspectRatio
        {
            Tall,
            Wide
        }

#if UNITY_EDITOR
        public static Vector2 GetEditorGameViewSize()
        {
            string[] res = UnityStats.screenRes.Split('x');
            return new Vector2((float) int.Parse(res[0]), (float) int.Parse(res[1]));
        }
#endif
    
        public static AspectRatio GetAspectRatio()
        {
            float ratio = (float) Screen.height / (float) Screen.width;

#if UNITY_EDITOR
            var editorGameViewSize = GetEditorGameViewSize();
            ratio = editorGameViewSize.y / editorGameViewSize.x;
#endif
        
            if (ratio > 1.5f)
                return AspectRatio.Tall;
            else
                return AspectRatio.Wide;
        }
    }
}