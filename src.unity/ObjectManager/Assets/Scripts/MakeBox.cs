using OA;
using UnityEngine;

public class MakeBox : MonoBehaviour
{
    void Awake()
    {
        ObjectTest.Awake();
    }

    // Use this for initialization
    void Start()
    {
        ObjectTest.Start();
    }

    void OnDestroy()
    {
        ObjectTest.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        ObjectTest.Update();
    }
}


//var chair = ResourcesEx.LoadFromUri("https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png");
//var box = Instantiate(GameObject.Find("Cube00"), this.transform);
//box.transform.position += new Vector3(2, 2, 2);
//Debug.Log("Test");
//GameObject.Destroy(box);
