using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1
{

    
    public enum EAssetLable
    {
        Prefab,
        Sprite,
        EnumCount,
    }


    public interface IOnDestroyAsync
    {
        UniTask OnDestroyAsync();
    }
}
