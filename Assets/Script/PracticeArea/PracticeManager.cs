using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class PracticeManager {
    private string serverlink = "http://140.115.126.137/Quik_E/";
    HttpWebRequest request;
    Xmlprocess xmlprocess;
    public Dictionary<int, string> E_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文單字
    public Dictionary<int, string> T_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文中譯

    public PracticeManager() {
        xmlprocess = new Xmlprocess();
    }

    public IEnumerator LoadVocabulary(string fileName)
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("action", fileName);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        string[] tmp, tmp2;
        if (reg.error == null)
        {
            tmp = reg.text.Split(';');//最後一個是空的
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                tmp2 = tmp[i].Split(',');

                E_vocabularyDic.Add(i, tmp2[0]);
                T_vocabularyDic.Add(i, tmp2[1]);
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    ///<summary>
    ///將題目用索引值亂數重新排序
    ///</summary>
    public int[] randomQuestion() {

        int randomindex = 0, dicLength = E_vocabularyDic.Count;
        int[] i_indexRand = new int[dicLength];
        for (int i = 0; i < dicLength; i++)
        {
            i_indexRand[i] = i;
        }
        int tmp =0;
        //亂數排列key(0~dicLength)
        for (int i = 0; i < i_indexRand.Length; i++)
        {
            randomindex = UnityEngine.Random.Range(i, i_indexRand.Length- 1);
            tmp = i_indexRand[randomindex];
            i_indexRand[randomindex] = i_indexRand[i];
            i_indexRand[i] = tmp;
        }
        return i_indexRand;
    }

    ///<summary>
    ///根據選項數量進行n次亂數排列，randomOption[0]為正解(correctID)
    ///</summary>

    public int[] randomOption(int optionCount,int correctID)
    {
        int randomindex = 0, dicLength = T_vocabularyDic.Count;
        int[] i_indexRand = new int[dicLength];
        for (int i = 0; i < dicLength; i++)
        {
            //將正確答案ID移到陣列第一個
            if (i == correctID)
            {
                i_indexRand[0] = correctID;
                i_indexRand[i] = 0;
            }
            else
            {
                i_indexRand[i] = i;
            }
        }
        //將正確答案ID剔除後,進行optionCount-1次亂數排序
        int tmp = 0;
        for (int i = 1; i < optionCount; i++)
        {
            randomindex = UnityEngine.Random.Range(i, i_indexRand.Length - 1);
            tmp = i_indexRand[randomindex];
            i_indexRand[randomindex] = i_indexRand[i];
            i_indexRand[i] = tmp;
        }
        return i_indexRand;
    }

    /// <summary>
    /// 取得單字預習次數
    /// </summary>
    public bool getReviewCount()
    {
       return xmlprocess.getReviewCount();
    }
    /// <summary>
    /// 新增回合單字練習紀錄
    /// </summary>
    public void startLeaning() {
        xmlprocess.createLearningRecord();
    }
    /// <summary>
    /// 更新回合單字練習不同題型的成績紀錄
    /// </summary>
    public void setLearningTypeScore(string typeName,int score)
    {
        xmlprocess.setLearningTypeScoreRecord(typeName,score);
    }


    /// <summary>
    /// 更新單字總練習次數
    /// </summary>
    /// <param name="eventname">要更新的attribute</param>
    public string setLearningCount(string eventname) {
        return xmlprocess.setLearningCount(eventname);
    }

    /// <summary>
    /// 回合單字練習的成績紀錄 0:highscore;1:improve
    /// </summary>
    public string[] setLearningScore(int score)
    {
        return xmlprocess.setLearningScoreRecord(score);
    }

    /// <summary>
    /// 一回合答對題數
    /// </summary>
    public string setLearningCorrect(int correctCount,int worngCount)
    {
        return xmlprocess.setLearningCorrect(correctCount, worngCount);
    }

    /// <summary>
    /// 連續答對題數
    /// </summary>
    public string setLearningMaxCorrect(int max_correctCount)
    {
        return  xmlprocess.setLearningMaxCorrect(max_correctCount);
    }

}
