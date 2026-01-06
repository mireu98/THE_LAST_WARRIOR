using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drakkar.GameUtils;

public class AnimationEvents : MonoBehaviour
{
    public DrakkarTrail Trail;

    public DrakkarTrail Trail2;
    public void StartTrail()
    {
        Trail.Begin();
    }

    public void StopTrail()
    {
        Trail.End();
    }

    public void StartTrail2()
    {
        Trail2.Begin();
    }

    public void StopTrail2()
    {
        Trail2.End();
    }
}
