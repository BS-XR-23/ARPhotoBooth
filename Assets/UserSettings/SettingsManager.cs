using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using TryOnPlus.Unity;


public class SettingsManager : MonoBehaviour
{
  public static SettingsManager Instance;
  public Config config;
  [SerializeField]
  private Solution solution;
  [SerializeField]
  private TMP_Dropdown resulotionDropdown;
  [SerializeField]
  private TMP_Dropdown sourceDropdown;
  [SerializeField]
  private TMP_Dropdown scaleTypeDropdown;
  [SerializeField]
  private TMP_Dropdown trackingVariant;
  [SerializeField]
  private TMP_Dropdown timerDropdown;
  [SerializeField]
  private Material brightnessMat;
  [SerializeField]
  private Slider brightnessSlider;
  [SerializeField]
  private Slider cameraDistanceSlider;
  [SerializeField]
  private TextMeshProUGUI cameraDistanceText;
  [SerializeField]
  private Slider scaleFactorSlider;
  [SerializeField]
  private TextMeshProUGUI scaleFactorText;
  [SerializeField]
  private TextMeshProUGUI intensityText;
  public Slider lightIntensity;
  public RectTransform settingsPanel;
  public Vector2 panelOpenPosition;
  public float openDuration = 3;
  private Vector2 initialPos;
  public GameObject settingsButton;
  public Light light;
  private Vector2 lastMousePos;
  private float settingsButtonShowingTime = 3;
  public SettingsData settingsData = new SettingsData();
  private string settingsPath;
  private Vector3 grassInitialPos;
  private bool settingsPanelIsOpen = false;
  private List<string> tryOnTimes=new List<string>() { "30 secs", "35 secs", "40 secs", "45 secs", "50 secs", "55 secs", "60 secs", "90 secs", "120 secs", "150 secs", "180 secs" };
  private void Awake()
  {
    Instance = this;
    Debug.Log("Setting Initialize");
    settingsPath = Path.Combine(Application.persistentDataPath, "settings.json");
    if (File.Exists(settingsPath))
    {
        string settingsJson = File.ReadAllText(settingsPath);
        settingsData = JsonConvert.DeserializeObject<SettingsData>(settingsJson);
        Debug.Log($"Settings Data2: {settingsData.cameraResolution}");

        Debug.Log(settingsJson);
    }

    Debug.Log("Scale Factor:"+ settingsData.scale_factor);
    settingsData.bodyRadius = 0;
    scaleFactorText.text = $"Scale Factor({settingsData.scale_factor.ToString("F2")})";
  
  }
   private void Start()
   {
        timerDropdown.AddOptions(tryOnTimes);
        settingsButton.SetActive(false);

        lightIntensity.onValueChanged.AddListener(UpdateLightIntensity);
        scaleFactorSlider.onValueChanged.AddListener(UpdateScaleFactor);
        cameraDistanceSlider.onValueChanged.AddListener(UpdateCameraDistance);

        initialPos = settingsPanel.anchoredPosition;
        lightIntensity.value = settingsData.lightIntensity >= 0 ? settingsData.lightIntensity : 1f;

        scaleFactorSlider.value = settingsData.scale_factor;
        scaleTypeDropdown.value = settingsData.scale_type;
        timerDropdown.value = settingsData.timer;
        trackingVariant.value = settingsData.tracking_variant;
        cameraDistanceSlider.value = settingsData.camera_distance;
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        //brightnessMat.SetFloat("_brightness", settingsData.brightness);
        brightnessSlider.value = settingsData.brightness;
        lastMousePos = Input.mousePosition;
        UpdateTryOnTime(settingsData.timer);

    }
  
    private void Update()
    {
      Vector2 currentMousePos = Input.mousePosition;
      if(Vector2.Distance(currentMousePos, lastMousePos)>1&& !settingsPanelIsOpen)
      {
        settingsButton.SetActive(true);
        CancelInvoke("Hide");
        Invoke("Hide", settingsButtonShowingTime);
      }
      
      lastMousePos = Input.mousePosition;
    }
      public void Hide()
      {
        settingsButton.SetActive(false);
      }
      public void Save()
      {
        string settingsJson = JsonConvert.SerializeObject(settingsData);
        Debug.Log(settingsJson);
        File.WriteAllText(settingsPath,settingsJson);
      }
      public void UpdateScaleFactor(float scale_factor)
      {
        Debug.Log("scale_factor"+ scale_factor);
        settingsData.scale_factor = scale_factor;
        scaleFactorText.text= $"Scale Factor({scale_factor.ToString("F2")})";
        config.scaleOffset =Vector3.one* scale_factor;
        Save();
      }
    public void UpdateCameraDistance(float distance)
    {
        settingsData.camera_distance = distance;
        cameraDistanceText.text = $"Cam Distance({settingsData.camera_distance.ToString("F2")})";
        //avatar.cameraDistance = distance;
        Save();
    }
    public void UpdateXOffset(float offset)
    {
        settingsData.xOffset = offset;
        config.positionOffset = new Vector3(offset, settingsData.yOffset);
        Save();
    }
    public void UpdateYOffset(float offset)
    {
        settingsData.yOffset = offset;
        config.positionOffset = new Vector3(settingsData.xOffset, offset);
        Save();
    }
   
