using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WorldTools : MonoBase
{
    public Transform m_Target;

    public Skill_Sorcerer m_Skill1;

    public Vector3 GetSpherePoint(float f_Angle, float f_Radius = 1)
    {
        var z = 1 / Mathf.Sin(f_Angle * Mathf.Deg2Rad);
        var x = 1 / Mathf.Cos(f_Angle * Mathf.Deg2Rad);

        return new Vector3(x, 0, z) * f_Radius;
    }



    public LineRenderer m_Spere;


    public async void ShowRange_Sphere(Func<Vector3> f_Pos, float f_Radius, Func<bool> f_IsStop)
    {


        while (!f_IsStop.Invoke())
        {
            var centrePos = f_Pos.Invoke();
            centrePos = transform.position;





            await UniTask.Delay(500);
        }

    }


    private void Update()
    {
        DrawGizom.Instance.AddLine(GetInstanceID(), transform.position, m_Target?.position ?? Vector3.zero);
    }

}
