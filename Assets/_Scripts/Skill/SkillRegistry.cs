using System.Collections.Generic;
using UnityEngine;

public class SkillRegistry : MonoBehaviour
{
    public static SkillRegistry Instance { get; private set; }

    private HashSet<GameObject> _skills = new HashSet<GameObject>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(GameObject go)
    {
        if (go != null) _skills.Add(go);
    }

    public void Unregister(GameObject go)
    {
        if (go != null) _skills.Remove(go);
    }

    public void ClearAll()
    {
        foreach (var s in _skills)
            if (s != null) Destroy(s);
        _skills.Clear();
    }
}
