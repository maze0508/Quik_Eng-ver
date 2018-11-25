using UnityEngine;
using System.Collections.Generic;
 
public class UIManager : Singleton<UIManager>
{
    private string UIRoot = "UIControl/";
 
    public GameObject CanvasObj;
 
    public Dictionary<string, GameObject> UIList = new Dictionary<string, GameObject>();
 
 
 
    private bool CheckCanvasRootIsNull()
    {
        if (CanvasObj == null)
        {
            Debug.LogError("CanvasObj is Null, Please in your Canvas add UIRootHandler.cs");
            return true;
        }
            return false;
    }
 
    public bool IsUILive(string _UIname)
    {
        return UIList.ContainsKey(_UIname);
    }
 
    public GameObject ShowPanel(string _UIname)
    {
        if (CheckCanvasRootIsNull())
            return null;
 
        if (IsUILive(_UIname))
        {
            Debug.LogErrorFormat("[{0}] is Showing, if you want to show, please close first!!", _UIname);
            return null;
        }
 
        GameObject loadGo = Utility.AssetRelate.ResourcesLoadCheckNull<GameObject>(UIRoot + _UIname);
        if (loadGo == null)
            return null;
 
 
        GameObject UI = Utility.GameObjectRelate.InstantiateGameObject(CanvasObj, loadGo);
        UI.name = _UIname;
 
 
        UIList.Add(_UIname, UI);
 
 
        return UI;
    }
    public void SetPosition(string _objName, Vector3 position) {//³]©wUI¦ì¸m
        Vector3 _pos = CanvasObj.gameObject.transform.position ;
        UIList[_objName].transform.position = new Vector3(position.x+_pos.x,position.y+_pos.y,0);
    }
    public void TogglePanel(string name, bool isOn)
    {
        if (IsUILive(name))
        {
            if (UIList[name] != null)
                UIList[name].SetActive(isOn);
        }
        else
        {
            Debug.LogErrorFormat("TogglePanel [{0}] not found.", name);
        }
    }
 
    public void ClosePanel(string name)
    {
        if (IsUILive(name))
        {
            if (UIList[name] != null)
                Object.Destroy(UIList[name]);
 
            UIList.Remove(name);
        }
        else
        {
            Debug.LogErrorFormat("ClosePanel [{0}] not found.", name);
        }
    }
 
    public void CloseAllPanel()
    {
        foreach (KeyValuePair<string, GameObject> item in UIList)
        {
            if (item.Value != null)
                Object.Destroy(item.Value);
        }
 
        UIList.Clear();
    }
 
    public Vector2 GetCanvasSize()
    {
        if (CheckCanvasRootIsNull())
            return Vector2.one * -1;
 
        RectTransform trans = CanvasObj.transform as RectTransform;
 
        return trans.sizeDelta;
    }
 
}