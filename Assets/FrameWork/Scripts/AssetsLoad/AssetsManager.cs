using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace B1
{
    public class AssetsManager : Singleton<AssetsManager>
    {
        private string GetAssetKey<TKey>(TKey f_Key, EAssetLable f_Lable) where TKey : struct => $"{f_Key}.{f_Lable}";

        Dictionary<string, (Type type, bool isIns, object assets, List<GameObject> objs)> m_DicAssets = new();
        private async UniTask<T> LoadAsync<T>(string f_Key, string f_Lable) where T : class
        {
            List<string> key = new List<string> { f_Key };
            var asset = await Addressables.LoadAssetAsync<GameObject>(f_Key);
            #region Console
            var color = asset != null ? "00FF00FF" : "FF0000FF";
            LogWarning($"加载资源  result = {asset != null}   <color=#{color}>path = {f_Key} </color>   ");
            #endregion
            return asset.GetComponent<T>();
        }

        public async UniTask<(bool result, T obj)> LoadPrefabAsync<T>(EPrefab f_EPrefab, Transform f_Parent) where T : MonoBehaviour
        {
            T obj = default(T);
            bool result = false;
            var dicKey = GetAssetKey(f_EPrefab, EAssetLable.Prefab);
            if (!m_DicAssets.TryGetValue(dicKey, out var value))
            {
                var asset = await LoadAsync<T>(f_EPrefab.ToString(), EAssetLable.Prefab.ToString());
                if (asset != null)
                {
                    result = true;
                    obj = GameObject.Instantiate<T>(asset, f_Parent);
                    m_DicAssets.Add(dicKey, (typeof(T), true, asset, new()));
                    m_DicAssets[dicKey].objs.Add(obj.gameObject);
                    LogWarning($"加载预制体实例化成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {dicKey}");
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
                    value.objs = new List<GameObject>();
                    result = true;
                    obj = GameObject.Instantiate<T>(value.assets as T);
                    value.objs.Add(obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            return (result, obj);
        }

        public async UniTask<T> LoadPoolPrefabAsync<T>(EPrefab f_EPrefab, Transform f_Parent) where T : MonoBehaviour
        {
            T obj = default(T);
            var dicKey = GetAssetKey(f_EPrefab, EAssetLable.Prefab);
            if (m_DicAssets.TryGetValue(dicKey, out var value))
            {
                if (value.assets as T != null)
                {
                    obj = GameObject.Instantiate<T>(value.assets as T, f_Parent);
                    m_DicAssets[dicKey].objs.Add(obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            else
            {
                var result = await LoadPrefabAsync<T>(f_EPrefab, f_Parent);
                if (result.obj != null)
                {
                    obj = result.obj;
                    LogWarning($"加载实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {dicKey}");
                }
            }
            return obj;
        }

        public async void LoadPrefab<TMono, TUserData>(
            EPrefab f_EPrefab, Transform f_Parent, TUserData f_UserData, Action<TMono, TUserData> f_Callback)
                where TMono : MonoBehaviour
        {
            var result = await LoadPrefabAsync<TMono>(f_EPrefab, f_Parent);
            f_Callback.Invoke(result.obj, f_UserData);
        }

        public async void LoadPoolPrefab<TMono, TUserData>(
            EPrefab f_EPrefab, Transform f_Parent, TUserData f_UserData, Action<TMono, TUserData> f_Callback)
                where TMono : MonoBehaviour
        {
            var obj = await LoadPoolPrefabAsync<TMono>(f_EPrefab, f_Parent);
            f_Callback.Invoke(obj, f_UserData);
        }


        /// <summary>
        /// 卸载一个资源或者多个资源  <see langword="if ( f_Obj == null )" />     并且资源存在 则卸载所有使用该资源的对象
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="f_Key"></param>
        /// <param name="f_Obj"></param>
        /// <returns></returns>
        public async UniTask UnloadAsync(string f_Key, GameObject f_Obj)
        {
            if (!m_DicAssets.TryGetValue(f_Key, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Key}     f_Obj = {f_Obj}");
                return;
            }

            // 设置是否卸载实例化的对象
            var baseType = Type.GetType("GameObject");

            // 判断卸载资源类型   单个 or 多个
            if (f_Obj == null)
            {
                // 判断当前资源是否是实例化对象
                if (value.isIns)
                {
                    List<UniTask> tasks = new();
                    foreach (var item in value.objs)
                    {
                        if (item != null)
                        {
                            var obj = item;
                            tasks.Add(UniTask.Create(async () =>
                            {
                                var coms = obj.GetComponents<IOnDestroyAsync>();
                                foreach (var com in coms)
                                {
                                    await com.OnDestroyAsync();
                                }
                                GameObject.Destroy(obj);
                            }));
                        }
                    }
                    await UniTask.WhenAll(tasks);
                }
                value.objs = new();
            }
            // 判断 该对象是否引用了该资源
            else if (value.objs.Contains(f_Obj))
            {
                value.objs.Remove(f_Obj);
                // 判断当前资源是否是实例化对象
                if (value.isIns)
                {
                    var coms = f_Obj.GetComponents<IOnDestroyAsync>();
                    foreach (var com in coms)
                    {
                        await com.OnDestroyAsync();
                    }
                    GameObject.Destroy(f_Obj);
                }
            }
            else
            {
                LogError($"该对象没有引用 当前资源    f_Obj = {f_Obj}     dicKey = {f_Key}  ");
            }

            // 判断是否从内存中移除资源
            if (value.objs.Count <= 0)
            {
                Addressables.ClearDependencyCacheAsync(value.assets);
                m_DicAssets.Remove(f_Key);
            }
        }
        public async UniTask UnloadAsync<TKey>(TKey f_Key, EAssetLable f_Lable, GameObject f_Obj)
            where TKey : struct
        {
            var dicKey = GetAssetKey(f_Key, f_Lable);
            await UnloadAsync(dicKey, f_Obj);
        }

        public async UniTask UnloadAllAsync()
        {
            UniTask[] tasks = new UniTask[m_DicAssets.Count];
            int index = 0;
            foreach (var item in m_DicAssets)
            {
                tasks[index++] = UniTask.Create(async () =>
                    {
                        await UnloadAsync(item.Key, null);
                    });
                
            }
            m_DicAssets = new();
            await UniTask.WhenAll(tasks);
        }
    }
}