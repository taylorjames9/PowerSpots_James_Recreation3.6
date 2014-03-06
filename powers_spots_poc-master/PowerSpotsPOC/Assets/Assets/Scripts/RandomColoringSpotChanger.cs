using UnityEngine;
using System.Collections;

public class RandomColoringSpotChanger : MonoBehaviour {
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
			Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			SpotManager.updateSpot(id, color);
		}
	}
}
