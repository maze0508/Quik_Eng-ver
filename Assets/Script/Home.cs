using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour {

    Button btn_practice, btn_compete,btn_exit;
    Button btn_LPractice, btn_LCompete;//排行榜
    AudioSource ClickBtn;

    Xmlprocess xmlprocess;
    public static bool showAchieve;

    void Awake()
    {
        showAchieve = true;
    }
    void Start () {
        xmlprocess = new Xmlprocess();
        ClickBtn = GetComponents<AudioSource>()[1];
        btn_practice = GetComponentsInChildren<Button>()[0];
        btn_compete = GetComponentsInChildren<Button>()[1];
        btn_LPractice = GetComponentsInChildren<Button>()[2];
        btn_LCompete = GetComponentsInChildren<Button>()[3];
        btn_exit = GetComponentsInChildren<Button>()[4];

        btn_practice.onClick.AddListener(goPractice);
        btn_compete.onClick.AddListener(goCompete);
        btn_LPractice.onClick.AddListener(delegate() { showLeaderboard(0); });
        btn_LCompete.onClick.AddListener(delegate () { showLeaderboard(1); });
        btn_exit.onClick.AddListener(UploadData);

        /*//必須先完成練習1次才可以進入對戰區
        if (!xmlprocess.getLearningCount())
        {
            btn_compete.interactable = false;
            btn_compete.image.color = Color.gray;
        }
        else {
            btn_compete.interactable = true;
            btn_compete.onClick.AddListener(goCompete);
        }
        */

    }

    void goPractice() {
        ClickBtn.Play();
        //xmlprocess.New_timeHistoryRecord(levelName + "_Practice", System.DateTime.Now.ToString("HH-mm-ss"));
        xmlprocess.ScceneHistoryRecord( "Learning", DateTime.Now.ToString("HH:mm:ss"));
        SceneManager.LoadScene("LearningArea");
    }

    void goCompete()
    {
        ClickBtn.Play();
        //xmlprocess.New_timeHistoryRecord(levelName + "_Compete", System.DateTime.Now.ToString("HH-mm-ss"));
        xmlprocess.ScceneHistoryRecord( "Compete", DateTime.Now.ToString("HH:mm:ss"));
        SceneManager.LoadScene("CompeteArea");
    }

    void showLeaderboard(int i) {
        switch (i) {
            case 0:
                break;
            case 1:
                break;
        }
    }

    void UploadData() {
        ClickBtn.Play();
        gameObject.AddComponent<UpdateSQL>();
    }
}
