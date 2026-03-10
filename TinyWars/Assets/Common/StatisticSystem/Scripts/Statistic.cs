using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Statistic
{
    public float InitializedBaseValue;
    public float BaseValue { get { return _baseValue; } }
    public float Value
    {
        get
        {
            return _value;
        }
    }
    public List<StatisticModifier> StandardModifiers { get { return _statisticModifiers; } }

    [ReadOnlyInspector] [SerializeField] private float _value;
    [ReadOnlyInspector] [SerializeField] private float _baseValue = float.MinValue;
    [ReadOnlyInspector] [SerializeField] private readonly List<StatisticModifier> _statisticModifiers;
    private readonly List<StatisticModifier> _permanentStatisticModifiersToApply;

    public delegate void StatisticValueChangeEvent(float from, float to);
    public event StatisticValueChangeEvent StatisticValueChanged;

    public Statistic(float baseValue)
    {
        InitializedBaseValue = baseValue;
        _baseValue = InitializedBaseValue;
        _value = BaseValue;

        _statisticModifiers = new List<StatisticModifier>();
        _permanentStatisticModifiersToApply = new List<StatisticModifier>();
    }

    public void AddModifiers(List<StatisticModifier> mods)
    {
        bool addedStandard = false;
        bool addedPermanent = false;

        //Add mod to appropriate list whether its standard or permanent.
        foreach(StatisticModifier mod in mods)
        {
            switch (mod.ApplicationType)
            {
                case ModifierApplicationType.Standard:
                    _statisticModifiers.Add(mod);
                    addedStandard = true;
                    break;
                case ModifierApplicationType.Permanent:
                    _permanentStatisticModifiersToApply.Add(mod);
                    addedPermanent = true;
                    break;
                default:
                    break;
            }
        }

        if (addedStandard) _statisticModifiers.Sort(CompareModifierPriority);
        if (addedPermanent) _permanentStatisticModifiersToApply.Sort(CompareModifierPriority);

        _value = CalculateFinalValue();
    }

    public void AddModifier(StatisticModifier mod)
    {
        List<StatisticModifier> newList = new List<StatisticModifier>();
        newList.Add(mod);
        AddModifiers(newList);
    }

    private int CompareModifierPriority(StatisticModifier a, StatisticModifier b)
    {
        if (a.Priority < b.Priority) return -1;
        else if (a.Priority > b.Priority) return 1;
        return 0;
    }

    public bool RemoveModifier(StatisticModifier mod)
    {
        if (_statisticModifiers.Remove(mod))
        {
            _value = CalculateFinalValue(); //only recalculate if mod was removed
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        bool removed = false;

        for (int i = _statisticModifiers.Count-1; i >= 0; i--)
        {
            if (_statisticModifiers[i].Source == source)
            {
                removed = true;
                _statisticModifiers.RemoveAt(i);
            }
        }

        if (removed) _value = CalculateFinalValue(); //only recalculate if mod was removed
        return removed;
    }

    private float CalculateFinalValue()
    {
        float oldValue = Value;
        //Calculate permanent mods first, then clear list of permanent mods.

        if (_permanentStatisticModifiersToApply.Count > 0)
        {
            _baseValue = CalculateValueFromMods(_permanentStatisticModifiersToApply);
            _permanentStatisticModifiersToApply.Clear();
        }

        float finalValue = BaseValue;

        if (_statisticModifiers.Count > 0)
        {
            finalValue = CalculateValueFromMods(_statisticModifiers);
        }

        if (oldValue != finalValue)
        {
            StatisticValueChanged?.Invoke(oldValue, finalValue);
        }

        return (float)Math.Round(finalValue, 4);        
    }

    private float CalculateValueFromMods(List<StatisticModifier> mods)
    {
        float modifiedValue = BaseValue;
        float sumPercentAdd = 0f;

        for (int i = 0; i < mods.Count; i++)
        {
            StatisticModifier mod = mods[i];
            switch (mod.Type)
            {
                case StatisticModifierType.Flat:
                    modifiedValue += mods[i].Value;
                    break;
                case StatisticModifierType.PercentAdd:
                    sumPercentAdd += mod.Value;

                    if (i + 1 >= mods.Count || mods[i + 1].Type != StatisticModifierType.PercentAdd)
                    {
                        modifiedValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                    break;
                case StatisticModifierType.PercentMult:
                    modifiedValue *= mod.Value;
                    break;
                case StatisticModifierType.Force:
                    modifiedValue = mod.Value;
                    break;
                default:
                    break;
            }
        }

        return (float)Math.Round(modifiedValue, 4);
    }
}
