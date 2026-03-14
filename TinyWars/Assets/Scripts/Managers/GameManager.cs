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

    [Header("PARAMETERS")]
    [SerializeField] private float _baseActionTime = 1f;
    public float BaseActionTime { get { return _baseActionTime; } }
    [SerializeField] private float _actionRate = 1f;
    public float ActionRate { get { return _actionRate; } }
    public float ActionTime { get { return _baseActionTime / _actionRate; } }

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
