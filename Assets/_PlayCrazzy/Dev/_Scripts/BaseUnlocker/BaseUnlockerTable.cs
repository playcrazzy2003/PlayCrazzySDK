using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseUnlockerTable", menuName = "Game/Base Unlocker Table")]
public class BaseUnlockerTable : ScriptableObject
{
    public List<BaseUnlockerEntry> entries = new List<BaseUnlockerEntry>();
}
