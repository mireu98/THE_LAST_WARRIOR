using UnityEngine;

public class SkillLifetime : MonoBehaviour
{
    private void OnEnable()
    {
        if (SkillRegistry.Instance != null)
            SkillRegistry.Instance.Register(gameObject);
    }

    private void OnDestroy()
    {
        if (SkillRegistry.Instance != null)
            SkillRegistry.Instance.Unregister(gameObject);
    }
}
