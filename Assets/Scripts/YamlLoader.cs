using System;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using Naninovel;
using System.Collections.Generic;
using UnityEngine.Networking;

public static class YamlLoader
{
    public static async UniTask<T> LoadStreamingAssetsYaml<T>(string yamlFilePath)
    {
        string a = "";
        try
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                UnityWebRequest request = UnityWebRequest.Get(yamlFilePath);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    a = request.downloadHandler.text;
                else
                    Debug.LogError($"Error loading YAML file: {request.error}");
            }
            else
            {
                a = File.ReadAllText(yamlFilePath);
            }

            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<T>(a);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading YAML file: {yamlFilePath}\n{e}");
            return default;
        }
    }
    // public static async UniTask<T> LoadYaml<T>(string yamlFilePath)
    // {
    //     try
    //     {

    //         string a = "";

    //         if (!File.Exists(yamlFilePath))
    //         {
    //             StartNani.SaveData saveData = new()
    //             {
    //                 friendship = 0,
    //                 scriptName = new List<string>()
    //             };
    //             saveData.scriptName.Add("C1_VB");
    //             SaveYaml(saveData);
    //         }
    //         string yamlContent = File.ReadAllText(yamlFilePath);
    //         a = yamlContent;
    //         var deserializer = new DeserializerBuilder() { }.Build();
    //         var items = deserializer.Deserialize<T>(a);
    //         return items;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Error loading YAML file: {yamlFilePath}\n{e.StackTrace}");
    //         return default;
    //     }
    // }
    // public static void SaveYaml(StartNani.SaveData saveData)
    // {
    //     var serialization = new SerializerBuilder() { }.Build();
    //     string data = serialization.Serialize(saveData);
    //     File.WriteAllText(Application.persistentDataPath + "/SaveData.yaml", data);
    // }
}
