using UnityEngine;
using UnityEngine.UI;
using Naninovel;

public class GameCheck : MonoBehaviour
{
    public Text Txtcheck, TxtGamequit, TxtGameback;
    public string check, quit, back;

    public Button Gamequit, Gameback;
    void Awake()
    {
        StartNani startNani = StartNani.Instance;
        InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
        Gamequit.onClick.AddListener(async () =>
        {
            var Player = Engine.GetService<IScriptPlayer>();
            await Player.PreloadAndPlayAsync("StartGame");
            startNani.InGameCheckPage.SetActive(!startNani.InGameCheckPage.activeSelf);
            startNani.OptionPage.SetActive(false);
            inGameSettingPage.Init();
            NaniCommandManger.Instance.isLooping = false;
            var sfx = Engine.GetService<IAudioManager>();
            if (sfx != null)
            {
                await sfx.PlayBgmAsync("GameStart", 0.2f, 0.5f, true);
            }
            // EventSystem.current.SetSelectedGameObject(Btn_StartGame.gameObject);
            await WebGLStreamController.Instance.PlayPause();
            startNani.VideoImage.GetComponent<CanvasGroup>().alpha = 0;

            SubtitlesManager.Instance.ObjChatBG.SetActive(false);
            SubtitlesManager.Instance.ObjNameBG.SetActive(false);
        });
        Gameback.onClick.AddListener(async () =>
        {
            startNani.InGameCheckPage.SetActive(!startNani.InGameCheckPage.activeSelf);
            await WebGLStreamController.Instance.PlayVideo();
        });
    }

}
