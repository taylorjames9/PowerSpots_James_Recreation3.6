using UnityEngine;
using System.Collections;

public class ResetButton : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//TODO update for touchpad
	public void OnMouseOver(){
	   if(Input.GetMouseButtonDown(0)){
			TeamStatusManager.Instance.ResetTeamSelections();
		}
	}
}
