using UnityEngine;
using System.Collections;

public class GoButton : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//TODO update for touchpad
	public void OnMouseOver(){
	   if(Input.GetMouseButtonDown(0) && gameObject.layer == SelectionManager.ENABLED){
			TeamStatusManager.Instance.SubmitColorSelection();
		}
	}
}
