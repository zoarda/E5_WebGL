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
        // var urlData = ServerManager.Instance.urlData;
        // if (!string.IsNullOrEmpty(urlData.token) && !string.IsNullOrEmpty(urlData.language))
        // {
        //     Debug.Log($"Token: {urlData.token}, Language: {urlData.language}");
        // }
        // else
        // {
        //     Debug.LogError("Failed to retrieve URL data.");
        // }
        // string absoluteURL = Application.absoluteURL;

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
        //尋找存檔紀錄
        // SaveData saveData = await YamlLoader.LoadYaml<SaveData>(Application.persistentDataPath + "/SaveData.yaml");
        // ServerManager.SaveData saveData = await ServerManager.Instance.Load();
        // saveData.friendship = allFriendship();
        // 將設置好的友誼值存入Manager變數
        // var varManager = Engine.GetService<ICustomVariableManager>();
        // varManager.TrySetVariableValue("friendship", saveData.friendship);
        // friednshipList.Add(saveData.friendship);
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

        // //動態生成語言選擇按鈕
        // foreach (var data in Language)
        // {
        //     var toggle = Instantiate(PreLanguageToggle, LanguageToggleGroup.transform);
        //     Text text = toggle.GetComponentInChildren<Text>();
        //     if (data.Contains("中文"))
        //     {
        //         toggle.GetComponent<Toggle>().isOn = true;
        //         text.AddComponent<LangugeText>().Id = "PackId90";
        //     }
        //     if (data.Contains("日文"))
        //     {
        //         text.AddComponent<LangugeText>().Id = "PackId88";
        //     }
        //     if (data.Contains("英文"))
        //     {
        //         text.AddComponent<LangugeText>().Id = "PackId89";
        //     }
        //     toggle.GetComponent<Image>().enabled = false;
        //     toggle.GetComponent<Toggle>().group = LanguageToggleGroup;
        //     toggle.transform.SetAsFirstSibling();
        //     toggle.GetComponent<Toggle>().onValueChanged.AddListener(async (value) =>
        //     {
        //         if (value)
        //         {
        //             if (data.Contains("中文"))
        //                 subtitlesManager.LanguageCase = SubtitlesManager.Language.中文;
        //             else if (data.Contains("日文"))
        //                 subtitlesManager.LanguageCase = SubtitlesManager.Language.日文;
        //             else if (data.Contains("英文"))
        //                 subtitlesManager.LanguageCase = SubtitlesManager.Language.英文;
        //         }
        //         await subtitlesManager.LoadSubtitles();
        //         LanguageManager.SetLanguage();
        //     });
        //     LanguageManager.languageTexts.Add(toggle.GetComponentInChildren<Text>());
        //     LanguageManager.SetLanguage();
        //     toggles.Add(toggle.GetComponent<Toggle>());
        // }
        // ConfigureToggleNavigation();
        //開始頁面畫廊
        // Btn_Artist.onClick.AddListener(() =>
        // {
        //     GalleryPage.SetActive(!GalleryPage.activeSelf);
        // });
        //開始頁面選項
        // Btn_StartSelect.onClick.AddListener(() =>
        // {
        //     // StartGamePage.SetActive(!StartGamePage.activeSelf);
        //     SelectOption.SetActive(!SelectOption.activeSelf);
        //     // EventSystem.current.SetSelectedGameObject(Btn_C1_VB.gameObject);
        // });
        // //開始遊戲
        // Btn_StartGame.onClick.AddListener(async () =>
        // {
        //     // AudioManager.instance.PlaySFX("Up");
        //     // OpenPage.SetActive(true);
        //     CanvasGroup canvasGroup = VideoImage.GetComponent<CanvasGroup>();
        //     canvasGroup.alpha = 1;
        //     // StartGamePage.SetActive(!StartGamePage.activeSelf);
        //     LobbyPage.SetActive(!LobbyPage.activeSelf);
        //     await Player.PreloadAndPlayAsync("S");
        //     ICharacterManager actorManager = Engine.GetService<ICharacterManager>();
        //     actorManager.RemoveAllActors();
        //     NaniCommandManger.Instance.SpeedButtonClearSpawn();
        //     EventSystem.current.SetSelectedGameObject(Btn_Option.gameObject);
        // });
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
        //播放暫停
        // Btn_PlayPause.onValueChanged.AddListener((value) =>
        // {
        //     videoPlayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();
        //     if (value)
        //         videoPlayer.Pause();
        //     else
        //         videoPlayer.Play();
        // });
        //選項視窗
        Btn_Option.onClick.AddListener(async () =>
        {
            // videoPlayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();
            // VideoPlayer videoPlayer = VideoManager.Instance.GetVideoPlayer();
            // if (videoPlayer.isPlaying)
            // videoPlayer.Pause();
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
        //遊戲確認
        // Btn_OK.onClick.AddListener(() =>
        // {
        //     if (Application.platform == RuntimePlatform.WebGLPlayer)
        //     {
        //         // Debug.Log("shrimp: ready to SendMessage");
        //         // // Example 1: 發送 'send:message' 事件
        //         // SendMessageToParent("open");

        //         // Example 2: 發送 'openurl' 事件
        //         Debug.Log("shrimp: back");
        //         Back();

        //         // // Example 3: 發送 'reload' 事件
        //         // ReloadPage();
        //         // Debug.Log("shrimp: SendMessage succese");
        //     }
        //     else
        //     {
        //         Application.Quit();
        //     }
        // });
        //遊戲取消
        // Btn_No.onClick.AddListener(() =>
        // {
        //     CheckPage.SetActive(!CheckPage.activeSelf);
        //     EventSystem.current.SetSelectedGameObject(Btn_EndGamestart.gameObject);
        // });
        //遊戲內確認
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
        // 選項視窗
        // Btn_SelectOption.onClick.AddListener(async () =>
        // {
        //     // videoPlayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();
        //     // VideoPlayer videoPlayer = VideoManager.Instance.GetVideoPlayer();
        //     // videoPlayer.Pause();
        //     if (webGLStreamController == null)
        //     {
        //         webGLStreamController = WebGLStreamController.Instance;
        //     }
        //     await webGLStreamController.PlayPause();
        //     SelectOption.SetActive(!SelectOption.activeSelf);
        //     var spawneObjects = Engine.GetService<ISpawnManager>().GetAllSpawned();
        //     var lovePlayPageObject = spawneObjects.FirstOrDefault(i => i.Path == "LovePlayPage");
        //     if (lovePlayPageObject != null && lovePlayPageObject.GameObject.activeInHierarchy == false)
        //     {
        //         // lovePlayPageObject.GameObject.SetActive(true);
        //         // buttonController.gameObject.SetActive(!buttonController.gameObject.activeSelf);

        //         // foreach (var image in BG.GetComponentsInChildren<Image>())
        //         // {
        //         //     image.enabled = !image.enabled;
        //         // }
        //         // foreach (var text in BG.GetComponentsInChildren<Text>())
        //         // {
        //         //     text.enabled = !text.enabled;
        //         // }
        //     }
        //     buttonController.gameObject.SetActive(!buttonController.gameObject.activeSelf);
        //     if (webGLStreamController == null)
        //     {
        //         webGLStreamController = WebGLStreamController.Instance;
        //     }
        //     if (!Btn_PlayPause.isOn)
        //         await webGLStreamController.PlayVideo();
        //     OptionPage.SetActive(!OptionPage.activeSelf);
        //     InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
        //     inGameSettingPage.Init();
        //     foreach (var image in BG.GetComponentsInChildren<Image>())
        //     {
        //         image.enabled = !image.enabled;
        //     }
        //     foreach (var text in BG.GetComponentsInChildren<Text>())
        //     {
        //         text.enabled = !text.enabled;
        //     }
        //     CheckPage.SetActive(false);
        //     // EventSystem.current.SetSelectedGameObject(Btn_C1_VB.gameObject);
        // });
        Btn_ArtistReturn.onClick.AddListener(() =>
        {
            GalleryPage.SetActive(!GalleryPage.activeSelf);
        });
        //結束遊戲
        // Btn_EndGame.onClick.AddListener(async () =>
        // {
        //     // Application.Quit();
        //     // videoPlayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();
        //     // VideoPlayer videoPlayer = VideoManager.Instance.GetVideoPlayer();
        //     // videoPlayer.Pause();
        //     if (webGLStreamController == null)
        //     {
        //         webGLStreamController = WebGLStreamController.Instance;
        //     }
        //     await webGLStreamController.PlayPause();
        //     InGameCheckPage.SetActive(!InGameCheckPage.activeSelf);
        //     EventSystem.current.SetSelectedGameObject(Btn_OK.gameObject);
        // });
        //開啟語言選擇視窗
        // Btn_Language.onClick.AddListener(async () =>
        // {
        //     // videoPlayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();
        //     // VideoPlayer videoPlayer = VideoManager.Instance.GetVideoPlayer();
        //     // videoPlayer.Pause();
        //     if (webGLStreamController == null)
        //     {
        //         webGLStreamController = WebGLStreamController.Instance;
        //     }
        //     await webGLStreamController.PlayPause();
        //     LanguageToggleGroup.gameObject.SetActive(!LanguageToggleGroup.gameObject.activeSelf);
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(LanguageToggle.GetComponent<RectTransform>());

        //     foreach (var image in LanguageToggleGroup.GetComponentsInChildren<Image>())
        //     {
        //         image.enabled = !image.enabled;
        //     }
        //     foreach (var text in LanguageToggleGroup.GetComponentsInChildren<Text>())
        //     {
        //         text.enabled = !text.enabled;
        //     }
        //     OptionPage.SetActive(!OptionPage.activeSelf);
        //     InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
        //     inGameSettingPage.Init();
        //     EventSystem.current.SetSelectedGameObject(Btn_LanguageReturn.gameObject);
        // });
        //遊戲設定
        // Btn_GameSetting.onClick.AddListener(() =>
        // {
        //     GameSettingPage.SetActive(!GameSettingPage.activeSelf);
        //     EventSystem.current.SetSelectedGameObject(Btn_OptionReturn.gameObject);
        // });
        //章節選擇視窗關閉
        Btn_Return.onClick.AddListener(() =>
        {
            SelectOption.SetActive(!SelectOption.activeSelf);
            EventSystem.current.SetSelectedGameObject(Btn_SelectOption.gameObject);
        });
        //選項視窗關閉
        // Btn_OptionReturn.onClick.AddListener(async () =>
        // {
        //     StartNani startNani = GameObject.Find("StartNani").GetComponent<StartNani>();
        //     var spawneObjects = Engine.GetService<ISpawnManager>().GetAllSpawned();
        //     var lovePlayPageObject = spawneObjects.FirstOrDefault(i => i.Path == "LovePlayPage");
        //     if (lovePlayPageObject != null && lovePlayPageObject.GameObject.activeInHierarchy == false)
        //     {
        //         lovePlayPageObject.GameObject.SetActive(true);
        //         startNani.buttonController.gameObject.SetActive(!startNani.buttonController.gameObject.activeSelf);

        //         foreach (var image in BG.GetComponentsInChildren<Image>())
        //         {
        //             image.enabled = !image.enabled;
        //         }
        //         foreach (var text in BG.GetComponentsInChildren<Text>())
        //         {
        //             text.enabled = !text.enabled;
        //         }
        //     }
        //     startNani.buttonController.gameObject.SetActive(!startNani.buttonController.gameObject.activeSelf);
        //     if (webGLStreamController == null)
        //     {
        //         webGLStreamController = WebGLStreamController.Instance;
        //     }
        //     if (!Btn_PlayPause.isOn)
        //         await webGLStreamController.PlayVideo();
        //     OptionPage.SetActive(!OptionPage.activeSelf);
        //     InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
        //     inGameSettingPage.Init();
        //     foreach (var image in BG.GetComponentsInChildren<Image>())
        //     {
        //         image.enabled = !image.enabled;
        //     }
        //     foreach (var text in BG.GetComponentsInChildren<Text>())
        //     {
        //         text.enabled = !text.enabled;
        //     }
        //     CheckPage.SetActive(false);
        //     EventSystem.current.SetSelectedGameObject(Btn_Option.gameObject);

        // });
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
        // //選擇選項
        // Btn_C1_VB.onClick.AddListener(() =>
        // {
        //     ChapterPage.SetActive(true);
        //     var chapter = ChapterPage.GetComponent<ChapterPage>();
        //     chapter.C1.SetActive(true);
        //     chapter.UpdateChapterPage();
        //     // if (isProcessing) return;
        //     // isProcessing = true;
        //     // StartGamePage.SetActive(false);
        //     // CheckSelectOption();

        //     // await PlayVideoAsync("C1_VB", "C1_S0", Btn_Option.gameObject);
        // });
        // Btn_C2_VB.onClick.AddListener(() =>
        // {
        //     ChapterPage.SetActive(true);
        //     var chapter = ChapterPage.GetComponent<ChapterPage>();
        //     chapter.C2.SetActive(true);
        //     chapter.UpdateChapterPage();
        //     // if (isProcessing) return;
        //     // isProcessing = true;
        //     // StartGamePage.SetActive(false);
        //     // CheckSelectOption();

        //     // await PlayVideoAsync("C2_VB", "C2_S0", Btn_Option.gameObject);
        // });
        // Btn_C3_VB.onClick.AddListener(() =>
        // {
        //     ChapterPage.SetActive(true);
        //     var chapter = ChapterPage.GetComponent<ChapterPage>();
        //     chapter.C3.SetActive(true);
        //     chapter.UpdateChapterPage();
        //     // if (isProcessing) return;
        //     // isProcessing = true;
        //     // StartGamePage.SetActive(false);
        //     // CheckSelectOption();

        //     // await PlayVideoAsync("C3_VB", "C3_S0", Btn_Option.gameObject);
        // });
        // Btn_C4_VB.onClick.AddListener(() =>
        // {
        //     ChapterPage.SetActive(true);
        //     var chapter = ChapterPage.GetComponent<ChapterPage>();
        //     chapter.C4.SetActive(true);
        //     chapter.UpdateChapterPage();
        //     // if (isProcessing) return;
        //     // isProcessing = true;
        //     // StartGamePage.SetActive(false);
        //     // CheckSelectOption();

        //     // await PlayVideoAsync("C4_VB", "C4_S0", Btn_Option.gameObject);
        // });
        // Btn_C5_VB.onClick.AddListener(() =>
        // {
        //     ChapterPage.SetActive(true);
        //     var chapter = ChapterPage.GetComponent<ChapterPage>();
        //     chapter.C5.SetActive(true);
        //     chapter.UpdateChapterPage();
        //     // if (isProcessing) return;
        //     // isProcessing = true;
        //     // StartGamePage.SetActive(false);
        //     // CheckSelectOption();

        //     // await PlayVideoAsync("C5_VB", "C5_S0", Btn_Option.gameObject);
        // });


        // async UniTask PlayVideoAsync(string naniScript, string videoName, GameObject gameObject)
        // {
        //     await Player.PreloadAndPlayAsync($"{naniScript}");
        //     // CheckSelectOption();
        //     videoPlayer = GameObject.Find($"{videoName}").GetComponent<VideoPlayer>();
        //     videoPlayer.time = videoPlayer.length - choiceTime - 2;
        //     lastChoiceName = videoPlayer.clip.name;
        //     ICharacterManager actorManager = Engine.GetService<ICharacterManager>();
        //     actorManager.RemoveAllActors();
        //     NaniCommandManger.Instance.isLooping = false;
        //     EventSystem.current.SetSelectedGameObject(gameObject);
        //     isProcessing = false;
        // }
    }
    public async UniTask PlayVideoAsync(string naniScript, string ButtonLable)
    {
        var player = Engine.GetService<IScriptPlayer>();
        await player.PreloadAndPlayAsync(naniScript, ButtonLable);
        // CheckSelectOption();
        // VideoPlayer videoPlayer = GameObject.Find($"{ButtonLable}").GetComponent<VideoPlayer>();

        // videoPlayer.time = videoPlayer.length - choiceTime - 2;
        // lastChoiceName = videoPlayer.clip.name;
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
    // public void OpenOption()
    // {
    //     // StartGamePage.SetActive(!StartGamePage.activeSelf);
    //     // SelectOption.SetActive(!SelectOption.activeSelf);
    //     ChapterPage.SetActive(!ChapterPage.activeSelf);
    //     // OptionPage.SetActive(false);
    //     Debug.Log("setFalse");
    // }
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
    //選擇章節顯示切換
    // public async UniTask SelectOptionSwtich(ServerManager.SaveData saveData)
    // {
    //     // SaveData saveData = await YamlLoader.LoadYaml<SaveData>(Application.persistentDataPath + "/SaveData.yaml");
    //     // SaveData saveData = new SaveData();
    //     // string? token = await ServerManager.Instance.Login();
    //     // if (string.IsNullOrEmpty(token))
    //     // {
    //     //     Debug.Log("login in failed");
    //     // }
    //     // else
    //     // {
    //     //     saveData = await ServerManager.Instance.Load(token);
    //     // }
    //     saveData.friendship = allFriendship();
    //     SelectButton C1 = Btn_C1_VB.GetComponent<SelectButton>();
    //     SelectButton C2 = Btn_C2_VB.GetComponent<SelectButton>();
    //     SelectButton C3 = Btn_C3_VB.GetComponent<SelectButton>();
    //     SelectButton C4 = Btn_C4_VB.GetComponent<SelectButton>();
    //     SelectButton C5 = Btn_C5_VB.GetComponent<SelectButton>();
    //     foreach (var data in saveData.scriptName)
    //     {
    //         if (data.Contains("C1_VB"))
    //         {
    //             Btn_C1_VB.interactable = true;
    //             C1.Disable.SetActive(false);
    //         }
    //         if (data.Contains("C2_VB"))
    //         {
    //             Btn_C2_VB.interactable = true;
    //             C2.Disable.SetActive(false);
    //         }
    //         if (data.Contains("C3_VB"))
    //         {
    //             Btn_C3_VB.interactable = true;
    //             C3.Disable.SetActive(false);
    //         }
    //         if (data.Contains("C4_VB"))
    //         {
    //             Btn_C4_VB.interactable = true;
    //             C4.Disable.SetActive(false);
    //         }
    //         if (data.Contains("C5_VB"))
    //         {
    //             Btn_C5_VB.interactable = true;
    //             C5.Disable.SetActive(false);
    //         }
    //     }
    // }
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
    // //存檔資料
    // public class SaveData
    // {
    //     public float friendship;
    //     public List<string> scriptName;
    // }

    // //設定語言選擇按鈕的按鍵設定
    // void ConfigureToggleNavigation()
    // {
    //     for (int i = 0; i < toggles.Count; i++)
    //     {
    //         Navigation navigation = new Navigation
    //         {
    //             mode = Navigation.Mode.Explicit
    //         };

    //         if (toggles[i].GetComponentInChildren<Text>().text.Contains("中文"))
    //         {
    //             navigation.selectOnRight = Btn_LanguageReturn;
    //             navigation.selectOnLeft = toggles.Find(t => t.GetComponentInChildren<Text>().text.Contains("日文"));
    //         }
    //         else if (toggles[i].GetComponentInChildren<Text>().text.Contains("日文"))
    //         {
    //             navigation.selectOnRight = toggles.Find(t => t.GetComponentInChildren<Text>().text.Contains("中文"));
    //             navigation.selectOnLeft = toggles.Find(t => t.GetComponentInChildren<Text>().text.Contains("英文"));
    //         }
    //         else if (toggles[i].GetComponentInChildren<Text>().text.Contains("英文"))
    //         {
    //             navigation.selectOnRight = toggles.Find(t => t.GetComponentInChildren<Text>().text.Contains("日文"));
    //             navigation.selectOnLeft = Btn_LanguageReturn;
    //         }

    //         toggles[i].navigation = navigation;
    //     }
    // }

}
