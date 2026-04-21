using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/SpawnFighter", fileName = "SpawnFighter_ActionData")]
public class Action_SpawnFighter : TWAction
{
    [Header("FIGHTER TO SPAWN")]
    [SerializeField] private CombatHandler _fighterToSpawn;

    [Header("PARAMETERS")]
    [SerializeField] private int _numberToSpawn = 1;
    [SerializeField] private CombatRowSpawnPosition _spawnPosition = CombatRowSpawnPosition.None;

    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets, int actionLevel = 1)
    {
        Action_SpawnFighter newAction = CreateInstance<Action_SpawnFighter>();
        newAction._actionType = _actionType;
        newAction._simultaneousAction = _simultaneousAction;
        newAction._initiator = initiator;
        newAction._targets = targets;

        newAction._iCombatHandler = newAction._initiator.GetComponent<CombatHandler>();
        foreach (GameObject target in newAction._targets)
        {
            CombatHandler tCombatHandler = target.GetComponent<CombatHandler>();
            newAction._tCombatHandlers.Add(target, tCombatHandler);
        }

        newAction._fighterToSpawn = _fighterToSpawn;
        newAction._numberToSpawn = _numberToSpawn;
        newAction._spawnPosition = _spawnPosition;

        newAction._generated = true;

        return newAction;
    }

    public override void EnactAction()
    {
        base.EnactAction();

        //List<CombatHandler> target = new List<CombatHandler>();
        CombatRow allyRow = _iCombatHandler.CurrentRow;
        int openSlot = -1;

        for (int i = 0; i < _numberToSpawn; i++)
        {
            switch (_spawnPosition)
            {
                case CombatRowSpawnPosition.None:
                    break;
                case CombatRowSpawnPosition.FrontMost:
                    if (!allyRow.SpawnFighter(_fighterToSpawn, 0))
                    {
                        if (!allyRow.MoveAllFightersDown())
                        {
                            return;
                        }
                        else
                        {
                            allyRow.SpawnFighter(_fighterToSpawn, 0);
                        }
                    }

                    break;
                case CombatRowSpawnPosition.BackMost:
                    openSlot = allyRow.GetBackmostOpenSlot();
                    if (openSlot < 0) return;

                    allyRow.SpawnFighter(_fighterToSpawn, openSlot);
                    break;
                case CombatRowSpawnPosition.AtContextSlot:
                    openSlot = allyRow.GetFighterSlotIndex(_iCombatHandler);
                    allyRow.RemoveFighter(_iCombatHandler);

                    allyRow.SpawnFighter(_fighterToSpawn, openSlot);
                    return; //only spawn 1 at position context.
                default:
                    break;
            }
        }
    }
}
