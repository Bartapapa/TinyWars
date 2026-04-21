using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    private static object _lockingObject = new object();
    private static PointerManager _instance = null;

    public static PointerManager Instance { get { return _instance; } }

    [Header("MANAGER REFERENCES")]
    [SerializeField] private Holder _currentlyHeldHolder = null;
    public Holder CurrentlyHeldHolder { get { return _currentlyHeldHolder; } }

    [SerializeField] private IHolderSlot _currentHolderSlot = null;
    public IHolderSlot CurrentHolderSlot { get { return _currentHolderSlot; } }

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

    public void SetNewHeldHolder(Holder heldHolder)
    {
        _currentlyHeldHolder = heldHolder;
    }

    public void SetNewHolderSlot(IHolderSlot holderSlot)
    {
        _currentHolderSlot = holderSlot;
    }
}
