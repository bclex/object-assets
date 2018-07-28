using UnityEngine;
//using Test = OA.Tes.ObjectTestEngine;
using Test = OA.Tes.ObjectTestPack;

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
