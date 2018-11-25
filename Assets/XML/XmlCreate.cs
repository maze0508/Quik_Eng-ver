using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using edu.ncu.list.util;

//原檔名叫做xmlfile
public class XmlCreate{
	public int count = 0;	
	public XmlDocument xmlDoc;

	public XmlCreate(string path,string filename) {
		//檔案不存在，因此初次創建table

		string Strcount = count.ToString();
		xmlDoc = new XmlDocument();
        
        XmlDeclaration xmldecl;
        xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
        XmlElement root = xmlDoc.DocumentElement;
        xmlDoc.InsertBefore(xmldecl, root);
        
        XmlElement Loadfile = xmlDoc.CreateElement("Loadfile");
			xmlDoc.AppendChild(Loadfile);

        /*====================================---基本資料---====================================*/
			XmlElement User = xmlDoc.CreateElement("User");
            Loadfile.AppendChild(User);
            User.SetAttribute("ID", "");
			User.SetAttribute("name","");
			User.SetAttribute("level", "");
            User.SetAttribute("sex", "");

        /*====================================---兩區域獲得的獎章總數量---====================================*/
            XmlElement badge = xmlDoc.CreateElement("badge");
            User.AppendChild(badge);
            badge.SetAttribute("learning_count", Strcount);
            badge.SetAttribute("compete_count", Strcount);

        /*===============================---每一關卡的練習狀況---===============================*/
           XmlElement learning  = xmlDoc.CreateElement("learning");
		    User.AppendChild(learning);
            learning.SetAttribute("review_count", Strcount);//查看單字的次數
            learning.SetAttribute("learning_count", Strcount);//完成練習的次數
            learning.SetAttribute("learning_correct", Strcount);//累積答對題數
            learning.SetAttribute("learning_wrong", Strcount);//累積答錯題數
            learning.SetAttribute("learning_improve", Strcount);//練習進步的次數
            learning.SetAttribute("highscore", Strcount);

        /*===============================---每一關卡的對戰狀況---===============================*/
            XmlElement compete = xmlDoc.CreateElement("compete");
		    User.AppendChild(compete);
            compete.SetAttribute("compete_count", Strcount);//對戰的次數
            compete.SetAttribute("compete_correct", Strcount);//累積答對題數
            compete.SetAttribute("compete_wrong", Strcount);//累積答錯題數
            compete.SetAttribute("compete_improve", Strcount);//對戰進步的次數
            compete.SetAttribute("highscore", Strcount);//對戰的最高分數

        /*====================================---各類獎章獲得紀錄---====================================*/
        XmlElement badge_record = xmlDoc.CreateElement("badge_record");
        Loadfile.AppendChild(badge_record);
        XmlElement badge_learning = xmlDoc.CreateElement("badge_learning");//個人學習區的獎章
        XmlElement badge_compete = xmlDoc.CreateElement("badge_compete");//同儕對戰區的獎章
        badge_record.AppendChild(badge_learning);
        badge_record.AppendChild(badge_compete);
        badge_learning.SetAttribute("count", Strcount);//目前獲得數量
        badge_compete.SetAttribute("count", Strcount);

        ///練習區獎章總共5個，對戰區獎章共7個///
        for (int i = 1; i <=12; i++)
        {
            XmlElement _badge = xmlDoc.CreateElement("badge"+i);
            if (i < 6) {
                badge_learning.AppendChild(_badge);
            }
            else {
                badge_compete.AppendChild(_badge);
            }
            _badge.SetAttribute("level", "0");//獎章目前等級(0:未獲得、1:銅、2:銀、3:金)
        }

        /*====================================---排行榜更新紀錄---====================================*/
        XmlElement rank_history = xmlDoc.CreateElement("rank_history");
        Loadfile.AppendChild(rank_history);
        XmlElement rank_record = xmlDoc.CreateElement("rank_record");
        rank_history.AppendChild(rank_record);
        rank_record.SetAttribute("highscore", "");//目前最高分數
        rank_record.SetAttribute("rank", "");//目前排名
        rank_record.SetAttribute("updateTime", "");//刷新最高分數的時間

        /*====================================---查看排行榜與成就次數---====================================*/
        XmlElement touch_history = xmlDoc.CreateElement("touch_history");
        Loadfile.AppendChild(touch_history);
        XmlElement touch_achieve = xmlDoc.CreateElement("touch_achieve");//查看成就頁面的次數
        touch_history.AppendChild(touch_achieve);
        touch_achieve.SetAttribute("showcount", Strcount);//主動點擊次數
        touch_achieve.SetAttribute("clickcount", Strcount);//畫面顯示次數
        /*
        XmlElement touch_leaderboard = xmlDoc.CreateElement("touch_leaderboard");
        touch_history.AppendChild(touch_leaderboard);
        touch_leaderboard.SetAttribute("count", Strcount);
        */

        /*====================================---Log紀錄---====================================*/
        XmlElement log_record = xmlDoc.CreateElement("log_record");
		    Loadfile.AppendChild(log_record);
            log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));//Log紀錄的日期

            /*-------學習紀錄---------*/
             XmlElement learning_history = xmlDoc.CreateElement("learning_history");
             log_record.AppendChild(learning_history);

        /*
          XmlElement learning_record = xmlDoc.CreateElement("learning_record");
          learning_history.AppendChild(learning_record);
          learning_record.SetAttribute("startTime", "");
          learning_record.SetAttribute("endTime", "");
          learning_record.SetAttribute("score", "");//本次練習分數
       */

        /*-------對戰紀錄---------*/
        XmlElement compete_history = xmlDoc.CreateElement("compete_history");
            log_record.AppendChild(compete_history);

        /*
        XmlElement compete_record = xmlDoc.CreateElement("compete_record");
        compete_history.AppendChild(compete_record);
        compete_record.SetAttribute("startTime", "");//對戰開始時間
        compete_record.SetAttribute("endTime", "");
        compete_record.SetAttribute("duration", "");
        compete_record.SetAttribute("score", "");//本次對戰分數
        compete_record.SetAttribute("rank", "");//本次對戰排名
        */
        /*-------當前對戰的回合紀錄---------*/
        /*
        XmlElement round_record = xmlDoc.CreateElement("round_record");
        compete_record.AppendChild(round_record);
        round_record.SetAttribute("ques_id", "");//題號
        round_record.SetAttribute("ans_state", "");//作答正確或錯誤
        round_record.SetAttribute("duration", "");//作答時間
        round_record.SetAttribute("rank", "");//當回合的排名
        */

        /*-------場景載入與離開紀錄---------*/
        XmlElement scene_history = xmlDoc.CreateElement("scene_history");
        log_record.AppendChild(scene_history);

        /*
        XmlElement scene_record = xmlDoc.CreateElement("scene_record");
        scene_history.AppendChild(scene_record);
        scene_record.SetAttribute("scene", "");
        scene_record.SetAttribute("startTime", "");
        */

        xmlDoc.Save(path+ filename);//存檔
		}
}