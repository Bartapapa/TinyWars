using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static PartyManager _instance = null;

    public static PartyManager Instance { get { return _instance; } }

    [Header("PARTY")]
    private List<CombatHandler> _currentParty = new List<CombatHandler>();
    private List<CombatHandler> _totalCharacterRoster = new List<CombatHandler>();
    public List<CombatHandler> TotalCharacterRoster { get { return _totalCharacterRoster; } }

    public List<CombatHandler> CurrentParty { get { return _currentParty; } }

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

    public void SetCurrentParty(List<CombatHandler> party)
    {
        _currentParty = party;
    }

    public void AddFighterToRoster(CombatHandler fighter)
    {
        _totalCharacterRoster.Add(fighter);
        Debug.Log(fighter.Character.CharacterData.Character.ToString() + " has been added to the roster.");
    }

    public CombatHandler AddCharacterToParty(CombatHandler character, int toIndex = -1)
    {
        //Adds the character to the party.
        //Adds them to the roster if they aren't present.
        //Finds an open slot in the party to place them in.
        //If no slot is found and no specific index was given, the character isn't placed.
        //If the index was given but it is used by another member, switch out that party member.
        //If party member was switched out, place them in an open slot.
        //If no open slot was found, return the character so as to continue determining what action to do with it (switching in another party member, placing in camp, etc).

        if (!_totalCharacterRoster.Contains(character))
        {
            AddFighterToRoster(character);
        }

        if (toIndex < 0)
        {
            int emptySlot = GetEmptyPartySlot();
            if (emptySlot >= 0)
            {
                _currentParty[emptySlot] = character;
            }
            else
            {
                Debug.LogWarning("There's no space in the party to add " + character.Character.CharacterData.Character.ToString() + " without switching out another party member.");
                return null;
            }
        }
        else
        {
            CombatHandler characterAtSlot = _currentParty[toIndex];
            if (characterAtSlot == null)
            {
                _currentParty[toIndex] = character;
            }
            else
            {
                _currentParty[toIndex] = character;
                int emptySlot = GetEmptyPartySlot();
                if (emptySlot >= 0)
                {
                    _currentParty[emptySlot] = characterAtSlot;
                }
                else
                {
                    Debug.LogWarning("Placing " + character.Character.CharacterData.Character.ToString() + " in the party has switched out " + characterAtSlot.Character.CharacterData.Character.ToString() + ". Returning character.");
                    return characterAtSlot;
                }
            }
        }

        return null;
    }

    private int GetEmptyPartySlot()
    {
        int emptySlot = -1;
        for (int i = 0; i < _currentParty.Count; i++)
        {
            if (_currentParty[i] == null)
            {
                emptySlot = i;
                break;
            }
        }
        return emptySlot;
    }
}
