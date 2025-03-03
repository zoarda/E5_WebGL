using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LobbyCheck : MonoBehaviour
{
    public Text TxtCheck, Txtlobbyquit, Txtlobbyback;
    public string check, quit, back;
    public Button lobbyquit, lobbyback;
    [DllImport("__Internal")]
    private static extern void Back();
    void Awake()
    {
        StartNani startNani = StartNani.Instance;
        lobbyquit.onClick.AddListener(() =>
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // Debug.Log("shrimp: ready to SendMessage");
                // // Example 1: 發送 'send:message' 事件
                // SendMessageToParent("open");

                // Example 2: 發送 'openurl' 事件
                Debug.Log("shrimp: back");
                Back();

                // // Example 3: 發送 'reload' 事件
                // ReloadPage();
                // Debug.Log("shrimp: SendMessage succese");
            }
            else
            {
                Application.Quit();
            }
        });
        lobbyback.onClick.AddListener(() =>
        {
            startNani.CheckPage.SetActive(!startNani.CheckPage.activeSelf);
        });
        TxtCheck.text = LanguageManager.Instance.GetLanguageValue(check);
        Txtlobbyback.text = LanguageManager.Instance.GetLanguageValue(back);
        Txtlobbyquit.text = LanguageManager.Instance.GetLanguageValue(quit);
    }

}
