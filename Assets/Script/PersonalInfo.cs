using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInfo : MonoBehaviour {
    Xmlprocess xmlprocess;
    string []userInfo;
	public Text userName,level;
    public Image userImg;
    public Button btn_achievement, btn_photo;
    public GameObject AchieveUI, PhotoUI;

    AudioSource ClickBtn;
    #region photo
    Button[] btn_photoCollect;
    Button btn_Backphoto;
    Sprite[] photo;
    Sprite currentPhoto;
    #endregion

    void Start(){
        xmlprocess = new Xmlprocess ();
        userInfo = xmlprocess.getUserInfo();
        userName.text = userInfo[1];
        level.text = userInfo[2];
        photo = Resources.LoadAll<Sprite>("Image/Main/Photo");
        switch (userInfo[3]) {
            case "0":
                userImg.sprite = photo[0];
                break;
            case "1":
                userImg.sprite = photo[2];
                break;
        }
        ClickBtn = GetComponent<AudioSource>();
        btn_photo.onClick.AddListener(clickshowPhotoUI);
        btn_achievement.onClick.AddListener(clickshowAchievementUI);

        //如果初次進入主畫面，顯示成就UI
        if (Home.showAchieve)
        {
            showAchievementUI();
            Home.showAchieve = false;
        }


        //如果完成練習，顯示成就UI
        if (PracticeView.showAchieve)
        {
            Debug.Log(Xmlprocess.levelVal);
            showAchievementUI();
            PracticeView.showAchieve = false;
        }
        //如果離開對戰畫面，顯示成就UI
        if (Btn_BackEvent.showAchieve)
        {
            showAchievementUI();
            Btn_BackEvent.showAchieve = false;
        }
    }

    void clickshowPhotoUI()
    {
        ClickBtn.Play();
        PhotoUI.SetActive(true);
        btn_photoCollect = new Button[6];
        for (int i = 0; i < btn_photoCollect.Length; i++) {
            btn_photoCollect[i] = PhotoUI.GetComponentsInChildren<Button>()[i+1];
        }
        btn_Backphoto = PhotoUI.GetComponentsInChildren<Button>()[0];
        btn_photoCollect[0].onClick.AddListener(delegate() { choosePhoto(0); });
        btn_photoCollect[1].onClick.AddListener(delegate () { choosePhoto(1); });
        btn_photoCollect[2].onClick.AddListener(delegate () { choosePhoto(2); });
        btn_photoCollect[3].onClick.AddListener(delegate () { choosePhoto(3); });
        btn_photoCollect[4].onClick.AddListener(delegate () { choosePhoto(4); });
        btn_photoCollect[5].onClick.AddListener(delegate () { choosePhoto(5); });
        btn_Backphoto.onClick.AddListener(closePhotoUI);
    }

    void choosePhoto(int photoNum) {
        currentPhoto = btn_photoCollect[photoNum].GetComponent<Image>().sprite;
        userImg.sprite = currentPhoto;
        closePhotoUI();
    }

    void closePhotoUI()
    {
        PhotoUI.SetActive(false);
    }

    void clickshowAchievementUI()
    {
        ClickBtn.Play();
        xmlprocess.setTouchACount("clickcount");
        AchieveUI.SetActive(true);
    }

    void showAchievementUI()
    {
        AchieveUI.SetActive(true);
    }

}
