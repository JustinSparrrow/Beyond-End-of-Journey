using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    // 静态方法，计算两个GameObject之间的距离
    public static float Distance(GameObject objA, GameObject objB)
    {
        if (objA == null || objB == null)
        {
            Debug.LogError("One or both GameObjects are null.");
            return -1f; //负数代表报错
        }
        return Vector2.Distance(objA.transform.position, objB.transform.position);
    }

    public static float Distance(Vector2 vecA, Vector2 vecB)
    {
        return Vector2.Distance(vecA, vecB);
    }
}
