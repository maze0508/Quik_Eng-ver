using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AccountManager {

    private string serverlink = "http://140.115.126.137/Quik_E/";
    string s_checksum;
    public int state;
    public string s_state;
    public string[] AccountInfo;
    Xmlprocess xmlprocess;
    //MySQLAccess mySQLAccess;


    public IEnumerator CheckLogin(string fileName, string[] str)
    {
        //mySQLAccess = new MySQLAccess("140.115.126.137", "maze", "106524006", "microbe");


        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", str[0]);
        phpform.AddField("user_pwd", str[1]);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        //s_state = reg.ToString();
        
        if (reg.error == null)
        {
            if (reg.text == "0")
            {
                state = 0;//帳密錯誤
            }
            else if (reg.text == "1")
            {
                state = 2;//連線失敗
            }
            else//帳密正確
             {
                 AccountInfo = reg.text.Split(',');
                 state = 1;
                 xmlprocess = new Xmlprocess(AccountInfo[0]);
                 xmlprocess.setUserInfo(AccountInfo);//ID,pwd,level,sex
                 xmlprocess.ScceneHistoryRecord("Login", DateTime.Now.ToString("HH:mm:ss"));
             }
           /* else {
                state = 0;
                s_state = reg.ToString();
            }*/
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }

    }

    public IEnumerator CheckRegister(string fileName, string[] str)
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", str[0]);
        phpform.AddField("user_pwd", str[1]);
        phpform.AddField("user_name", str[2]);
        phpform.AddField("user_sex", str[3]);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        if (reg.error == null)
        {
            if (reg.text == "0")
            {
                AccountInfo = new string[]{str[0],str[2],"1", str[3] };
                state = 0;//帳號不重複
                Debug.Log(state+" "+ AccountInfo);
                xmlprocess = new Xmlprocess(AccountInfo[0]);
                xmlprocess.setUserInfo(AccountInfo);//將註冊資訊傳至XmlNode
                //xmlprocess.timeHistoryRecord("Register");
                xmlprocess.ScceneHistoryRecord("Register", DateTime.Now.ToString("HH:mm:ss"));

            }
            else
            {
                state = 1;
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

}
