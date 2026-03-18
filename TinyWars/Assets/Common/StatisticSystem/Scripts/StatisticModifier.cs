using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatisticModifierType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
    Force = 400,
    MinValue = 500,
    MaxValue = 600,
}

public enum ModifierApplicationType
{
    Standard,
    Permanent,
}

public class StatisticModifier
{
    public float Value;
    public readonly StatisticModifierType Type;
    public readonly int Priority;
    public readonly ModifierApplicationType ApplicationType;
    public readonly object Source;
    public readonly Statistic AssociatedStatistic;

    public delegate void AssociatedStatisticModifierValueChangeEvent(float from, float to);
    public event AssociatedStatisticModifierValueChangeEvent AssociatedStatisticValueChanged;

    public StatisticModifier(float value, StatisticModifierType type, int priority, ModifierApplicationType applicationType, object source, Statistic associatedStatistic = null)
    {
        if (associatedStatistic == null)
        {
            Value = value;          
        }
        else
        {
            Value = associatedStatistic.Value;
            associatedStatistic.StatisticValueChanged -= OnAssociatedStatisticValueChanged;
            associatedStatistic.StatisticValueChanged += OnAssociatedStatisticValueChanged;
        }

        Type = type;
        Priority = priority;
        ApplicationType = applicationType;
        Source = source;

        //Associated statistics should only be used on a single given entity. If a modifier's value is correlated to the statistic of another entity, then if that other entity is deleted
        //the correlation breaks. Securities must be provided to prevent this from happening.
        AssociatedStatistic = associatedStatistic;
    }

    public StatisticModifier(float value, StatisticModifierType type, ModifierApplicationType applicationType) : this(value, type, (int)type, applicationType, null) { }
    public StatisticModifier(float value, StatisticModifierType type) : this(value, type, (int)type, ModifierApplicationType.Standard, null) { }
    public StatisticModifier(float value) : this(value, StatisticModifierType.Flat, (int)StatisticModifierType.Flat, ModifierApplicationType.Standard, null) { }

    public StatisticModifier(float value, StatisticModifierType type, ModifierApplicationType applicationType, object source) : this(value, type, (int)type, applicationType, source) { }
    public StatisticModifier(float value, StatisticModifierType type, object source) : this(value, type, (int)type, ModifierApplicationType.Standard, source) { }
    public StatisticModifier(float value, object source) : this(value, StatisticModifierType.Flat, (int)StatisticModifierType.Flat, ModifierApplicationType.Standard, source) { }

    private void OnAssociatedStatisticValueChanged(float from, float to)
    {
        float oldValue = Value;
        Value = AssociatedStatistic.Value;
        AssociatedStatisticValueChanged?.Invoke(oldValue, Value);
    }
}
