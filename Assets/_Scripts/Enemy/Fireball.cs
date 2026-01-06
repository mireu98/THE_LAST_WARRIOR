using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Enemy _owner;
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Player"))
        {
            _owner.DamageToPlayer();
            Destroy(gameObject);
        }
    }
}
