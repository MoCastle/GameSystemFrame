using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager {
    #region 对外接口
    public static UIManager Manager
    {
        get
        {
            if(_Manager == null)
            {
                _Manager = new UIManager( );
            }
            return _Manager;
        }
    }
    #endregion

    static UIManager _Manager;

    #region 画布属性、层级属性
    Transform _UICanvas;
    //普通层UI队列
    Transform _NormQue;
    //通用BG
    Transform _BG;
    EventTrigger _BGTrigger;

    //当前已打开的UI
    Dictionary<string, Transform> _CurShowUI;
    Dictionary<string, float> _GCUIDict;
    #endregion

    #region 初始化相关
    UIManager( )
    {
        InitWindowCanvas();
    }

    //初始化UI画布
    void InitWindowCanvas( )
    {
        Transform _UICanvas = null;
        
        GameObject IlegalCanvas = GameObject.Find("UICanvas");
        if(IlegalCanvas != null)
        {
            GameObject.Destroy(IlegalCanvas);
        }

        Object uiCanvas = Resources.Load("Prefab/Base/UI/UICanvas");
        if (uiCanvas == null)
        {
            Debug.Log("Canvas Prefab Missed");
            return;
        }
        
        GameObject uiGameObj = GameObject.Instantiate(uiCanvas) as GameObject;
        _UICanvas = uiGameObj.transform;

        _NormQue = _UICanvas.transform.FindChild("NormUI");
        _BG = _NormQue.FindChild("BG");
        _BG.gameObject.SetActive(false);
        if (_NormQue == null)
        {
            Debug.Log("UICanvas Lay NormUI Missed");
        }
        _CurShowUI = new Dictionary<string, Transform>();
        _GCUIDict = new Dictionary<string, float>();
    }
    #endregion

    #region 窗口开关
    public BaseUI ShowUI(string UIName)
    {
        Transform outUIWnd = null;

        //如果已经打开了 则只需要显示在前面就行了
        if (_CurShowUI.TryGetValue( UIName,out outUIWnd))
        {
            outUIWnd.gameObject.SetActive(true);
            outUIWnd.SetAsLastSibling();
            return outUIWnd.GetComponent<BaseUI>();
        }

        //没有打开 重新加载
        GameObject uiGameObj = LoadUI( UIName );
        if(uiGameObj == null)
        {
            return null;
        }

        //检查脚本有没有加
        BaseUI UIScr = uiGameObj.GetComponent<BaseUI>();
        if(UIScr == null)
        {
            Debug.Log("UI NotAddScript");
            return null;
        }
        HideCurMutex();

        uiGameObj.transform.SetParent(_NormQue,false);
        ShowOPForUI(UIScr);
        _CurShowUI.Add( UIName ,uiGameObj.transform);
        _GCUIDict.Remove(UIName);

        return UIScr;
    }

    //关闭窗口操作
    public void CloseWnd( BaseUI baseUI )
    {
        _GCUIDict.Add(baseUI.UIName,Time.time);
        ReShowUI();
        CloseOPForUI();
    }

    //获取UI
    public GameObject LoadUI(string UIName)
    {
        Object uiCanvas = Resources.Load("Prefab/UI/" + UIName);
        if (uiCanvas == null)
        {
            Debug.Log("UI Missed");
            return null;
        }
        string Name = uiCanvas.name;
        GameObject loadedUIModel = GameObject.Instantiate(uiCanvas) as GameObject;
        loadedUIModel.name = uiCanvas.name;

        return loadedUIModel;
    }

    //互斥窗口处理 隐藏当前已经打开的窗口
    public void HideCurMutex( )
    {
        //没有打开后需要隐藏的窗口
        if(_NormQue.childCount<1)
        {
            return;
        }
        //获取之前最后一个窗口
        BaseUI ShowingWnd = GetCurUI();
        
        //对上一个已打开窗口处理 若互斥则隐藏
        if (ShowingWnd!=null&& ShowingWnd.Type == UIType.Mutex)
        {
            ShowingWnd.Hide();
        }
        
    }

    //获取当前打开窗口
    public BaseUI GetCurUI()
    {
        if( _NormQue.childCount <1 )
        {
            return null;
        }
        BaseUI CurUI = null;
        
        for (int index = _NormQue.childCount - 1; index >= 0; --index)
        {
            Transform WndObj = _NormQue.GetChild(index);
            CurUI = WndObj.GetComponent<BaseUI>();
            //跳过被关掉的窗口
            if (CurUI == null || CurUI.IsOpenning == false)
            {
                continue;
            }
            else
            {
                //找到上一个打开中的窗口了
                break;
            }
        }
        return CurUI;
    }

    //关掉窗口后 处理上一个窗口 若隐藏则打开
    public void ReShowUI( )
    {
       
        if(_NormQue.childCount < 1)
        {
            return;
        }
        BaseUI ShowingWnd = GetCurUI();
        
        if(ShowingWnd==null || ShowingWnd.Type != UIType.Mutex)
        {
            return;
        }else
        {
            ShowOPForUI(ShowingWnd);
        }
    }

    //针对显示UI的操作
    void ShowOPForUI( BaseUI OPUI)
    {
        OPUI.Show();
        bool NeedBG = OPUI.IsUseBG;
        if(NeedBG)
        {
            int BGIdx = _BG.transform.GetSiblingIndex();
            int ShowIdx = OPUI.transform.GetSiblingIndex();

            _BG.SetSiblingIndex(BGIdx>ShowIdx? ShowIdx:ShowIdx - 1);
            _BG.gameObject.SetActive(true);
            if( OPUI.IsUseBGFunc )
            {
                
                //PointerEventData Func = new PointerEventData(OPUI.BGFunc);
                /*    OPUI.BGFunc;
                _BGTrigger.OnPointerClick(OPUI.BGFunc);*/
            }
        }
        else
        {
            _BG.gameObject.SetActive(false);
        }
    }
    //针对单个UI的关闭操作
    void CloseOPForUI(  )
    {
        _BG.gameObject.SetActive(false);
    }
    #endregion
}
