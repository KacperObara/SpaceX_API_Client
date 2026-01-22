using UnityEngine;

namespace Tools
{
    public static class ScreenUtils
    {
        public enum AspectRatio
        {
            Tall,
            Wide
        }

        // Works in both Editor and Build.
        public static AspectRatio GetAspectRatio()
        {
            float width = Screen.width;
            float height = Screen.height;

            if (width <= 0 || height <= 0) return AspectRatio.Wide;

            float ratio = height / width;

            // 1.5f threshold (3:2 is 1.5, 16:9 is 1.77)
            return ratio > 1.5f ? AspectRatio.Tall : AspectRatio.Wide;
        }
    }
}

