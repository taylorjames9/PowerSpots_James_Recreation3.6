using UnityEngine;
using System.Collections;
using Parse;

public class TeamColorButton : MonoBehaviour {
	public Light SelectedLight;
	public ParseObject ParseObject {get; set;}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void OnMouseOver(){
		if(Input.GetMouseButtonDown(0) && gameObject.layer != SelectionManager.DISABLED){
			SelectionManager.Instance.UpdateSelection(this);
		}
	}
}
