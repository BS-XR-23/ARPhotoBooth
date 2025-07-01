using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ImageData
{
  public string url;
  public string imageId;
  [JsonIgnore]
  public string id;
}
