using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapChunk : MonoBehaviour
{
    [SerializeField]
    private Image m_Icon;
    public Image Image => m_Icon;
    [SerializeField]
    public uint m_Index;
    [SerializeField]
    public Vector2Int m_ChunkInfo;
    [SerializeField]
    public string m_AssetPath = "";
    public void SetSettings(uint f_Index, Vector2Int f_ChunkInfo, string f_AssetPath)
    {
        m_Index = f_Index;
        m_ChunkInfo = f_ChunkInfo;
        m_AssetPath = f_AssetPath;
    }
}
