using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spirit : MonoBehaviour
{
    public int maxShields = 4;
    public int shields = 0;
    void Start() {
        shields = maxShields;    
    }
}