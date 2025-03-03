using UnityEngine;
using Naninovel;

[CommandAlias("NaniSaveData")]
public class NaniSaveData : Command
{
    [ParameterAlias("ScriptName")]
    public StringParameter ScriptName;
    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        await SaveDataAsync(ScriptName, asyncToken);
    }
    private static async UniTask SaveDataAsync(string name , AsyncToken asyncToken)
    {
        StartNani startNani = StartNani.Instance;
        // await startNani.SaveYaml(name);
        // await startNani.SelectOptionSwtich();
        await UniTask.CompletedTask;
    }
}
