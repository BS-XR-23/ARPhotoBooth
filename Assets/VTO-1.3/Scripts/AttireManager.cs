using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TryOnPlus.Unity;
using TryOnPlus.Unity.PoseTracking;
using UnityEngine;
using UnityEngine.UI;

public class AttireManager : MonoBehaviour
{
    public static AttireManager Instance;
    public GameObject tPoseInstruction;
    public List<AttireModel> attires;
    public List<AttireModel> beltJewelleryattires;
    public Toggle attireCheckboxPrefab;
    public Transform attireSelectionPanel;
    private bool settingsPanelIsOpen = false;
    private Vector2 lastMousePos;
    private float settingsButtonShowingTime = 3;
    public AvatarMocap avatarMocap;
    [HideInInspector]
    public float cameraDistance;
    [HideInInspector]
    public Vector3 scaleOffset;
    [HideInInspector]
    public Vector3 positionOffset;
    [HideInInspector]
    public bool wearAttire = false;
    public int defaultSelect = 1;
    public List<DressUIItem> dressUIItems=new List<DressUIItem>();
    [SerializeField]
    public List<RectTransform> cursors;
    [SerializeField]
    private DressUIItem dressItemPrefab;
    [SerializeField] private List<Transform> dressRoots = new();
    [SerializeField] private Transform beltJewelleryRoot;
    [SerializeField]
    private RectTransform activeInSide;
    public GameObject screenshotBtn;
    public GameObject screenshotUI;
    private AttireModel previousAttireModel;
    private bool previewMode = false;
    private Camera camera;
    public Transform area;
    public Transform UI;
    public TextMeshProUGUI selectYourNecklaceText;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(ShowDefaultJewellery());
    }

    private void ControlDressSelectionUIVisibility(bool isVisible)
    {
        foreach (var dressRoot in dressRoots)
        {
            dressRoot.gameObject.SetActive(isVisible);
        }
    }
    IEnumerator ShowDefaultJewellery()
    {
        yield return new WaitUntil(() => attires.Count > defaultSelect);
        previousAttireModel = attires[defaultSelect];
        Initialize();
    }
    public void Initialize()
    {
        wearAttire = true;
        camera = Camera.main;
        //tPoseInstruction.gameObject.SetActive(true);
        MapBones();
        avatarMocap.OnTPose.AddListener(() =>
        {
            //if(!previewMode) StartTryOn();
        });
        avatarMocap.OnTrackLose.AddListener(() =>
        {
            if (!previewMode)
            {
                wearAttire = false;
                //tPoseInstruction.gameObject.SetActive(true);
                StopTryOn();
            }
            //wearAttire = false;
            //HideAttire(previousAttireModel);

        });
        avatarMocap.OnTrack.AddListener(() =>
        {
            //if(wearAttire)  TrackAttire();
            if (!previewMode&& !wearAttire)
            {
                wearAttire = true;
                StartTryOn();
            }
        });
        //InitAttireUI();
        cursors[0].gameObject.SetActive(false);
        cursors[1].gameObject.SetActive(false);
        ControlDressSelectionUIVisibility(false);
        selectYourNecklaceText.gameObject.SetActive(false);
        screenshotBtn.gameObject.SetActive(false);
        StartTryOn();
    }
    public void StartTryOn()
    {
        previewMode = false;
        wearAttire = true;
        cursors[0].gameObject.SetActive(true);
        cursors[1].gameObject.SetActive(true);
        ApplicationManager.Instance.IsHoverActive = true;
        tPoseInstruction.gameObject.SetActive(false);
        ControlDressSelectionUIVisibility(true);
        screenshotBtn.gameObject.SetActive(true);
        selectYourNecklaceText.gameObject.SetActive(true);
        ShowAttire(previousAttireModel);
    }
    public void StartPreview()
    {
        previewMode = true;
        HideAttire(previousAttireModel);
    }
    public void StopTryOn()
    {
        selectYourNecklaceText.gameObject.SetActive(false);
        ControlDressSelectionUIVisibility(false);
        screenshotBtn.gameObject.SetActive(false);
    }
    public void DisableGesture()
    {
        wearAttire = false;
        ApplicationManager.Instance.IsHoverActive = false;
        cursors[0].gameObject.SetActive(false);
        cursors[1].gameObject.SetActive(false);
    }
    
    public void ShowAttire(AttireModel attireModel)
    {
        if (attireModel == null) return;
        if (previousAttireModel != null) HideAttire(previousAttireModel);
        attireModel.show = true;
        attireModel.attireModel.ReInit(avatarMocap);
        attireModel.attireModel.gameObject.SetActive(true);
        foreach(AttireModel dependentAttire in attireModel.dependentAttire)
        {
            dependentAttire.show = true;
            dependentAttire.attireModel.gameObject.SetActive(true);
        }
        previousAttireModel=attireModel;
    }
    private void HideAttire(AttireModel attireModel)
    {
        attireModel.show = false;
        attireModel.attireModel.gameObject.SetActive(false);
        foreach (AttireModel dependentAttire in attireModel.dependentAttire)
        {
            dependentAttire.show = false;
            dependentAttire.attireModel.gameObject.SetActive(false);
        }
    }
    private void MapBones()
    {
        foreach (AttireModel attireModel in attires)
        {
            attireModel.attireModel.MapBones(avatarMocap);
            attireModel.attireModel.gameObject.SetActive(false);
        }
    }
    public void TrackAttire()
    {
        foreach (AttireModel attireModel in attires)
        {
            if(attireModel.show)
            {
                attireModel.attireModel.AdjustAttirePosition(avatarMocap);
                attireModel.attireModel.AdjustAttireScale(avatarMocap);
                attireModel.attireModel.AdjustAttireRotation(avatarMocap);
                foreach (AttireModel dependentAttire in attireModel.dependentAttire)
                {
                    dependentAttire.attireModel.AdjustAttirePosition(avatarMocap);
                    dependentAttire.attireModel.AdjustAttireScale(avatarMocap);
                    dependentAttire.attireModel.AdjustAttireRotation(avatarMocap);
                }
            }
        }
        cursors[0].position =Vector2.Lerp(cursors[0].position,avatarMocap.bones[(int)Body.LEFT_INDEX].transform.position,Time.deltaTime*10);
        cursors[1].position = Vector2.Lerp(cursors[1].position, avatarMocap.bones[(int)Body.RIGHT_INDEX].transform.position, Time.deltaTime * 10);

        var neckPosition = (avatarMocap.bones[(int)Body.LEFT_SHOULDER].transform.position +
                            avatarMocap.bones[(int)Body.RIGHT_SHOULDER].transform.position) / 2f;
        cursors[2].position = neckPosition;
    }
    private void LateUpdate()
    {
        if (wearAttire) TrackAttire();
    }
    public void Hide()
    {
        //attireSelectionPanel.gameObject.SetActive(false);
    }
    private void RotateToward(Transform parent, Transform child, RectTransform target)
    {
        Vector2 targetDirection = (Vector2)child.position - (Vector2)parent.position;
        target.up = targetDirection;
    }

}
[Serializable]
public class AttireModel
{
    public string attireName;
    public BaseAttire attireModel;
    [HideInInspector]
    public Toggle checkbox;
    [SerializeField]
    public List<AttireModel> dependentAttire;
    [HideInInspector]
    public bool show=true;
    public Sprite thumbnail;
}

