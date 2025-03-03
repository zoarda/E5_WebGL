using Naninovel;
using UnityEngine;

[CommandAlias("speedButtonEnable")]
public class SpeedButtonEnable : Command
{
    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        await ButtonEnableAsync(asyncToken);
    }

    private static async UniTask ButtonEnableAsync(AsyncToken asyncToken)
    {
        StartNani startNani = GameObject.Find("StartNani").GetComponent<StartNani>();
        startNani.buttonController.gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }
}
