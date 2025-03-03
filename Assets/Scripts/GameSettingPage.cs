using UnityEngine;
using UnityEngine.UI;
using Naninovel;

public class GameSettingPage : MonoBehaviour
{
    public Button Btn_Chinese, Btn_English, Btn_Japanese, Btn_Save, Btn_Cancel, selectedButton, Btn_Close, Btn_LeftArrow, Btn_RightArrow;
    // public Image Title, Volum, Language, Ch, En, Jp, Save, Cancel;
    public Text TxtTitle, TxtVoice, TxtFex, TxtBgm, TxtLanguage, TxtCH, TxtEN, TxtJP, TxtSave, TxtCancel
    , TxtVoiceValue, TxtBgmValue, TxtFexValue;
    public string StrTitle, StrVoice, StrFex, StrBgm, StrLanguage, StrCH, StrEN, StrJP, StrSave, StrCancel;
    public Toggle Tog_Language, Tog_Audio;
    public Slider Slider_StartGame, Slider_BGM, Slider_SFX, Slider_Voice;

    public Sprite SecltedSprite, UnSelectedSprite;
    public GameObject LanguagePage, VoicePage;
    public Animator aniLanguageList;
    public CanvasGroup Cag_CH, Cag_EN, Cag_JP;
    private int languageIndex = 0; // 0 = English, 1 = Chinese, 2 = Japanese
    private float originalBGMVolume, originalSFXVolume, originalVoiceVolume, originalStartGameVolume;
    private float tempBGMVolume, tempSFXVolume, tempVoiceVolume, tempStartGameVolume;
    private float BGMlv;
    public float VideoVolum = 1;
    private static GameSettingPage instance;
    private static readonly object lockObj = new object();
    public static GameSettingPage Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<GameSettingPage>();

