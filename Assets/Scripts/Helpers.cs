using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelpersFunctions
{
    public static class Helpers
    {
        public const float distanceToLinkLines = 0.1f;

        public static bool isVectorsSame(Vector3 v1, Vector3 v2, float tolerance = 0.001f)
        {
            return Vector3.Distance(v1, v2) <= tolerance;
        }

        public static bool isVectorFinite(Vector3 v)
        {
            return !(float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z));
        }

        public static Vector3 ProjectPointOnLine(Vector3 pointP, Vector3 origin, Vector3 direction)
        {
            direction.Normalize();
            Vector3 AP = pointP - origin;
            float projectionLength = Vector3.Dot(AP, direction);
            Vector3 projection = origin + projectionLength * direction;
            return projection;
        }
    }

}
