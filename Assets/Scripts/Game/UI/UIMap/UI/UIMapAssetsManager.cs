using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class UIMapAssetsManager : MonoBehaviour
{
    public static UIMapAssetsManager Instance = null;
    private void Awake()
    {
        Instance ??= this;
    }

    #region Tools
    public void Log(object message)
    {
        Debug.Log($"【 {GetType()} 】: {message}");
    }
    public void ConsoleAssetDictionary()
    {
        Profiler.BeginSample("console debuger");
        string message = default(string);
        uint index = default(uint);
        foreach (var spriteAsset in m_SpriteAssets)
        {
            message += $"\n[ {index++} ] =" +
                $"\n{{" +
                $"\n\tAssets Path        = {spriteAsset.Key}" +
                $"\n\tSprite Name        = {spriteAsset.Value.sprite.name}" +
                $"\n\tSprite References  = ";
            uint referenceIndex = 0;
            foreach (var reference in spriteAsset.Value.list)
            {
                message += $"\n\t\t[ {referenceIndex++} ]  = {reference}";
            }
            message += $"\n}}";
        }
        Log(message);
        Profiler.EndSample();
    }
    #endregion

    public Dictionary<string, (Sprite sprite, int instanceID, List<string> list)> m_SpriteAssets = new();
    public Dictionary<string, GameObject> m_GameobjecAssets = new();



    #region 外部依赖
    private T LoadAsync<T>(string f_Path) where T : UnityEngine.Object
    {
        T retDate = default(T);
        var request = Resources.LoadAsync<T>(f_Path);
        if (request.asset != null)
        {
            retDate = request.asset as T;
        }
        return retDate;
    }
    private void UnLoadAsync<T>(T f_Assets) where T : UnityEngine.Object
    {
        Resources.UnloadAsset(f_Assets);
    }
    #endregion

    public void LoadSpriteAssets(Image image, string layer, string assetPath)
    {
        Sprite sprite;
        if (m_SpriteAssets.TryGetValue(assetPath, out var value))
        {
            sprite = value.sprite;
            if (!value.list.Contains(layer))
            {
                value.list.Add(layer);

                Log($"成功加载资源 layer = {layer}, assetPath = {assetPath}");
                ConsoleAssetDictionary();
            }
            else
            {
                Log($"上次加载的资源没有释放 重复加载 layer = {layer}");
            }
        }
        else
        {
            sprite = LoadAsync<Sprite>(assetPath);

            if (sprite != null)
            {
                m_SpriteAssets.Add(assetPath, (sprite, sprite.GetInstanceID(), new List<string>()));
                m_SpriteAssets[assetPath].list.Add(layer);

                Log($"成功加载资源 layer = {layer}, assetPath = {assetPath}");
                ConsoleAssetDictionary();
            }
            else
            {
                Log($"图片资源加载失败 layer = {layer}, assetPath = {assetPath}");
            }
        }

        if (image != null)
        {
            image.sprite = sprite;
        }
        else
        {
            UnLoadSpriteAssets(layer, assetPath);
        }
    }
    public void UnLoadSpriteAssets(string layer, string assetPath)
    {
        if (m_SpriteAssets.TryGetValue(assetPath, out var value))
        {
            if (value.list.Contains(layer))
            {
                value.list.Remove(layer);
                if (value.list.Count <= 0)
                {
                    UnLoadAsync(value.sprite);
                    m_SpriteAssets.Remove(assetPath);
                }
                Log($"成功 移除资源引用  layer = {layer}, assetPath = {assetPath}");
                ConsoleAssetDictionary();
            }
            else
            {
                Log($"移除资源引用 失败 不存在 Layer layer = {layer}");
            }
        }
        else
        {
            Log($"资源引用 不存在 assetPath = {assetPath}");
        }
    }

    public void LoadGameobjectAssets<T>(string assetPath, Transform parent, Action<T> callback) where T : MonoBehaviour
    {
        T retData = default(T);
        if (m_GameobjecAssets.TryGetValue(assetPath, out var value))
        {
            retData = value.GetComponent<T>();
        }
        else
        {
            var request = LoadAsync<GameObject>(assetPath);
            if (request != null)
            {
                retData = request.GetComponent<T>();
                m_GameobjecAssets.Add(assetPath, request);
            }
            else
            {
                Log($"资源为空  assetPath = {assetPath}");
                return;
            }
        }
        retData = GameObject.Instantiate<T>(retData, parent);
        retData.gameObject.SetActive(true);
        callback.Invoke(retData);
    }
    public void UnloadGameObjectAssets(string f_AssetPath)
    {
        if (m_GameobjecAssets.TryGetValue(f_AssetPath, out var obj) && obj != null)
        {
            UnLoadAsync<GameObject>(obj.gameObject);
            m_GameobjecAssets.Remove(f_AssetPath);
        }
        else
        {
            Log($"卸载 game object 不存在   path = {f_AssetPath}   obj = {obj}");
        }
    }

    private void OnDestroy()
    {
        foreach (var item in m_GameobjecAssets)
        {
            if (item.Value != null) UnLoadAsync(item.Value);
        }
        m_GameobjecAssets = new Dictionary<string, GameObject>();

        foreach (var item in m_SpriteAssets)
        {
            if (item.Value.sprite != null) UnLoadAsync(item.Value.sprite);

        }
        m_SpriteAssets = new();
        Instance = null;
    }


    public void ReleaseUnuseSpritedAssets()
    {
        List<string> removeKeys = new();
        foreach (var item in m_SpriteAssets)
        {
            if (item.Value.list.Count <= 0)
            {
                UnLoadAsync(item.Value.sprite);
                removeKeys.Add(item.Key);
                Log($"成功卸载资源  assetPath = {item.Key}");
            }   
        }

        foreach (var item in removeKeys)
        {
            m_SpriteAssets.Remove(item);
        }
    }
}
