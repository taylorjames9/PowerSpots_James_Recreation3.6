using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

public class TeamStatusManager : Singleton<TeamStatusManager> {
	private static int TOTAL_TEAMS = 5;
	private static string STATUS_TEXT = " Team - Ready";
	private static float INITIAL_Y_POS = 0.95f;
	private static float Y_INCREMENT = 0.07f;
	private static float X_POS = 0.66f;
	private static float Z_POS = 0f;
	private DateTime? lastUpdatedTime;
	private float nextStatusPosition = INITIAL_Y_POS;
	private string teamID;
	private int readyTeamsCount = 0;
	
	protected TeamStatusManager(){}
		
	// Use this for initialization
	void Start () {
//		StartCoroutine(InitStatus());
//		StartCoroutine(CheckStatus());
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public IEnumerator InitStatus(){
		var query = ParseObject.GetQuery("Color").FindAsync();
		while(!query.IsCompleted) yield return new WaitForSeconds(1f);
		IEnumerable results = query.Result;
		foreach(ParseObject color in results){
			lastUpdatedTime = ParseUtil.GetLatestTime(color, lastUpdatedTime);
			if(!"available".Equals(color.Get<string>("team"))){
				AddTeamStatus(color.Get<string>("name"), ParseUtil.GetColor(color));
			}
		}
		StartCoroutine(CheckStatus());
	}
	
	private IEnumerator CheckStatus(){
	    while(true){
	        yield return new WaitForSeconds(5f);
			var query = ParseObject.GetQuery("Color").WhereGreaterThan("updatedAt", lastUpdatedTime).FindAsync();
			while(!query.IsCompleted) yield return null;
			IEnumerable results = query.Result;
			foreach(ParseObject color in results){
				lastUpdatedTime = ParseUtil.GetLatestTime(color, lastUpdatedTime);
				if(!"available".Equals(color.Get<string>("team"))){
					AddTeamStatus(color.Get<string>("name"), ParseUtil.GetColor(color));
				}
			}
		}
	}
	
	private void AddTeamStatus(string team, Color color){
		GameObject go = new GameObject(team + " Team Status");
		go.transform.position = new Vector3(X_POS, nextStatusPosition, Z_POS);
		GUIText text = (GUIText) go.AddComponent(typeof(GUIText));
		text.text =  team + STATUS_TEXT;
		text.color = color;
		text.fontSize = 22;
		nextStatusPosition -= Y_INCREMENT;
		readyTeamsCount++;
	}
	
	public void SubmitColorSelection(){
		ParseObject color = SelectionManager.Instance.GetCurrentSelection().ParseObject;
		string teamColor = color.Get<string>("name");
		//TODO identify team IDs - currently setting the team name the same as the Color's name
		color["team"] = teamColor;
		var save = color.SaveAsync();
		if(!save.IsFaulted){
			StartCoroutine(DisableButtons());
			StartCoroutine(WaitForOtherTeamsOrStartGame());
		}
		else{
			Debug.Log("Could not claim color: " + save.Exception.Message);	
		}
	}
	
	private IEnumerator DisableButtons(){
		yield return null;
		SelectionManager.Instance.DisableAllButtons();
	}
	
	private IEnumerator WaitForOtherTeamsOrStartGame(){
		while(readyTeamsCount < TOTAL_TEAMS) yield return null;
		SelectionManager instance = SelectionManager.Instance;
		Application.LoadLevel("ColoringSpots");
	}
	
	public void ResetTeamSelections(){
		GameObject[] buttons = SelectionManager.Instance.GetAllButtons();
		foreach(GameObject go in buttons){
			ParseObject color = go.GetComponent<TeamColorButton>().ParseObject;
			color["team"] = "reset";
			color.SaveAsync();
		}
	}
}
