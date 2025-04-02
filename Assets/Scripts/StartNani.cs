using System.Collections.Generic;
using Naninovel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Runtime.InteropServices;
public class StartNani : MonoBehaviour
{
    [Header("Page")]

    public GameObject GalleryPage, LobbyPage,
    VideoImage, ErrorPage, ChapterPage, OpenPage, CheckPage,
    InGameCheckPage, OptionPage, GameSettingPage;
    public GameObject PreLanguageToggle, SelectOption, BlackBg, LanguageToggle;
    [Header("Scripts")]
    public List<float> friednshipList;
    [SerializeField] private SubtitlesManager subtitlesManager;
    [SerializeField] private LanguageManager LanguageManager;
    [Header("Button")]
    public Button buttonController;
    [SerializeField]
    public Button Btn_SpeedViewBack, Btn_ChoiceViewBack, Btn_Option,
    Btn_EndGame, Btn_Artist, Btn_OK, Btn_No, Btn_InGameOK, Btn_InGameNo, Btn_Language, Btn_OptionReturn, Btn_LanguageReturn, Btn_ArtistReturn, Btn_Error,
     Btn_SelectOption, Btn_Return, Btn_StartSelect, Btn_StartGame, Btn_GameSetting;
    public Button videoSkip;
    //  Btn_C1_VB, Btn_C2_VB, Btn_C3_VB, Btn_C4_VB, Btn_C5_VB,
    [Header("Image")]
    [SerializeField] public Image BG;
    [Header("Animator")]
    [SerializeField] private Animator ErrorAnimator;
    [Header("Toggle")]
    [SerializeField] public Toggle Btn_PlayPause;
    [SerializeField] private ToggleGroup LanguageToggleGroup;
    [Header("Materiali")]
    [SerializeField] private Material material1, material2;
    public int choiceTime;
    public int languageIndex = 0;
    public string lastChoiceName;
    public Text txtSubtitles;

    // ChapterPage chapterPage;

    [SerializeField] private List<Toggle> toggles;

    WebGLStreamController webGLStreamController;
    public static StartNani Instance { get; private set; }
    [SerializeField] private List<string> Language = new();
    // 定義與 JavaScript 函數的交互接口
    // [DllImport("__Internal")]
    // private static extern void SendMessageToParent(string message);

    // [DllImport("__Internal")]
    // private static extern void OpenUrl(string url);
    // [DllImport("__Internal")]
    // private static extern void Back();

