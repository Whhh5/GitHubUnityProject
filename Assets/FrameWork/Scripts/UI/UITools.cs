using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1.UI
{
    public enum EUIRoot
    {
        Scene,
        App1,
        App2,
        App3,
        System,
        EnumCount,
    }
    public enum EWindow
    {
        #region UILobby
        UILogin,
        #endregion

        #region UILobby
        UILobby,
        UILobbyItemInfo,
        UILobbyNavigationBar,
        #endregion

        #region UIMap
        UIMap,
        #endregion

        EnumCount,
    }
    public enum EUIWindowPage
    {
        None,
        UILoginPage,
        UILobbyPage,
        UIMapPage,
        UILoginPage1,
        UILobbyPage1,
        UIMapPage1,
        UILoginPage3,
        UILobbyPage3,
        UIMapPage3,
        UILoginPage4,
        UILobbyPage4,
        UIMapPage4,
        UILoginPage5,
        UILobbyPage5,
        UIMapPage5,
        UILoginPage6,
        UILobbyPage6,
        UIMapPage6,
        UILoginPage7,
        UILobbyPage7,
        UIMapPage7,
        UILoginPage8,
        UILobbyPage8,
        UIMapPage8,
        UILoginPage9,
        UILobbyPage9,
        UIMapPage9,
        EnumCount,
    }
    public enum EScrollViewListItem
    {
        EnumCount,
    }
    public interface IUIWindowPage
    {

    }
}
