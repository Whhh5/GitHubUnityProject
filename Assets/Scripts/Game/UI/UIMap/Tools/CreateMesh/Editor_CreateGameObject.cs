using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreateGameObject))]
public class Editor_CreateGameObject : Editor
{
    private CreateGameObject _target = null;

    void OnEnable()
    {
        _target = (CreateGameObject)target;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // ��˵�ĵ�
#if UNITY_EDITOR
        GUILayout.Label("--------- ʹDocumentation ---------");
        _target._documentation = EditorGUILayout.TextArea(_target._documentation, GUILayout.Height(100));
#endif

        EditorGUILayout.Space(10);

        // �������ò���
#if UNITY_EDITOR
        GUILayout.Label("--------- �������� ---------");
        _target._meshSize = (E_MeshSize)EditorGUILayout.EnumPopup("Mesh Size", _target._meshSize);
        switch (_target._meshSize)
        {
            case E_MeshSize.None:
                EditorGUILayout.HelpBox("Please secele mesh size !", MessageType.Error);
                break;
            case E_MeshSize.Normal:
                break;
            case E_MeshSize.Random:
                break;
            case E_MeshSize.Count:
                EditorGUILayout.HelpBox("Please secele mesh size !", MessageType.Error);
                break;
            default:
                break;
        }
        _target._meshCount = EditorGUILayout.LongField("Mesh Count", _target._meshCount);
#endif

        EditorGUILayout.Space(10);

        // ԭ��ƫ����
#if UNITY_EDITOR
        GUILayout.Label("--------- ���ĵ�ƫ�������� ( -1 , 1) ---------");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Origin");
        EditorGUILayout.TextField("X", GUILayout.MaxWidth(20));
        _target._originX = (E_Origin)EditorGUILayout.EnumPopup("", _target._originX, GUILayout.MinWidth(50));
        EditorGUILayout.TextField("Y", GUILayout.MaxWidth(20));
        _target._originY = (E_Origin)EditorGUILayout.EnumPopup("", _target._originY, GUILayout.MinWidth(50));
        EditorGUILayout.TextField("Z", GUILayout.MaxWidth(20));
        _target._originZ = (E_Origin)EditorGUILayout.EnumPopup("", _target._originZ, GUILayout.MinWidth(50));
        EditorGUILayout.EndHorizontal();
        // �Զ���ƫ����
        EditorGUILayout.BeginHorizontal();
        Rect progressRectX = GUILayoutUtility.GetRect(50, 20);
        EditorGUI.ProgressBar(progressRectX, (_target._offsetXYZ.x + 1) / 2, $"X {(_target._offsetXYZ.x + 1) / 2}");
        Rect progressRectY = GUILayoutUtility.GetRect(50, 20);
        EditorGUI.ProgressBar(progressRectY, (_target._offsetXYZ.y + 1) / 2, $"Y {(_target._offsetXYZ.y + 1) / 2}");
        Rect progressRectZ = GUILayoutUtility.GetRect(50, 20);
        EditorGUI.ProgressBar(progressRectZ, (_target._offsetXYZ.z + 1) / 2, $"Z {(_target._offsetXYZ.z + 1) / 2}");
        EditorGUILayout.EndHorizontal();
        // ����  slider
        EditorGUILayout.BeginHorizontal();
        if (_target._originX == E_Origin.Custom)
        {
            _target._offsetXYZ.x = EditorGUILayout.Slider(_target._offsetXYZ.x, -1, 1);
        }
        if (_target._originY == E_Origin.Custom)
        {
            _target._offsetXYZ.y = EditorGUILayout.Slider(_target._offsetXYZ.y, -1, 1);
        }
        if (_target._originZ == E_Origin.Custom)
        {
            _target._offsetXYZ.z = EditorGUILayout.Slider(_target._offsetXYZ.z, -1, 1);
        }
        EditorGUILayout.EndHorizontal();


#endif


        EditorGUILayout.Space(30);


        // Type Module
#if UNITY_EDITOR
        GUILayout.Label("--------- ������������ ---------");
        var gemeotryType = (E_Gemeotry)EditorGUILayout.EnumPopup("Gemeotry Type", _target._gemeotryType);
        _target._gemeotryType = gemeotryType;

        switch (gemeotryType)
        {
            case E_Gemeotry.None | E_Gemeotry.Count:
                break;
            case E_Gemeotry.Box | E_Gemeotry.Circle:
                _target._isLink = EditorGUILayout.Toggle("Is Link", _target._isLink);
                _target._width = EditorGUILayout.LongField("Width", _target._width);
                _target._height = EditorGUILayout.LongField("Height", _target._height);
                break;
            case E_Gemeotry.Sphere:
                _target._radius = EditorGUILayout.LongField("Radius", _target._radius);
                break;
            case E_Gemeotry.Capsule | E_Gemeotry.Cylinder:
                _target._radius = EditorGUILayout.LongField("Radius", _target._radius);
                _target._height = EditorGUILayout.LongField("Height", _target._height);
                break;
            case E_Gemeotry.Plane:
                _target._radius = EditorGUILayout.LongField("Length", _target._radius);
                _target._width = EditorGUILayout.LongField("Width", _target._width);
                break;
            default:
                break;
        }
#endif

        EditorGUILayout.Space(50);

        // Button Module
#if UNITY_EDITOR
        EditorGUILayout.BeginHorizontal();
        var btn_Down_Create = GUILayout.Button("Create");
        var btn_Down_Pause = GUILayout.Button("Pause");
        var btn_Down_ClearUp = GUILayout.Button("Clear Up");
        if (btn_Down_Create && !_target._isButton_Play)
        {
            _target.Button_Create();
        }
        if (btn_Down_Pause)
        {
            _target.Button_Pause();
        }
        if (btn_Down_ClearUp)
        {
            _target.Button_ClearUp();
        }
        EditorGUILayout.EndHorizontal();
        if (_target._isButton_Play)
        {
            EditorGUILayout.HelpBox("Please Waitting Create Finish ......", MessageType.Info);
        }
#endif




        EditorGUILayout.EndVertical();
    }




}
