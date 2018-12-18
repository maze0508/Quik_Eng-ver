using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;
using System.Text;
using edu.ncu.list.util;
using UnityEngine.UI;

public class UpdateSQL : MonoBehaviour {
    public Button test;

    protected Xmlprocess xmlprocess;
    MySQLAccess mySQLAccess;
    public XmlDocument xmlDoc;

    //private string serverlink = "http://140.115.126.167/microbe/uploadData.php";


    public int stateBG;
    static string host = "140.115.126.167";
    static string id = "maze";
    static string pwd = "106524006";
    static string database = "quik_ani";
    public static string result = "";

    string userID = "";

    void Start () {
        mySQLAccess = new MySQLAccess(host, id, pwd, database);
        xmlprocess = new Xmlprocess();
        StartCoroutine("ReloadXMLtoDB", 0.5F);

    }

    IEnumerator ReloadXMLtoDB(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
       // xmlDoc = new XmlDocument();
        //xmlDoc.Load(xmlprocess.getPath());
        XmlNode node = xmlprocess.xmlDoc.SelectSingleNode("Loadfile/User");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("ID");
        userID = attribute.Value;

        /*等級*/
        string userlevel = element.GetAttributeNode("level").Value;
        mySQLAccess.UpdateInto("member", "level", userlevel, "user_id", userID.ToString());

        /*學習狀態*/
        string[] learning_task_col = new string[8];
        learning_task_col[0] = "user_id";
        learning_task_col[1] = "review_count";
        learning_task_col[2] = "learning_count";
        learning_task_col[3] = "learning_correct";
        learning_task_col[4] = "learning_wrong";
        learning_task_col[5] = "learning_improve";
        learning_task_col[6] = "highscore";
        learning_task_col[7] = "uploadTime";

        node = xmlprocess.xmlDoc.SelectSingleNode("Loadfile/User/learning");
        element = (XmlElement)node;
        string[] learning_task = new string[8];
        learning_task[0] = userID;
        learning_task[1] = element.GetAttributeNode("review_count").Value;
        learning_task[2] = element.GetAttributeNode("learning_count").Value;
        learning_task[3] = element.GetAttributeNode("learning_correct").Value;
        learning_task[4] = element.GetAttributeNode("learning_wrong").Value;
        learning_task[5] = element.GetAttributeNode("learning_improve").Value;
        learning_task[6] = element.GetAttributeNode("highscore").Value;
        learning_task[7] = DateTime.Now.ToString();
        mySQLAccess.InsertInto("learning_task", learning_task_col,learning_task);

        /*對戰狀態*/
        string[] compete_task_col = new string[7];
        compete_task_col[0] = "user_id";
        compete_task_col[1] = "compete_count";
        compete_task_col[2] = "compete_correct";
        compete_task_col[3] = "compete_wrong";
        compete_task_col[4] = "compete_improve";
        compete_task_col[5] = "highscore";
        compete_task_col[6] = "uploadTime";

        node = xmlprocess.xmlDoc.SelectSingleNode("Loadfile/User/compete");
        element = (XmlElement)node;
        string[] compete_task = new string[7];
        compete_task[0] = userID;
        compete_task[1] = element.GetAttributeNode("compete_count").Value;
        compete_task[2] = element.GetAttributeNode("compete_correct").Value;
        compete_task[3] = element.GetAttributeNode("compete_wrong").Value;
        compete_task[4] = element.GetAttributeNode("compete_improve").Value;
        compete_task[5] = element.GetAttributeNode("highscore").Value;
        compete_task[6] = DateTime.Now.ToString();
        mySQLAccess.InsertInto("compete_task", compete_task_col, compete_task);

        /*學習類獎章紀錄*/
        string[] badge_learning_col = new string[4];
        badge_learning_col[0] = "user_id";
        badge_learning_col[1] = "badge_id";
        badge_learning_col[2] = "badge_level";
        badge_learning_col[3] = "uploadTime";
        for (int i = 1; i <= 5; i++)//學習類有5種獎章
        {
            string[] badge_learning = new string[4];
            badge_learning[0] = userID;
            badge_learning[1] = i.ToString();
            node = xmlprocess.xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge"+i);
            element = (XmlElement)node;
            badge_learning[2] = element.GetAttributeNode("level").Value;
            badge_learning[3] = DateTime.Now.ToString();
            mySQLAccess.InsertInto("badge_record", badge_learning_col, badge_learning);
        }
        /*對戰類獎章紀錄*/
        for (int i = 6; i <= 12; i++)//對戰類有7種獎章
        {
            string[] badge_compete = new string[4];
            badge_compete[0] = userID;
            badge_compete[1] = i.ToString();
            node = xmlprocess.xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge" + i);
            element = (XmlElement)node;
            badge_compete[2] = element.GetAttributeNode("level").Value;
            badge_compete[3] = DateTime.Now.ToString();
            mySQLAccess.InsertInto("badge_record", badge_learning_col, badge_compete);
        }

        /*點擊成就頁面紀錄*/
        string[] touch_record_col = new string[4];
        touch_record_col[0] = "user_id";
        touch_record_col[1] = "clickcount";
        touch_record_col[2] = "showcount";
        touch_record_col[3] = "uploadTime";

        node = xmlprocess.xmlDoc.SelectSingleNode("Loadfile/touch_history/touch_achieve");
        element = (XmlElement)node;
        string[] touch_record = new string[4];
        touch_record[0] = userID;
        touch_record[1] = element.GetAttributeNode("clickcount").Value;
        touch_record[2] = element.GetAttributeNode("showcount").Value;
        touch_record[3] = DateTime.Now.ToString();
        mySQLAccess.InsertInto("touch_record", touch_record_col, touch_record);

        /*學習紀錄*/
        string[] learning_record_col = new string[9];
        learning_record_col[0] = "user_id";
        learning_record_col[1] = "startTime";
        learning_record_col[2] = "endTime";
        learning_record_col[3] = "option_score";
        learning_record_col[4] = "cloze_score";
        learning_record_col[5] = "score";
        learning_record_col[6] = "correct";
        learning_record_col[7] = "maxcorrect";
        learning_record_col[8] = "uploadTime";

        XmlNodeList nodelist = xmlprocess.xmlDoc.SelectNodes("//learning_record");
        foreach (XmlNode itemsNode in nodelist)
        {
            element = (XmlElement)itemsNode;
            string[] learning_record = new string[9];
            learning_record[0] = userID;
            learning_record[1] = element.GetAttributeNode("startTime").Value;
            learning_record[2] = element.GetAttributeNode("endTime").Value; 
            learning_record[3] = element.GetAttributeNode("option_score").Value; 
            learning_record[4] = element.GetAttributeNode("cloze_score").Value; 
            learning_record[5] = element.GetAttributeNode("score").Value; 
            learning_record[6] = element.GetAttributeNode("correct").Value; 
            learning_record[7] = element.GetAttributeNode("maxcorrect").Value; 
            learning_record[8] = DateTime.Now.ToString();
            mySQLAccess.InsertInto("learning_record", learning_record_col, learning_record);

            
        }

        /*對戰紀錄*/
        string[] compete_record_col = new string[11];
        compete_record_col[0] = "user_id";
        compete_record_col[1] = "compete_id";
        compete_record_col[2] = "startTime";
        compete_record_col[3] = "endTime";
        compete_record_col[4] = "hint_LA";
        compete_record_col[5] = "hint_ST";
        compete_record_col[6] = "correct";
        compete_record_col[7] = "maxcorrect";
        compete_record_col[8] = "score";
        compete_record_col[9] = "rank";
        compete_record_col[10] = "uploadTime";

        nodelist = xmlprocess.xmlDoc.SelectNodes("//compete_record ");
        foreach (XmlNode itemsNode in nodelist)
        {
            element = (XmlElement)itemsNode;
            string[] compete_record = new string[11];
            compete_record[0] = userID;
            compete_record[1] = element.GetAttributeNode("compete_id").Value;
            compete_record[2] = element.GetAttributeNode("startTime").Value; 
            compete_record[3] = element.GetAttributeNode("endTime").Value; 
            compete_record[4] = element.GetAttributeNode("hint_LA").Value;
            compete_record[5] = element.GetAttributeNode("hint_ST").Value; 
            compete_record[6] = element.GetAttributeNode("correct").Value; 
            compete_record[7] = element.GetAttributeNode("maxcorrect").Value; 
            compete_record[8] = element.GetAttributeNode("score").Value; 
            compete_record[9] = element.GetAttributeNode("rank").Value; 
            compete_record[10] = DateTime.Now.ToString();
            mySQLAccess.InsertInto("compete_record", compete_record_col, compete_record);
        }

        /*回合紀錄*/
        string[] round_record_col = new string[11];
        round_record_col[0] = "user_id";
        round_record_col[1] = "compete_id";
        round_record_col[2] = "round_id";
        round_record_col[3] = "ques_id";
        round_record_col[4] = "ans_state";
        round_record_col[5] = "duration";
        round_record_col[6] = "hint_LA";
        round_record_col[7] = "hint_ST";
        round_record_col[8] = "score";
        round_record_col[9] = "rank";
        round_record_col[10] = "uploadTime";

        nodelist = xmlprocess.xmlDoc.SelectNodes("//round_record ");
        foreach (XmlNode itemsNode in nodelist)
        {
            element = (XmlElement)itemsNode;
            string[] round_record = new string[11];
            round_record[0] = userID;
            round_record[1] = element.GetAttributeNode("compete_id").Value;
            round_record[2] = element.GetAttributeNode("round_id").Value;
            round_record[3] = element.GetAttributeNode("ques_id").Value; 
            round_record[4] = element.GetAttributeNode("ans_state").Value; 
            round_record[5] = element.GetAttributeNode("duration").Value;
            round_record[6] = element.GetAttributeNode("hint_LA").Value; 
            round_record[7] = element.GetAttributeNode("hint_ST").Value; 
            round_record[8] = element.GetAttributeNode("score").Value;
            round_record[9] = element.GetAttributeNode("rank").Value;
            round_record[10] = DateTime.Now.ToString();
            mySQLAccess.InsertInto("round_record", round_record_col, round_record);
        }

        /*場景紀錄*/
        string[] scene_record_col = new string[4];
        scene_record_col[0] = "user_id";
        scene_record_col[1] = "scene";
        scene_record_col[2] = "startTime";
        scene_record_col[3] = "uploadTime";

        nodelist = xmlprocess.xmlDoc.SelectNodes("//scene_record ");
        foreach (XmlNode itemsNode in nodelist)
        {
            element = (XmlElement)itemsNode;
            string[] scene_record = new string[4];
            scene_record[0] = userID;
            scene_record[1] = element.GetAttributeNode("scene").Value;
            scene_record[2] = element.GetAttributeNode("startTime").Value;
            scene_record[3] = DateTime.Now.ToString();
            mySQLAccess.InsertInto("scene_record", scene_record_col, scene_record);

            
        }
        Application.Quit();
    }



    public static void OnApplicationQuit()
    {
        MySQLAccess.Close();
        
    }



}
