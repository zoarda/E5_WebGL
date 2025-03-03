using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using System;
using Cysharp.Threading.Tasks;


public class SubtitlesManager : MonoBehaviour
{
    public Text subtitlesText;
    public Text NameText;
    // private List<(double start, double end, string text)> subtitles = new();
    private List<(double start, double end, string name, string text)> subtitles = new();
    private int currentSubtitleIndex = -1;
    private string videoName;
    [SerializeField]
    public GameObject ObjChatBG, ObjNameBG;

    [SerializeField] private StartNani startNani;
    public Language LanguageCase = Language.中文;

    static SubtitlesManager instance;
    WebGLStreamController webGLStreamController;

    UrlToScence urlToScence;

    void Start()
    {

    }

    void Update()
    {
        if (webGLStreamController == null)
        {
            webGLStreamController = WebGLStreamController.Instance;
            return;
        }
        if (StartNani.Instance.VideoImage.GetComponent<CanvasGroup>().alpha == 0)
        {
            subtitlesText.text = "";
            return;
        }
        UpdateSubtitle(webGLStreamController.GetVideotime());
    }
    public static SubtitlesManager Instance
    {

        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SubtitlesManager>();
            }
            return instance;

        }
    }
    public async UniTask Init()
    {
        try
        {
            urlToScence = await YamlLoader.LoadStreamingAssetsYaml<UrlToScence>(Application.streamingAssetsPath + "/Yaml/URLToScence.yaml");
            Debug.Log($"{urlToScence}");
        }
        catch (Exception e)
        {
            Debug.Log($"script urltosence{e}");
        }
    }
    public async UniTask LoadSubtitles()
    {
        webGLStreamController = WebGLStreamController.Instance;
        switch (LanguageCase)
        {
            case Language.中文:
                try
                {
                    string url = webGLStreamController.multiStreamProperties[0].url[0];
                    if (urlToScence.videoDictionary.TryGetValue(url, out string script))
                    {
                        videoName = script;
                        Debug.Log($"URL:{url}");
                        Debug.Log($"Find srt script{videoName}");
                    }
                    else
                    {
                        Debug.Log($"url not found");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
                break;
            case Language.日文:
                try
                {
                    string url = webGLStreamController.multiStreamProperties[0].url[0];
                    if (urlToScence.videoDictionary.TryGetValue(url, out string script))
                    {
                        videoName = script + "_JP";
                        Debug.Log($"URL:{url}");
                        Debug.Log($"Find srt script{videoName}");
                    }
                    else
                    {
                        Debug.Log($"url not found");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
                break;
            case Language.英文:
                try
                {
                    string url = webGLStreamController.multiStreamProperties[0].url[0];
                    if (urlToScence.videoDictionary.TryGetValue(url, out string script))
                    {
                        videoName = script + "_EN";
                        Debug.Log($"URL:{url}");
                        Debug.Log($"Find srt script{videoName}");
                    }
                    else
                    {
                        Debug.Log($"url not found");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
                break;
            default:
                Debug.LogError($"未匹配的 LanguageCase 值: {LanguageCase}");
                break;
        }
        // if (videoPlayer != null)
        // {
        //     switch (LanguageCase)
        //     {
        //         case Language.中文:
        //             videoName = videoPlayer.clip.name;
        //             break;
        //         case Language.日文:
        //             videoName = videoPlayer.clip.name + "_JP";
        //             break;
        //         case Language.英文:
        //             videoName = videoPlayer.clip.name + "_EN";
        //             break;
        //     }
        // }
        // else
        // {
        //     Debug.Log("videoPlayer is null");
        // }

        string subtitlePath = Application.streamingAssetsPath + "/Subtitles/" + videoName + ".srt";
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                WWW reader = new WWW(subtitlePath);
                await reader.ToUniTask();
                ParseSubtitles(reader.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading YAML file: {subtitlePath}\n{e}");
            }
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            try
            {
                WWW reader = new WWW(subtitlePath);
                await reader.ToUniTask();
                ParseSubtitles(reader.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading YAML file: {subtitlePath}\n{e}");
            }
        }
        else
        {
            ParseSubtitles(subtitlePath);
        }
    }
    void ParseSubtitles(string subtitlesContent)
    {
        subtitles.Clear();
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (StringReader reader = new StringReader(subtitlesContent))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string timeLine = reader.ReadLine();
                        string[] times = timeLine.Split(new string[] { " --> " }, StringSplitOptions.None);
                        double startTime = ParseTime(times[0].Trim());
                        double endTime = ParseTime(times[1].Trim());
                        string textLine = reader.ReadLine();
                        // Debug.Log(textLine);
                        string[] texts = textLine.Split(new string[] { "@" }, StringSplitOptions.None);
                        string name = "";
                        string text = "";
                        if (texts.Length == 2)
                        {
                            name = texts[0];
                            text = texts[1];
                        }
                        else
                        {
                            text = textLine;
                        }
                        // subtitles.Add((startTime, endTime, text));
                        subtitles.Add((startTime, endTime, name, text));
                        reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("加载字幕文件出错：" + ex.Message);
            }
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            try
            {
                using (StringReader reader = new StringReader(subtitlesContent))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string timeLine = reader.ReadLine();
                        string[] times = timeLine.Split(new string[] { " --> " }, StringSplitOptions.None);
                        double startTime = ParseTime(times[0].Trim());
                        double endTime = ParseTime(times[1].Trim());
                        string textLine = reader.ReadLine();
                        // Debug.Log(textLine);
                        string[] texts = textLine.Split(new string[] { "@" }, StringSplitOptions.None);
                        string name = "";
                        string text = "";
                        if (texts.Length == 2)
                        {
                            name = texts[0];
                            text = texts[1];
                        }
                        else
                        {
                            text = textLine;
                        }
                        // subtitles.Add((startTime, endTime, text));
                        subtitles.Add((startTime, endTime, name, text));
                        reader.ReadLine();
                    }
                    // string line;
                    // while ((line = reader.ReadLine()) != null)
                    // {
                    //     string timeLine = reader.ReadLine();
                    //     string[] times = timeLine.Split(new string[] { " --> " }, StringSplitOptions.None);
                    //     double startTime = ParseTime(times[0].Trim());
                    //     double endTime = ParseTime(times[1].Trim());
                    //     string text = reader.ReadLine();
                    //     subtitles.Add((startTime, endTime, text));
                    //     reader.ReadLine();
                    // }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("加载字幕文件出错：" + ex.Message);
            }
        }
        else
        {
            try
            {
                using (StreamReader reader = new StreamReader(subtitlesContent))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string timeLine = reader.ReadLine();
                        string[] times = timeLine.Split(new string[] { " --> " }, StringSplitOptions.None);
                        double startTime = ParseTime(times[0].Trim());
                        double endTime = ParseTime(times[1].Trim());
                        // string text = reader.ReadLine();
                        string textLine = reader.ReadLine();
                        string[] texts = textLine.Split(new string[] { "@" }, StringSplitOptions.None);
                        string name = "";
                        string text = "";
                        if (texts.Length == 2)
                        {
                            name = texts[0];
                            text = texts[1];
                        }
                        else
                        {
                            text = textLine;
                        }
                        // subtitles.Add((startTime, endTime, text));
                        subtitles.Add((startTime, endTime, name, text));
                        reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("加载字幕文件出错：" + ex.Message);
            }

        }
    }
    double ParseTime(string time)
    {
        string[] parts = time.Split(':');
        double hours = double.Parse(parts[0]);
        double minutes = double.Parse(parts[1]);
        double seconds = double.Parse(parts[2].Replace(',', '.'));
        return hours * 3600 + minutes * 60 + seconds;
    }

    // void UpdateSubtitle(long HISplayertime)
    // {
    //     float currentTime = HISplayertime / 1000f;
    //     if (currentSubtitleIndex >= 0 && currentSubtitleIndex < subtitles.Count)
    //     {
    //         var subtitle = subtitles[currentSubtitleIndex];
    //         if (currentTime >= subtitle.start && currentTime <= subtitle.end)
    //         {
    //             return;
    //         }
    //     }

    //     int index = subtitles.FindIndex(s => currentTime >= s.start && currentTime <= s.end);
    //     if (index != -1)
    //     {
    //         subtitlesText.text = subtitles[index].text;
    //         currentSubtitleIndex = index;
    //     }
    //     else
    //     {
    //         subtitlesText.text = "";
    //     }
    // }
    void UpdateSubtitle(long HISplayertime)
    {
        float currentTime = HISplayertime / 1000f;
        if (currentSubtitleIndex >= 0 && currentSubtitleIndex < subtitles.Count)
        {
            var subtitle = subtitles[currentSubtitleIndex];
            if (currentTime >= subtitle.start && currentTime <= subtitle.end)
            {
                return;
            }
        }

        int index = subtitles.FindIndex(s => currentTime >= s.start && currentTime <= s.end);
        if (index != -1)
        {
            ObjChatBG.SetActive(true);
            ObjNameBG.SetActive(true);
            var subtitle = subtitles[index];
            string name = subtitle.name;
            string text = subtitle.text;
            subtitlesText.text = text;
            NameText.text = name;
            // subtitlesText.text = subtitles[index].text;
            // NameText.text = subtitles[index].name;
            // currentSubtitleIndex = index;
        }
        else
        {
            ObjChatBG.SetActive(false);
            ObjNameBG.SetActive(false);
            subtitlesText.text = "";
            NameText.text = "";
        }
    }
    public enum Language
    {
        中文,
        日文,
        英文,
    }
    public class UrlToScence
    {
        public Dictionary<string, string> videoDictionary;
    }
}
