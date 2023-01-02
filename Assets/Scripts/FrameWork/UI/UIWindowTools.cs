using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1.UIWindow
{
    public enum EUIRoot
    {
        App1,
        App2,
        App3,
        System,
        EnumCount,
    }
    public enum EWindow
    {
        UILogin,
        UILobby,
        UIMap,
        EnumCount,
    }
    public interface IUIWindowPage
    {

    }
}
