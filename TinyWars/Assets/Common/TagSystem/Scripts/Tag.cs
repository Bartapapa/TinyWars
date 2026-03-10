using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tag
{
    public readonly string Handle;
    public readonly object Source;

    public Tag(string handle, object source)
    {
        Handle = handle;
        Source = source;
    }

    public Tag(string handle) : this(handle, null) { }
}
