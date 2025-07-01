using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DressUIItem : MonoBehaviour
{
  [HideInInspector]
  public AttireModel DressModel;
  public Image thumbnail;
  [HideInInspector]
  public List<Color> colors;
  public void Init(AttireModel dressModel,Sprite thumb)
  {
    DressModel = dressModel;
    thumbnail.sprite = thumb;
    this.colors = colors;
  }
}
