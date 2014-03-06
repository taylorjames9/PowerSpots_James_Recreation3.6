using UnityEngine;
using System.Collections;

public class SpotChanger : MonoBehaviour {
	public string id;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//TODO update for touchpad
	public void OnMouseOver(){
	   if(Input.GetMouseButtonDown(0)){
			Color color = ParseUtil.GetColor(SelectionManager.Instance.GetCurrentSelection().ParseObject);
			SpotManager.updateSpot(id, color);
		}
	}
}
