using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltiShader : MonoBehaviour
{
    public Renderer SwordRenderer;    // Ä® Renderer
    public Transform Startportal;     // Æ÷Å» Transform
    public string planePosName = "_PlanePos";
    public string planeNormalName = "_PlaneNormal";

    MaterialPropertyBlock _block;

    void Awake()
    {
        if (_block == null)
            _block = new MaterialPropertyBlock();

        if (SwordRenderer == null)
            SwordRenderer = GetComponentInChildren<Renderer>();
    }

    void LateUpdate()
    {
        if (SwordRenderer == null || Startportal == null) return;

        Vector3 pos = Startportal.position;
        Vector3 normal = Startportal.forward;

        SwordRenderer.GetPropertyBlock(_block);
        _block.SetVector(planePosName, pos);
        _block.SetVector(planeNormalName, normal);
        SwordRenderer.SetPropertyBlock(_block);
    }
}
