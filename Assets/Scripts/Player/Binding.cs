using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Binding", menuName = "ScriptableObjects/Binding", order = 1)]
public class Binding : ScriptableObject
{
    public KeyCode jump;
    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode down;
    public KeyCode attack;
    public KeyCode dash;
}
