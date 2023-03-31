using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace B1
{
    public enum EUIAppRoot : ushort
    {
        None,
        Scene,
        App1,
        App2,
        App3,
        System,
        EnumCount,
    }
    public enum EAssetName : ulong
    {
        None = ulong.MinValue,

        #region 预制体
        #region UILogin
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

        #region UISystem
        UGUISystem,
        UINavigationBar,
        #endregion

        #region UIAppPlane
        UIAppPlane,
        #endregion

        #endregion



        #region 图集

        UILobbySpriteAltas,


        #endregion

        EnumCount,
    }
    public enum ESpriteName
    {
        Caiji_cj_famutubiao,
        Caiji_cj_shougetubiao,
        Caiji_cj_wakuangtubiao,
        Caiji_cj_zhipitubiao,
        EnumCount,
    }
    public enum EUIWindowPage : int
    {
        None,
        UILoginPage,
        UILobbyPage,
        UIAppPlanePage,
        UIMapPage,
        EnumCount,
    }
    public enum EScrollViewListItem
    {
        EnumCount,
    }
    public interface IUIWindowPage
    {

    }



    public enum EAssetLable
    {
        Prefab,
        Sprite,
        spriteAtlas,
        EnumCount,
    }


    public interface IAppRoot
    {
        EUIAppRoot AppRoot { get; }
    }
    public interface IOnDestroyAsync
    {
        /// <summary>
        /// 当对象被加载出来首先被调用
        /// </summary>
        /// <returns></returns>
        UniTask OnLoadAsync();
        /// <summary>
        /// 当对象被卸载调用
        /// </summary>
        /// <returns></returns>
        UniTask OnDestroyAsync();
    }
    public enum EUIElementName
    {
        None,


        Tex_Name,
        Tex_Dec,
        Tex_Num,


        Img_Icon,
        Img_Bg,

        Btn_Close,
        Btn_Play,
        Btn_Change,

    }
}
