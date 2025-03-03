using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using UnityEngine.UI;

public class LovePlayPage : MonoBehaviour
{
    public GameObject  SettingButtonPrefab;
    public Button  SettingButton, SpeedButton, ToggleDisplayButton;
    public CanvasGroup DisplayCanvasGroup;
    public List<GameObject> ChoiceButtonList = new List<GameObject>();
    public Canvas MainCanvas;

    void Awake()
    {
        var camera = Engine.GetService<ICameraManager>();
        MainCanvas.worldCamera = camera.UICamera;
        ToggleDisplayButton.onClick.AddListener(() =>
        {
            if (DisplayCanvasGroup.alpha == 1)
            {
                DisplayCanvasGroup.alpha = 0;
                DisplayCanvasGroup.blocksRaycasts = false;
            }
            else
            {
                DisplayCanvasGroup.alpha = 1;
                DisplayCanvasGroup.blocksRaycasts = true;
            }
        });
    }
}