    // [DllImport("__Internal")]
    // private static extern void ReloadPage();
    // public Texture2D cursor;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SelectOption.activeSelf || OptionPage.activeSelf || CheckPage.activeSelf || LanguageToggleGroup.gameObject.activeSelf)
                return;
            foreach (var image in BG.GetComponentsInChildren<Image>())
            {
                image.enabled = !image.enabled;
            }
            foreach (var text in BG.GetComponentsInChildren<Text>())
            {
                text.enabled = !text.enabled;
            }
        }
    }
    async void Start()
    {
        // OpenPageMessage();
        // if (Application.platform == RuntimePlatform.WebGLPlayer)
        // await ServerManager.Instance.InitializeUrlQueryAsync();
        Init();
        await subtitlesManager.Init();
        NaniCommandManger naniCommandManger = NaniCommandManger.Instance;
        await naniCommandManger.Init();
        // if (isAddressable)
        //     await AddressablesLoad();
    }
    public async UniTask StartPlayVideo()
    {
        // OpenPage.SetActive(true);
        videoSkip.onClick.AddListener(() =>
        {
            OpenPageMessage();
        });
    }
    public async void OpenPageMessage()
    {
        LobbyPage.SetActive(false);
        ErrorPage.SetActive(true);
        CanvasGroup canvasGroup = VideoImage.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        var sfx = Engine.GetService<IAudioManager>();

        Btn_Error.onClick.AddListener(() =>
        {
            //啟動初始場景
            ErrorPage.SetActive(false);
            // StartGamePage.SetActive(true);
            LobbyPage.SetActive(true);
            // EventSystem.current.SetSelectedGameObject(Btn_StartGame.gameObject);
            if (sfx != null)
            {
                sfx.PlayBgmAsync("GameStart", 0.2f, 0.5f, true).Forget();
            }
        });
        // OpenPage.SetActive(true);
        ErrorAnimator.Play("FadeIn");
        await UniTask.Delay(2000);
        if (!ErrorPage.activeInHierarchy)
        {
            return;
        }
        ErrorAnimator.Play("FadeOut");
        await UniTask.Delay(2200);
        if (!ErrorPage.activeInHierarchy)
        {
            return;
        }
        ErrorPage.SetActive(false);
        // StartGamePage.SetActive(true);
        LobbyPage.SetActive(true);
        if (sfx != null)
        {
            await sfx.PlayBgmAsync("GameStart", 0.2f, 0.5f, true);
        }
    }
    async void Init()
    {

        //初始化nani
        await RuntimeInitializer.InitializeAsync();
        //自適應螢幕大小
        await NaniCommandManger.Instance.switchCamera();
        var camera = Engine.GetService<ICameraManager>();
        if (!camera.Camera.gameObject.TryGetComponent(out AspectRatioControl arc))
        {
            AspectRatioControl aspectRatioControl = camera.Camera.gameObject.AddComponent<AspectRatioControl>();
            aspectRatioControl.isMainCamera = true;
        }
        if (!camera.UICamera.gameObject.TryGetComponent(out AspectRatioControl arc2))
            camera.UICamera.gameObject.AddComponent<AspectRatioControl>();
        //初始化語言 並設置語言
        await LanguageManager.Init();
        var Player = Engine.GetService<IScriptPlayer>();
        // await Player.PreloadAndPlayAsync("StartGame");
        //設定選擇按鈕頁面
        // await SelectOptionSwtich(saveData);

        await VideoManager.Instance.InitVideoControll();
        //設定初始選擇按鈕(給遙控器用)
        // EventSystem.current.SetSelectedGameObject(Btn_StartGame.gameObject);
        
        //設定影片控制顯示
        buttonController.onClick.AddListener(() =>
        {
            EventSystem.current.SetSelectedGameObject(Btn_Option.gameObject);
            foreach (var image in BG.GetComponentsInChildren<Image>())
            {
                image.enabled = !image.enabled;
            }
            foreach (var text in BG.GetComponentsInChildren<Text>())
            {
                text.enabled = !text.enabled;
            }
        });

        //快退
        Btn_SpeedViewBack.onClick.AddListener(() =>
        {
            WebGLStreamController.Instance.AddTime(-5000);
            // SkipVideo();
            // AudioManager.Instance.SyncAudioWithVideo(videoPlayer);
        });
        Btn_ChoiceViewBack.onClick.AddListener(async () =>
        {
            if (webGLStreamController == null)
            {
                webGLStreamController = WebGLStreamController.Instance;
            }

            // 获取当前播放时间和总时长（秒）
            float videoPlayerTime = webGLStreamController.GetVideotime() / 1000f;

            // 清除按钮状态
            NaniCommandManger.Instance.SpeedButtonClearSpawn();

            // 计算目标时间点
            int choiceTime = StartNani.Instance.choiceTime;
            float targetTime = videoPlayerTime - choiceTime - 2; // 往回退的时间点（秒）

            // 条件检查：目标时间不能小于 0
            if (targetTime <= 0)
            {
                Debug.Log($"快退无效：目标时间 {targetTime:F2}s 已超出视频起始时间。");
                return;
            }

            // 快退到目标时间点
            long targetTimeInMilliseconds = (long)(targetTime * 1000);
            await webGLStreamController.SeekTime(targetTimeInMilliseconds);

            Debug.Log($"视频成功快退到时间点: {targetTime:F2}s");
        });

        //選項視窗
        Btn_Option.onClick.AddListener(async () =>
        {
            await WebGLStreamController.Instance.PlayPause();
            OptionPage.SetActive(!OptionPage.activeSelf);
            InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
            inGameSettingPage.Init();
            StartNani startNani = GameObject.Find("StartNani").GetComponent<StartNani>();
            startNani.buttonController.gameObject.SetActive(!startNani.buttonController.gameObject.activeSelf);
            foreach (var image in BG.GetComponentsInChildren<Image>())
            {
                image.enabled = !image.enabled;
            }
            foreach (var text in BG.GetComponentsInChildren<Text>())
            {
                text.enabled = !text.enabled;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(OptionPage.GetComponent<RectTransform>());
            // EventSystem.current.SetSelectedGameObject(Btn_OptionReturn.gameObject);
        });
        Btn_InGameOK.onClick.AddListener(async () =>
        {
            var Player = Engine.GetService<IScriptPlayer>();
            await Player.PreloadAndPlayAsync("StartGame");
            InGameCheckPage.SetActive(!InGameCheckPage.activeSelf);
            OptionPage.SetActive(false);
            InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
            inGameSettingPage.Init();
            NaniCommandManger.Instance.isLooping = false;
            var sfx = Engine.GetService<IAudioManager>();
            if (sfx != null)
            {
                await sfx.PlayBgmAsync("GameStart", 0.2f, 0.5f, true);
            }
            // EventSystem.current.SetSelectedGameObject(Btn_StartGame.gameObject);
            await WebGLStreamController.Instance.PlayPause();
            Instance.VideoImage.GetComponent<CanvasGroup>().alpha = 0;
        });
        //遊戲內取消
        Btn_InGameNo.onClick.AddListener(() =>
        {
            InGameCheckPage.SetActive(!InGameCheckPage.activeSelf);
            EventSystem.current.SetSelectedGameObject(Btn_OptionReturn.gameObject);
        });
        
        Btn_ArtistReturn.onClick.AddListener(() =>
        {
            GalleryPage.SetActive(!GalleryPage.activeSelf);
        });
        
        //章節選擇視窗關閉
        Btn_Return.onClick.AddListener(() =>
        {
            SelectOption.SetActive(!SelectOption.activeSelf);
            EventSystem.current.SetSelectedGameObject(Btn_SelectOption.gameObject);
        });
       
        //語言選擇視窗關閉
        Btn_LanguageReturn.onClick.AddListener(() =>
        {
            // videoPlayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();
            // VideoPlayer videoPlayer = VideoManager.Instance.GetVideoPlayer();
            LanguageToggleGroup.gameObject.SetActive(!LanguageToggleGroup.gameObject.activeSelf);

            foreach (var image in LanguageToggleGroup.GetComponentsInChildren<Image>())
            {
                image.enabled = false;
            }
            foreach (var text in LanguageToggleGroup.GetComponentsInChildren<Text>())
            {
                text.enabled = false;
            }
            OptionPage.SetActive(!OptionPage.activeSelf);
            InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
            inGameSettingPage.Init();
            LayoutRebuilder.ForceRebuildLayoutImmediate(OptionPage.GetComponent<RectTransform>());
            EventSystem.current.SetSelectedGameObject(Btn_Language.gameObject);
        });
       
    }
    public async UniTask PlayVideoAsync(string naniScript, string ButtonLable)
    {
        var player = Engine.GetService<IScriptPlayer>();
        await player.PreloadAndPlayAsync(naniScript, ButtonLable);
        ICharacterManager actorManager = Engine.GetService<ICharacterManager>();
        actorManager.RemoveAllActors();
        NaniCommandManger.Instance.isLooping = false;
        // EventSystem.current.SetSelectedGameObject(gameObject);
    }
    //關閉章節選擇選項
    public void CloseOption()
    {
        LobbyPage.SetActive(false);
        SelectOption.SetActive(false);
        ChapterPage.SetActive(false);
        OptionPage.SetActive(false);
        InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
        inGameSettingPage.Init();
        Debug.Log("setFalse");
    }
   
    //新增友誼值
    public void SetfriendshipList(float intFriend)
    {
        // Debug.Log($"SetfriendshipList" + intFriend);
        friednshipList.Add(intFriend);
    }
    //計算總友誼值
    public float allFriendship()
    {
        // Debug.Log("allFriendship:" + friednshipList.Count);
        float sum = 0;
        foreach (var data in friednshipList)
        {
            sum += data;
        }
        // Debug.Log("allFriendship:" + sum);
        return sum;
    }
   
    //存檔
    public async UniTask SaveYaml(string scriptName)
    {
        // SaveData saveData = await YamlLoader.LoadYaml<SaveData>(Application.persistentDataPath + "/SaveData.yaml");
        ServerManager.SaveData saveData = await ServerManager.Instance.Load();
        saveData.friendship = allFriendship();
        // saveData.friendship = allFriendship();
        foreach (var data in saveData.scriptName)
        {
            if (data == scriptName)
                return;
        }
        saveData.scriptName.Add(scriptName);
        await ServerManager.Instance.Save(saveData);
        // await SelectOptionSwtich(saveData);

        // YamlLoader.SaveYaml(saveData);
    }
}
