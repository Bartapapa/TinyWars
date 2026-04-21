using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Tag
{
    public string Handle;
    public object Source;

    public Tag(string handle, object source)
    {
        Handle = handle;
        Source = source;
    }

    public Tag(string handle) : this(handle, null) { }
}
