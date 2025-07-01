using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ScreenshotController : MonoBehaviour
{
    public static ScreenshotController Instance;
    [SerializeField]
    private Camera _targetCamera;
    [SerializeField]
    private RawImage _screenshot;
    [SerializeField]
    private RawImage _QRCode;
    [SerializeField]
    private GameObject _qrCodePage;
    [SerializeField]
    private Transform _screenshotInPreviewContainer;
    [SerializeField]
    private CountDownTimer _countDownTimer;
    [SerializeField]
    private ParticleSystem _particle;
    [SerializeField]
    private Vector3 _qrRectScale;
    [SerializeField]
    private float _imagePreviewTime = 30;
    [SerializeField]
    private float _qRPreviewTime = 30;
    private Texture2D _screenshotImage;
    public UnityEvent OnImagePreviewFinish;
    public UnityEvent OnQRPreviewFinish;
    public UnityEvent OnScreenshotComplete;
    private RenderTexture _cameraRenderTexture;
    private bool _startDissolveAnim = false;
    private int retryCount = 0;
    private int maxRetry = 3;
    private bool IsImageUploading = false;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        _qrCodePage.SetActive(false);
        _countDownTimer.gameObject.SetActive(false);
    }
    [ContextMenu("TakeScreenshot")]
    public void TakeScreenshot()
    {
        gameObject.SetActive(true);
        StartCoroutine(TakeScreenshotCoroutine());
    }
    public IEnumerator TakeScreenshotCoroutine()
    {
        Debug.Log("Takescreenshot");
        //GestureManager.Instance.checkGesture = false;
        _targetCamera.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        _cameraRenderTexture = new RenderTexture(_targetCamera.targetTexture.width, _targetCamera.targetTexture.height, 24);
        Graphics.Blit(_targetCamera.targetTexture, _cameraRenderTexture);
        RenderTexture.active = _cameraRenderTexture;
        _screenshotImage = new Texture2D(_cameraRenderTexture.width, _cameraRenderTexture.height, TextureFormat.RGBA32, false);
        _screenshotImage.ReadPixels(new Rect(0, 0, _cameraRenderTexture.width, _cameraRenderTexture.height), 0, 0);
        _screenshotImage.Apply();
        _screenshot.texture = _screenshotImage;
        OnScreenshotComplete?.Invoke();
        byte[] image = _screenshotImage.EncodeToPNG();
        SaveImage(image);
        ShowPreview();
        RenderTexture.active = null;
        _targetCamera.gameObject.SetActive(false);

    }
    public async void ShowPreview()
    {
        if (_startDissolveAnim) return;
        _startDissolveAnim = true;
        _qrCodePage.SetActive(true);
        _QRCode.gameObject.SetActive(false);
        _screenshotInPreviewContainer.localScale = Vector3.one;
        await _screenshotInPreviewContainer.DOScale(_qrRectScale, 1f).AsyncWaitForCompletion();
        _countDownTimer.gameObject.SetActive(true);
        _countDownTimer.totalTime = _imagePreviewTime;
        UploadImage(_screenshotImage);
        _countDownTimer.StartTimer();
        _countDownTimer.OnFinished.RemoveAllListeners();
        _countDownTimer.OnFinished.AddListener(()=>
        {
            OnImagePreviewFinish?.Invoke();
            ShowQRCodePage();
        });
        
    }
    
    public async void ShowQRCodePage()
    {
        _countDownTimer.gameObject.SetActive(false);
        await ShowDissolveAnim();
        _countDownTimer.gameObject.SetActive(true);
        _countDownTimer.totalTime = _qRPreviewTime;
        _countDownTimer.StartTimer();
        _countDownTimer.OnFinished.RemoveAllListeners();
        _countDownTimer.OnFinished.AddListener(()=>
        {
            Hide();
            OnQRPreviewFinish?.Invoke();
            Debug.Log("QR preview finish");

        });

    }
    public void Hide()
    {
        _countDownTimer.OnFinished.RemoveListener(Hide);
        _countDownTimer.StopTimer();
        _countDownTimer.gameObject.SetActive(false);
        _qrCodePage.SetActive(false);
    }
    public void Test()
    {
        Debug.Log("I am called");
    }
    public void SaveImage(byte[] image)
    {
        string screenshot_path = Path.Combine(Application.persistentDataPath, "screenshots");
        if (!Directory.Exists(screenshot_path)) Directory.CreateDirectory(screenshot_path);
        File.WriteAllBytes(Path.Combine(screenshot_path, "output.png"), image);
    }
    [ContextMenu("ShowDissolveAnim")]
    public async UniTask ShowDissolveAnim()
    {
        _QRCode.gameObject.SetActive(true);
        Material dissolve =_QRCode.material;
        dissolve.SetFloat("_cutoff_height", -142f);
        _particle.Stop();
        _particle.Play();
        await UniTask.Delay(10);
        DOTween.To(() => -142f, x =>
        {
            dissolve.SetFloat("_cutoff_height", x);
        },261f, 2f)
        .OnComplete(() =>
        {
            Debug.Log("Tween Complete");
        });
        await UniTask.Delay(1350);
        await UniTask.Delay(1);
        Debug.Log("Done. Thanks");
        _startDissolveAnim = false;
    }

    public async UniTask Retry()
    {
        if (retryCount < maxRetry)
        {
            retryCount++;

        }
    }
    public async void UploadImage(Texture2D texture2D)
    {
        if (IsImageUploading) return;
        IsImageUploading = true;
        byte[] image = texture2D.EncodeToPNG();
        Debug.Log($"Start Upload Image");
        string reponseText = await APIRequestManager.Instance.UploadImageAsync(image, APIEndPoints.uploadScreenshot, async () =>
        {
            await Retry();
        });
        if (!string.IsNullOrEmpty(reponseText))
        {
            Response<ImageData> response = JsonConvert.DeserializeObject<Response<ImageData>>(reponseText);
            Debug.Log($"Download Image:{response.data.url}");
            byte[] bytes = await APIRequestManager.Instance.DownloadFile(response.data.url);
            Debug.Log($"Download Image:{bytes.Length}");
            if (bytes != null)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                texture.Apply();
                _QRCode.texture = texture;
                //SaveImage(bytes);
            }
            else
            {
                Debug.Log("Failed To Download QR Code");
                await Retry();

            }

        }
        else
        {
            Debug.Log("Failed To Upload Image");
            await Retry();

        }
        IsImageUploading = false;
    }

}
