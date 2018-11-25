using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PracticeView : MonoBehaviour {

    public GameObject UI_showAnsfeedback, UI_ShowMes,score;
    Text text_score;
    AudioSource ClickBtn;
    int vocabularyID,totalQuesNum, C_correctNum,max_correctNum, correctNum, wrongNum;
    static int p_score;
    public static bool showAchieve;
    string []achievementState;
    PracticeManager pm;

    #region ReviewVocabulary UI
    Text text_English,text_Translation;
    Button btn_pronun,btn_pre, btn_next, btn_gotonext, btn_skip;
    AudioSource VocabularyAS;
    #endregion

    #region PracticeMuitiselect UI
    Text text_totalQues,text_Question;
    Button[] btn_option;
    int quesID,correctOption;//quesID:題數;correctOption:正確的選項編號
    int[] randomQuestion, randomOption;//隨機排列後的題目與選項
    #endregion

    #region PracticeCompose UI
    Button btn_alphabet,btn_clear,btn_submit;
    Text text_quescontent;
    GameObject[] CollectBtnObj;
    Color c_original;
    string userAns;
    #endregion

    #region Result UI
    Text text_correct,text_wrong;
    #endregion

    private void Awake()
    {
        achievementState = new string[5];//學習區獎章數量
        pm = new PracticeManager();
    }

    void Start () {
        text_score = score.GetComponentsInChildren<Text>()[0];
        ClickBtn = GetComponentsInChildren<AudioSource>()[0];
        p_score = 0;
        vocabularyID = 0;
        C_correctNum = 0;//當前連續答對題數
        max_correctNum = -1;//最大連續答對數
        correctNum = 0;//累計正確題數
        wrongNum = 0;//累計錯誤題數
        totalQuesNum = 10;//練習題數
        showAchieve = false;
        StartCoroutine(showReviewVocabulary());
        UIManager.Instance.CloseAllPanel();

        showReviewUI();
    }
    #region Review function
    
    void showReviewUI()
    {
        UIManager.Instance.ShowPanel("P_ReviewUI");

        text_English = GetComponentsInChildren<Text>()[0];
        text_Translation = GetComponentsInChildren<Text>()[1];
        btn_pronun = GetComponentsInChildren<Button>()[1];
        btn_pre = GetComponentsInChildren<Button>()[2];
        btn_next = GetComponentsInChildren<Button>()[3];
        btn_gotonext = GetComponentsInChildren<Button>()[4];
        btn_skip = GetComponentsInChildren<Button>()[5];
        btn_gotonext.gameObject.SetActive(false);
        VocabularyAS = GetComponentsInChildren<AudioSource>()[2];
        btn_pronun.onClick.AddListener(delegate () { playAudio(vocabularyID); });
        btn_pre.onClick.AddListener(delegate () { changeVocabularyID(-1); });
        btn_next.onClick.AddListener(delegate () { changeVocabularyID(1); });
        btn_skip.onClick.AddListener(skip);

        if (!pm.getReviewCount())
        {
            btn_skip.gameObject.SetActive(false);
        }
        else {
            btn_skip.gameObject.SetActive(true);
        }
    }


    IEnumerator showReviewVocabulary(){
        StartCoroutine(pm.LoadVocabulary("loadVocabulary"));
        yield return new WaitForSeconds(0.2f);
        changeVocabularyID(0);
    }

    void changeVocabularyID(int count) {
        ClickBtn.Play();
        if (vocabularyID >= 0 && (vocabularyID + count)>=0)
        {
            if (pm.E_vocabularyDic.ContainsKey(vocabularyID + count))
            {
                vocabularyID += count;
                playAudio(vocabularyID);
                btn_gotonext.gameObject.SetActive(false);
                text_English.text = pm.E_vocabularyDic[vocabularyID];
                text_Translation.text = pm.T_vocabularyDic[vocabularyID];
            }
            else
            {
                string _state = pm.setLearningCount("review_count");//更新單字瀏覽次數
                btn_gotonext.gameObject.SetActive(true);
                btn_gotonext.onClick.AddListener(showPracticeUI);
            }
        }
    }

    void playAudio(int _vocabularyID) {
        ClickBtn.Play();
        VocabularyAS.clip = Resources.Load("Sound/" + pm.E_vocabularyDic[_vocabularyID], typeof(AudioClip)) as AudioClip;
        VocabularyAS.Play();
    }

    void skip()
    {
        ClickBtn.Play();
        showPracticeUI();
    }
    #endregion

    #region PracticeMuitiselect function

    void showPracticeUI()
    {
        pm.startLeaning();//創建單字練習紀錄
        btn_option = new Button[4];
        UIManager.Instance.TogglePanel("P_ReviewUI",false);
        if (!UIManager.Instance.IsUILive("P_PracticeUI"))
        {
            UIManager.Instance.ShowPanel("P_PracticeUI");
        }
        score.SetActive(true);
        VocabularyAS = GetComponentsInChildren<AudioSource>()[2];
        text_totalQues =  GetComponentsInChildren<Text>()[1];
        text_Question = GetComponentsInChildren<Text>()[2];

        for (int i = 0; i < btn_option.Length; i++)
        {
            btn_option[i] = GetComponentsInChildren<Button>()[i+1];
        }
        c_original = btn_option[0].GetComponent<Image>().color;
        btn_option[0].onClick.AddListener(delegate () {StartCoroutine(compareAns(0,quesID)); });
        btn_option[1].onClick.AddListener(delegate () { StartCoroutine(compareAns(1, quesID)); });
        btn_option[2].onClick.AddListener(delegate () { StartCoroutine(compareAns(2, quesID)); });
        btn_option[3].onClick.AddListener(delegate () { StartCoroutine(compareAns(3, quesID)); });
        initialQuestion();
    }

    void initialQuestion() {
        quesID = 0;
        randomQuestion = pm.randomQuestion();
        showPracticeQues(quesID);
    }

    void showPracticeQues(int quesID) {//更新每回合的題目與選項
        //Debug.Log("題號"+ quesID);
        playAudio(randomQuestion[quesID]);
        text_totalQues.text = (quesID+1).ToString()+"/"+ totalQuesNum;
        text_Question.text = pm.E_vocabularyDic[randomQuestion[quesID]];
        showPracticeOption(randomQuestion[quesID]);
    }

    void showPracticeOption(int correctOptID)
    {
        randomOption = pm.randomOption(4, correctOptID);
        correctOption = UnityEngine.Random.Range(0, btn_option.Length-1);//隨機選擇正確答案的位置
        //Debug.Log("正確選項ID"+correctOption);
        for (int i = 0, randomOptionIndex = 1; i < btn_option.Length; i++)
        {
            if (i == correctOption)
            {
                btn_option[i].GetComponentInChildren<Text>().text = pm.T_vocabularyDic[correctOptID];
            }
            else
            {
                btn_option[i].GetComponentInChildren<Text>().text = pm.T_vocabularyDic[randomOption[randomOptionIndex]];
                randomOptionIndex++;
            }
        }
    }

    IEnumerator compareAns(int optionID,int _quesID) {
        ClickBtn.Play();
        if (_quesID == quesID)
        {
            if (correctOption.Equals(optionID))//答對
            {
                btn_option[optionID].GetComponent<Button>().interactable = false;//避免重複點擊,增加分數
                StartCoroutine(showfeedback(0));
                p_score += (int)(p_score *0.2+ max_correctNum*0.3+correctNum*0.15) + 3;
                text_score.text = p_score.ToString();
            }
            else//答錯
            {
                btn_option[correctOption].GetComponent<Button>().interactable = false;//避免重複點擊,增加分數
                btn_option[correctOption].GetComponent<Image>().color = Color.red;
                StartCoroutine(showfeedback(1));
            }

            yield return new WaitForSeconds(0.5f);
            resetButton(optionID);
            checkNextQues(_quesID, "practice");
        }
    }
    //重設按鈕
    void resetButton(int optionID) {
        btn_option[optionID].GetComponent<Button>().interactable = true;
        btn_option[correctOption].GetComponent<Button>().interactable = true;
        btn_option[correctOption].GetComponent<Image>().color = c_original;
    }

    IEnumerator PracticeEnd()
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.TogglePanel("P_PracticeUI", false);
        if (!UIManager.Instance.IsUILive("P_ComposeUI"))
        {
            UIManager.Instance.ShowPanel("P_ComposeUI");
        }
        showComposeUI();
    }

    #endregion

    #region PracticeCompose function
    void showComposeUI() {

        btn_alphabet = Resources.Load("UI/Btn_Alphabet", typeof(Button)) as Button;
        text_totalQues = GetComponentsInChildren<Text>()[1];
        text_Question = GetComponentsInChildren<Text>()[2];
        text_quescontent = GetComponentsInChildren<Text>()[3];
        VocabularyAS = GetComponentsInChildren<AudioSource>()[2];

        btn_clear = GetComponentsInChildren<Button>()[1];
        btn_submit = GetComponentsInChildren<Button>()[2];
        btn_clear.onClick.AddListener(resetAns);
        btn_submit.onClick.AddListener(delegate () { StartCoroutine(compareComposeAns(quesID)); });

        quesID = 0;
        randomQuestion = pm.randomQuestion();
        showComposeQues(quesID);
    }

    ////刪除所有字母按鈕
    void initialComposeButton(int quesID)
    {
        for (int i = 0; i < CollectBtnObj.Length; ++i)
        {
            if (CollectBtnObj[i] != null)
            {
                Destroy(CollectBtnObj[i].gameObject);
            }
        }
        showComposeQues(quesID);
    }

    //初始化題目
    void showComposeQues(int quesID)
    {
        text_quescontent.text = "";//初始化題目空格
        userAns = "";

        playAudio(randomQuestion[quesID]);
        text_totalQues.text = (quesID + 1).ToString() + "/" + totalQuesNum;
        text_Question.text = pm.T_vocabularyDic[randomQuestion[quesID]];
        for (int i = 0; i < pm.E_vocabularyDic[randomQuestion[quesID]].Length; i++) {
            text_quescontent.text += "_ ";
        }
        randomSort(randomQuestion[quesID]);
    }

    //重新排列字母
    void randomSort(int index) {
        int random;
        char tmp;
        char []randomAns= pm.E_vocabularyDic[randomQuestion[quesID]].ToCharArray();
        for (int i = 0; i < randomAns.Length; i++) {
            random = Random.Range(i, randomAns.Length-1);
            tmp = randomAns[random];
            randomAns[random] = randomAns[i];
            randomAns[i] = tmp;
        }
        //生成按鈕
        creatAlphabetBtn(randomAns);
    }

    void creatAlphabetBtn(char[] randomAns) {
        int pointer = 0;//當前字母指標
        CollectBtnObj = new GameObject[randomAns.Length];

        while (pointer<randomAns.Length) {
            Button g_btnObj = Instantiate(btn_alphabet);//Options
            g_btnObj.transform.SetParent(GameObject.Find("Content").transform);
            g_btnObj.GetComponentInChildren<Text>().text = randomAns[pointer].ToString();
            g_btnObj.transform.localPosition = new Vector3(80 + pointer * 150, 0.0f, 0.0f);
            g_btnObj.transform.localScale = Vector3.one;
            g_btnObj.name = randomAns[pointer].ToString();
            g_btnObj.onClick.AddListener(() => clickAlphabet(g_btnObj));
            CollectBtnObj[pointer] = g_btnObj.gameObject;
            pointer++;
        }
    }

    void clickAlphabet(Button _trigger)
    {
        userAns += _trigger.name;//將點擊的選項存入usrAns
        setQuesContent(_trigger.name);
        _trigger.gameObject.SetActive(false);
        //Destroy(_trigger.gameObject);//按鈕點擊後消失
    }

    void setQuesContent(string alphabet)
    {
        int underline_index = text_quescontent.text.IndexOf('_');
        //Debug.Log(underline_index);
        if (underline_index != -1)
        {
            text_quescontent.text = text_quescontent.text.Remove(underline_index, 1);
        }
        text_quescontent.text = text_quescontent.text.Insert(underline_index, alphabet);
    }

    void resetAns() {
        ClickBtn.Play();
        initialComposeButton(quesID);
    }

    IEnumerator compareComposeAns(int _quesID) {
        ClickBtn.Play();
        btn_submit.GetComponent<Button>().interactable = false;//避免重複點擊,增加分數
        if (userAns == pm.E_vocabularyDic[randomQuestion[quesID]])
        {
            StartCoroutine(showfeedback(0));
            p_score += (int)(p_score *0.1);
            text_score.text = p_score.ToString();

        }
        else {
            //Debug.Log("你的答案:" + userAns);
            //Debug.Log("正確答案:" + pm.E_vocabularyDic[randomQuestion[quesID]]);
            text_quescontent.text = pm.E_vocabularyDic[randomQuestion[quesID]];
            StartCoroutine(showfeedback(1));
        }
        yield return new WaitForSeconds(0.5f);
        btn_submit.GetComponent<Button>().interactable = true;
        checkNextQues(_quesID, "compose");
    }

    IEnumerator ComposeEnd()
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.TogglePanel("P_ComposeUI", false);
        if (!UIManager.Instance.IsUILive("P_ResultUI"))
        {
            UIManager.Instance.ShowPanel("P_ResultUI");
        }
        showResultUI();
    }
    #endregion

    void showResultUI()
    {
        score.SetActive(false);
        text_score = GetComponentsInChildren<Text>()[0];
        text_correct = GetComponentsInChildren<Text>()[1];
        text_wrong = GetComponentsInChildren<Text>()[2];
        text_score.text = p_score.ToString();
        text_correct.text = correctNum.ToString();
        text_wrong.text = wrongNum.ToString();
        StartCoroutine(LearningEnd());
    }

    IEnumerator LearningEnd() {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.TogglePanel("P_ResultUI",false);
        achievementState[0] = pm.setLearningCount("learning_count");//更新單字練習次數

        string[] s_state = pm.setLearningScore(p_score);//紀錄此次單字練習成績
        if (s_state!=null) {
            if (s_state[0] != null) achievementState[1] = s_state[0];//有達標
            if (s_state[1] != null) achievementState[2] = s_state[1];//分數進步
        }
        if (pm.setLearningCorrect(correctNum, wrongNum) != null) achievementState[3] = pm.setLearningCorrect(correctNum, wrongNum);//更新單字答對與錯誤題數
        if (pm.setLearningMaxCorrect(max_correctNum) != null) achievementState[4] = pm.setLearningMaxCorrect(max_correctNum);//連續答對題數

        /*---顯示獲得獎章與稱號---*/
        for (int i = 0; i < achievementState.Length; i++)
        {
            if (achievementState[i] != null)
            {
                UI_ShowMes.SetActive(true);
                UI_ShowMes.GetComponentInChildren<Text>().text = achievementState[i];
                switch (i)
                {
                    case 0:
                        Achievement.badgeName[0] = "Practice for lnog time";
                        break;
                    case 1:
                        Achievement.badgeName[1] = "High score Master";
                        break;
                    case 2:
                        Achievement.badgeName[2] = "Harder and harder";
                        break;
                    case 3:
                        Achievement.badgeName[3] = "Can't defeat you";
                        break;
                    case 4:
                        Achievement.badgeName[4] = "Many a little makes a mickle";
                        break;
                }
            }
        }
    /*--------------------------*/
    yield return new WaitForSeconds(1f);
        showAchieve = true;
        SceneManager.LoadScene("Home");
        UIManager.Instance.CloseAllPanel();

    }

    void checkNextQues(int _quesID, string functionName)
    {
        if (_quesID == quesID)
        {
            if (quesID >= totalQuesNum-1)
            {
                switch (functionName)
                {
                    case "practice":
                        pm.setLearningTypeScore("option", p_score);
                        StartCoroutine(PracticeEnd());
                        break;
                    case "compose":
                        pm.setLearningTypeScore("cloze", p_score);
                        StartCoroutine(ComposeEnd());
                        //Debug.Log("Learning End");
                        break;
                }
            }
            else
            {
                quesID++;
                switch (functionName)
                {
                    case "practice":
                        showPracticeQues(quesID);
                        break;
                    case "compose":
                        initialComposeButton(quesID);
                        break;
                }
            }
        }
    }



    IEnumerator showfeedback(int _state)
    {
        GameObject fb  = Instantiate(UI_showAnsfeedback);//Options
        fb.transform.SetParent(this.gameObject.transform);
        fb.transform.localPosition = Vector3.zero;
        fb.transform.localScale = Vector3.one;
        Image img_correct = fb.GetComponentsInChildren<Image>()[0];
        Image img_wrong = fb.GetComponentsInChildren<Image>()[1];
        img_correct.gameObject.SetActive(false);
        img_wrong.gameObject.SetActive(false);

        if (_state == 0)//答對
        {
            C_correctNum++;
            correctNum++;
            img_correct.gameObject.SetActive(true);
        }
        else//答錯
        {
            if (C_correctNum >= max_correctNum)
            {
                max_correctNum = C_correctNum;
                C_correctNum = 0;
            }
            wrongNum++;
            img_wrong.gameObject.SetActive(true);

        }
        yield return new WaitForSeconds(0.5f);
        Destroy(fb);
    }

}
