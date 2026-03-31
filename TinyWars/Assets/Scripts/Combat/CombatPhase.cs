using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPhase
{
    private readonly List<TWAction> _phaseActions = new List<TWAction>();
    public List<TWAction> PhaseActions { get { return _phaseActions; } }

    public bool HasActionsToProcess { get { return _phaseActions.Count > 0; } }

    public void EnactAllPhaseActions()
    {
        foreach(TWAction action in _phaseActions)
        {
            action.EnactAction();          
        }

        _phaseActions.Clear();
    }

    public bool EnactNextPhaseAction()
    {
        if (_phaseActions.Count <= 0)
        {
            //No more actions.
            return false;
        }
        else
        {
            List<TWAction> simultaneousActions = new List<TWAction>();
            int currentPrio = _phaseActions[0].ActionPriority;
            for (int i = 0; i < _phaseActions.Count; i++)
            {
                if (_phaseActions[i].ActionPriority <= currentPrio && _phaseActions[i].SimultaneousAction)
                {
                    //Gather all actions of same priority.
                    simultaneousActions.Add(_phaseActions[i]);
                }
                else
                {
                    break;
                }
            }

            if(simultaneousActions.Count > 0)
            {
                for (int i = 0; i < simultaneousActions.Count; i++)
                {
                    simultaneousActions[i].EnactAction();
                    _phaseActions.Remove(simultaneousActions[i]);
                }
            }
            else
            {
                _phaseActions[0].EnactAction();
                _phaseActions.RemoveAt(0);
            }
            return true;
        }
    }

    public void SortPhaseActions()
    {
        _phaseActions.Sort(CompareActionPriority);
    }

    private int CompareActionPriority(TWAction a, TWAction b)
    {
        if (a.ActionPriority < b.ActionPriority) return -1;
        else if (a.ActionPriority > b.ActionPriority) return 1;
        return 0;
    }
}
