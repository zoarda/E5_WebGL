using System.Collections.Generic;
using Naninovel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Runtime.InteropServices;

public class Lobby : MonoBehaviour
{
    [SerializeField]
    private Button Btn_Exit, Btn_Gallery, Btn_Start, Btn_Setting, Btn_Chapter, Btn_Question;

    WebGLStreamController webGLStreamController;
    private static readonly object lockObj = new object();
    private static Lobby instance;
    public static Lobby Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<Lobby>();

                        // 確保場景中只有一個實例
                        if (FindObjectsOfType<Lobby>().Length > 1)
                        {
                            Debug.LogError("多個 Lobby 實例存在於場景中。");
                        }
                    }
                }
            }
            return instance;
        }
    }
    async void Start()
    {
        StartNani startNani = StartNani.Instance;

        var Player = Engine.GetService<IScriptPlayer>();
        Btn_Exit.onClick.AddListener(() =>
        {
            startNani.CheckPage.SetActive(true);
        });
        //開始遊戲
        Btn_Start.onClick.AddListener(async () =>
        {
            // AudioManager.instance.PlaySFX("Up");
            // OpenPage.SetActive(true);

            CanvasGroup canvasGroup = StartNani.Instance.VideoImage.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            // StartGamePage.SetActive(!StartGamePage.activeSelf);
            startNani.LobbyPage.SetActive(false);
            await Player.PreloadAndPlayAsync("F");
            ICharacterManager actorManager = Engine.GetService<ICharacterManager>();
            actorManager.RemoveAllActors();
            NaniCommandManger.Instance.SpeedButtonClearSpawn();
            // EventSystem.current.SetSelectedGameObject(Btn_Option.gameObject);
        });
        Btn_Gallery.onClick.AddListener(() =>
        {
            startNani.GalleryPage.SetActive(!startNani.GalleryPage.activeSelf);
        });
        //開始頁面選項
        Btn_Chapter.onClick.AddListener(() =>
        {
            // StartGamePage.SetActive(!StartGamePage.activeSelf);
            startNani.SelectOption.SetActive(!startNani.SelectOption.activeSelf);
            // EventSystem.current.SetSelectedGameObject(Btn_C1_VB.gameObject);
        });
        Btn_Setting.onClick.AddListener(() =>
        {
            startNani.GameSettingPage.SetActive(!startNani.GameSettingPage.activeSelf);
            GameSettingPage gameSettingPage = GameSettingPage.Instance;
            gameSettingPage.Init();
        });
    }
}
