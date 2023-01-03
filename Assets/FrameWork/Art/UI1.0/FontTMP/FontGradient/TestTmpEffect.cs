/*******************************************************************
** 文件名: TestTmpEffect.cs
** 版  权: (C) 深圳冰川网络技术有限公司 
** 创建人: 代文鹏 
** 日  期: 2020/xx/xx
** 版  本: 1.0
** 描  述: 
** 应  用: 
**************************** 修改记录 ******************************
** 修改人:  
** 日  期: 
** 描  述: 
********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestTmpEffect : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI textMeshPro2;
    public TextMeshProUGUI textMeshPro3;
    private void Start()
    {
        Material newMat = Material.Instantiate(textMeshPro.fontMaterial);
        newMat.SetColor("_OutlineColor", Color.blue);
        newMat.SetFloat("_OutlineWidth", 0.1f);
        textMeshPro.fontMaterial = newMat;

        Material newMat2 = Material.Instantiate(textMeshPro2.fontMaterial);
        newMat2.SetColor("_OutlineColor", Color.yellow);
        newMat2.SetFloat("_OutlineWidth", 0.1f);
        textMeshPro2.fontMaterial = newMat2;

        Material newMat3 = Material.Instantiate(textMeshPro3.fontMaterial);
        newMat3.SetColor("_OutlineColor", Color.red);
        newMat3.SetFloat("_OutlineWidth", 0.2f);
        textMeshPro3.fontMaterial = newMat2;
    }

    private void Update()
    {

    }

}
