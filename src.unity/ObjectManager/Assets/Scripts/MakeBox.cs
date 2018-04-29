using UnityEngine;
using UnityEngine.SceneManagement;
using Test = OA.Ultima.ObjectTestEngine;

public class MakeBox : MonoBehaviour
{
    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnLevelFinishedLoading;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    //}

    //void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log($"Level Loaded: {scene.name}-{mode}");
    //}

    void Awake()
    {
        Test.Awake();
    }

    // Use this for initialization
    void Start()
    {
        Test.Start();
    }

    void OnDestroy()
    {
        Test.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        Test.Update();
    }
}

//var chair = ResourcesEx.LoadFromUri("https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png");
//var box = Instantiate(GameObject.Find("Cube00"), this.transform);
//box.transform.position += new Vector3(2, 2, 2);
//GameObject.Destroy(box);
