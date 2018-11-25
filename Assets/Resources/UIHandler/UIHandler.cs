using UnityEngine;

using System.Collections;


public class UIHandler : MonoBehaviour {



    void Awake(){
	
        UIManager.Instance.CanvasObj = gameObject;
    }
}