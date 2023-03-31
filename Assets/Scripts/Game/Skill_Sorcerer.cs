using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill_Sorcerer", menuName = "Skill Box/Sorcerer")]
public class Skill_Sorcerer : SkillBase
{
    [SerializeField]
    private Transform m_Tran = null;



    public void InitParam(Transform f_Tran)
    {
        m_Tran = f_Tran;
    }
    public void StartExecute()
    {

    }
    public void StopExecube()
    {

    }
}
