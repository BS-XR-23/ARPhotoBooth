using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class APIRequestManager
{
  public UnityWebRequest[] requests=new UnityWebRequest[2];
  private static APIRequestManager instance;
  public static APIRequestManager Instance
  {
    get
    {
      if (instance == null)
      {
        instance = new APIRequestManager();
      }
      return instance;
    }
  }
  public async UniTask<string> UploadImageAsync(byte[] image, string url,UnityAction OnFailure = null)
  {
    if (requests[0] != null) requests[0].Dispose();
    WWWForm form = new WWWForm();
    form.AddBinaryData("image", image, "image.png", "image/png");
    form.AddField("activationId", "123456");
    form.AddField("isDevelopment","true");
    UnityWebRequest uploadRequest = UnityWebRequest.Post(url, form);
    requests[0] = uploadRequest;
    //www.timeout = ApplicationManager.Instance.timeout;
    var operation = uploadRequest.SendWebRequest();
    await UniTask.WaitUntil(() => operation.isDone);

    if (uploadRequest.result != UnityWebRequest.Result.Success)
    {
      Debug.LogError("Error uploading image: " + uploadRequest.error);
    }
    else
    {
      var jsonResponse = operation.webRequest.downloadHandler.text;
      Debug.Log(jsonResponse);
      Debug.Log("Image uploaded successfully!");
      uploadRequest.Dispose();
      return jsonResponse;
    }
    uploadRequest.Dispose();
    return null;
  }
    
  public async UniTask<byte[]> DownloadFile(string url, UnityAction OnFailure = null)
  {
    if (requests[1] != null) requests[1].Dispose();
    UnityWebRequest downloadRequest = UnityWebRequest.Get(url);
    requests[1] = downloadRequest;
    UnityWebRequestAsyncOperation operation = downloadRequest.SendWebRequest();

    while (!operation.isDone)
    {
      await UniTask.Yield();
    }
   
    if (downloadRequest.error != null || downloadRequest.responseCode != 200)
    {
      OnFailure?.Invoke();
      downloadRequest.Dispose();
      return null;
    }
    else
    {
      byte[] data = downloadRequest.downloadHandler.data;
      downloadRequest.Dispose();
      return data;

    }
  }
}
