using System;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionCulling : MonoBehaviour
{
    [SerializeField]
    private List<CullingObject> cullingObjects;
    void Start()
    {
        int order = 0;
        foreach(CullingObject cullingObject in cullingObjects)
        {
            Material[] materials = cullingObject.transparentObject.materials;
            foreach(Material material in materials)
            {
                material.renderQueue = 4000 + order - 1;
            }
            materials = cullingObject.cullingTarget.materials;
            foreach (Material material in materials)
            {
                material.renderQueue = 4000 + order + 1;
            }
            if (cullingObject.OptionalJewelryOccluderPlane != null)
            {
                materials = cullingObject.OptionalJewelryOccluderPlane.materials;
                foreach(Material material in materials)
                {
                    material.renderQueue = 4000 + order - 2;
                }
            }
            order++;
        }
    }

}
[Serializable]
public class CullingObject
{
    public Renderer OptionalJewelryOccluderPlane;
    public Renderer transparentObject;
    public Renderer cullingTarget;
}