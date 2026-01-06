using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StartBtnUI : MonoBehaviour
{
    public void HoverEvent()
    {
        transform.DOShakeScale(1, new Vector3(0.1f, 0, 0), 10, 0, false);
    }
}
