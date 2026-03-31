using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static PartyManager _instance = null;

    [Header("PARTY")]
    private Character[] _currentParty;
    public Character[] CurrentParty { get { return _currentParty; } }
    private List<Character> _totalCharacterRoster = new List<Character>();
    public List<Character> TotalCharacterRoster { get { return _totalCharacterRoster; } }

    public List<Character> CurrentPartyList { get { return GetCurrentPartyAsList(); } }

    private List<Character> GetCurrentPartyAsList()
    {
        List<Character> currentPartyAsList = new List<Character>();

        for (int i = 0; i < _currentParty.Length; i++)
        {
            currentPartyAsList.Add(_currentParty[i]);
        }

        return currentPartyAsList;
    }

    public List<CombatHandler> CurrentPartyCombatList { get { return GetCurrentPartyAsCombatList(); } }

    private List<CombatHandler> GetCurrentPartyAsCombatList()
    {
        List<CombatHandler> currentPartyAsCombatList = new List<CombatHandler>();

        for (int i = 0; i < _currentParty.Length; i++)
        {
            CombatHandler combatHandler = _currentParty[i].GetComponent<CombatHandler>();
            currentPartyAsCombatList.Add(combatHandler);
        }

        return currentPartyAsCombatList;
    }

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

        _currentParty = new Character[GameManager.Instance.UniversalData.MaxTeamSlots];
    }

    public void AddCharacterToRoster(Character character)
    {
        _totalCharacterRoster.Add(character);
        Debug.Log(character.CharacterData.Character.ToString() + " has been added to the roster.");
    }

    public Character AddCharacterToParty(Character character, int toIndex = -1)
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
            AddCharacterToRoster(character);
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
                Debug.LogWarning("There's no space in the party to add " + character.CharacterData.Character.ToString() + " without switching out another party member.");
                return null;
            }
        }
        else
        {
            Character characterAtSlot = _currentParty[toIndex];
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
                    Debug.LogWarning("Placing " + character.CharacterData.Character.ToString() + " in the party has switched out " + characterAtSlot.CharacterData.Character.ToString() + ". Returning character.");
                    return characterAtSlot;
                }
            }
        }

        return null;
    }

    private int GetEmptyPartySlot()
    {
        int emptySlot = -1;
        for (int i = 0; i < _currentParty.Length; i++)
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
