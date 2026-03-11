using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TWAction : ScriptableObject
{
    protected GameObject _initiator;
    protected List<GameObject> _targets;

    public GameObject Initiator { get { return _initiator; } }
    public List<GameObject> Targets { get { return _targets; } }

    protected CombatHandler _iCombatHandler;
    protected Dictionary<GameObject, CombatHandler> _tCombatHandlers = new Dictionary<GameObject, CombatHandler>();

    protected bool _generated = false;

    public virtual TWAction GenerateAction (GameObject initiator, List<GameObject> targets)
    {
        TWAction newAction = CreateInstance<TWAction>();
        newAction._initiator = initiator;
        newAction._targets = targets;

        newAction._iCombatHandler = newAction._initiator.GetComponent<CombatHandler>();
        foreach(GameObject target in newAction._targets)
        {
            CombatHandler tCombatHandler = target.GetComponent<CombatHandler>();
            newAction._tCombatHandlers.Add(target, tCombatHandler);
        }
        newAction._generated = true;

        return newAction;
    }

    public virtual void EnactAction()
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated! Returning.");
            return;
        }
        //Action logic goes here.
    }
}
