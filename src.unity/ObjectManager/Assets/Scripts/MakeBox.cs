using UnityEngine;
using Test = OA.Ultima.ObjectTestEngine;

public class MakeBox : MonoBehaviour
{
    void Awake()
    {
        Test.Awake();
    }

    void Start()
    {
        Test.Start();
    }

    void OnDestroy()
    {
        Test.OnDestroy();
    }

    void Update()
    {
        Test.Update();
    }
}
