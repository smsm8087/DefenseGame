using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket.Models;
using UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nicknameInput; // 회원가입 전용
    public Button loginButton;
    public Button signupButton;
    public ToastMessage toastMessage;
    private bool ui_lock = false;

    private async void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
        signupButton.onClick.AddListener(() => StartCoroutine(Signup()));
        await WebSocketClient.Instance.TryConnect();
        GameDataManager.Instance.LoadAllData();
    }

    void showMessage(string msg)
    {
        toastMessage.HideToast();
        toastMessage.ShowToast(msg);
    }
    
    IEnumerator Login()
    {
        if (ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "username", usernameInput.text },
            { "password", passwordInput.text }
        };

        yield return ApiManager.Instance.Post(
            "auth/login",
            data,
            onSuccess: (res) =>
            {
                var loginRes = JsonUtility.FromJson<ApiResponse.LoginResponse>(res);
                UserSession.Set(loginRes.userId, loginRes.nickname);
                SceneLoader.Instance.LoadScene("LobbyScene");
            },
            onError: (err) =>
            {
                showMessage($"로그인 실패: {err}");
            }
        );
        ui_lock = false;
    }

    IEnumerator Signup()
    {
        if (ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "username", usernameInput.text },
            { "password", passwordInput.text },
            { "nickname", nicknameInput.text }
        };

        yield return ApiManager.Instance.Post(
            "auth/signup",
            data,
            onSuccess: (res) =>
            {
                var loginRes = JsonUtility.FromJson<ApiResponse.LoginResponse>(res);
                UserSession.Set(loginRes.userId, loginRes.nickname);
                SceneLoader.Instance.LoadScene("LobbyScene");
            },
            onError: (err) =>
            {
                showMessage( $"회원가입 실패: {err}");
            }
        );
        ui_lock = false;
    }
}