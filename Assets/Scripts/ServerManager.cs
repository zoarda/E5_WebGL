using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    const string serverUrl = "https://av1-api-dev.funplaytech.com";
    // const string serverUrl = "http://localhost:5688";

    public static ServerManager Instance { get; private set; }

    public UrlData urlData = new UrlData();

    public TokenData tokenData = new TokenData();

    string curToken;

    async void Awake()
    {
        // 單例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 可選，確保在場景切換時保留
    }

    /// <summary>
    /// 初始化 URL 查詢數據
    /// </summary>
    public async UniTask InitializeUrlQueryAsync()
    {
        await SetUrlQueryAsync();
    }

    private async UniTask SetUrlQueryAsync()
    {
        // 測試用 URL，實際使用 Application.absoluteURL
        string absoluteURL = Application.absoluteURL;
#if UNITY_EDITOR
        absoluteURL = "http://localhost:13948/?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZnVuQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiIiwiaXNzIjoiU2l4IFBhcnR5IFBsYXllciIsInN1YiI6ImNjNmY5N2I3LTk2NjAtNGEzNy1hYTlkLTExYjRmMjg0ODY5ZiIsImVtYWlsIjoiZnVuQGV4YW1wbGUuY29tIiwiYXVkIjoiZnVuQGV4YW1wbGUuY29tIiwiZXhwIjoxNzM3NTk3NjE5LCJqdGkiOiJhMzRjNmJjZi05MmJhLTQ5NTktYTNmZi1iYjZjN2M3ZDAzMjkiLCJpYXQiOjE3MzUxOTc2MTksIm5iZiI6MTczNTE5NzYxOX0.wkjZb913JqIbH6PD7V90ckOGFGd9xE0I2Y7C0bOand8&lang=456";
#endif
        Debug.Log($"absoluteURL: {absoluteURL}");
        // string p = "https:d1seruguac4v04.cloudfront.net/?token=123&lang=";
        string? token = null;
        string? language = null;

        if (absoluteURL.Contains("?"))
        {
            string[] stringP = absoluteURL.Split('?');
            string qs = stringP[1];
            string[] data = qs.Split('&');

            foreach (var a in data)
            {
                if (a.Contains("="))
                {
                    string[] b = a.Split('=');
                    if (b.Length == 2)
                    {
                        var key = b[0];
                        switch (key)
                        {
                            case "token":
                                token = b[1];
                                break;
                            case "lang":
                                language = b[1];
                                break;
                            default:
                                Debug.LogWarning($"Unrecognized key: {key}, value: {b[1]}");
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid data format: {a}");
                    }
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                await ShowErrorPageAsync("Token is null. Please log in again.");
                return;
            }

            if (string.IsNullOrEmpty(language))
            {
                Debug.LogWarning("Language not specified. Defaulting to English.");
                language = "en"; // 設定預設語言
            }
        }
        else
        {
            await ShowErrorPageAsync("No query string found in the URL. Please try again.");
            return;
        }

        // 更新 UrlData
        urlData = new UrlData
        {
            token = token,
            language = language,
            platform = 1
        };

        Debug.Log($"URL Data Initialized: Token={urlData.token}, Language={urlData.language}");
        await Login();

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 顯示錯誤頁面（模擬異步行為）
    /// </summary>
    private async UniTask ShowErrorPageAsync(string message)
    {
        Debug.LogError(message);

        // 模擬等待，例如顯示錯誤提示頁面或等待用戶操作
        await UniTask.Delay(2000);

        // 可選：退出應用程式或執行其他操作
        // Application.Quit();
    }
    public async UniTask Login()
    {
        // HttpClient httpClient = new();


        var url = $"{serverUrl}/api/o/Player/Create";
        Debug.Log($"login with url: {url}");
        try
        {
            var jurlData = JsonUtility.ToJson(urlData);
            Debug.Log($"jLoginData: {jurlData}");
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jurlData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
                return;
            }

            string responseBody = request.downloadHandler.text;
            Debug.Log($"Response: {responseBody}");

            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(responseBody);
            Debug.Log($"shrimp api response Message {apiResponse.message ?? "null"}");
            // Debug.Log($"shrimp apiResponse Data {apiResponse.Data ?? "null"} shrimp apiResponse Succes {(apiResponse.Success.HasValue ? apiResponse.Success.Value.ToString() : "null")}");
            if (apiResponse.success == false ||
                apiResponse.success == null)
            {
                Debug.Log($"Request failed");
                return;
            }

            curToken = apiResponse.data;
            Debug.Log($"curToken: {curToken}");
        }
        catch (Exception ex)
        {
            Debug.Log($"Request failed: {ex.Message}");
            return;
        }
    }
    public async UniTask<SaveData?> Load()
    {
        string url = $"{serverUrl}/api/a/Player/Load";

        Debug.Log("Shrimp try loading");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, string.Empty))
        {

            request.SetRequestHeader("Authorization", $"Bearer {curToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log($"Authorization Header: Bearer {curToken}");
            Debug.Log("Content-Type Header: application/json");

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Request Failed: {request.error}");
                return null;
            }
            Debug.Log($"Request succes: {request.result}");
            try
            {

                string responseBody = request.downloadHandler.text;
                var apiResponse = JsonUtility.FromJson<ApiResponse>(responseBody);

                if (apiResponse.success == false || apiResponse.success == null)
                {
                    Debug.LogError($"Request failed: {apiResponse.message}");
                    return null;
                }
                if (string.IsNullOrEmpty(apiResponse.data))
                {
                    Debug.Log($"Shrimp load success but no playerdata: {apiResponse.data}");

                    // No data found, save a default SaveData object
                    SaveData defaultSaveData = new SaveData()
                    {
                        friendship = 0,
                        scriptName = new List<string>()
                    }; // Create a default SaveData
                    defaultSaveData.scriptName.Add("C1_VB");
                    defaultSaveData.scriptName.Add("C1_S0");
                    await Save(defaultSaveData); // Save it to the server
                    Debug.Log("Default SaveData has been saved");

                    // Return the default SaveData object after saving it
                    return defaultSaveData;
                }

                var saveData = JsonUtility.FromJson<SaveData>(apiResponse.data);
                Debug.Log($"Request SuccessMessage: {apiResponse.message}");
                Debug.Log($"Shrimp load success: {saveData}");
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing response: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }

    }
    public async UniTask Save(SaveData saveData)
    {
        string url = $"{serverUrl}/api/a/Player/Save";

        string save = JsonUtility.ToJson(saveData);
        Debug.Log($"shrimp jsave {save}");
        JasonSaveData datajson = new JasonSaveData() { Data = save };
        string data = JsonUtility.ToJson(datajson);
        Debug.Log($"shrimp savedata{data}");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, data))
        {

            request.SetRequestHeader("Authorization", $"Bearer {curToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log($"Authorization Header: Bearer {curToken}");
            Debug.Log("Content-Type Header: application/json");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();


            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Request Failed: {request.error}");
                return;
            }
            Debug.Log("requset SuccessMessage");
            try
            {
                string responseBody = request.downloadHandler.text;
                var apiResponse = JsonUtility.FromJson<ApiResponse>(responseBody);

                if (apiResponse.success == false || apiResponse.success == null)
                {
                    Debug.LogError($"Request failed: {apiResponse.message}");
                    return;
                }

                Debug.Log($"Shrimp save success: {saveData}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing response: {e.Message}\n{e.StackTrace}");
            }
        }
    }
    class JasonSaveData
    {
        public string Data;
    }
    [System.Serializable]
    public class ApiResponse
    {
        /// <summary>
        /// Message
        /// </summary>
        public string message;

        /// <summary>
        /// Error code
        /// </summary>
        public string error;

        /// <summary>
        /// Success
        /// </summary>
        public bool success;

        /// <summary>
        /// Data
        /// </summary>
        public string data;
    }
    /// <summary>
    /// URL 數據結構
    /// </summary>
    public class UrlData
    {
        public string token;
        public string language;
        public int platform;
    }
    public class TokenData
    {
        public string token;
    }
    //存檔資料
    public class SaveData
    {
        public float friendship;
        public List<string> scriptName;
    }
}
