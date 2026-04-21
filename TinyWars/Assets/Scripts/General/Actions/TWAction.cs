using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionType
{
    ClearCorpse = 0,
    Die = 1,
    Damaging = 100,
    Buff = 200,
    Spawn = 300,
    None = 999,
}

public class TWAction : ScriptableObject
{
    [Header("BASE ACTION PARAMETERS")]
    [SerializeField] protected ActionType _actionType = ActionType.None;
    public int ActionPriority { get { return (int)_actionType; } }
    [SerializeField] protected bool _simultaneousAction = false;
    public bool SimultaneousAction { get { return _simultaneousAction; } }

    protected GameObject _initiator;
    protected List<GameObject> _targets;

    public GameObject Initiator { get { return _initiator; } }
    public List<GameObject> Targets { get { return _targets; } }

    protected CombatHandler _iCombatHandler;
    public CombatHandler ICombatHandler { get { return _iCombatHandler; } }
    protected Dictionary<GameObject, CombatHandler> _tCombatHandlers = new Dictionary<GameObject, CombatHandler>();
    public Dictionary<GameObject, CombatHandler> TCombatHandler { get { return _tCombatHandlers; } }

    protected bool _generated = false;

    public virtual TWAction GenerateAction (GameObject initiator, List<GameObject> targets, int actionLevel = 1)
    {
        TWAction newAction = CreateInstance<TWAction>();
        newAction._actionType = _actionType;
        newAction._simultaneousAction = _simultaneousAction;
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
