
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using B1;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class EditorEditor
{
    private static readonly Type kToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    private static ScriptableObject sCurrentToolbar;


    static EditorEditor()
    {
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        if (sCurrentToolbar == null)
        {
            UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(kToolbarType);
            sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
            if (sCurrentToolbar != null)
            {
                FieldInfo root = sCurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                VisualElement concreteRoot = root.GetValue(sCurrentToolbar) as VisualElement;

                VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                VisualElement parent = new VisualElement()
                {
                    style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                };
                IMGUIContainer container = new IMGUIContainer();
                container.onGUIHandler += OnGuiBody;
                parent.Add(container);
                toolbarZone.Add(parent);
            }
        }

    }

    private static void OnGuiBody()
    {
        //自定义按钮加在此处
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Debuger", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<DebugerWindow>())
            {
                var window = EditorWindow.CreateWindow<DebugerWindow>();
                window.Show();
            }
        }
        if (GUILayout.Button(new GUIContent("My Node Editor", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            var window = EditorWindow.GetWindow<MyNodeEditor>();

            window.titleContent = new GUIContent("My Node Editor");
        }
        GUILayout.EndHorizontal();
    }
}



public class DebugerWindow : EditorWindow
{
    private void OnGUI()
    {
        //自定义按钮加在此处
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Asset Manager", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<AssetManagerWindow>())
            {
                var window = EditorWindow.CreateWindow<DebugerWindow>();
                window.Show();
            }
        }
        if (GUILayout.Button(new GUIContent("UI Window Manager", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<UIWindowManagerWindow>())
            {
                var window = EditorWindow.CreateWindow<UIWindowManagerWindow>();
                window.Show();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Property Window", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<PropertyWindow>())
            {
                var window = EditorWindow.CreateWindow<PropertyWindow>();
                window.Show();
            }
        }
        GUILayout.EndHorizontal();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

public class AssetManagerWindow : EditorWindow
{
    Dictionary<string, (Type type, bool isIns, object assets, Dictionary<int, GameObject> objs)> m_DicAssets = null;

    private void OnEnable()
    {
        var type = typeof(AssetsManager)?.GetField("m_DicAssets", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);

        if (type != null)
        {
            m_DicAssets = type.GetValue(AssetsManager.Instance) as Dictionary<string, (Type type, bool isIns, object assets, Dictionary<int, GameObject> objs)>;
        }
    }
    private void OnDisable()
    {
        m_DicAssets = null;
    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }


    Vector2 m_ScrollV2Pos = Vector2.zero;
    Vector2 m_ScrollV2Pos2 = Vector2.zero;
    int m_LastCount = 0;
    string m_DebugStr = "";
    private void OnGUI()
    {
        if (m_DicAssets == null) return;

        EditorGUILayout.BeginHorizontal();
        m_ScrollV2Pos = EditorGUILayout.BeginScrollView(m_ScrollV2Pos, GUILayout.Width(position.width * 0.8f), GUILayout.Height(position.height));


        EditorGUILayout.LabelField($"Count: {m_DicAssets.Count}");
        EditorGUILayout.Space(20);


        EditorGUILayout.BeginHorizontal();

        foreach (var item in m_DicAssets)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"key: {item.Key}");
            EditorGUILayout.LabelField($"Type: {item.Value.type}");
            EditorGUILayout.LabelField($"Is Ins: {item.Value.isIns}");
            EditorGUILayout.LabelField($"Asset: {item.Value.assets}");
            EditorGUILayout.LabelField($"List: {item.Value.objs.Count}");

            foreach (var obj in item.Value.objs)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"\t[{obj.Key}]:", GUILayout.Width(100));
                EditorGUILayout.ObjectField(obj.Value, typeof(GameObject));

                EditorGUILayout.EndHorizontal();
            }



            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();






        m_ScrollV2Pos2 = EditorGUILayout.BeginScrollView(m_ScrollV2Pos2, GUILayout.Width(position.width * 0.2f), GUILayout.Height(position.height));


        if (m_DicAssets.Count != m_LastCount)
        {

            m_LastCount = m_DicAssets.Count;

            AddLog($"{System.DateTime.Now}: {m_LastCount}");

        }
        EditorGUILayout.TextArea(m_DebugStr);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }


    void AddLog(object f_Message)
    {
        m_DebugStr += $"{f_Message}\n";
    }

}


public class UIWindowManagerWindow : EditorWindow
{
    DicStack<Type, UIWindowPage> m_PageStack = null;

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        m_PageStack = null;
    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }


    Vector2 m_ScrollV2Pos = Vector2.zero;
    Vector2 m_ScrollV2Pos2 = Vector2.zero;
    int m_LastCount = 0;
    EUIWindowPage m_search = EUIWindowPage.None;
    private void OnGUI()
    {
        if (UIWindowManager.Instance != null && m_PageStack == null)
        {
            var type = typeof(UIWindowManager)?.GetField("m_PageStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
            m_PageStack = type.GetValue(UIWindowManager.Instance) as DicStack<Type, UIWindowPage>;
        }

        if (m_PageStack == null) return;

        EditorGUILayout.BeginScrollView(m_ScrollV2Pos, GUILayout.Width(position.width), GUILayout.Height(position.height));




        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField($"Count = {m_PageStack.Count}");

        EditorGUILayout.LabelField($"搜索：", GUILayout.Width(100));
        m_search = (EUIWindowPage)EditorGUILayout.EnumPopup(m_search);


        EditorGUILayout.EndHorizontal();







        EditorGUILayout.BeginHorizontal();
        var fieldInfo = typeof(UIWindowPage).GetField("m_WindowStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
        var getWindow = typeof(UIWindowPage).GetMethod("GetWindowAsync", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default);
        var index = 0;
        foreach (var item in m_PageStack)
        {
            if (m_search != EUIWindowPage.None && m_search != item.Value.CurPage)
            {
                continue;
            }
            var windowStack = fieldInfo.GetValue(item.Value) as ListStack<EAssetName>;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"[{index++}]Page: {item.Value.CurPage}");


            EditorGUILayout.BeginVertical();
            foreach (var element in windowStack.GetEnumerator())
            {
                var window = getWindow.Invoke(item.Value, new object[] { element.Value }) as UIWindow;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"\t[{element.Key}] = ", GUILayout.Width(100));
                EditorGUILayout.ObjectField(window, window?.GetType());

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.EndScrollView();
    }
}






public class PropertyWindow : EditorWindow
{
    enum EClass
    {
        None,
        UIWindow,
        UILobby,
        
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }


    Vector2 m_ScrollV2Pos = Vector2.zero;
    Vector2 m_ScrollV2Pos2 = Vector2.zero;



    EClass m_ClassType = EClass.None;
    Object m_Class = null;
    Type m_Type = null;
    private void OnGUI()
    {
        m_ScrollV2Pos = EditorGUILayout.BeginScrollView(m_ScrollV2Pos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        EditorGUILayout.BeginHorizontal();
        var typeStr = EditorGUILayout.TextField($"{m_Type}");
        m_ClassType = (EClass)EditorGUILayout.EnumPopup(m_ClassType);
        EditorGUILayout.EndHorizontal();

        m_Type = Type.GetType(m_ClassType.ToString());
        if (m_Type == null)
        {
            m_Type = Type.GetType(typeStr);
        }
        if (m_Type != null)
        {
            m_Class = EditorGUILayout.ObjectField(m_Class, m_Type);

            if (m_Class != null)
            {
                var curType = m_Type;
                do
                {
                    EditorGUILayout.Space(20);
                    EditorGUILayout.LabelField($" ---- {curType} ----- ");

                    var fields = curType.GetFields(
                            BindingFlags.IgnoreCase
                            | BindingFlags.DeclaredOnly
                            | BindingFlags.Instance
                            | BindingFlags.Static
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.FlattenHierarchy
                            | BindingFlags.InvokeMethod
                            | BindingFlags.CreateInstance
                            | BindingFlags.GetField
                            | BindingFlags.SetField
                            | BindingFlags.GetProperty
                            | BindingFlags.SetProperty
                            | BindingFlags.PutDispProperty
                            | BindingFlags.PutRefDispProperty
                            | BindingFlags.ExactBinding
                            | BindingFlags.SuppressChangeType
                            | BindingFlags.OptionalParamBinding
                            | BindingFlags.IgnoreReturn
                            | BindingFlags.DoNotWrapExceptions
                        );

                    foreach (var field in fields)
                    {
                        var value = field.GetValue(m_Class);

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(field.Name, GUILayout.Width(100));
                        EditorGUILayout.LabelField($"{value}", GUILayout.Width(100));


                        EditorGUILayout.EndHorizontal();
                    }

                    curType = curType.BaseType;

                } while (curType != null);


            }
        }




        EditorGUILayout.EndScrollView();
    }
}

public class BehaviourWindow: EditorWindow
{
    private void OnInspectorUpdate()
    {
        Repaint();
    }




    Vector2 m_ScrollPoint;
    Vector2 ViewSize => position.size;


    
    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal(); // 1



        m_ScrollPoint = EditorGUILayout.BeginScrollView(m_ScrollPoint, GUILayout.Width(ViewSize.x), GUILayout.Height(ViewSize.y)); // 1 - 1


        EditorGUILayout.BeginVertical(); //  1 - 1 - 1


        if (GUILayout.Button("Create Point"))
        {

        }


        //EditorGUILayout.b




        EditorGUILayout.EndVertical(); // 1 - 1 - 1



        EditorGUILayout.EndScrollView(); // 1 - 1







        m_ScrollPoint = EditorGUILayout.BeginScrollView(m_ScrollPoint, GUILayout.Width(ViewSize.x), GUILayout.Height(ViewSize.y)); // 1 - 2



        EditorGUILayout.BeginHorizontal(); // 1 - 2 - 1


        EditorGUILayout.BeginVertical(); // 1 - 2 - 1 - 1





        EditorGUILayout.EndVertical(); // 1 - 2 - 1 - 1


        EditorGUILayout.BeginVertical(); //  1 - 2 - 1 - 2


        EditorGUILayout.EndVertical(); // 1 - 2 - 1 - 2




        EditorGUILayout.EndHorizontal(); // 1 - 2 - 1



        EditorGUILayout.EndScrollView(); // 1 - 2


        EditorGUILayout.BeginVertical(); // 1
    }
}