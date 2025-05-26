using Naninovel;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Button BtnBack, BtnCh1;
    void Start()
    {
        StartNani startNani = StartNani.Instance;

        var Player = Engine.GetService<IScriptPlayer>();
        BtnCh1.onClick.AddListener(async () =>
        {
            CanvasGroup canvasGroup = StartNani.Instance.VideoImage.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            startNani.Map.SetActive(false);
            var sfx = Engine.GetService<IAudioManager>();
            await Player.PreloadAndPlayAsync("F");
            if (sfx != null)
            {
                await sfx.StopSfxAsync("GameStart", 0.2f);
            }
            ICharacterManager actorManager = Engine.GetService<ICharacterManager>();
            actorManager.RemoveAllActors();
            NaniCommandManger.Instance.SpeedButtonClearSpawn();
        });
        BtnBack.onClick.AddListener(() =>
        {
            startNani.Map.SetActive(false);
            startNani.LobbyPage.SetActive(true);
        });
    }

}
