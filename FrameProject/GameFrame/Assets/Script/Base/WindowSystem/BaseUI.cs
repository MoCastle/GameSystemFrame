using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour {
    public UIManager _uiMgr;
    public UIManager UIMgr
    {
        get
        {
            if( _uiMgr == null )
            {
                _uiMgr = UIManager.Manager;
            }
            return _uiMgr;
        }
    }

    //UI的名字就是模板UI的名字
    public string UIName
    {
        get
        {
            return gameObject.name;
        }
    }

    public void Close( )
    {
        //
        gameObject.SetActive(false);
        //UIMgr.CloseWnd();
        Debug.Log(UIName);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
