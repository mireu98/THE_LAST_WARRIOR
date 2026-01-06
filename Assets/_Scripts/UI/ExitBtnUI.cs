using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ExitBtnUI : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ClickExit);
    }

    public void HoverEvent()
    {
        transform.DOShakeScale(1, new Vector3(0.1f, 0, 0), 10, 0, false);
    }

    public void ClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임이면 종료
        Application.Quit();
#endif
    }
}
