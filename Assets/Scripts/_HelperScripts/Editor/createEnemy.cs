using UnityEngine;
using UnityEditor;
using System.Collections;

public class createEnemy : MonoBehaviour
{

    [MenuItem("Custom Tools/Create AI")]
    static void create()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule); ;
        go.AddComponent<StatePatternEnemy>();
        go.name = "Enemy";
    }
}
