using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension 
{
    public static float InverseLerp(this Vector3 value, Vector3 a, Vector3 b)
    {
        var AB = b - a;
        var AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    public static Vector3 FindClosestPoint(this Vector3 p, Vector3 a, Vector3 b)
    {
        var vector1 = p - a;
        var vector2 = (b - a).normalized;

        var d = Vector3.Distance(a, b);
        var t = Vector3.Dot(vector2, vector1);

        if (t <= 0) return a;
        if (t >= d) return b;

        var vector3 = vector2 * t;
        return a + vector3;
    }
}
