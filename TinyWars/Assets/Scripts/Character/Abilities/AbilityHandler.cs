using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    [Header("STARTING ABILITIES")]
    [SerializeField] private List<TWAbility> _startingAbilities = new List<TWAbility>();

    [Header("ABILITIES")]
    [SerializeField] [ReadOnlyInspector] private List<TWAbility> _abilities = new List<TWAbility>();
    public List<TWAbility> Abilities { get { return _abilities; } }

    private void Start()
    {
        foreach (TWAbility ability in _startingAbilities)
        {
            AddAbility(ability);
        }
    }

    public void AddAbility(TWAbility ability)
    {
        //Take the ability, create an instance of it, and set it into the list of abilities. These instances are going to listen to their specific contexts and events by the EventDispatcher.
        TWAbility newAbility = ability.GenerateAbility(this);
        InitializeMessageListen(newAbility);
        _abilities.Add(newAbility);
    }

    private void InitializeMessageListen(TWAbility ability)
    {
        if (!ability.Generated)
        {
            Debug.LogWarning("Warning! " + ability.name + " has not been properly generated. Returning.");
            return;
        }

        foreach(EventMessageType messageType in ability.ListenedMessages)
        {
            switch (messageType)
            {
                case EventMessageType.None:
                    break;
                case EventMessageType.ActionCalled:
                    //No ability should have this, as it'll cause a loop unless there's a check to verify that it's not the same action. Also, insanely powerful for obvious reasons.
                    EventDispatcher.Instance.ActionCalled -= ability.OnMessage_ActionCalled;
                    EventDispatcher.Instance.ActionCalled += ability.OnMessage_ActionCalled;
                    break;
                case EventMessageType.FighterHealthReachedZero:
                    EventDispatcher.Instance.FighterHealthReachedZero -= ability.OnMessage_FighterHealthReachedZero;
                    EventDispatcher.Instance.FighterHealthReachedZero += ability.OnMessage_FighterHealthReachedZero;
                    break;
                case EventMessageType.FighterAttacked:
                    EventDispatcher.Instance.FighterAttacked -= ability.OnMessage_FighterAttacked;
                    EventDispatcher.Instance.FighterAttacked += ability.OnMessage_FighterAttacked;
                    break;
                case EventMessageType.FighterDamagedDefender:
                    EventDispatcher.Instance.FighterDamagedDefender -= ability.OnMessage_FighterDamagedDefender;
                    EventDispatcher.Instance.FighterDamagedDefender += ability.OnMessage_FighterDamagedDefender;
                    break;
                case EventMessageType.FighterMoved:
                    EventDispatcher.Instance.FighterMoved -= ability.OnMessage_FighterMoved;
                    EventDispatcher.Instance.FighterMoved += ability.OnMessage_FighterMoved;
                    break;
                case EventMessageType.FighterSpawned:
                    EventDispatcher.Instance.FighterMoved -= ability.OnMessage_FighterSpawned;
                    EventDispatcher.Instance.FighterMoved += ability.OnMessage_FighterSpawned;
                    break;
                case EventMessageType.FighterCorpseCleared:
                    EventDispatcher.Instance.FighterCorpseCleared -= ability.OnMessage_FighterCorpseCleared;
                    EventDispatcher.Instance.FighterCorpseCleared += ability.OnMessage_FighterCorpseCleared;
                    break;
                default:
                    break;
            }
        }
    }

    public bool RemoveAbility(TWAbility ability)
    {
        string name = ability.InternalName;
        TWAbility abilityToRemove = null;
        for (int i = _abilities.Count-1; i >= 0; i--)
        {
            if (_abilities[i].InternalName == name)
            {
                abilityToRemove = _abilities[i];
                _abilities.Remove(abilityToRemove);

                EventDispatcher.Instance.ActionCalled -= ability.OnMessage_ActionCalled;
                EventDispatcher.Instance.FighterHealthReachedZero -= ability.OnMessage_FighterHealthReachedZero;
                EventDispatcher.Instance.FighterAttacked -= ability.OnMessage_FighterAttacked;
                EventDispatcher.Instance.FighterDamagedDefender -= ability.OnMessage_FighterDamagedDefender;
                EventDispatcher.Instance.FighterMoved -= ability.OnMessage_FighterMoved;

                break;
            }
        }

        return abilityToRemove != null;
    }
}
