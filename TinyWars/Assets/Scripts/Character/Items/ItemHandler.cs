using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ItemHandler : MonoBehaviour
{
    public AbilityHandler AbilityHandler { get { return _abilityHandler; } }
    private AbilityHandler _abilityHandler;

    [Header("ITEMS")]
    [SerializeField] private int _maximumEquipableItems = 1;
    public int MaximumEquipableItems { get { return _maximumEquipableItems; } }

    [SerializeField] private List<TWItem> _equippedItems = new List<TWItem>();
    public List<TWItem> EquippedItems { get { return _equippedItems; } }
    [SerializeField] private List<TWItem> _appliedItems = new List<TWItem>();
    public List<TWItem> AppliedItems { get { return _appliedItems; } }

    private bool _initialized = false;

    private void Awake()
    {
        if (!_initialized)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        _abilityHandler = GetComponent<AbilityHandler>();
        if (!_abilityHandler)
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated ability handler. Returning.");
            return;
        }
    }

    public void AcquireItem(TWItem item)
    {
        switch (item.Type)
        {
            case ItemType.None:
                break;
            case ItemType.Consumable:
                //Item gives a permanent or temporary bonus. Use the item as source for item bonuses, so as to be able to remove them at the end of combat by checking if item is consumable.
                //If item has standard modifier, then add to _appliedItems to be able to remove them later.
                //Consume item event is called.
                break;
            case ItemType.Equipable:
                //Item is added to _equippedItems, granting bonus while it is equipped. It can have an associated ability, which is parsed through to the character's abilityHandler.
                //Add to _applieditems to remove them later.
                //Equip item event is called.
                break;
            default:
                break;
        }
    }

    public void LoseItem (TWItem item)
    {
        //If item is equipable, place it back into player's general inventory, so that they can recuperate it later.
        //If a consumable item is lost with standard modifiers, remove all modifiers applied by the item from the itemhandler's stats, as long as that item was consumable.
    }
}
