using Retro.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    private float _dmg;
    public float Dmg { get => _dmg; set => _dmg = value; }
    private BoxCollider[] _cols;
    public ParticleSystem HitEffect;

    private void Awake()
    {
        HitEffect.Stop();
        Dmg = GameManager.Instance.PlayerDmg * 1.5f;
        _cols = GetComponentsInChildren<BoxCollider>(); 
    }

    public void Enable()
    {
        foreach (var col in _cols)
            col.enabled = true;
    }

    public void Disable()
    {
        HitEffect.Stop();
        foreach (var col in _cols)
            col.enabled = false;
    }
}
