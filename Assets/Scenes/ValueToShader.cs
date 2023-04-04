using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[ExecuteInEditMode]
public class ValueToShader : MonoBehaviour
{
    [SerializeField]
    private Material m_ToMat;
    [SerializeField]
    private bool m_IsPlay = false;

    [HideInInspector]
    public List<RectTransform> m_PointList = new();
    [HideInInspector]
    public List<Vector4> m_ParamsList = new();

    public void Play()
    {
        if (m_IsPlay) return;
        m_IsPlay = true;
        StartDraw();
    }
    public void Stop()
    {
        m_IsPlay = false;
    }
    private async void StartDraw()
    {
        while (m_IsPlay)
        {
            await UniTask.Delay(500);
            List<Vector4> arrParams = new();
            var sizeDelta = GetComponent<RectTransform>().sizeDelta;

            for (int i = 0; i < m_PointList.Count; i++)
            {
                var item = m_PointList[i];
                var param = m_ParamsList[i];
                if (item == null) continue;
                var pos = (item.anchoredPosition + item.sizeDelta * item.pivot) / sizeDelta;
                arrParams.Add(new Vector4(pos.x, pos.y, param.x, param.y));
            }

            m_ToMat?.SetInteger("_ArrLength", arrParams.Count);
            m_ToMat?.SetVectorArray("_ArrPoint", arrParams.ToArray());
        }
    }
}
