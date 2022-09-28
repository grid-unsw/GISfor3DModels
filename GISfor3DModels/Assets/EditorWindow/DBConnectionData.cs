using System;
using UnityEngine;

[Serializable]
public class DBConnectionData : ScriptableObject
{
    public string Host;
    public string Username;
    public string Password;
    public string Database;
}
