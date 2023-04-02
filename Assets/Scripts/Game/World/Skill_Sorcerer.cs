using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Skill_Sorcerer", menuName = "Skill Box/Sorcerer")]
public class Skill_Sorcerer : SkillBase
{
    [SerializeField]
    private Transform m_Tran = null;


    [SerializeField]
    private Transform m_Item;
    [SerializeField]
    private Material m_ItemMat;


    [SerializeField, Range(10, 100)]
    private ushort m_ItemCount = 20;
    [SerializeField, Range(0.1f, 0.5f)]
    private float m_IntervalTime = 0.1f;
    [SerializeField, Range(0.2f, 10)]
    private float m_Speed = 1;
    [SerializeField, Range(2, 10)]
    private float m_Radius = 5;


    public void InitParam(Transform f_Tran)
    {
        m_Tran = f_Tran;
    }

    [Button]
    public override async void StartExecute()
    {

        UniTask[] tasks = new UniTask[m_ItemCount];

        for (int i = 0; i < m_ItemCount; i++)
        {
            var index = i;
            tasks[index] = UniTask.Create(async () =>
                {
                    var item = GameObject.Instantiate(m_Item, m_Tran);
                    var itemMat = GameObject.Instantiate(m_ItemMat);
                    m_DicItem.Add(item, itemMat);

                    item.GetComponent<MeshRenderer>().material = itemMat;
                });
        }

        await UniTask.WhenAll(tasks);
    }

    [Button]
    public override void StopExecube()
    {
        Debug.Log($"释放完成");

        foreach (var item in m_DicItem)
        {
            GameObject.DestroyImmediate(item.Value);
            GameObject.DestroyImmediate(item.Key.gameObject);
        }
        m_DicItem.Clear();
    }

    [SerializeReference]
    Dictionary<Transform, Material> m_DicItem = new();
}