                        // 確保場景中只有一個實例
                        if (FindObjectsOfType<GameSettingPage>().Length > 1)
                        {
                            Debug.LogError("多個 GameSettingPage 實例存在於場景中。");
                        }
                    }
                }
            }
            return instance;
        }
    }
    public void Init()
    {
        ChangeLanguage(0);
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("GameSettingPage 已經存在，銷毀多餘的實例。");
            Destroy(gameObject);
        }
        var Bgm = Engine.GetService<IAudioManager>();
        LanguageManager languageManager = LanguageManager.Instance;
        SetSettingPageLanguage();
        languageManager.SetLanguage();
        languageManager.SetLanguageImage();
        AudioManager audioManager = AudioManager.Instance;
        TxtVoiceValue.text = (Bgm.BgmVolume * 100).ToString("0");

        // 初始化音量初始值
        originalBGMVolume = audioManager.BGMSource.volume;
        originalSFXVolume = audioManager.SFXSource.volume;
        originalVoiceVolume = Bgm.BgmVolume;


        // 初始化臨時音量值
        tempBGMVolume = originalBGMVolume;
        tempSFXVolume = originalSFXVolume;
        tempVoiceVolume = originalVoiceVolume;

        // 初始化 Slider 的值
        Slider_BGM.value = originalBGMVolume;
        Slider_SFX.value = originalSFXVolume;
        Slider_Voice.value = originalVoiceVolume;

        // 初始化 Slider 的事件監聽
        Slider_BGM.onValueChanged.AddListener((value) =>
        {
            tempBGMVolume = value; // 更新臨時值
            AdjustVolume(audioManager.BGMSource, value);
        });
        Slider_SFX.onValueChanged.AddListener((value) =>
        {
            tempSFXVolume = value; // 更新臨時值
            AdjustVolume(audioManager.SFXSource, value);
        });
        Slider_Voice.onValueChanged.AddListener((value) =>
        {
            Bgm.BgmVolume = value;
            tempVoiceVolume = value;
            VideoVolum = value;


            AudioManager.Instance.SFXSource.volume = value;
            TxtVoiceValue.text = (value * 100).ToString("0");
        });

        // 語言按鈕邏輯
        // Btn_Chinese.onClick.AddListener(() =>
        // {
        //     SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.中文;
        //     SetSettingPageLanguage();
        // });
        // Btn_English.onClick.AddListener(() =>
        // {
        //     SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.英文;
        //     SetSettingPageLanguage();
        // });
        // Btn_Japanese.onClick.AddListener(() =>
        // {
        //     SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.日文;
        //     SetSettingPageLanguage();
        // });
        Btn_LeftArrow.onClick.AddListener(() => ChangeLanguage(-1)); // 切換到上一個語言
        Btn_RightArrow.onClick.AddListener(() => ChangeLanguage(1)); // 切換到下一個語言
        // 保存按鈕邏輯
        Btn_Save.onClick.AddListener(async () =>
        {
            await SubtitlesManager.Instance.LoadSubtitles();
            languageManager.SetLanguage();
            languageManager.SetLanguageImage();
            originalBGMVolume = tempBGMVolume;
            originalSFXVolume = tempSFXVolume;
            originalVoiceVolume = tempVoiceVolume;

            audioManager.BGMSource.volume = originalBGMVolume;
            audioManager.SFXSource.volume = originalSFXVolume;
            Bgm.BgmVolume = originalVoiceVolume;

            StartNani.Instance.GameSettingPage.SetActive(false);
        });

        // 取消按鈕邏輯
        Btn_Cancel.onClick.AddListener(() =>
        {
            SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.中文;
            languageManager.SetLanguage();
            languageManager.SetLanguageImage();
            SetSettingPageLanguage();

            tempBGMVolume = originalBGMVolume;
            tempSFXVolume = originalSFXVolume;
            tempVoiceVolume = originalVoiceVolume;

            Slider_BGM.value = originalBGMVolume;
            Slider_SFX.value = originalSFXVolume;
            Slider_Voice.value = originalVoiceVolume;

            audioManager.BGMSource.volume = originalBGMVolume;
            audioManager.SFXSource.volume = originalSFXVolume;

            StartNani.Instance.GameSettingPage.SetActive(false);
        });
        Btn_Close.onClick.AddListener(() =>
        {
            SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.中文;
            languageManager.SetLanguage();
            languageManager.SetLanguageImage();
            SetSettingPageLanguage();

            tempBGMVolume = originalBGMVolume;
            tempSFXVolume = originalSFXVolume;
            tempVoiceVolume = originalVoiceVolume;

            Slider_BGM.value = originalBGMVolume;
            Slider_SFX.value = originalSFXVolume;
            Slider_Voice.value = originalVoiceVolume;

            audioManager.BGMSource.volume = originalBGMVolume;
            audioManager.SFXSource.volume = originalSFXVolume;


            StartNani.Instance.GameSettingPage.SetActive(false);
        });

        void OnbuttonClick(Button button)
        {
            if (selectedButton != null)
            {
                selectedButton.image.sprite = UnSelectedSprite;
            }
            selectedButton = button;
            selectedButton.image.sprite = SecltedSprite;
        }
    }

    private void SetSettingPageLanguage()
    {
        LanguageManager languageManager = LanguageManager.Instance;
        TxtTitle.text = languageManager.GetLanguageValue(StrTitle);
        TxtVoice.text = languageManager.GetLanguageValue(StrVoice);
        TxtBgm.text = languageManager.GetLanguageValue(StrBgm);
        TxtFex.text = languageManager.GetLanguageValue(StrFex);
        TxtLanguage.text = languageManager.GetLanguageValue(StrLanguage);
        TxtCH.text = languageManager.GetLanguageValue(StrCH);
        TxtEN.text = languageManager.GetLanguageValue(StrEN);
        TxtJP.text = languageManager.GetLanguageValue(StrJP);
        TxtCH.text = languageManager.GetLanguageValue(StrCH);
        TxtSave.text = languageManager.GetLanguageValue(StrSave);
        TxtCancel.text = languageManager.GetLanguageValue(StrCancel);
    }
    private void AdjustVolume(AudioSource audioSource, float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value;
        }
    }
    private void ChangeLanguage(int direction)
    {
        StartNani startNani = StartNani.Instance;
        languageIndex = startNani.languageIndex;
        languageIndex += direction;

        // 確保 index 在 0~2 之間循環
        if (languageIndex < 0) languageIndex = 2;
        if (languageIndex > 2) languageIndex = 0;

        switch (languageIndex)
        {
            case 0: // 中文
                SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.中文;
                break;
            case 1: // 日文
                SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.日文;
                break;
            case 2: // 英文
                SubtitlesManager.Instance.LanguageCase = SubtitlesManager.Language.英文;
                break;
        }
        SetSettingPageLanguage();
        aniLanguageList.SetInteger("language", languageIndex);
        startNani.languageIndex = languageIndex;

    }

    private void UpdateButtonTransparency()
    {
        // if (languageIndex == 0)
        // {
        //     Cag_CH.enabled = false; // 顯示按鈕
        //     Cag_EN.enabled = true;
        //     Cag_JP.enabled = true;
        // }
        // if (languageIndex == 1)
        // {
        //     Cag_CH.enabled = true; // 顯示按鈕
        //     Cag_JP.enabled = false;
        //     Cag_EN.enabled = true;
        // }
        // if (languageIndex == 2)
        // {
        //     Cag_CH.enabled = true; // 顯示按鈕
        //     Cag_JP.enabled = true;
        //     Cag_EN.enabled = false;
        // }
    }

}