    public void UpdateScaleType(int scale_type)
      {
        settingsData.scale_type = scale_type;
        Save();
      }

    public void UpdateVariant(int variant)
  {
    settingsData.tracking_variant = variant;
    Save();
  }
  public void UpdateTryOnTime(int timeIndex)
  {
    string filteredTime = tryOnTimes[timeIndex].Replace("secs", "");
    int.TryParse(filteredTime,out int tryOnTime);
    settingsData.timer = timeIndex;
    Save();
  }
  public void UpdateLightIntensity(float intensity)
  {
    light.intensity = intensity;
    intensityText.text = $"Lighting:({settingsData.lightIntensity.ToString("F2")})";
    settingsData.lightIntensity = intensity;
    Save();
  }
  public void UpdateBrightness(float brightness)
  {
    brightnessMat.SetFloat("_brightness", brightness);
    settingsData.brightness = brightness;
    Save();
  }
  public void OpenSettingPanel()
  {
    settingsPanelIsOpen=true;
    settingsPanel.gameObject.SetActive(true);
    settingsPanel.DOAnchorPosX(panelOpenPosition.x, openDuration);
    InitializeContents();
  }
  public void HideSettingPanel()
  {
    settingsPanelIsOpen = false;
    settingsPanel.DOAnchorPosX(initialPos.x, openDuration*.75f).OnComplete(()=>
    {
      settingsPanel.gameObject.SetActive(false);
    });
  }
  public void Quit()
  {
    Application.Quit();
  }
  public void InitializeContents()
  {
    InitializeSource();
    InitializeResolution();
  }
  public void InitializeSource()
  {
    sourceDropdown.ClearOptions();
    sourceDropdown.onValueChanged.RemoveAllListeners();

    var imageSource = ImageSourceProvider.ImageSource;
    var sourceNames = imageSource.sourceCandidateNames;

    if (sourceNames == null)
    {
      sourceDropdown.enabled = false;
      return;
    }

    var options = new List<string>(sourceNames);
    sourceDropdown.AddOptions(options);

    var currentSourceName = imageSource.sourceName;
   int defaultValue =string.IsNullOrEmpty( settingsData.cameraSource)? options.FindIndex(option => option == currentSourceName) : options.FindIndex(option => option == settingsData.cameraSource);

    sourceDropdown.onValueChanged.AddListener(delegate
    {
      imageSource.SelectSource(sourceDropdown.value);
      settingsData.cameraSource = options[sourceDropdown.value];
      Debug.Log($"Source Init:{settingsData.cameraSource}");
      Save();
      InitializeResolution();
    });
    sourceDropdown.value = defaultValue;
    imageSource.SelectSource(sourceDropdown.value);

  }
 
  private void InitializeResolution()
  {
    resulotionDropdown.ClearOptions();
    resulotionDropdown.onValueChanged.RemoveAllListeners();

    var imageSource = ImageSourceProvider.ImageSource;
    var resolutions = imageSource.availableResolutions;
    if (resolutions == null)
    {
      //resulotionDropdown.enabled = false;
      return;
    }

    var options = resolutions.Select(resolution => resolution.ToString()).ToList();
    resulotionDropdown.AddOptions(options);

    var currentResolutionStr = imageSource.resolution.ToString();
    var defaultValue = settingsData.cameraResolution >= 0 ? settingsData.cameraResolution : options.FindIndex(option => option == currentResolutionStr);

    Debug.Log($"Settings Data: {settingsData.cameraResolution}");
    resulotionDropdown.onValueChanged.AddListener(delegate
    {
      Debug.Log($"Resolution Init: {defaultValue}");
      imageSource.SelectResolution(resulotionDropdown.value);
      settingsData.cameraResolution = resulotionDropdown.value;
      Save();
      RestartSolution(true);
    });
    Debug.Log($"Resolution Init2: {defaultValue}");
    if (defaultValue >= 0)
    {
      resulotionDropdown.value = defaultValue;
    }
    imageSource.SelectResolution(resulotionDropdown.value);
    RestartSolution(true);
  }
  public void Reset()
  {
    
    settingsData=new SettingsData(); 
    lightIntensity.value = settingsData.lightIntensity >= 0 ? settingsData.lightIntensity : 1;
        brightnessSlider.value = settingsData.brightness;
    scaleFactorSlider.value = settingsData.scale_factor;
    scaleTypeDropdown.value = settingsData.scale_type;
    config.scaleOffset = Vector3.one * settingsData.scale_factor;
    cameraDistanceSlider.value=settingsData.camera_distance;
    InitializeContents();
    Save();
  }
  public async void RestartSolution(bool forceRestart = false)
  {
    solution.Pause();
    await UniTask.Delay(1000);
    if (forceRestart)
    {
      Debug.Log("Restart Solution");
      solution.Play();
    }
    else
    {
      solution.Resume();
    }
  }
  public void RestartApplication()
  {
    solution.Stop();
    string currentSceneName = SceneManager.GetActiveScene().name;
    SceneManager.LoadScene(currentSceneName);
    Debug.Log(currentSceneName);
  }
  
}
public class Vector_2
{
    public float x;
    public float y;
    public Vector_2(float x,float y)
    {
        this.x = x;
        this.y = y;
    }
    
}
