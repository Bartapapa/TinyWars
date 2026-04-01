using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static UIManager _instance = null;

    public static UIManager Instance { get { return _instance; } }

    [Header("OBJECT REFERENCES")]
    public Transitioner Transitioner;


    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
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
}
