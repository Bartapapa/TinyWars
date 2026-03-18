using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Consumable,
    Equipable,
}

[CreateAssetMenu(menuName = "TinyWars/Item/ItemData", fileName = "ItemName_ItemType_ItemData")]
public class TWItem : ScriptableObject
{
    [Header("ITEM ID")]
    [SerializeField] protected string _internalName = "";
    public string InternalName { get { return _internalName; } }

    [Header("PARAMETERS")]
    [SerializeField] protected ItemType _itemType = ItemType.None;
    public ItemType Type { get { return _itemType; } }

    [Header("ASSOCIATED ABILITIES")]
    [SerializeField] protected List<TWAbility> _associatedAbilities = new List<TWAbility>();
    public List<TWAbility> AssociatedAbilities { get { return _associatedAbilities; } }

    [Header("MODIFIER PARAMETERS")]
    [SerializeField] protected ModifierApplicationType _modifierApplicationType = ModifierApplicationType.Standard;
    public ModifierApplicationType ModifierApplicationType { get { return _modifierApplicationType; } }

    protected ItemHandler _itemHandler;
    public ItemHandler ItemHandler { get { return _itemHandler; } }

    protected bool _generated;
    public bool Generated { get { return _generated; } }

    public virtual TWItem GenerateItem(ItemHandler itemHandler)
    {
        TWItem newItem = CreateInstance<TWItem>();

        newItem._internalName = this._internalName;
        newItem._itemType = this._itemType;

        newItem._itemHandler = itemHandler;
        newItem._generated = true;

        return newItem;
    }

    public virtual void ApplyBonus()
    {
        if (!_generated)
        {
            Debug.LogWarning("Warning! " + this.name + " has not been properly generated. Returning.");
            return;
        }

        //override with application of status modifiers in correct places
        //Additionally, if there is an associated ability, apply said ability to character's abilityHandler;
    }
}
