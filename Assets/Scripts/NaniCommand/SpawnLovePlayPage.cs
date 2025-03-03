using Naninovel;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpawnLovePlayPage : Command
{
    [ParameterAlias("PoseName")]
    public StringListParameter PoseName;
    [ParameterAlias("Label")]
    public StringListParameter Label;
    [ParameterAlias("CumLabel")]
    public StringParameter CumLabel;
    [ParameterAlias("HidenCumLabel"), ParameterDefaultValue("null")]
    public StringParameter HidenCumLabel;
    [ParameterAlias("Type")]
    public StringParameter Type;
    [ParameterAlias("LabelCompany")]
    public StringListParameter LabelCompany;

    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        await SetLovePlayPageAsync(PoseName, Label, CumLabel, Type, LabelCompany, asyncToken);
        // await SetCumButtonAsync(CumLabel, HidenCumLabel, Type, asyncToken);
    }
    public static async UniTask SetLovePlayPageAsync(List<string> poseName, List<string> label, string cumLabel, string type, List<string> labelcompany, AsyncToken asyncToken)
    {
        int dataCount = poseName.Count;
        ISpawnManager spawnManager = Engine.GetService<ISpawnManager>();
        NaniCommandManger naniCommandManger = NaniCommandManger.Instance;
        var lovePlayPagePrefab = spawnManager.GetSpawned("LovePlayPage");
        lovePlayPagePrefab.GameObject.SetActive(true);
        LovePlayPage lovePlayPage = lovePlayPagePrefab.GameObject.GetComponent<LovePlayPage>();
        for (int i = 0; i < lovePlayPage.ChoiceButtonList.Count; i++)
        {
            if (i < dataCount)
            {
                lovePlayPage.ChoiceButtonList[i].SetActive(true);
                Button button = lovePlayPage.ChoiceButtonList[i].GetComponent<Button>();
                Sex_Button sex_Button = lovePlayPage.ChoiceButtonList[i].GetComponent<Sex_Button>();
                CanvasGroup canvasGroup = sex_Button.sliSex.GetComponent<CanvasGroup>();
                sex_Button.ImaSex.sprite = Resources.Load<Sprite>("Sex/" + poseName[i]);

                sex_Button.sliSex.value = 0;
                canvasGroup.alpha = 0;
                sex_Button.Sexlabel = label[i];
                if (label[i] == "Test")
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
                lovePlayPage.SettingButton.onClick.RemoveAllListeners();
                lovePlayPage.SettingButton.onClick.AddListener(() =>
                {
                    StartNani startNani = GameObject.Find("StartNani").GetComponent<StartNani>();
                    startNani.OptionPage.SetActive(!startNani.OptionPage.activeSelf);
                    InGameSettingPage inGameSettingPage = InGameSettingPage.Instance;
                    inGameSettingPage.Init();
                    ISpawnManager spawnManager = Engine.GetService<ISpawnManager>();
                    var lovePlayPagePrefab = spawnManager.GetSpawned("LovePlayPage");
                    lovePlayPagePrefab.GameObject.SetActive(!lovePlayPagePrefab.GameObject.activeSelf);
                });
                button.onClick.RemoveAllListeners();
                int index = i;
                button.onClick.AddListener(async () => await naniCommandManger.OnPoseButtonClick(poseName[index], label[index], labelcompany[index], type, lovePlayPage.ChoiceButtonList[index]));
            }
            else if (i == 7)
            {
                //sex_P_02 sex_N_02
                lovePlayPage.ChoiceButtonList[i].SetActive(true);
                Button button = lovePlayPage.ChoiceButtonList[i].GetComponent<Button>();
                Sex_Button sex_Button = lovePlayPage.ChoiceButtonList[i].GetComponent<Sex_Button>();
                if (type == "demoForPlay")
                {
                    sex_Button.ImaSex.sprite = Resources.Load<Sprite>("Sex/" + "sex_P_01");
                }
                if (type == "demo")
                {
                    button.interactable = false;
                    sex_Button.ImaSex.sprite = Resources.Load<Sprite>("Sex/" + "sex_N_03");
                }
                else
                {
                    sex_Button.ImaSex.sprite = Resources.Load<Sprite>("Sex/" + "sex_N_01");
                }
                button.onClick.RemoveAllListeners();
                int index = i;
                button.onClick.AddListener(async () => await naniCommandManger.OnSpecialButtonClick(cumLabel, type, lovePlayPage.ChoiceButtonList[index]));
            }
        }
        // NaniCommandManger.Instance.ClearLovePlayPage();
        // for (int i = 0; i < poseName.Count; i++)
        // {
        //     NaniCommandManger.Instance.SpawnLovePlayPageButton(lovePlayPage, poseName[i], label[i], cumLabel, type);
        // }
        await UniTask.CompletedTask;
    }
    // public static async UniTask SetCumButtonAsync(string cumLabel, string hidenCumLabel, string type, AsyncToken asyncToken)
    // {
    //     ISpawnManager spawnManager = Engine.GetService<ISpawnManager>();
    //     var lovePlayPagePrefab = spawnManager.GetSpawned("LovePlayPage");
    //     LovePlayPage lovePlayPage = lovePlayPagePrefab.GameObject.GetComponent<LovePlayPage>();
    //     NaniCommandManger.Instance.SetLovePlayPageMode(lovePlayPage, cumLabel, hidenCumLabel, type);
    //     await UniTask.CompletedTask;
    // }
}
