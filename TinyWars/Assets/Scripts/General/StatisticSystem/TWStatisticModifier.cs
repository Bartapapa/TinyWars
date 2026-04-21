using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TWStatisticModifierDuration
{
    None,
    Combat,
}

public class TWStatisticModifier : StatisticModifier
{
    public TWStatisticModifierDuration Duration;

    public TWStatisticModifier(float value, StatisticModifierType type, int priority, ModifierApplicationType applicationType, TWStatisticModifierDuration duration, object source, Statistic associatedStatistic = null)
        : base(value, type, priority, applicationType, source, associatedStatistic)
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
        Duration = duration;
        if (Duration == TWStatisticModifierDuration.Combat)
        {
            ApplicationType = ModifierApplicationType.Standard;
        }
        else
        {
            ApplicationType = applicationType;
        }
        Source = source;

        //Associated statistics should only be used on a single given entity. If a modifier's value is correlated to the statistic of another entity, then if that other entity is deleted
        //the correlation breaks. Securities must be provided to prevent this from happening.
        AssociatedStatistic = associatedStatistic;
    }

    public TWStatisticModifier(float value, StatisticModifierType type, ModifierApplicationType applicationType, TWStatisticModifierDuration duration, object source)
        : this(value, type, (int)type, applicationType, duration, null) { } 
}
