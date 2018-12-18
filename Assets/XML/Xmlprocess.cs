using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using edu.ncu.list.util;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

public class Xmlprocess
{
    public XmlDocument xmlDoc;
    XmlCreate xmlCreate;
    public int count_onetime = 0;
    public static float levelVal = 0.0f;
    public string Strtime = (System.DateTime.Now).ToString();
    public static string path, _FileName;


    ///<summary>
    ///initial file,search the same xml file with the same userID
    ///</summary>
    public Xmlprocess(string filename, string[] userInfo)
    { //database initial
        if (Application.platform == RuntimePlatform.Android)
        {
            //path = Constants.DATABASE_PATH;
            path = Application.persistentDataPath + "//";
        }
        else
        {
            path = Application.dataPath + "/Resources/";
        }

        _FileName = filename + ".xml";

        if (!isExits())
        {
            xmlCreate = new XmlCreate(path, _FileName);//若檔案不存在，則創建xml
            xmlDoc = new XmlDocument();
            xmlDoc.Load(path + _FileName);
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute[] attributeList = { element.GetAttributeNode("ID"), element.GetAttributeNode("name"), element.GetAttributeNode("level"), element.GetAttributeNode("sex") };
            for (int i = 0; i < userInfo.Length; i++)
            {
                attributeList[i].Value = userInfo[i];
            }
            saveData();
        }
        else
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(path + _FileName);
        }
    }

    public Xmlprocess()
    {
        if (isExits())
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(path + _FileName);
        }
    }

    /*private Boolean isExits()
    {
        if (!System.IO.File.Exists(path + _FileName))
        {
            xmlCreate = new XmlCreate(path, _FileName);//若檔案不存在，則創建xml
        }
        return true;
    }*/
    private Boolean isExits()
    {
        if (!System.IO.File.Exists(path + _FileName))
        {
            return false;
        }
        return true;
    }


    public void saveData()
    {
        xmlDoc.Save(path + _FileName);
    }

    public string getPath()
    {
        return path + _FileName;
    }


    //---------------------------------個人狀態--------------------------------------

    ///<summary>
    ///When registering,initial set userInfo.
    ///</summary>
   /* public void setUserInfo(string[] userInfo)
    {

        if (isExits())//如果沒有xml檔，讀SQL
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute[] attributeList = { element.GetAttributeNode("ID"), element.GetAttributeNode("name"), element.GetAttributeNode("level"), element.GetAttributeNode("sex") };
            for (int i = 0; i < userInfo.Length; i++)
            {
                attributeList[i].Value = userInfo[i];
            }
            saveData();
        }
    }*/

    ///<summary>
    ///return an array, 0=ID,1=name,2=level,3=sex
    ///</summary>
    public string[] getUserInfo()
    {
        if (isExits())
        {
            string[] info = new string[4];
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute[] attribute = { element.GetAttributeNode("ID"), element.GetAttributeNode("name"), element.GetAttributeNode("level"), element.GetAttributeNode("sex")};
            for (int i = 0; i < info.Length; i++)
            {
                info[i] = attribute[i].Value.ToString();

            }

            return info;
        }
        return null;
    }


    public void setLevel()//升等
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("level");
            int level = XmlConvert.ToInt32(attribute.Value);
            float standardVal = 100 + ((level - 1) * 35f);
            if (levelVal >= standardVal)
            {
                level++;
                levelVal = levelVal - standardVal;
            }
            attribute.Value = level.ToString();
            saveData();
        }
    }


    public void ExitTimeHistoryRecord(string endTime)
    {
        if (isExits())
        {
            //XmlNode nodeLast = null;
            XmlNode nodeLast_Previous = null;

            // Find the previous scene start time ********************************************************************************************
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//time_history_day");
            foreach (XmlNode item_File_Previous in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = item_File_Previous.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLast_Previous = item_File_Previous;
                }
            }
            XmlElement elementLast_Previous = (XmlElement)nodeLast_Previous;
            XmlAttribute attributeLast_StartTime = elementLast_Previous.GetAttributeNode("startTime");
            XmlAttribute attributeLast_EndTime = elementLast_Previous.GetAttributeNode("endTime");
            XmlAttribute attributeLast_Duration = elementLast_Previous.GetAttributeNode("duration");

            DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
            DateTime getTime = Convert.ToDateTime(attributeLast_StartTime.Value.ToString());

            System.TimeSpan diff = nowTime.Subtract(getTime);
            int timerNum = (int)diff.TotalSeconds;

            attributeLast_EndTime.Value = getTime.ToString();
            attributeLast_Duration.Value = (timerNum / 60).ToString() + ":" + (timerNum % 60).ToString();
            // *********************************************************************************************************************************************
            saveData();
        }
    }



    /*-------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /// <summary>
    /// 取得目前個人學習區練習次數
    /// </summary>
    /// <returns></returns>
    public bool getLearningCount()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("learning_count");
            int learningcount = XmlConvert.ToInt32(attribute.Value);
            if (learningcount > 0) { return true; }
            return false;
        }
        return false;
    }

    /// <summary>
    /// 取得目前個人學習區預習次數
    /// </summary>
    /// <returns></returns>
    public bool getReviewCount()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("review_count");
            int learningcount = XmlConvert.ToInt32(attribute.Value);
            if (learningcount > 0) { return true; }
            return false;
        }
        return false;
    }

    /// <summary>
    /// 更新單字瀏覽次數與練習次數
    /// </summary>
    /// <param name="attributeName">review_count或是learning_count</param>
    public string setLearningCount(string attributeName)
    {
        string state = null;
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode(attributeName);
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            if (attributeName == "learning_count")
            {
                state = setBadgeLearningCounts(count);
            }
            saveData();
            return state;
        }
        return null;
    }

    /// <summary>
    /// 新增每回單字學習紀錄
    /// </summary>
    public void createLearningRecord()
    {
        XmlNode nodeLast = null;
        XmlElement learning_history = null;

        if (isExits())
        {

            XmlNodeList nodelist = xmlDoc.SelectNodes("//log_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement element = (XmlElement)nodeLast;
            XmlAttribute attributeLast = element.GetAttributeNode("day");
            if (attributeLast.Value.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))//如果最近一筆紀錄不是今天的日期
            {

                XmlNode n_Loadfile = xmlDoc.SelectSingleNode("Loadfile/");
                XmlElement loadfile = (XmlElement)n_Loadfile;
                XmlElement log_record = xmlDoc.CreateElement("log_record");
                log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                loadfile.AppendChild(log_record);//創新log與learning節點

                XmlElement learninghistory = xmlDoc.CreateElement("learning_history"); ;
                log_record.AppendChild(learninghistory);
                learning_history = learninghistory;
            }
            else
            {
                XmlNode n_learning_history = nodeLast.SelectSingleNode("learning_history");//在最近一筆的log下。找到learning節點
                learning_history = (XmlElement)n_learning_history;
            }

            XmlElement learning_record = xmlDoc.CreateElement("learning_record");
            learning_history.AppendChild(learning_record);

            learning_record.SetAttribute("startTime", DateTime.Now.ToString("HH: mm:ss"));
            learning_record.SetAttribute("option_score", "0");
            learning_record.SetAttribute("cloze_score", "0");
            learning_record.SetAttribute("score", "0");
            learning_record.SetAttribute("correct", "0");
            learning_record.SetAttribute("maxcorrect", "0");
            learning_record.SetAttribute("endTime", "");

            saveData();
        }
    }

    /// <summary>
    /// 更新每回單字學習紀錄中，不同題型的分數
    /// </summary>
    /// <param name="quesType">option_score or cloze_score</param>
    public void setLearningTypeScoreRecord(string TypeName,int score)
    {
        if (isExits())
        {
            XmlNode nodeLastLearning = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//learning_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastLearning = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastLearning;
            XmlAttribute attr_optionscore = element.GetAttributeNode("option_score");

            switch (TypeName) {
                case "option":
                    attr_optionscore.Value = score.ToString();
                    break;
                case "cloze":
                    int optionScore = XmlConvert.ToInt32(attr_optionscore.Value);
                    XmlAttribute attr_clozescore = element.GetAttributeNode("cloze_score");
                    attr_clozescore.Value = (score - optionScore).ToString();//總分減選擇題分數
                    break;
            }
            saveData();
        }
    }


    /// <summary>
    /// 更新每回單字學習紀錄的分數與結束時間
    /// </summary>
    public string[] setLearningScoreRecord(int score)
    {
        if (isExits())
        {
            XmlNode nodeLastLearning = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//learning_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastLearning = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastLearning;
            XmlAttribute attr_score = element.GetAttributeNode("score");
            XmlAttribute attr_endTime = element.GetAttributeNode("endTime");

            attr_score.Value = score.ToString();
            attr_endTime.Value = DateTime.Now.ToString("HH: mm:ss");
            string[] _state = updateHighScore(score);
            saveData();
            if (_state != null) {//表示刷新分數
                return _state;
            }
            return null;
        }
        return null;
    }

    string[] updateHighScore(int score)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("highscore");
        int highscore = XmlConvert.ToInt32(attribute.Value);
        string[] _state = new string[2];
        if (score > highscore)
        {
            attribute.Value = score.ToString();
            int improveCounts = setLearningScoreImprove();//當前進步次數
            _state[0] = setBadgeLearningHighScore(score);
            _state[1]= setBadgeLearningImprove(improveCounts);
            return _state;
        }
        saveData();
        return null;
    }

    /// <summary>
    /// 更新該回單字學習紀錄的正確與錯誤題數
    /// </summary>
    public string setLearningCorrect(int correctCount,int wrongCount)
    {
        if (isExits())
        {
            /*更新學習紀錄的答對題數*/
            XmlNode nodeLastLearning = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//learning_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastLearning = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastLearning;
            XmlAttribute attr_correct = element.GetAttributeNode("correct");
            attr_correct.Value = correctCount.ToString();

            /*更新學習區的累積答對答錯題數*/
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement element1 = (XmlElement)node;
            XmlAttribute attr_correctSum = element1.GetAttributeNode("learning_correct");
            XmlAttribute attr_wrongSum = element1.GetAttributeNode("learning_wrong");
            int correctSum = XmlConvert.ToInt32(attr_correctSum.Value);
            int wrongSum = XmlConvert.ToInt32(attr_wrongSum.Value);
            attr_correctSum.Value =(correctSum+ correctCount).ToString();
            attr_wrongSum.Value = (wrongSum + wrongCount).ToString();


            /*更新獎章狀況*/
            string state = null;
            state = setBadgeLearningCorrect(correctSum + correctCount);
            saveData();
            return state;
        }
        return null;
    }


    /// <summary>
    /// 更新每回單字學習紀錄的最高連續正確題數
    /// </summary>
    public string setLearningMaxCorrect(int max_correctCount)
    {
        if (isExits())
        {
            XmlNode nodeLastLearning = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//learning_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastLearning = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastLearning;
            XmlAttribute attr_maxcorrect = element.GetAttributeNode("maxcorrect");
            string state = null;
            attr_maxcorrect.Value = max_correctCount.ToString();
            state = setBadgeLearningMaxCorrect(max_correctCount);
            saveData();
            return state;
        }
        return null;
    }

    /// <summary>
    /// 更新練習進步總次數
    /// </summary>
    /// <returns></returns>
    int setLearningScoreImprove()
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
        XmlElement element = (XmlElement)node;
        XmlAttribute attr_count = element.GetAttributeNode("learning_count");
        XmlAttribute attr_improve = element.GetAttributeNode("learning_improve");
        if (attr_count.Value != "0")
        {
            int count = XmlConvert.ToInt32(attr_improve.Value);
            count = count + 1;
            attr_improve.Value = count.ToString();
            saveData();
            return count;
        }
        return 0;
    }


    /*===============================---同儕對戰區的紀錄---===============================*/
    /// <summary>
    /// 新增一筆對戰紀錄
    /// </summary>
    public void createCompeteRecord()
    {
        XmlNode nodeLast = null;
        XmlElement compete_history = null;

        if (isExits())
        {

            XmlNodeList nodelist = xmlDoc.SelectNodes("//log_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement element = (XmlElement)nodeLast;
            XmlAttribute attributeLast = element.GetAttributeNode("day");
            if (attributeLast.Value.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))//如果最近一筆紀錄不是今天的日期
            {

                XmlNode n_Loadfile = xmlDoc.SelectSingleNode("Loadfile/");
                XmlElement loadfile = (XmlElement)n_Loadfile;
                XmlElement log_record = xmlDoc.CreateElement("log_record");
                log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                loadfile.AppendChild(log_record);//創新log與learning節點

                XmlElement competehistory = xmlDoc.CreateElement("compete_history"); ;
                log_record.AppendChild(competehistory);
                compete_history = competehistory;
            }
            else
            {
                XmlNode n_compete_history = nodeLast.SelectSingleNode("compete_history");//在最近一筆的log下。找到節點
                compete_history = (XmlElement)n_compete_history;
            }

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");//根據當前對戰完成次數+1作為對戰id
            XmlElement element_compete = (XmlElement)node;
            XmlAttribute attr_competecount = element_compete.GetAttributeNode("compete_count");
            int count = XmlConvert.ToInt32(attr_competecount.Value);
            int compete_id = ++count;


            XmlElement compete_record = xmlDoc.CreateElement("compete_record");
            compete_history.AppendChild(compete_record);
            compete_record.SetAttribute("compete_id", compete_id.ToString());
            compete_record.SetAttribute("startTime", DateTime.Now.ToString("HH: mm:ss"));
            compete_record.SetAttribute("endTime", "");
            compete_record.SetAttribute("hint_LA", "0");//使用提示再聽一次的總次數
            compete_record.SetAttribute("hint_ST", "0");//使用提示中譯的總次數
            compete_record.SetAttribute("correct", "0");
            compete_record.SetAttribute("maxcorrect", "0");
            compete_record.SetAttribute("score", "0");
            compete_record.SetAttribute("rank", "0");//本次對戰排名
            saveData();
        }
    }

    /// <summary>
    /// 進入遊戲，更新個人對戰的次數
    /// </summary>
    /// <param name="attributeName"></param>
    public string setCompeteCount()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("compete_count");
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();

            saveData();
            return setBadgeCompeteCounts(count);
        }
        return null;
    }


    /// <summary>
    ///對戰結束更新對戰紀錄 0:improve;1:highscore;2:rank
    /// </summary>
    public string[] setCompeteScoreRecord(int hintLACount, int hintSTCount, int score, int rank)
    {
        if (isExits())
        {
            XmlNode nodeLastCompete = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//compete_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastCompete = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastCompete;
            XmlAttribute attr_score = element.GetAttributeNode("score");
            XmlAttribute attr_endTime = element.GetAttributeNode("endTime");
            XmlAttribute attr_hintLA = element.GetAttributeNode("hint_LA");
            XmlAttribute attr_hintST = element.GetAttributeNode("hint_ST");
            XmlAttribute attr_rank = element.GetAttributeNode("rank");
            attr_hintLA.Value = hintLACount.ToString();
            attr_hintST.Value = hintSTCount.ToString();
            attr_score.Value = score.ToString();
            attr_endTime.Value = DateTime.Now.ToString("HH: mm:ss");
            attr_rank.Value = rank.ToString();
            string []_state = updateCompeteHighScore(score,rank);
            saveData();
            return _state;
        }
        return null;
    }

    string [] updateCompeteHighScore(int score,int rank)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("highscore");
        int highscore = XmlConvert.ToInt32(attribute.Value);
        string[] _state = new string[3];
        if (score > highscore)
        {
            attribute.Value = score.ToString();
            int improveCounts = CompeteScoreImprove();//當前進步次數
            _state[0] = setBadgeCompeteHighScore(score);
            _state[1] = setBadgeCompeteImprove(improveCounts);
        }
        _state[2] = setBadgeCompeteRank(rank);//將三個狀態一起存在同一陣列
        saveData();
        return _state;

    }


    /// <summary>
    /// 更新本次對戰紀錄的正確與錯誤題數
    /// </summary>
    public string setCompeteCorrectRecord(int correctCount, int wrongCount)
    {
        if (isExits())
        {
            /*更新對戰紀錄的答對題數*/
            XmlNode nodeLastCompete = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//compete_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastCompete = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastCompete;
            XmlAttribute attr_correct = element.GetAttributeNode("correct");
            attr_correct.Value = correctCount.ToString();

            /*更新對戰區的累積答對答錯題數*/
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
            XmlElement element1 = (XmlElement)node;
            XmlAttribute attr_correctSum = element1.GetAttributeNode("compete_correct");
            XmlAttribute attr_wrongSum = element1.GetAttributeNode("compete_wrong");
            int correctSum = XmlConvert.ToInt32(attr_correctSum.Value);
            int wrongSum = XmlConvert.ToInt32(attr_wrongSum.Value);
            attr_correctSum.Value = (correctSum + correctCount).ToString();
            attr_wrongSum.Value = (wrongSum + wrongCount).ToString();

            /*更新獎章狀況*/
            //string state = null;
            string _state = setBadgeCompeteCorrect(correctCount);
            saveData();
            return _state;
        }
        return null;
    }


    /// <summary>
    /// 更新每回單字學習紀錄的最高連續正確題數
    /// </summary>
    public string setCompeteMaxCorrectRecord(int max_correctCount)
    {
        if (isExits())
        {
            XmlNode nodeLastCompete = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//compete_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastCompete = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastCompete;
            XmlAttribute attr_maxcorrect = element.GetAttributeNode("maxcorrect");
            //string state = null;
            attr_maxcorrect.Value = max_correctCount.ToString();
            string _state = setBadgeCompeteMaxCorrect(max_correctCount);
            saveData();
            return _state;
        }
        return null;
    }

    /// <summary>
    /// 對戰進步次數
    /// </summary>
    /// <returns></returns>
    int CompeteScoreImprove()
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("compete_improve");
        int count = XmlConvert.ToInt32(attribute.Value);
        count = count + 1;
        attribute.Value = count.ToString();
        saveData();

        return count;
    }


    /// <summary>
    /// 新增每回合的對戰紀錄
    /// </summary>
    /// <param name="quesID">題號</param>
    public void createRoundRecord(int turn,string quesID)
    {
        XmlNode nodeLast = null;
        if (isExits())
        {

            XmlNodeList nodelist = xmlDoc.SelectNodes("//compete_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }


            XmlElement compete_record = (XmlElement)nodeLast;
            XmlAttribute attr_competeid = compete_record.GetAttributeNode("compete_id");

            XmlElement round_record = xmlDoc.CreateElement("round_record");
            compete_record.AppendChild(round_record);

            round_record.SetAttribute("compete_id", attr_competeid.Value);
            round_record.SetAttribute("round_id", turn.ToString());//當前回合
            round_record.SetAttribute("ques_id", quesID.ToString());//題號
            round_record.SetAttribute("ans_state", "");//作答正確或錯誤
            round_record.SetAttribute("duration", "0");//作答時間
            round_record.SetAttribute("hint_LA", "0");//提示再聽一次的次數
            round_record.SetAttribute("hint_ST", "0");//提示中譯的次數
            round_record.SetAttribute("score", "0");//作答時間
            round_record.SetAttribute("rank", "0");//當回合的排名
            saveData();
        }
    }

    /// <summary>
    /// 設置當回合的答案
    /// </summary>
    /// <param name="ans_state">正確或錯誤</param>
    /// <param name="duration">花費時間</param>
    public string setRoundAns(string ans_state, int duration)
    {
        XmlNode nodeLast = null;
        XmlNodeList nodelist = xmlDoc.SelectNodes("//round_record");
        foreach (XmlNode item_File in nodelist)
        {
            XmlAttributeCollection xAT = item_File.Attributes;
            for (int i = 0; i < xAT.Count; i++)
            {
                nodeLast = item_File;
            }
        }
        XmlElement round_record = (XmlElement)nodeLast;
        XmlAttribute attr_ansState = round_record.GetAttributeNode("ans_state");
        XmlAttribute attr_duration = round_record.GetAttributeNode("duration");
        attr_ansState.Value = ans_state;
        attr_duration.Value = duration.ToString();
        if (ans_state == "correct")
        {
            return setBadgeCompeteSpendtime(duration);
        }
        saveData();
        return null;
    }

    /// <summary>
    /// 當回合使用提示次數
    /// </summary>
    /// <param name="hintName">提示名稱</param>
    public void setRoundHintcount(string hintName,int currentcount)
    {
        XmlNode nodeLast = null;
        XmlNodeList nodelist = xmlDoc.SelectNodes("//round_record");
        foreach (XmlNode item_File in nodelist)
        {
            XmlAttributeCollection xAT = item_File.Attributes;
            for (int i = 0; i < xAT.Count; i++)
            {
                nodeLast = item_File;
            }
        }
        XmlElement round_record = (XmlElement)nodeLast;
        XmlAttribute attr_hint = round_record.GetAttributeNode(hintName);
        attr_hint.Value = currentcount.ToString();

        saveData();
    }

    /// <summary>
    /// 取得回合使用提示的次數
    /// </summary>
    /// <param name="hintName">提示名稱</param>
    /// <returns></returns>
    public int getRoundHintcount(string hintName)
    {
        if (isExits())
        {
            XmlNode nodeLast = null;
            XmlNodeList nodelist = xmlDoc.SelectNodes("//round_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }
            XmlElement round_record = (XmlElement)nodeLast;
            XmlAttribute attr_hint = round_record.GetAttributeNode(hintName);
            int count = XmlConvert.ToInt32(attr_hint.Value);
            return count;
        }
        return 0;
    }

    /// <summary>
    /// 設置當前回合的分數
    /// </summary>
    /// <param name="score">分數</param>
    /// <param name="rank">當前排名</param>
    public void setRoundScore(int score, int rank)
    {
        XmlNode nodeLast = null;
        XmlNodeList nodelist = xmlDoc.SelectNodes("//round_record");
        foreach (XmlNode item_File in nodelist)
        {
            XmlAttributeCollection xAT = item_File.Attributes;
            for (int i = 0; i < xAT.Count; i++)
            {
                nodeLast = item_File;
            }
        }
        XmlElement round_record = (XmlElement)nodeLast;
        XmlAttribute attr_score = round_record.GetAttributeNode("score");
        XmlAttribute attr_rank = round_record.GetAttributeNode("rank");
        attr_score.Value = score.ToString();
        attr_rank.Value = rank.ToString();

        saveData();
    }

    /*===============================---點擊紀錄---===============================*/
    /// <summary>
    ///成就UI點擊次數
    /// </summary>
    /// <param name="attributeName">showcount or clickcount</param>
    public void setTouchACount(string attributeName)
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/touch_history/touch_achieve");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode(attributeName);
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    /// <summary>
    ///排行榜UI點擊次數
    /// </summary>
    /// <param name="attributeName"></param>
    public void setTouchLCount()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/touch_history/touch_leaderboard");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("count");
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    /*===============================---場景紀錄---===============================*/

    public void ScceneHistoryRecord(string scene, string starttime)
    {
        XmlNode nodeLast = null;
        XmlElement scene_history = null;
        if (isExits())
        {
            XmlNodeList nodelist = xmlDoc.SelectNodes("//log_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement element = (XmlElement)nodeLast;
            XmlAttribute attributeLast = element.GetAttributeNode("day");
            if (attributeLast.Value.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))//如果最近一筆紀錄不是今天的日期
            {

                XmlNode n_Loadfile = xmlDoc.SelectSingleNode("Loadfile/");
                XmlElement loadfile = (XmlElement)n_Loadfile;
                XmlElement log_record = xmlDoc.CreateElement("log_record");
                log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                loadfile.AppendChild(log_record);

                XmlElement scenehistory = xmlDoc.CreateElement("scene_history"); ;
                log_record.AppendChild(scenehistory);
                scene_history = scenehistory;
            }
            else
            {
                XmlNode n_scene_history = nodeLast.SelectSingleNode("scene_history");
                scene_history = (XmlElement)n_scene_history;
            }

            XmlElement scene_record = xmlDoc.CreateElement("scene_record"); ;
            scene_history.AppendChild(scene_record);
            scene_record.SetAttribute("scene", scene);
            scene_record.SetAttribute("startTime", starttime);
            saveData();
        }
    }

    /*===============================---成就頁面---===============================*/
    /// <summary>
    ///Get學習狀況 0:最高分;1:次數;2:累計答對;3:累計答錯
    /// </summary>
    public string[] getAchieveLearningState()
    {
        if (isExits())
        {
            string[] _tmp = new string[4];
            XmlNode learningNode = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement learningElement = (XmlElement)learningNode;
            XmlAttribute learning_highscore = learningElement.GetAttributeNode("highscore");
            XmlAttribute learning_count = learningElement.GetAttributeNode("learning_count");
            XmlAttribute learning_correct = learningElement.GetAttributeNode("learning_correct");
            XmlAttribute learning_wrong = learningElement.GetAttributeNode("learning_wrong");

            _tmp[0] = learning_highscore.Value;
            _tmp[1] = learning_count.Value;
            _tmp[2] = learning_correct.Value;
            _tmp[3] = learning_wrong.Value;
            return _tmp;
        }
        return null;

    }
    /// <summary>
    ///Get學習獎章狀況
    /// </summary>
    public int[] getAchieveLearningBadges(int learningBadgeCount)
    {
        if (isExits())
        {
            int[] badgesLevel = new int[learningBadgeCount]; ;
            for (int badgeID = 1; badgeID <= learningBadgeCount; badgeID++)
            {
                string badgeid = "badge" + badgeID;
                XmlNode competeBadgesNode = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/" + badgeid);
                XmlElement competeBadgesElement = (XmlElement)competeBadgesNode;
                XmlAttribute level = competeBadgesElement.GetAttributeNode("level");
                int targetLevel = XmlConvert.ToInt32(level.Value) + 1;//目標獎章為目前階段的下一階段
                badgesLevel[badgeID - 1] = targetLevel;//因為陣列索引值初始為0，故減1

            }
            return badgesLevel;
        }
        return null;

    }

    /// <summary>
    ///Get對戰狀況 0:最高分;1:次數;2:累計答對;3:累計答錯
    /// </summary>
    public string[] getAchieveCompeteState()
    {
        if (isExits())
        {
            string[] _tmp = new string[4];
            XmlNode competeNode = xmlDoc.SelectSingleNode("Loadfile/User/compete");
            XmlElement competeElement = (XmlElement)competeNode;
            XmlAttribute compete_highscore = competeElement.GetAttributeNode("highscore");
            XmlAttribute compete_count = competeElement.GetAttributeNode("compete_count");
            XmlAttribute compete_correct = competeElement.GetAttributeNode("compete_correct");
            XmlAttribute compete_wrong = competeElement.GetAttributeNode("compete_wrong");

            _tmp[0] = compete_highscore.Value;
            _tmp[1] = compete_count.Value;
            _tmp[2] = compete_correct.Value;
            _tmp[3] = compete_wrong.Value;
            return _tmp;
        }
        return null;

    }

    /// <summary>
    ///Get對戰獎章狀況
    /// </summary>
    /// <param name="initialIndex">ID起始值</param>
    /// <param name="totalBadgeCount">獎章總數</param>
    public int[] getAchieveCompeteBadges(int initialIndex, int totalBadgeCount)
    {
        if (isExits())
        {
            int[] badgesLevel = new int[(totalBadgeCount - initialIndex) + 1]; ;
            for (int badgeID = initialIndex; badgeID <= totalBadgeCount; badgeID++)
            {
                string badgeid = "badge" + badgeID;
                XmlNode competeBadgesNode = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/" + badgeid);
                XmlElement competeBadgesElement = (XmlElement)competeBadgesNode;
                XmlAttribute level = competeBadgesElement.GetAttributeNode("level");
                int targetLevel = XmlConvert.ToInt32(level.Value) + 1;//目標獎章為目前階段的下一階段
                badgesLevel[badgeID - initialIndex] = targetLevel;

            }
            return badgesLevel;
        }
        return null;

    }

    /*===============================---獎章資料---===============================*/

    /// <summary>
    /// badge1練習次數獎章
    /// </summary>
    /// <param name="learningCounts">當前練習總次數</param>
    string setBadgeLearningCounts(int learningCounts) {
        levelVal += 30f;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge1");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (learningCounts > 0 && learningCounts < 5) {
            if (attribute.Value == "0"){//首次獲得
                _state = "Get the new badge!";
                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning");//個人學習區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();
              
            }
            _level = 1;
        }
        else if (learningCounts >= 5 && learningCounts < 8) {
            _level = 2;
        }else if (learningCounts>=8) {
            if (attribute.Value == "2")//金牌達標
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }


    /// <summary>
    /// badge2練習分數達標獎章
    /// </summary>
    /// <param name="highscore">最高分數</param>
    string setBadgeLearningHighScore(int highscore) {
        levelVal += 50f;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge2");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (highscore >= 100 && highscore < 250)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning");//個人學習區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (highscore >= 250 && highscore < 400)
        {
            _level = 2;
        }
        else if (highscore >= 400)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        else {
            return null;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }



    /// <summary>
    /// badge3練習進步獎章
    /// </summary>
    /// <param name="improveCounts">當前進步總次數</param>
    string setBadgeLearningImprove(int improveCounts)
    {
        levelVal += 35.3f;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge3");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        switch (improveCounts)
        {
            case 2:
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning");//個人學習區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

                _level = 1;
                break;
            case 4:
                _level = 2;
                break;
            case 6:
                _state = "Congratulation! You get the golden badge!";
                _level = 3;
                break;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }

    /// <summary>
    /// badge4練習累積答對題數獎章
    /// </summary>
    /// <param name="correct">累積正確題數</param>
    string setBadgeLearningCorrect(int correct)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge4");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (correct >= 15 && correct < 30)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning");//個人學習區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();
            }
            _level = 1;
        }
        else if (correct >= 30 && correct < 60)
        {
            _level = 2;
        }
        else if (correct >= 60)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }


    /// <summary>
    /// badge5練習最高連續答對獎章
    /// </summary>
    /// <param name="max_correct">連續答對題數</param>
    string setBadgeLearningMaxCorrect(int max_correct)
    {
        levelVal += 10.2f* max_correct;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge5");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (max_correct >= 7 && max_correct < 14)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (max_correct >= 14 && max_correct < 20)
        {
            _level = 2;
        }
        else if (max_correct >= 20)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }
    /// <summary>
    /// badge6對戰答題迅速獎章
    /// </summary>
    /// <param name="spendtime">答題時間</param>
    string setBadgeCompeteSpendtime(int spendtime)
    {

        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge9");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        string _state = null;
        if (spendtime <= 10 && spendtime > 8)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();
            }
            _level = 1;
        }
        else if (spendtime <= 8 && spendtime > 5)
        {
            _level = 2;
        }
        else if (spendtime <= 5)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }



    /// <summary>
    /// badge7對戰次數獎章
    /// </summary>
    /// <param name="competeCounts">當前對戰總次數</param>
    string setBadgeCompeteCounts(int competeCounts)
    {
        levelVal += 36f;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge7");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (competeCounts > 0 && competeCounts < 5)
        {
            if (attribute.Value == "0")
            {//首次獲得
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (competeCounts >= 5 && competeCounts < 8)
        {
            _level = 2;
        }
        else if (competeCounts >= 8)
        {
            if (attribute.Value == "2")//金牌達標
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }


    /// <summary>
    /// badge8對戰累積答對題數獎章
    /// </summary>
    /// <param name="correct">累積答對</param>
    string setBadgeCompeteCorrect(int correct)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge8");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (correct >= 15 && correct < 30)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (correct >= 30 && correct < 60)
        {
            _level = 2;
        }
        else if (correct >= 60)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }


    /// <summary>
    /// badge9對戰最高連續答對獎章
    /// </summary>
    /// <param name="max_correct">連續答對題數</param>
    string setBadgeCompeteMaxCorrect(int max_correct)
    {
        levelVal += 13.6f* max_correct;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge9");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (max_correct >= 2 && max_correct < 4)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (max_correct >= 4 && max_correct < 8)
        {
            _level = 2;
        }
        else if (max_correct >= 8)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }

    /// <summary>
    /// badge10對戰分數達標獎章
    /// </summary>
    /// <param name="highscore">最高分數</param>
    string setBadgeCompeteHighScore(int highscore) {
        levelVal += 54f;
        setLevel();

        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge10");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        string _state = null;

        if (highscore >= 60 && highscore < 120)
        {
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (highscore >= 120 && highscore < 200)
        {
            _level = 2;
        }
        else if (highscore >= 200)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            _level = 3;
        }

        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }

    /// <summary>
    /// badge11對戰進步獎章
    /// </summary>
    /// <param name="improveCounts">當前進步總次數</param>
    string setBadgeCompeteImprove(int improveCounts)
    {
        levelVal += 44.7f;
        setLevel();
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge11");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        switch (improveCounts)
        {
            case 2:
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

                _level = 1;
                break;
            case 4:
                _level = 2;
                break;
            case 6:
                _state = "Congratulation! You get the golden badge!";
                _level = 3;
                break;
        }
        attribute.Value = _level.ToString();
        saveData();
        return _state;
    }



    /// <summary>
    /// badge12對戰回合排名獎章
    /// </summary>
    /// <param name="rank">排名</param>
    string setBadgeCompeteRank(int rank)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge12");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        string _state = null;
        int _level = 0;
        if (rank==3)
        {
            levelVal += 20f;
            if (attribute.Value == "0")
            {
                _state = "Get the new badge!";

                XmlNode node2 = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete");//同儕對戰區獎章+1
                XmlElement element2 = (XmlElement)node2;
                XmlAttribute attr_count = element2.GetAttributeNode("count");
                int count = XmlConvert.ToInt32(attr_count.Value);
                count = count + 1;
                attr_count.Value = count.ToString();

            }
            _level = 1;
        }
        else if (rank==2)
        {
            levelVal += 30f;
            _level = 2;
        }
        else if (rank==1)
        {
            if (attribute.Value == "2")
            {
                _state = "Congratulation! You get the golden badge!";
            }
            levelVal += 40f;
            _level = 3;
        }
        attribute.Value = _level.ToString();
        setLevel();
        saveData();
        return _state;
    }


}

