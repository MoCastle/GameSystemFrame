using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return null;
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
        HideMutex();

        uiGameObj.transform.SetParent(_NormQue,false);
        UIScr.Show();
        _CurShowUI.Add( UIName ,uiGameObj.transform);
        _GCUIDict.Remove(UIName);

        return UIScr;
    }

    //关闭窗口操作
    public void CloseWnd( BaseUI baseUI )
    {
        _GCUIDict.Add(baseUI.UIName,Time.time);
        ReShowUI();
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

    //互斥窗口处理 隐藏上一个已打开窗口
    public void HideMutex( )
    {
        //没有打开后需要隐藏的窗口
        if(_NormQue.childCount<1)
        {
            return;
        }
        //获取之前最后一个窗口
        BaseUI ShowingWnd = null;
        for(int index = _NormQue.childCount-1;index>=0;--index)
        {
            Transform WndObj = _NormQue.GetChild(index);
            ShowingWnd = WndObj.GetComponent<BaseUI>();
            //跳过被关掉的窗口
            if (ShowingWnd.IsOpenning == false)
            {
                continue;
            }else
            {
                //找到上一个打开中的窗口了
                break;
            }
        }
        //对上一个已打开窗口处理 若互斥则隐藏
        if (ShowingWnd.Type == UIType.Mutex)
        {
            ShowingWnd.Hide();
        }
        
    }

    //关掉窗口后 处理上一个窗口 若隐藏则打开
    public void ReShowUI( )
    {
        BaseUI ShowingWnd = null;
        if(_NormQue.childCount < 1)
        {
            return;
        }

        //找到上一个窗口
        for (int index = _NormQue.childCount - 1; index >= 0; --index)
        {
            Transform WndObj = _NormQue.GetChild(index);
            ShowingWnd = WndObj.GetComponent<BaseUI>();
            //跳过被关掉的窗口
            if (ShowingWnd.IsOpenning == false)
            {
                continue;
            }
            else
            {
                //找到上一个打开中的窗口了
                break;
            }
        }

        if(ShowingWnd.Type != UIType.Mutex)
        {
            return;
        }else
        {
            ShowingWnd.Show();
        }
    }
    #endregion
}
