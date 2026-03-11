using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static GameManager _instance = null;

    public static GameManager Instance { get { return _instance; } }


    [Header("MANAGERS")]
    public EventDispatcher EventDispatcher;
    public CombatManager CombatManager;

    private void Awake()
    {
        if (!_instance)
        {
            lock (_lockingObject)
            {
                if (!_instance)
                {
                    _instance = this;
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        InitializeAllManagers();

        CombatManager.StartCombat();
    }

    private void InitializeAllManagers()
    {
        //Be careful of the order of these. Some need to be initialized before others.

        //High priority.
        EventDispatcher.Initialize();

        //Low priority.
        CombatManager.Initialize();
    }
}
