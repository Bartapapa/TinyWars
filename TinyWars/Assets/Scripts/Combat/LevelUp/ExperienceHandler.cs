using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExperienceHandler : MonoBehaviour
{
    private CombatHandler _combatHandler;
    public CombatHandler CombatHandler { get { return _combatHandler; } }
    public StatisticHandler StatisticHandler { get { return _statisticHandler; } }
    private StatisticHandler _statisticHandler;


    [Header("LEVEL")]
    [SerializeField][ReadOnlyInspector] private Statistic _currentLevel;
    public int CurrentLevel { get { return _currentLevel.IntValue; } }

    [Header("EXPERIENCE")]
    [SerializeField][ReadOnlyInspector] private Statistic _currentXP;
    public float CurrentXP { get { return _currentXP.Value; } }
    [SerializeField][ReadOnlyInspector] private Statistic _XPgainMultiplier;
    public float XPGainMultiplier { get { return _XPgainMultiplier.Value; } }
    [SerializeField][ReadOnlyInspector] private Statistic _nextXPThreshold;
    public float NextXPThreshold { get { return _nextXPThreshold.Value; } }

    private bool _initialized = false;

    public void Initialize(int atLevel)
    {
        _combatHandler = GetComponent<CombatHandler>();
        if (!_combatHandler)
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated combat handler. Returning.");
            return;
        }

        _statisticHandler = GetComponent<StatisticHandler>();
        if (!_statisticHandler)
        {
            Debug.LogWarning("Warning! " + this.gameObject.name + " has no associated statistic handler. Returning.");
            return;
        }

        _currentLevel = new Statistic("CurrentLevel", atLevel, 1f);
        _currentXP = new Statistic("CurrentXP", 0f, 0f);
        _XPgainMultiplier = new Statistic("XPGainMultiplier", 1f, 0f);
        _nextXPThreshold = new Statistic("NextXPThreshold", 0f, 0f);

        SetNextXPThreshold(CurrentLevel);

        _currentXP.StatisticValueChanged -= OnCurrentXPChangedValue;
        _currentXP.StatisticValueChanged += OnCurrentXPChangedValue;

        _initialized = true;
    }

    private void OnCurrentXPChangedValue(float from, float to)
    {
        if (to > from)
        {
            //Increased XP, such as after combat.
            //Upon XP gain, increase health and attack by that same value.
            if (to > NextXPThreshold)
            {
                LevelUp();
            }
        }

        if (to < from)
        {
            //Decreased XP, such as after a level up.
        }
    }

    private void LevelUp()
    {
        //Add a level, and reset current XP.
        _currentLevel.AddModifier(new StatisticModifier(1f, StatisticModifierType.Flat, ModifierApplicationType.Permanent));
        _currentXP.AddModifier(new StatisticModifier(0f, StatisticModifierType.Force, ModifierApplicationType.Permanent));

        SetNextXPThreshold(CurrentLevel);

        if (EventDispatcher.Instance)
        {
            FighterContext context = new FighterContext(_combatHandler);
            EventDispatcher.Instance.Message_FighterLevelUp(ref context);
        }
    }

    private void SetNextXPThreshold(int level)
    {
        _nextXPThreshold.AddModifier(new StatisticModifier(GameManager.Instance.UniversalData.GetXPValueByLevel(level), StatisticModifierType.Force, ModifierApplicationType.Permanent));
    }
}
