using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace B1
{
    public class AssetsManager : Singleton<AssetsManager>
    {
        Dictionary<string, (Type type, object assets, List<GameObject> objs)> m_DicAssets = new();
        public async UniTask<T> LoadAsync<T>(string f_Path) where T : class
        {
            var asset = await Addressables.LoadAssetAsync<GameObject>(f_Path);
            #region Console
            var color = asset != null ? "00FF00FF" : "FF0000FF";
            LogWarning($"加载资源  result = {asset != null}   <color=#{color}>path = {f_Path} </color>   ");
            #endregion
            return asset.GetComponent<T>();
        }

        public async UniTask<T> LoadPrefabAsync<T>(string f_Path, Transform f_Parent) where T : MonoBehaviour
        {
            T obj = default(T);
            if (!m_DicAssets.TryGetValue(f_Path, out var value))
            {
                var asset = await LoadAsync<T>(f_Path);
                if (asset != null)
                {
                    obj = GameObject.Instantiate<T>(asset, f_Parent);
                    m_DicAssets.Add(f_Path, (typeof(T), asset, new List<GameObject>()));
                    m_DicAssets[f_Path].objs.Add(obj.gameObject);
                    LogWarning($"加载预制体实例化成功   Component = {typeof(T)}   path = {f_Path}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {f_Path}");
                }
            }
            else if (value.objs.Count > 0 && value.objs[0] != null)
            {
                if (value.objs[0].TryGetComponent(out obj))
                {
                    LogWarning("该资源已经加载过还没有卸载  当前读取的是之前就已经加载过的实例");
                }
                else
                {
                    LogWarning($"该对象没有该组件   object name = {value.objs[0].name}   Component = {typeof(T)}");
                }
            }
            else
            {
                if (value.assets as T != null)
                {
                    foreach (var item in value.objs)
                    {
                        if (item != null && item.gameObject != null) GameObject.Destroy(item.gameObject);
                    }
                    value.objs = new List<GameObject>();
                    obj = GameObject.Instantiate<T>(value.assets as T);
                    value.objs.Add(obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {f_Path}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            return obj;
        }

        public async UniTask<T> LoadPoolPrefabAsync<T>(string f_Path, Transform f_Parent) where T : MonoBehaviour
        {
            T obj = default(T);
            if (m_DicAssets.TryGetValue(f_Path, out var value))
            {
                if (value.assets as T != null)
                {
                    obj = GameObject.Instantiate<T>(value.assets as T, f_Parent);
                    m_DicAssets[f_Path].objs.Add(obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {f_Path}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            else
            {
                obj = await LoadPrefabAsync<T>(f_Path, f_Parent);
                if (obj != null)
                {
                    LogWarning($"加载实例化预制体成功   Component = {typeof(T)}   path = {f_Path}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {f_Path}");
                }
            }
            return obj;
        }

        public async void LoadPrefab<TMono, TUserData>(
            string f_Path, Transform f_Parent, TUserData f_UserData, Action<TMono, TUserData> f_Callback)
                where TMono : MonoBehaviour
        {
            var obj = await LoadPrefabAsync<TMono>(f_Path, f_Parent);
            f_Callback.Invoke(obj, f_UserData);
        }

        public async void LoadPoolPrefab<TMono, TUserData>(
            string f_Path, Transform f_Parent, TUserData f_UserData, Action<TMono, TUserData> f_Callback)
                where TMono : MonoBehaviour
        {
            var obj = await LoadPoolPrefabAsync<TMono>(f_Path, f_Parent);
            f_Callback.Invoke(obj, f_UserData);
        }

        public async UniTask UnloadAsync(string f_Path)
        {
            if (m_DicAssets[f_Path].objs != null)
            {
                UniTask[] tasks = new UniTask[m_DicAssets[f_Path].objs.Count];
                uint index = 0;
                foreach (var item in m_DicAssets[f_Path].objs)
                {
                    var asset = item;
                    tasks[index++] = UniTask.Create(async () =>
                    {
                        await UniTask.Delay(0);
                        if (asset.gameObject != null)
                        {
                            GameObject.Destroy(asset.gameObject);
                        }
                    });
                }
                await UniTask.WhenAll(tasks);
            }
            if (m_DicAssets[f_Path].assets != null)
            {
                Addressables.ClearDependencyCacheAsync(m_DicAssets[f_Path].assets);
            }
            m_DicAssets.Remove(f_Path);
        }
        public async UniTask UnLoadAllAsync()
        {
            UniTask[] tasks = new UniTask[m_DicAssets.Count];
            uint index = 0;
            foreach (var item in m_DicAssets)
            {
                var asset = item;
                tasks[index++] = UniTask.Create(async () =>
                {
                    await UnloadAsync(asset.Key);
                });
            }
            await UniTask.WhenAll(tasks);
        }
    }
}