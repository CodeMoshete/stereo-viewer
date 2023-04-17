using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : MonoBehaviour
{
    private const string BASE_URL = "http://codemoshete.com/3dImageViewer";
    private const string MANIFEST_NAME = "manifest.json";

    public void GetManifest(Action<string> callback)
    {
        StartCoroutine(DownloadManifest(callback));
    }

    private IEnumerator DownloadManifest(Action<string> onDone)
    {
        string manifestUrl = string.Format("{0}/{1}", BASE_URL, MANIFEST_NAME);
        Debug.Log(string.Format("Downloading manifest from: {0}", manifestUrl));
        UnityWebRequest www = UnityWebRequest.Get(manifestUrl);
        AddHeadersToWebRequest(www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error downloading manifest: " + www.error);
            onDone(null);
        }
        else
        {
            Debug.Log("Manifest loaded successfully!");
            onDone(www.downloadHandler.text);
        }
    }

    public void GetTexture(string texturePath, Action<Texture> callback)
    {
        StartCoroutine(DownloadTexture(texturePath, callback));
    }

    private IEnumerator DownloadTexture(string texturePath, Action<Texture> onDone)
    {
        string textureUrl = string.Format("{0}/{1}", BASE_URL, texturePath);
        Debug.Log(string.Format("Downloading texture from: {0}", textureUrl));
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(textureUrl);
        AddHeadersToWebRequest(www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error downloading texture: " + www.error);
            onDone(null);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            onDone(myTexture);
        }
    }

    private void AddHeadersToWebRequest(UnityWebRequest www)
    {
        www.SetRequestHeader("Accept", "*/*");
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate");
        www.SetRequestHeader("User-Agent", "runscope/0.1");
    }
}
