using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    string testString;

    // Start is called before the first frame update
    void Start()
    {
        testString = "1-4-5-3-2-8-5-12-11-10";

        string[] array = testString.Split("-");

        for (int i = 0; i < array.Length; i++)
        {
            print(array[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
