using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GraphicRaycaster;

public class RecruitmentManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static RecruitmentManager _instance = null;

    public void Initialize()
    {
        if (!_instance)
        {
            lock (_lockingObject)
            {
                if (!_instance)
                {
                    _instance = this;
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartRecruitmentPhase()
    {
        //Get all fighters from the player's current team. Must be referenced on an instantiated-object basis, kept in a separate additive scene.
        //Get all fighters from the enemy team, and put them up for selection. These can be taken directly from the referenced enemies before combat.
        //Player chooses 1 fighter from selection - either using gamepad to move between selections using axis or Dpad, or mouse by hovering over and clicking.
            //This process can be skipped entirely.
            //If using gamepad, selection will move to party-side to choose which slot to fill (or which fighter to replace, if fighter is present). Can also select Camp icon to send to camp immediately.
                //Player can set the fighter back down.
            //If using mouse, player's clicked enemy fighter will be dragged along with mouse position, and another slot can be selected (or which fighter to replace, if fighter is present). Can move to Camp icon to send to camp immediately.
            //If switched with another fighter, this continues until the player's 'hand' does not hold any character, after which a prompt will show up to confirm.
    }
}
