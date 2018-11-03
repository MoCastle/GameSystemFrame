using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    BaseUI OneUI;
	// Use this for initialization
	void Start () {
        UIManager Mgr = UIManager.Manager;
        Mgr.ShowUI("TestWndTwo");
        OneUI = Mgr.ShowUI("TestWndOne");
        OneUI.Close();
        // Mgr.ShowUI("TestWndTwo");
    }
    private void Update()
    {
       // OneUI.Close();
    }
}
