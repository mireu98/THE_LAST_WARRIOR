using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Boss _boss;
    private Animator _bossAnimator;
    private Coroutine _closeCo;
    public GameObject CloseWall;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_closeCo != null) return;
        _closeCo = StartCoroutine(CloseStage());
        _boss.WakeUp();
        CharacterController cc = other.GetComponent<CharacterController>();
        cc.Move(new Vector3(0,0,4));
    }

    IEnumerator CloseStage()
    {
        Vector3 start = CloseWall.transform.localPosition;
        Vector3 end = start - new Vector3(0, 3.5f, 0);

        float duration = 3f;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / duration;
            CloseWall.transform.localPosition = Vector3.Lerp(start, end, time);
            yield return null;
        }

        CloseWall.transform.localPosition = end;
    }
}
