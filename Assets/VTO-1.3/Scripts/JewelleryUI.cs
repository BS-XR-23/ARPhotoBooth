using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class JewelleryUI : MonoBehaviour
{
    public DressUIItem attireUIItem;
    public RectTransform root;
    [SerializeField] private List<AttireModel> attires = new();
    private void Start()
    {
        InitAttires();
    }

    private void InitAttires()
    {
        for (int i = 0; i < attires.Count; i++)
        {
            DressUIItem dressUIItem = Instantiate(attireUIItem, root);
            dressUIItem.name = attires[i].attireName;
            dressUIItem.Init(attires[i], attires[i].thumbnail);
            AttireManager.Instance.attires.Add(attires[i]);
            AttireManager.Instance.dressUIItems.Add(dressUIItem);
            SetupHover(dressUIItem);
        }
    }
    public void SetupHover(DressUIItem dressUIItem)
    {
        HoverDetector hoverDetector = dressUIItem.GetComponent<HoverDetector>();
        hoverDetector.cursors = AttireManager.Instance.cursors;
        hoverDetector.dressItem = true;
        hoverDetector.showTimer = true;
        hoverDetector.OnHoverExit.AddListener(() =>
        {
            ApplicationManager.Instance.StopTimer();
        });
        hoverDetector.OnHoverEnter.AddListener(() =>
        {
            hoverDetector.OnTimerFinish.RemoveAllListeners();
            hoverDetector.OnTimerFinish.AddListener(() =>
            {
                AttireManager.Instance.ShowAttire(dressUIItem.DressModel);
            });
            ApplicationManager.Instance.StartTimer();
        });
    }
}
