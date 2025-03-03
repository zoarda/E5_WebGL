using UnityEngine;
using UnityEngine.UI;
using Naninovel;


public class SelectOptionPage : MonoBehaviour
{
    [SerializeField]
    private Button Btn_C1_VB, Btn_C2_VB, Btn_C3_VB, Btn_C4_VB, Btn_C5_VB, Btn_Return;
    void Start()
    {
        StartNani startNani = StartNani.Instance;
        //選擇選項
        Btn_C1_VB.onClick.AddListener(() =>
        {

            startNani.ChapterPage.SetActive(true);
            var chapter = startNani.ChapterPage.GetComponent<ChapterPage>();
            chapter.C1.SetActive(true);
            chapter.UpdateChapterPage();
            // if (isProcessing) return;
            // isProcessing = true;
            // StartGamePage.SetActive(false);
            // CheckSelectOption();

            // await PlayVideoAsync("C1_VB", "C1_S0", Btn_Option.gameObject);
        });
        Btn_C2_VB.onClick.AddListener(() =>
        {
            startNani.ChapterPage.SetActive(true);
            var chapter = startNani.ChapterPage.GetComponent<ChapterPage>();
            chapter.C2.SetActive(true);
            chapter.UpdateChapterPage();
            // if (isProcessing) return;
            // isProcessing = true;
            // StartGamePage.SetActive(false);
            // CheckSelectOption();

            // await PlayVideoAsync("C2_VB", "C2_S0", Btn_Option.gameObject);
        });
        Btn_C3_VB.onClick.AddListener(() =>
        {
            startNani.ChapterPage.SetActive(true);
            var chapter = startNani.ChapterPage.GetComponent<ChapterPage>();
            chapter.C3.SetActive(true);
            chapter.UpdateChapterPage();
            // if (isProcessing) return;
            // isProcessing = true;
            // StartGamePage.SetActive(false);
            // CheckSelectOption();

            // await PlayVideoAsync("C3_VB", "C3_S0", Btn_Option.gameObject);
        });
        Btn_C4_VB.onClick.AddListener(() =>
        {
            startNani.ChapterPage.SetActive(true);
            var chapter = startNani.ChapterPage.GetComponent<ChapterPage>();
            chapter.C4.SetActive(true);
            chapter.UpdateChapterPage();
            // if (isProcessing) return;
            // isProcessing = true;
            // StartGamePage.SetActive(false);
            // CheckSelectOption();

            // await PlayVideoAsync("C4_VB", "C4_S0", Btn_Option.gameObject);
        });
        Btn_C5_VB.onClick.AddListener(() =>
        {
            startNani.ChapterPage.SetActive(true);
            var chapter = startNani.ChapterPage.GetComponent<ChapterPage>();
            chapter.C5.SetActive(true);
            chapter.UpdateChapterPage();
            // if (isProcessing) return;
            // isProcessing = true;
            // StartGamePage.SetActive(false);
            // CheckSelectOption();

            // await PlayVideoAsync("C5_VB", "C5_S0", Btn_Option.gameObject);
        });
    }
    public async UniTask SelectOptionSwtich(ServerManager.SaveData saveData)
    {
        // SaveData saveData = await YamlLoader.LoadYaml<SaveData>(Application.persistentDataPath + "/SaveData.yaml");
        // SaveData saveData = new SaveData();
        // string? token = await ServerManager.Instance.Login();
        // if (string.IsNullOrEmpty(token))
        // {
        //     Debug.Log("login in failed");
        // }
        // else
        // {
        //     saveData = await ServerManager.Instance.Load(token);
        // }
        saveData.friendship = StartNani.Instance.allFriendship();
        SelectButton C1 = Btn_C1_VB.GetComponent<SelectButton>();
        SelectButton C2 = Btn_C2_VB.GetComponent<SelectButton>();
        SelectButton C3 = Btn_C3_VB.GetComponent<SelectButton>();
        SelectButton C4 = Btn_C4_VB.GetComponent<SelectButton>();
        SelectButton C5 = Btn_C5_VB.GetComponent<SelectButton>();
        foreach (var data in saveData.scriptName)
        {
            if (data.Contains("C1_VB"))
            {
                Btn_C1_VB.interactable = true;
                C1.Disable.SetActive(false);
            }
            if (data.Contains("C2_VB"))
            {
                Btn_C2_VB.interactable = true;
                C2.Disable.SetActive(false);
            }
            if (data.Contains("C3_VB"))
            {
                Btn_C3_VB.interactable = true;
                C3.Disable.SetActive(false);
            }
            if (data.Contains("C4_VB"))
            {
                Btn_C4_VB.interactable = true;
                C4.Disable.SetActive(false);
            }
            if (data.Contains("C5_VB"))
            {
                Btn_C5_VB.interactable = true;
                C5.Disable.SetActive(false);
            }
        }
    }
}
