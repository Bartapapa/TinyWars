using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPhase
{
    private readonly List<TWAction> _phaseActions = new List<TWAction>();
    public List<TWAction> PhaseActions { get { return _phaseActions; } }

    public void EnactAllPhaseActions()
    {
        foreach(TWAction action in PhaseActions)
        {
            action.EnactAction();
        }
    }
}
