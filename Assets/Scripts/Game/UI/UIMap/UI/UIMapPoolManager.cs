using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapPoolManager : MonoBehaviour
{
    private void Awake()
    {
        Instance ??= this;
    }
    public static UIMapPoolManager Instance = null;



    /// <summary>
    /// 存放加载的实例对象
    /// </summary>
    [SerializeField]
    public Dictionary<EUIMapAssetType, List<GameObject>> m_AssetsType_Instance = new();

    /// <summary>
    /// 对象池根节点
    /// </summary>
    [SerializeField]
    private RectTransform m_PoolRoot;
    public void PoolPop<T>(in EUIMapAssetType assetType, Transform parent, Action<T> callback) where T : MonoBehaviour
    {
        T retData = default(T);
        if (m_AssetsType_Instance.TryGetValue(assetType, out var value) && value.Count > 0)
        {
            retData = value[0].GetComponent<T>();
            retData.transform.SetParent(parent);
            value.RemoveAt(0);
            retData.gameObject.SetActive(true);
            callback.Invoke(retData);
        }
        else
        {
            UIMapAssetsManager.Instance.LoadGameobjectAssets($"{UIMapPath.m_PrefabPath}{assetType}", parent, callback);
        }
    }
    public void PoolPush(EUIMapAssetType assetType, GameObject obj)
    {
        obj.SetActive(false);
        if (!m_AssetsType_Instance.ContainsKey(assetType))
        {
            m_AssetsType_Instance.Add(assetType, new List<GameObject>());
        }
        m_AssetsType_Instance[assetType].Add(obj);
        obj.transform.SetParent(m_PoolRoot);
    }
    public void ReleasePoolAssets(EUIMapAssetType f_AssetType = EUIMapAssetType.EnumCount)
    {
        if (f_AssetType == EUIMapAssetType.EnumCount)
        {
            var list = m_AssetsType_Instance;
            m_AssetsType_Instance = new();
            foreach (var item in list)
            {
                foreach (var item2 in item.Value)
                    if (item2 != null) GameObject.Destroy(item2);
                UIMapAssetsManager.Instance.UnloadGameObjectAssets($"{UIMapPath.m_PrefabPath}{item.Key}");
            }
        }
        else if (m_AssetsType_Instance.TryGetValue(f_AssetType, out var value))
        {
            foreach (var item2 in value)
                if (item2 != null) GameObject.Destroy(item2);
            UIMapAssetsManager.Instance.UnloadGameObjectAssets($"{UIMapPath.m_PrefabPath}{f_AssetType}");
            m_AssetsType_Instance.Remove(f_AssetType);
        }
    }

    private void OnDestroy()
    {
        ReleasePoolAssets();
    }
}
