using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData
{
  public float lightIntensity=1;
  public float brightness = 1;
  public float bodyRadius = 0;
  public int scale_type = 0;
  public int tracking_variant = 0;
  public int timer = 6;
  public float scale_factor = 0;
  public float xOffset =0;
  public float yOffset = 0;
  public string cameraSource;
  public int cameraResolution=9;
  public float camera_distance = 8;
  public string ip;
  public string storeId= "development";
  public LogoPosition[] logosPosition;
    
}
public class LogoPosition
{
    public float x;
    public float y;
}