using UnityEditor;
using UnityEngine;

public class ServerConfigEditorWindow : EditorWindow
{
    private ServerConfig config;
    private ServerMode mode;
    private string multiplayerIP;
    private string testPlayIP;
    private int port;

    [MenuItem("Tools/Network Configurator")]
    public static void ShowWindow()
    {
        GetWindow<ServerConfigEditorWindow>("Network Config");
    }

    private void OnEnable()
    {
        // 자동으로 Resources 폴더에서 불러옴
        config = Resources.Load<ServerConfig>("ServerConfig");
        if (config == null)
        {
            Debug.LogError("Resources/ServerConfig.asset 파일이 필요해요!");
        }
        else
        {
            mode = config.serverMode;
            testPlayIP = config.TestLocalIp;
            multiplayerIP = config.renderServerIp;
            port = config.port;
        }
    }

    private void OnGUI()
    {
        if (config == null)
        {
            EditorGUILayout.HelpBox("Resources 폴더에 ServerConfig.asset을 만들어주세요!", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("서버 설정", EditorStyles.boldLabel);
        mode = (ServerMode)EditorGUILayout.EnumPopup("모드", mode);

        if (mode == ServerMode.TestLocal)
        {
            testPlayIP = EditorGUILayout.TextField("테스트IP", testPlayIP);
        }
        
        port = EditorGUILayout.IntField("포트", port);

        if (GUILayout.Button("적용하기"))
        {
            config.serverMode = mode;
            config.TestLocalIp = testPlayIP;
            config.port = port;
            EditorUtility.SetDirty(config); // 변경사항 저장
            AssetDatabase.SaveAssets();
            Debug.Log("설정 적용 완료");
        }
    }
}