using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


/// <summary>
/// Ʒ��
/// </summary>
public enum EEuality : int
{
    None = 0,
    /// <summary>
    /// ϡ�ж�
    /// </summary>
    Rarity = None + 1 << 0,
    /// <summary>
    /// ��ͨ
    /// </summary>
    Common = Rarity + 1 << 1,
    /// <summary>
    /// �߼�
    /// </summary>
    Advanced = Common + 1 << 2,
    /// <summary>
    /// ϡ��
    /// </summary>
    Rare = Advanced + 1 << 3,
    /// <summary>
    /// ��˵
    /// </summary>
    Legend = Rare + 1 << 4,
    /// <summary>
    /// ʷʫ
    /// </summary>
    Epic = Legend + Legend + 1 << 5,
}
/// <summary>
/// ϡ�ж�
/// </summary>
public enum ERarity : int
{
    None = 0,
    Rarity1,
    Rarity2,
    Rarity3,
    Rarity4,
    Rarity5,
    Rarity6,

}
public abstract class SkillBase : ScriptableObject
{
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    protected string b_Name;
    /// <summary>
    /// Ʒ��
    /// </summary>
    [SerializeField]
    protected EEuality b_Euality;
    /// <summary>
    /// ϡ�ж�
    /// </summary>
    [SerializeField]
    protected ERarity b_Rarity;
    /// <summary>
    /// ��ȴʱ��
    /// </summary>
    [SerializeField]
    protected float b_CollingTime;
    /// <summary>
    /// Ԫ��
    /// </summary>
    [SerializeField]
    protected float b_ChemicalElement;
    /// <summary>
    /// ����Ŀ��㼶
    /// </summary>
    [SerializeField]//[SerializeField, EnumToggleButtons]
    protected LayerMask b_AttackLayer;




    public abstract void StartExecute();
    public abstract void StopExecube();

}
