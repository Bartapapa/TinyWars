using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.Rendering.DebugUI;

public class TagHandler : MonoBehaviour
{
    public List<Tag> Tags { get { return _tags; } }
    [SerializeField] private List<Tag> _tags = new List<Tag>();

    public bool HasTag(Enum tag)
    {
        Tag foundTag = new Tag();
        string tagHandle = tag.ToString();
        for (int i = 0; i < _tags.Count; i++)
        {
            if (_tags[i].Handle == tagHandle)
            {
                foundTag = _tags[i];
                break;
            }
        }
        return _tags.Contains(foundTag);
    }

    public void AddTag(Enum tag, object source = null)
    {
        Tag newTag = new Tag(tag.ToString(), source);
        _tags.Add(newTag);
    }

    public bool AddTagIfNotPresent(Enum tag, object source = null)
    {
        if (HasTag(tag)) return false;
        else
        {
            AddTag(tag, source);
            return true;
        }
    }

    private void RemoveTagAt(int i)
    {
        _tags.RemoveAt(i);
    }

    public bool RemoveTag(Enum tag, bool clear = false)
    {
        string tagHandle = tag.ToString();
        bool tagRemoved = false;
        for (int i = _tags.Count -1; i >= 00; i--)
        {
            if (tagHandle == _tags[i].Handle)
            {
                RemoveTagAt(i);
                tagRemoved = true;
                if (!clear) break;
            }
        }

        return tagRemoved;
    }

    public bool ClearAllTagsFromSource(object source)
    {
        bool tagRemoved = false;

        for (int i = _tags.Count - 1; i >= 0; i--)
        {
            if (_tags[i].Source == source)
            {
                tagRemoved = true;
                RemoveTagAt(i);
            }
        }

        return tagRemoved;
    }

    public bool ClearTagsOfHandle(Enum tag)
    {
        bool tagRemoved = false;
        string handle = tag.ToString();

        for (int i = _tags.Count - 1; i >= 0; i--)
        {
            if (_tags[i].Handle == handle)
            {
                tagRemoved = true;
                RemoveTagAt(i);
            }
        }

        return tagRemoved;
    }

    public bool RemoveTagsFromSource(Enum tag, object source)
    {
        bool tagRemoved = false;
        string tagHandle = tag.ToString();

        for (int i = _tags.Count - 1; i >= 0; i--)
        {
            if (_tags[i].Source == source && _tags[i].Handle == tagHandle)
            {
                tagRemoved = true;
                RemoveTagAt(i);
            }
        }

        return tagRemoved;
    }
}
