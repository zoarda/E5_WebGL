using Naninovel;
using UnityEngine;

public class EndMovie : Command
{
    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        WebGLStreamController webGLStreamController = WebGLStreamController.Instance;
        // var videoplayer = GameObject.Find("VideoBackground").GetComponentInChildren<VideoPlayer>();

        IInputManager inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;
        // Debug.Log($"Loop in");
        while (Application.isPlaying && asyncToken.EnsureNotCanceledOrCompleted())
        {
            // await AsyncUtils.WaitEndOfFrameAsync(asyncToken);
            await AsyncUtils.WaitEndOfFrameAsync(asyncToken);
            if (webGLStreamController == null)
            {
                Debug.Log($"still loop");
                return;
            }
            var waitedEnough = webGLStreamController.EndPlay;
            if (waitedEnough) break;
        }
        Debug.Log("unloop");
        await UniTask.CompletedTask;
    }
}
