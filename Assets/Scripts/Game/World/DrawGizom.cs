using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizom : MonoBehaviour
{
    public static DrawGizom Instance = null;
    private void Awake()
    {
        Instance = this;
    }


    private Dictionary<int, List<(Vector3 tFrom, Vector3 tTo)>> m_DicLine = new();




    public void AddLine(int f_Key, Vector3 f_From, Vector3 f_To)
    {
        if (!m_DicLine.ContainsKey(f_Key))
        {
            m_DicLine.Add(f_Key, new());
        }

        if (!m_DicLine[f_Key].Contains((f_From, f_To)))
        {
            m_DicLine[f_Key].Add((f_From, f_To));
        }
    }




    private void OnDrawGizmos()
    {
        foreach (var item in m_DicLine)
        {
            foreach (var line in item.Value)
            {
                Gizmos.DrawLine(line.tFrom, line.tTo);
            }
        }
    }



    private void OnDrawGizmosSelected()
    {

    }
}
