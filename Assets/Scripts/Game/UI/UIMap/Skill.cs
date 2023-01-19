using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Skill : MonoBehaviour
{
    public Transform m_Pool;
    public Transform m_Target;


    public float n_Time;
    public float m_Angle;
    public Vector3 m_Pos;

    [Range(1, 10)] public float a = 4.0f;
    [Range(1, 10)] public float b = 1.0f;
    [Range(0, 10)] public float r = 5.0f;


    private bool isPlaying = false;
    public async void Action()
    {
        Clear();
        isPlaying = true;
        n_Time = 0;
        var objTran = GameObject.Instantiate<Transform>(m_Target, m_Pool);
        while (isPlaying)
        {
            await UniTask.Delay(30);
            n_Time += 30.0f / 1000.0f;
            if (objTran == null) return;
            m_Angle = n_Time * 360 / 40;
            var pos = GetSpherePoint(m_Angle);
            pos.y = pos.y * Mathf.Sin(m_Angle) / Mathf.Abs(Mathf.Sin(m_Angle));
            pos.x = pos.x * Mathf.Cos(m_Angle) / Mathf.Abs(Mathf.Cos(m_Angle));
            m_Pos = transform.position + new Vector3(pos.x, 0, pos.y);
            objTran.position = m_Pos;
        }
    }
    public void Stop()
    {
        isPlaying = false;
    }

    public void Clear()
    {
        for (int i = m_Pool.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(m_Pool.GetChild(i).gameObject);
        }
    }



    public Vector2 GetSpherePoint(float angle)
    {
        float y = Mathf.Pow(r * (Mathf.Pow(a, 2) * Mathf.Pow(b, 2) * Mathf.Pow(Mathf.Tan(angle), 2) /
            (Mathf.Pow(b, 2) + Mathf.Pow(a, 2) * Mathf.Pow(Mathf.Tan(angle), 2))), 0.5f);
        float x = Mathf.Pow(Mathf.Pow(y, 2) / Mathf.Pow(Mathf.Tan(angle), 2), 0.5f);

        return new Vector2(x, y);
    }
    public void MoveToTime(Transform target, Vector3 point, Vector3 pointDirection, float time)
    {
        time = Mathf.Max(time, 0);
        float timePoint = 0.5f;

        var tDir = target.forward;
        var pDir = point - target.position;
        var TdotP = Vector3.Dot(tDir, pointDirection);



        var tPos = target.position;
        tPos = new Vector3(
            tPos.x * (1 - timePoint) + point.x * timePoint,
            tPos.y * (1 - timePoint) + point.y * timePoint,
            tPos.z * (1 - timePoint) + point.z * timePoint);
        target.position = tPos;
    }

    public float m_AngleXY;
    public float m_AngleXZ;
    public float m_Radius;

    public float m_CosValue;
    public float m_SinValue;
    public float m_Random;

    public Vector3 m_Pos2;
    public Transform m_Target2;
    public void UpdateShperePoint()
    {
        m_CosValue = Mathf.Cos(m_AngleXY / 180.0f * Mathf.PI);
        m_SinValue = Mathf.Sin(m_AngleXY / 180.0f * Mathf.PI);
        float x = m_CosValue * Mathf.Sin(m_AngleXZ / 180.0f * Mathf.PI);
        float y = m_SinValue * Mathf.Cos(m_AngleXZ / 180.0f * Mathf.PI);
        float z = Mathf.Pow(1 - Mathf.Pow(x, 2) - Mathf.Pow(y, 2), 0.5f) * (Mathf.Abs(Mathf.Sin(m_AngleXZ / 180.0f * Mathf.PI)) / Mathf.Sin(m_AngleXZ / 180.0f * Mathf.PI));
        m_Pos2 = new Vector3(x, y, z) * m_Radius;
        m_Target2.position = m_Pos2;
    }
    void Update()
    {
        //UpdateShperePoint();
        //m_AngleXY = (m_AngleXY + Time.deltaTime * 360) % 360.0f;
        //m_AngleXZ = (m_AngleXZ + (Time.deltaTime * 360 / 2.0f)) % 360.0f;

        MatrixRotation();
        angleX = (angleX + Time.deltaTime * 180.0f) % 360;
        angleY = (angleY + Time.deltaTime * 180.0f / 3) % 360;
        angleZ = (angleZ + Time.deltaTime * 180.0f / 5) % 360;
    }

    public float angleX = 0.0f;
    public float angleY = 0.0f;
    public float angleZ = 0.0f;
    public void MatrixRotation()
    {

        var cosX = Mathf.Cos(angleX / 180 * Mathf.PI);
        var sinX = Mathf.Sin(angleX / 180 * Mathf.PI);

        var cosY = Mathf.Cos(angleY / 180 * Mathf.PI);
        var sinY = Mathf.Sin(angleY / 180 * Mathf.PI);

        var cosZ = Mathf.Cos(angleZ / 180 * Mathf.PI);
        var sinZ = Mathf.Sin(angleZ / 180 * Mathf.PI);

        Matrix4x4 rotXMatrix = new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, cosX, -sinX, 0),
            new Vector4(0, sinX, cosX, 0),
            new Vector4(0, 0, 0, 1));
        Matrix4x4 rotYMatrix = new Matrix4x4(
            new Vector4(cosY, 0, sinY, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-sinY, 0, cosY, 0),
            new Vector4(0, 0, 0, 1));
        Matrix4x4 rotZMatrix = new Matrix4x4(
            new Vector4(cosZ, -sinZ, 0, 0),
            new Vector4(sinZ, cosZ, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1));

        var value = rotYMatrix * rotXMatrix * rotZMatrix * Vector4.one;
        transform.rotation = new Quaternion(value.x / value.w, value.y / value.w, value.z / value.w, 1);
    }
}
