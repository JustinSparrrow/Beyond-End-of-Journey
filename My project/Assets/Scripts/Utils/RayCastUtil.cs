using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastUtil
{
    public static RaycastHit2D[] CircleHit(Vector2 position, float radius, int mask)
    {
        RaycastHit2D[] hits;
        // 执行圆形射线检测，获取所有射线命中的对象
        hits = Physics2D.CircleCastAll((Vector3)position + new Vector3(0, 0, 1), radius, Vector2.zero, 2f, mask); //空中某个点，垂直于2D平面向下
        return hits.Length == 0 ? null : hits;
    }

    public static void CircleDamanage(UnitControl unitControl, Vector2 position, float radius, int mask, float damage, HashSet<GameObject>harmedObject_set = null)
    {
        RaycastHit2D[] hits = RayCastUtil.CircleHit(position, radius, mask);
        if (hits == null) return;
        foreach (RaycastHit2D hit in hits)
        {
            GameObject obj = hit.collider.gameObject;
            if (harmedObject_set == null || !harmedObject_set.Contains(obj))
            {
                if(harmedObject_set != null)
                    harmedObject_set.Add(obj);
                unitControl.AttackOn(obj, damage);
            }
        }
    }
}
