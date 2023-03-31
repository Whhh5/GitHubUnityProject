using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


/// <summary>
/// 品级
/// </summary>
public enum EEuality : int
{
    None = 0,
    /// <summary>
    /// 稀有度
    /// </summary>
    Rarity = None + 1 << 0,
    /// <summary>
    /// 普通
    /// </summary>
    Common = Rarity + 1 << 1,
    /// <summary>
    /// 高级
    /// </summary>
    Advanced = Common + 1 << 2,
    /// <summary>
    /// 稀有
    /// </summary>
    Rare = Advanced + 1 << 3,
    /// <summary>
    /// 传说
    /// </summary>
    Legend = Rare + 1 << 4,
    /// <summary>
    /// 史诗
    /// </summary>
    Epic = Legend + Legend + 1 << 5,
}
/// <summary>
/// 稀有度
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
    /// 名字
    /// </summary>
    [SerializeField]
    protected string b_Name;
    /// <summary>
    /// 品级
    /// </summary>
    [SerializeField]
    protected EEuality b_Euality;
    /// <summary>
    /// 稀有度
    /// </summary>
    [SerializeField]
    protected ERarity b_Rarity;
    /// <summary>
    /// 冷却时间
    /// </summary>
    [SerializeField]
    protected float b_CollingTime;
    /// <summary>
    /// 元素
    /// </summary>
    [SerializeField]
    protected float b_ChemicalElement;
    /// <summary>
    /// 攻击目标层级
    /// </summary>
    [SerializeField]//[SerializeField, EnumToggleButtons]
    protected LayerMask b_AttackLayer;


}
