using UnityEngine;

public class PortalClipPlaneSetter : MonoBehaviour
{
    public Renderer SwordRenderer;    // 칼 Renderer
    public Transform Startportal;          // 포탈 Transform
    public string planePosName = "_PlanePos";
    public string planeNormalName = "_PlaneNormal";

    MaterialPropertyBlock _block;

    void Awake()
    {
        if (_block == null)
            _block = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (SwordRenderer == null || Startportal == null) return;

        Vector3 pos = Startportal.position;
        Vector3 normal = Startportal.forward;   // 앞/뒤 반대로 보이면 여기 바꾸면 됨

        SwordRenderer.GetPropertyBlock(_block);
        _block.SetVector(planePosName, pos);
        _block.SetVector(planeNormalName, normal);
        SwordRenderer.SetPropertyBlock(_block);
    }
}
