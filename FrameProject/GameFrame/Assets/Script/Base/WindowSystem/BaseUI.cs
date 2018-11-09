using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    Normal,//普通
    Mutex,//互斥
    Pop//弹窗
}

public class BaseUI : MonoBehaviour {
    [SerializeField]
    [Title("UI类型", "black")]
    UIType _uiType;
    public UIType Type
    {
        get
        {
            return _uiType;
        }
    }
    //判断该窗口是否正打开中(隐藏也属于打开状态)
    bool _Openning;
    public bool IsOpenning
    {
        get
        {
            return _Openning;
        }
    }

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
        _Openning = false;
        UIMgr.CloseWnd(this);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _Openning = true;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
