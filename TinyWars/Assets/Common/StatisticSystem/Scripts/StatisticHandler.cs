using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticHandler : MonoBehaviour
{
    [Header("STATS")]
    [SerializeField] private List<Statistic> _stats = new List<Statistic>();
    public List<Statistic> Stats { get { return _stats; } }

    public Statistic CreateStat(string statName, float baseValue = 0f, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        Statistic newStat = new Statistic(statName, baseValue, minValue, maxValue);
        _stats.Add(newStat);
        return newStat;
    }

    public Statistic GetStat(string statName)
    {
        Statistic chosenStat = null;

        foreach(Statistic stat in _stats)
        {
            if (stat.Name == statName)
            {
                chosenStat = stat;
            }
        }

        return chosenStat;
    }
}
