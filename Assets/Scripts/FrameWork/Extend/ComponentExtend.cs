using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtend
{
    public static void Normalize(this RectTransform f_Rect)
    {
        f_Rect.anchoredPosition3D = Vector3.zero;
        f_Rect.localScale = Vector3.one;
        f_Rect.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public static void NormalFullScene(this RectTransform f_Rect)
    {
        f_Rect.anchoredPosition3D = Vector3.zero;
        f_Rect.sizeDelta = Vector2.zero;
        f_Rect.anchorMin = Vector2.zero;
        f_Rect.anchorMax = Vector2.one;
        f_Rect.pivot = Vector2.one * 0.5f;
        f_Rect.localScale = Vector3.one;
        f_Rect.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public static void Normalize(this Transform f_Rect)
    {
        f_Rect.localPosition = Vector3.zero;
        f_Rect.localScale = Vector3.one;
        f_Rect.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public static bool TryFind<T>(this Transform f_Tran, string f_Name, out T f_Target)
        where T : MonoBehaviour
    {
        bool isTry = false;
        var obj = f_Tran.Find(f_Name);
        f_Target = null;
        if (obj != null && obj.TryGetComponent(out f_Target))
        {
            isTry = true;
        }
        return isTry;
    }
}