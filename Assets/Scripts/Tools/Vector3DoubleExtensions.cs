using RG.OrbitalElements;
using UnityEngine;

namespace Tools
{
    public static  class Vector3DoubleExtensions
    {
        public static Vector3 ToVector3(this Vector3Double v)
        {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }
    }
}
