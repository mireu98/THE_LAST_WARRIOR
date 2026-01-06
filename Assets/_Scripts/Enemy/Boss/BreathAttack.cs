using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathAttack : MonoBehaviour
{
    public float damage = 5000f;
    public string playerTag = "Player";

    private HashSet<int> _hitPlayers = new HashSet<int>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        int id = other.GetInstanceID();
        if (_hitPlayers.Contains(id)) return;
        _hitPlayers.Add(id);

        var player = other.GetComponentInParent<PlayerAdd>();
        if (player == null) return;

        var gm = GameManager.Instance;
        if (gm.IsDefense)
        {
            gm.IsDefense = false;
            player.GetComponent<Animator>().SetBool("isDefense", gm.IsDefense);
            player.DefenseEffect.Play();

            var cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                Vector3 knockDir = (cc.transform.position - transform.position).normalized;
                StartCoroutine(PlayerKnockBack(cc, knockDir, 12f, 0.15f));
                NewBreath();
            }
            return;
        }

        player.TakeDamage(damage);
        NewBreath();
    }

    IEnumerator PlayerKnockBack(CharacterController controller, Vector3 dir, float power, float duration)
    {
        float tick = 0f;
        while (tick < duration)
        {
            controller.Move(dir * power * Time.deltaTime);
            tick += Time.deltaTime;
            yield return null;
        }
    }

    public void NewBreath() => _hitPlayers.Clear();

}
