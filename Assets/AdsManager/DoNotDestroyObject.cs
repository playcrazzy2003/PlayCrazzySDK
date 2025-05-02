using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
