using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private string GetAssetKey(EAssetName f_Key, EAssetLable f_Lable) => $"{f_Key}.{f_Lable}";

        /// <summary>
        /// 场景加载的资源列表
        /// </summary>
        Dictionary<string, (Type type, bool isIns, object assets, Dictionary<int, GameObject> objs)> m_DicAssets = new();
        #region 公用方法
        /// <summary>
        /// 加载一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_Key"></param>
        /// <returns></returns>
        private async UniTask<T> LoadAsync<T>(EAssetName f_Key, EAssetLable f_Lable) where T : class
        {
            var asset = await Addressables.LoadAssetAsync<T>(f_Key.ToString());
            #region Console
            var color = asset != null ? "00FF00FF" : "FF0000FF";
            LogWarning($"加载资源  result = {asset != null}   <color=#{color}>path = {f_Key} </color>   ");
            #endregion
            return asset;
        }
        private async UniTask UnLoadAsync<T>(T f_Asset)
        {
            await DelayAsync();
            Addressables.Release<T>(f_Asset);
        }
        /// <summary>
        /// 卸载一个资源
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="f_Key"></param>
        /// <param name="f_Obj"></param>
        /// <returns></returns>
        public async UniTask UnloadAsync(EAssetName f_Asset, EAssetLable f_Lable, int f_Key)
        {
            var disKey = GetAssetKey(f_Asset, f_Lable);
            if (!m_DicAssets.TryGetValue(disKey, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Asset}     f_Key = {f_Key}");
                return;
            }

            
            // 判断 该对象是否引用了该资源
            if (value.objs.ContainsKey(f_Key))
            {
                value.objs.Remove(f_Key);
                // 判断当前资源是否是实例化对象
            }
            else
            {
                LogError($"该对象没有引用 当前资源      dicKey = {disKey}  ");
            }

            // 判断是否从内存中移除资源
            if (value.objs.Count <= 0)
            {
                await UnLoadAsync(value.assets);
                m_DicAssets.Remove(disKey);
            }
        }
        /// <summary>
        /// 卸载一个继承 mono 的预制体资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_Asset"></param>
        /// <param name="f_Lable"></param>
        /// <param name="f_Obj"></param>
        /// <returns></returns>
        public async UniTask UnLoadPrefabAsync<T>(EAssetName f_Asset, EAssetLable f_Lable, T f_Obj) where T : UnityEngine.MonoBehaviour, IOnDestroyAsync
        {
            var disKey = GetAssetKey(f_Asset, f_Lable);
            if (!m_DicAssets.TryGetValue(disKey, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Asset}     f_Obj = {f_Obj}");
                return;
            }

            // 设置是否卸载实例化的对象
            var insID = f_Obj.GetInstanceID();

            // 判断 该对象是否引用了该资源
            if(value.objs.ContainsKey(insID))
            {
                var coms = f_Obj.GetComponents<IOnDestroyAsync>();
                foreach (var com in coms)
                {
                    await com.OnDestroyAsync();
                }
                GameObject.Destroy(f_Obj);


                await UnloadAsync(f_Asset, f_Lable, insID);

            }
        }
        /// <summary>
        /// 卸载一个类型全部资源
        /// </summary>
        /// <param name="f_Asset"></param>
        /// <param name="f_Lable"></param>
        /// <returns></returns>
        public async UniTask UnLoadByTypeAsync(EAssetName f_Asset, EAssetLable f_Lable)
        {
            var disKey = GetAssetKey(f_Asset, f_Lable);
            if (!m_DicAssets.TryGetValue(disKey, out var value))
            {
                LogError($"正在卸载一个没有加载的资源      dicKey = {f_Asset}     f_Lable = {f_Lable}");
                return;
            }
            if (value.isIns)
            {
                List<UniTask> tasks = new();
                foreach (var item in value.objs)
                {
                    var tempItem = item;
                    tasks.Add(UniTask.Create(async () =>
                    {
                        var coms = tempItem.Value.GetComponents<IOnDestroyAsync>();
                        foreach (var com in coms)
                        {
                            await com.OnDestroyAsync();
                        }
                        GameObject.Destroy(tempItem.Value);

                        await UnloadAsync(f_Asset, f_Lable, tempItem.Key);

                    }));
                }
                await UniTask.WhenAll(tasks);
            }
        }

        public async UniTask<(bool tResult, TAsset tObj)> LoadAssetAsync<TAsset>(EAssetName f_Name, EAssetLable f_Lable, int f_key)
            where TAsset : UnityEngine.Object
        {
            
            var dicKey = GetAssetKey(f_Name, f_Lable);
            var resuult = false;
            TAsset obj = null;
            if (m_DicAssets.TryGetValue(dicKey, out var value))
            {
                if (value.assets is TAsset)
                {
                    obj = value.assets as TAsset;
                    if (!value.objs.ContainsKey(f_key))
                    {
                        value.objs.Add(f_key, null);
                    }
                    else
                    {
                        LogWarning("该 key 已经请求过该资源    请检查此处是否合理，合理则忽略该消息");
                    }
                    resuult = true;
                }
                else
                {
                    LogError($"加载资源失败， 传入类型和已经存在的资源类型不符， 已经存在的资源：{value.assets.GetType()},  请求类型：{typeof(TAsset)}");
                }
            }
            else
            {
                var loadRes = await LoadAsync<TAsset>(f_Name, f_Lable);
                if (loadRes !=null)
                {
                    obj = loadRes;

                    m_DicAssets.Add(dicKey, (typeof(TAsset), false, loadRes, new() { { f_key, null } }));

                    resuult = true;
                }
            }

            return (resuult, obj);
        }
        #endregion



        #region 预制体相关
        /// <summary>
        /// 加载一个继承 mono 的预制体, 该方法加载的预制体只能存在一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_EPrefab"></param>
        /// <param name="f_Parent"></param>
        /// <returns></returns>
        public async UniTask<(bool result, T obj)> LoadPrefabAsync<T>(EAssetName f_EPrefab, Transform f_Parent = null) where T : MonoBehaviour
        {
            T obj = default(T);
            bool result = false;
            var dicKey = GetAssetKey(f_EPrefab, EAssetLable.Prefab);
            if (!m_DicAssets.TryGetValue(dicKey, out var value))
            {
                var asset = await LoadAsync<GameObject>(f_EPrefab, EAssetLable.Prefab);
                var com = asset?.GetComponent<T>();
                if (com != null)
                {
                    result = true;
                    obj = GameObject.Instantiate<T>(com, f_Parent);
                    m_DicAssets.Add(dicKey, (typeof(T), true, asset, new()));
                    m_DicAssets[dicKey].objs.Add(obj.gameObject.GetInstanceID(), obj.gameObject);
                    LogWarning($"加载预制体实例化成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {dicKey}");
                }
            }
            else if (value.objs.Count > 0)
            {
                var firstElement = value.objs.First();
                if (firstElement.Value.TryGetComponent(out obj))
                {
                    LogWarning("该资源已经加载过还没有卸载  当前读取的是之前就已经加载过的实例");
                }
                else
                {
                    LogWarning($"该对象没有该组件   id = {firstElement.Key}  object name = {firstElement.Value?.name}   Component = {typeof(T)}");
                }
            }
            else
            {
                if (value.assets as T != null)
                {
                    result = true;
                    obj = GameObject.Instantiate<T>(value.assets as T);
                    value.objs.Add(obj.gameObject.GetInstanceID(), obj.gameObject);
                    LogWarning($"实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"加载类型不匹配  type = {typeof(T)}   value type = {value.type}");
                }
            }
            return (result, obj);
        }
        /// <summary>
        /// 该方法加载预制体允许存在多个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_EPrefab"></param>
        /// <param name="f_Parent"></param>
        /// <returns></returns>
        public async UniTask<T> LoadPrefabsAsync<T>(EAssetName f_EPrefab, Transform f_Parent = null) where T : MonoBehaviour, IOnDestroyAsync
        {
            T obj = default(T);
            var dicKey = GetAssetKey(f_EPrefab, EAssetLable.Prefab);
            if (m_DicAssets.TryGetValue(dicKey, out var value))
            {
                if (value.assets as T != null)
                {
                    obj = GameObject.Instantiate<T>(value.assets as T, f_Parent);
                    await obj.OnLoadAsync();
                    m_DicAssets[dicKey].objs.Add(obj.gameObject.GetInstanceID(), obj.gameObject);
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
                if (result.result)
                {
                    obj = result.obj;
                    await obj.OnLoadAsync();
                    LogWarning($"加载实例化预制体成功   Component = {typeof(T)}   path = {dicKey}");
                }
                else
                {
                    LogWarning($"资源加载失败  asset path = {dicKey}");
                }
            }
            return obj;
        }


        
        #endregion
    }
}