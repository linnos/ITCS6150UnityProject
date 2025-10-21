using System;
using UnityEngine;
 using UnityEngine.Events;
 
 public class EventTest : MonoBehaviour {

    [Serializable]
    public class MyEventType : UnityEvent { }

    public MyEventType OnEvent;
public void TestIn()
    {
        Debug.Log("TestIn called");
    }
}