using UnityEngine;
using System.Collections.Generic;

public class KeyInventory : MonoBehaviour
{
    private List<string> _keys = new List<string>();

    public void AddKey(string keyId)
    {
        _keys.Add(keyId);
        Debug.Log("Clé obtenue : " + keyId);
    }

    public bool HasKey(string keyId)
    {
        return _keys.Contains(keyId);
    }

    public void UseKey(string keyId)
    {
        _keys.Remove(keyId);
        Debug.Log("Clé utilisée : " + keyId);
    }
}