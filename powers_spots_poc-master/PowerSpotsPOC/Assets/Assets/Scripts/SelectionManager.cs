using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parse;

public class SelectionManager : Singleton<SelectionManager> {
	public static int DISABLED = 9;
	public static int ENABLED = 0;
	public GameObject buttonPrefab;
	public GameObject goButton;
	private TeamColorButton currentSelection;
	private Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>(5);
	private DateTime? lastUpdatedTime;
	private Vector3[] buttonPositions = new Vector3[]{new Vector3(-3,2,0), new Vector3(-3,0,0), new Vector3(-3,-2,0), new Vector3(0,1,0), new Vector3(0,-1,0)};
	
	protected SelectionManager(){}
		
	// Use this for initialization
	void Start () {
		//TODO figure out why the Singleton is getting destroyed without this
		//according to the docs and the community it should already be done
		DontDestroyOnLoad(this);
		goButton.layer = DISABLED;
		StartCoroutine(InitButtons());
	}
	
	private IEnumerator InitButtons(){
		var query = ParseObject.GetQuery("Color").Limit(5).FindAsync();
		while(!query.IsCompleted) yield return null;
		IEnumerable<ParseObject> results = query.Result;
		int index = 0;
		foreach(ParseObject color in results){
			addButton(color, buttonPositions[index]);
			DateTime? updatedAt = color.UpdatedAt;
			lastUpdatedTime = updatedAt > lastUpdatedTime || lastUpdatedTime == null ? updatedAt: lastUpdatedTime;
			index++;
		}
		StartCoroutine(TeamStatusManager.Instance.InitStatus());
	}
	
	private void addButton(ParseObject color, Vector3 position){
		Color colorValue = ParseUtil.GetColor(color);
		GameObject go = (GameObject) Instantiate(buttonPrefab, position, Quaternion.identity);
		go.renderer.material.color = colorValue;
		TeamColorButton button = (TeamColorButton) go.GetComponent<TeamColorButton>();
		button.SelectedLight.color = colorValue;
		button.ParseObject = color;
		string id = color.ObjectId;
		buttons.Add(id, go);
		if("available".Equals(color.Get<string>("team"))){	
			EnableButton(id);	
		}
		else{
			DisableButton(id);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void UpdateSelection(TeamColorButton newSelection){
		if(newSelection == null){
			return;
		}
		if(currentSelection != null){
			currentSelection.SelectedLight.intensity = 0;
		}
		if(currentSelection == newSelection){
			currentSelection = null;
			goButton.layer = DISABLED;
			return;
		}
		newSelection.SelectedLight.intensity = 8;
		currentSelection = newSelection;
		goButton.layer = ENABLED;
	}

	public void DisableButton(string id){
		GameObject go;
		buttons.TryGetValue(id, out go);
		go.layer = DISABLED;
	}
	
	public void DisableAllButtons(){
		foreach(string key in buttons.Keys){
			DisableButton(key);
		}
		goButton.layer = DISABLED;
	}
	
	public void EnableButton(string id){
		GameObject go;
		buttons.TryGetValue(id, out go);
		go.layer = ENABLED;
	}
	
	public TeamColorButton GetCurrentSelection(){
		return currentSelection;	
	}
	
	public GameObject[] GetAllButtons(){
		return buttons.Values.ToArray();	
	}
}
