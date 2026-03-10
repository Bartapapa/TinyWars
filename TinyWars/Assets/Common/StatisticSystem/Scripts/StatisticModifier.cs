using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatisticModifierType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
    Force = 400,
}

public enum ModifierApplicationType
{
    Standard,
    Permanent,
}

public class StatisticModifier
{
    public readonly float Value;
    public StatisticModifierType Type;
    public readonly int Priority;
    public ModifierApplicationType ApplicationType;
    public readonly object Source;

    public StatisticModifier(float value, StatisticModifierType type, int priority, ModifierApplicationType applicationType, object source)
    {
        Value = value;
        Type = type;
        Priority = priority;
        ApplicationType = applicationType;
        Source = source;
    }

    public StatisticModifier(float value, StatisticModifierType type, ModifierApplicationType applicationType) : this(value, type, (int)type, applicationType, null) { }
    public StatisticModifier(float value, StatisticModifierType type) : this(value, type, (int)type, ModifierApplicationType.Standard, null) { }
    public StatisticModifier(float value) : this(value, StatisticModifierType.Flat, (int)StatisticModifierType.Flat, ModifierApplicationType.Standard, null) { }

    public StatisticModifier(float value, StatisticModifierType type, ModifierApplicationType applicationType, object source) : this(value, type, (int)type, applicationType, source) { }
    public StatisticModifier(float value, StatisticModifierType type, object source) : this(value, type, (int)type, ModifierApplicationType.Standard, source) { }
    public StatisticModifier(float value, object source) : this(value, StatisticModifierType.Flat, (int)StatisticModifierType.Flat, ModifierApplicationType.Standard, source) { }
}
