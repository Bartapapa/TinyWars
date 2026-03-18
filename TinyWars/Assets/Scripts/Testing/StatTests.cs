using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTests : MonoBehaviour
{
    public float InitialStat = 5f;
    public float StandardFlatAdd = 2f;
    public float StandardPercentMultAdd = 1.2f;
    public float StandardForce = 1f;
    public float PermanentFlat = -3f;

    [ReadOnlyInspector] [SerializeField] private Statistic TestStat;

    private void Awake()
    {
        TestStat = new Statistic("TestStat", InitialStat);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddStandardFlatModifier();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            RemoveAllStandardModifiers();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddStandardPercentMultModifier();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddStandardForceModifier();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddPermanentFlatModifier();
        }
    }

    private void AddPermanentFlatModifier()
    {
        float initialValue = TestStat.Value;
        float initialBaseValue = TestStat.BaseValue;
        TestStat.AddModifier(new StatisticModifier(PermanentFlat, StatisticModifierType.Flat, 100, ModifierApplicationType.Permanent, this));
        Debug.Log(
            this.gameObject.name + " should have increased by " + PermanentFlat + " permanently. Initial base value was " + initialBaseValue + ", it now is " + TestStat.BaseValue
            + ". Initial value was " + initialValue + ", modified value is " + TestStat.Value + ".");
    }

    private void AddStandardForceModifier()
    {
        float initialValue = TestStat.Value;
        TestStat.AddModifier(new StatisticModifier(StandardForce, StatisticModifierType.Force, 400, ModifierApplicationType.Standard, this));
        Debug.Log(this.gameObject.name + " should now be " + StandardForce + ". Initial value was " + initialValue + ", modified value is " + TestStat.Value + ".");
    }

    private void AddStandardPercentMultModifier()
    {
        float initialValue = TestStat.Value;
        TestStat.AddModifier(new StatisticModifier(StandardPercentMultAdd, StatisticModifierType.PercentMult, 300, ModifierApplicationType.Standard, this));
        Debug.Log(this.gameObject.name + " should have increased by *" + StandardPercentMultAdd + ". Initial value was " + initialValue + ", modified value is " + TestStat.Value + ".");
    }

    private void RemoveAllStandardModifiers()
    {
        float initialValue = TestStat.Value;
        TestStat.RemoveAllModifiersFromSource(this);
        Debug.Log(
            this.gameObject.name + " has had all of its standard modifiers removed. Initial value was " + initialValue + ", modified value is " + TestStat.Value
            + ". There are " + TestStat.StandardModifiers.Count + " standard modifiers left.");
    }

    private void AddStandardFlatModifier()
    {
        float initialValue = TestStat.Value;
        TestStat.AddModifier(new StatisticModifier(StandardFlatAdd, StatisticModifierType.Flat, 100, ModifierApplicationType.Standard, this));
        Debug.Log(this.gameObject.name + " should have increased by " + StandardFlatAdd + ". Initial value was " + initialValue + ", modified value is " + TestStat.Value + ".");
    }
}
