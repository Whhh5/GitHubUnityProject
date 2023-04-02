using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using B1.Event;

public class UIAppPlanePage : UIWindowPage, IMessageSystem
{
    protected override EAssetName SpriteAltas => EAssetName.None;

    public void ReceptionEvent(EEvent f_Event, object f_Param, object f_UserData, string f_SubDesc, string f_SendDesc)
    {
        switch (f_Event)
        {
            case EEvent.None:
                break;
            case EEvent.UI_WINDOW_LOAD_FINISH:
                Log($"{f_Param}   {f_UserData}  {f_SubDesc}  {f_SendDesc}");
                break;
            default:
                break;
        }
    }

    public Dictionary<EEvent, List<(object tUserdata, string tDesc)>> SubscribeList()
    {
        return new()
        {
            {
                EEvent.UI_WINDOW_LOAD_FINISH, new() { ($"{Random.Range(0, 100)}", "UIAppPlanePage") }
            },
        };
    }

    protected override List<EAssetName> GetWindowNameAsync()
    {
        return new List<EAssetName>()
        {
            EAssetName.UIAppPlane,
        };
    }
}
