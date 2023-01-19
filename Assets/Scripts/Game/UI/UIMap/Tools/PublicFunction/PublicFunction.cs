using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PublicFunction
{
    public static Vector2 GetRandomVector2()
    {
        var lenght = 1.0f;
        var radius = Random.Range(0, 360);
        var x = Mathf.Cos(radius * Mathf.Deg2Rad) * lenght;
        var y = Mathf.Sin(radius * Mathf.Deg2Rad) * lenght;
        return new Vector2(x, y);
    }
    public static void AddExList<T>(ref List<T> original, params T[] parameters)
    {
        foreach (var param in parameters)
        {
            original.Add(param);
        }
    }
    public static Vector3 MoveToNormalDri()
    {
        return new Vector3();
    }
}
