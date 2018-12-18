using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour {

    public GameObject LearningPanel, CompetePanel,LearningState, CompeteState, LearningBadge, CompeteBadge;
    public Button btn_learningBadge, btn_competeBadge,btn_close;
    int learningBadgeCount, competeBadgeCount;
    GameObject[] g_badgeDescription ;
    AudioSource ClickBtn;

    string[] s_LearningState, s_CompeteState;
    public static string[]badgeName;
    string [,] badgeDesciption;

    Xmlprocess xmlprocess;
    private string serverlink = "http://140.115.126.167/Quik_E/";
    private string fileName = "getBadgeDescription.php";

    private int[] _targetLevel;//存取X區所有獎章的階段
    private Button[] _badgeBtn;//存取X區所有獎章按鈕物件


    private void Awake()
    {

        learningBadgeCount = 5;//設定練習區的獎章數量
        competeBadgeCount = 7;//設定對戰區的獎章數量

        xmlprocess = new Xmlprocess();
        badgeName = new string[learningBadgeCount+ competeBadgeCount];
        badgeDesciption = new string[(learningBadgeCount + competeBadgeCount),4];
        s_LearningState = xmlprocess.getAchieveLearningState();
        s_CompeteState = xmlprocess.getAchieveCompeteState();
        StartCoroutine(LoadBadgeData());
    }

    void Start() {
        g_badgeDescription = GameObject.FindGameObjectsWithTag("badgeDescription");//抓取所有獎章描述的物件
        for (int i = 0; i < g_badgeDescription.Length; i++) {
            g_badgeDescription[i].gameObject.SetActive(false);//隱藏全部
        }
        LearningPanel.SetActive(true);
        CompetePanel.SetActive(false);
        ClickBtn = GetComponent<AudioSource>();

        StartCoroutine(getLearningBadges());//預設顯示學習區
        btn_learningBadge.onClick.AddListener(delegate () { StartCoroutine(getLearningBadges()); });
        btn_competeBadge.onClick.AddListener(getCompeteBadges);
        btn_close.onClick.AddListener(closeAchieveUI);

        for (int i = 0; i < s_LearningState.Length; i++)
        {
            LearningState.GetComponentsInChildren<Text>()[i].text = s_LearningState[i];
            CompeteState.GetComponentsInChildren<Text>()[i].text = s_CompeteState[i];
        }
    }


    public IEnumerator getLearningBadges() {
        ClickBtn.Play();
        yield return new WaitForSeconds(0.1f);

        LearningPanel.SetActive(true);
        CompetePanel.SetActive(false);

        Text _badgeName;//獎章名稱
        _badgeBtn = new Button[learningBadgeCount];
        _targetLevel = xmlprocess.getAchieveLearningBadges(learningBadgeCount);

        for (int i = 1; i <= learningBadgeCount; i++) {

            _badgeBtn[i - 1] = LearningBadge.GetComponentsInChildren<Button>()[i - 1];//存取學習區的所有獎章物件
            _badgeName = _badgeBtn[i - 1].GetComponentsInChildren<Text>()[0];//獎章名稱
            _badgeName.text = badgeName[i - 1];//因為陣列索引直起始為0,故要減1

            switch (_targetLevel[i - 1]-1)//當前階段
            {
                case 0:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.Load<Sprite>("Image/Main/null");
                    break;
                case 1:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.LoadAll<Sprite>("Image/Main/bg")[2];
                    _badgeBtn[i - 1].GetComponentsInChildren<Image>()[1].color = Color.white;
                    break;
                case 2:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.LoadAll<Sprite>("Image/Main/bg")[1];
                    _badgeBtn[i - 1].GetComponentsInChildren<Image>()[1].color = Color.white;

                    break;
                case 3:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.LoadAll<Sprite>("Image/Main/bg")[0];
                    _badgeBtn[i - 1].GetComponentsInChildren<Image>()[1].color = Color.white;

                    break;
            }
        }

        _badgeBtn[0].onClick.AddListener(delegate () { showDescription(1, _targetLevel[0]); });
        _badgeBtn[1].onClick.AddListener(delegate () { showDescription(2, _targetLevel[1]); });
        _badgeBtn[2].onClick.AddListener(delegate () { showDescription(3, _targetLevel[2]); });
        _badgeBtn[3].onClick.AddListener(delegate () { showDescription(4, _targetLevel[3]); });
        _badgeBtn[4].onClick.AddListener(delegate () { showDescription(5, _targetLevel[4]); });
    }


    void getCompeteBadges()
    {
        ClickBtn.Play();
        LearningPanel.SetActive(false);
        CompetePanel.SetActive(true);

        Text _badgeName;//獎章名稱
        _badgeBtn = new Button[competeBadgeCount];
        _targetLevel = xmlprocess.getAchieveCompeteBadges(learningBadgeCount+1, learningBadgeCount+competeBadgeCount);

        for (int i = 1; i <= competeBadgeCount; i++)
        {
            _badgeBtn[i - 1] = CompeteBadge.GetComponentsInChildren<Button>()[i - 1];//存取學習區的所有獎章物件
            _badgeName = _badgeBtn[i - 1].GetComponentsInChildren<Text>()[0];//獎章名稱
            _badgeName.text = badgeName[(learningBadgeCount-1)+i];//因為陣列0~learningBadgeCount-1存放值為學習區的Badge名稱，因此在此起始值為learningBadgeCount

            switch (_targetLevel[i - 1] - 1)//當前階段
            {
                case 0:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.Load<Sprite>("Image/Main/null");
                    break;
                case 1:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.LoadAll<Sprite>("Image/Main/bg")[5];
                    _badgeBtn[i - 1].GetComponentsInChildren<Image>()[1].color = Color.white;
                    break;
                case 2:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.LoadAll<Sprite>("Image/Main/bg")[4];
                    _badgeBtn[i - 1].GetComponentsInChildren<Image>()[1].color = Color.white;

                    break;
                case 3:
                    _badgeBtn[i - 1].image.sprite = _badgeBtn[i - 1].image.sprite = Resources.LoadAll<Sprite>("Image/Main/bg")[3];
                    _badgeBtn[i - 1].GetComponentsInChildren<Image>()[1].color = Color.white;

                    break;
            }
        }
        _badgeBtn[0].onClick.AddListener(delegate () { showDescription(6, _targetLevel[0]); });
        _badgeBtn[1].onClick.AddListener(delegate () { showDescription(7, _targetLevel[1]); });
        _badgeBtn[2].onClick.AddListener(delegate () { showDescription(8, _targetLevel[2]); });
        _badgeBtn[3].onClick.AddListener(delegate () { showDescription(9, _targetLevel[3]); });
        _badgeBtn[4].onClick.AddListener(delegate () { showDescription(10, _targetLevel[4]); });
        _badgeBtn[5].onClick.AddListener(delegate () { showDescription(11, _targetLevel[5]); });
        _badgeBtn[6].onClick.AddListener(delegate () { showDescription(12, _targetLevel[6]); });


    }
    public IEnumerator LoadBadgeData() {
        WWWForm phpform = new WWWForm();
        phpform.AddField("action", fileName);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        string[] _tmp, _tmp1, _tmp2;
        string _des = "";
        if (reg.error == null)
        {
            _tmp = reg.text.Split('/');//將回傳字串依據badge劃分,最後一個是空的(ex:1,完成練習,完成練習1次;2,完成練習,完成練習5次;3,完成練習,完成練習10次/....)
            for (int i = 0; i < _tmp.Length - 1; i++)
            {
                _tmp1 = _tmp[i].Split(';');//將同一個badge分成3個階段(ex:1,完成練習,完成練習1次)
                _tmp2 = _tmp1[0].Split(',');//將同一個階段分成3階段與敘述
                badgeName[i] = _tmp2[1];
                badgeDesciption[i,0] = "Not yet:\n"+_tmp2[2];//目標第一階段

                _des = _tmp2[2];
                _tmp2 = _tmp1[1].Split(',');
                badgeDesciption[i,1] = "Have already" + _des+"\nNext step:" +_tmp2[2];

                _des = _tmp2[2];
                _tmp2 = _tmp1[2].Split(',');
                badgeDesciption[i,2] = "Have already" + _des + "\nNext step:\n" + _tmp2[2];

                badgeDesciption[i,3] = "All complete!" + _tmp2[2];//已達第三階段

            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

   public  void showDescription(int badgeID,int targetBadgeLevel) {
        //Debug.Log("click badge "+badgeID+" level: "+ targetBadgeLevel);
        ClickBtn.Play();
        Text _badgeDesText;
        showBadgeDes(badgeID);
        _badgeDesText = g_badgeDescription[badgeID - 1].GetComponentInChildren<Text>();
        _badgeDesText.text = badgeDesciption[badgeID - 1, targetBadgeLevel - 1];//敘述內容，badgeDesciption索引值初始為0，故減1
    }

    void showBadgeDes(int badgeID){//顯示點擊的獎章的描述物件
        for (int i = 0; i < g_badgeDescription.Length; i++)
        {
            if (i == badgeID - 1)
            {
                g_badgeDescription[i].gameObject.SetActive(true);
            }
            else
            {
                g_badgeDescription[i].gameObject.SetActive(false);
            }
        }
    }

    void closeAchieveUI() {
        ClickBtn.Play();
        xmlprocess.setTouchACount("showcount");
        showBadgeDes(-1);//隱藏全部
        gameObject.SetActive(false);
    }


}
