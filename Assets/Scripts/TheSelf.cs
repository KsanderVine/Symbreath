using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSelf : MonoBehaviour
{
    public static TheSelf instance { get; private set; }
    public void Awake()
    {
        instance = this;
    }
}
