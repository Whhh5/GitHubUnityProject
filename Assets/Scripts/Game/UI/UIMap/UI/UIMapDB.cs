using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapDB : MonoBehaviour
{
    public static UIMapDB Instance = null;

    private void Awake()
    {
        Instance ??= this;
    }

    public void MSG()
    {

    }
}
