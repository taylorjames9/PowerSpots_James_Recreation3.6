using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

//TODO look into any threading issues
public class SpotManager : Singleton<SpotManager> {
	private static string SPOT_INFO = "SpotInfo";
	private static string SPOT_INFO_RED = "red";
	private static string SPOT_INFO_GREEN = "green";
	private static string SPOT_INFO_BLUE = "blue";
	private static string SPOT_INFO_X_POS = "xPos";
	private static string SPOT_INFO_Y_POS = "yPos";
	private static float Z_PLANE = 10;
	private static float POLL_INTERVAL = 1f;
	
	private static Dictionary<string, GameObject> spots = new Dictionary<string, GameObject>();
	private static Dictionary<string, ParseObject> parseSpots = new Dictionary<string, ParseObject>();
	public GameObject spotPrefab;
	private DateTime? lastUpdatedTime;
	
	protected SpotManager(){}
	
	// Use this for initialization
	void Start () {
		StartCoroutine("InitSpots");
		StartCoroutine("CheckForUpdates");
	}
	
	private IEnumerator InitSpots(){
		var query = ParseObject.GetQuery(SPOT_INFO).FindAsync();
		while(!query.IsCompleted) yield return null;
		IEnumerable<ParseObject> results = query.Result;
		foreach(ParseObject spotInfo in results){
			Color color = ParseUtil.GetColor(spotInfo);
			Vector3 position = getSpotPosition(spotInfo);
			GameObject spot = (GameObject) Instantiate(spotPrefab, position, Quaternion.identity);
			spot.renderer.material.color = color;
			spot.GetComponent<SpotChanger>().id = spotInfo.ObjectId;
			spots.Add(spotInfo.ObjectId, spot);
			parseSpots.Add(spotInfo.ObjectId, spotInfo);
			lastUpdatedTime = ParseUtil.GetLatestTime(spotInfo, lastUpdatedTime);
		}
	}
	
	private IEnumerator CheckForUpdates(){
	    while(true){
	        yield return new WaitForSeconds(POLL_INTERVAL);
			
			var query = ParseObject.GetQuery(SPOT_INFO).WhereGreaterThan("updatedAt", lastUpdatedTime).FindAsync();
			while(!query.IsCompleted) yield return null;
			IEnumerable<ParseObject> results = query.Result;
			foreach(ParseObject spotInfo in results){
				Color color = ParseUtil.GetColor(spotInfo);
				GameObject spot;
				spots.TryGetValue(spotInfo.ObjectId, out spot);
				spot.renderer.material.color = color;
				lastUpdatedTime = ParseUtil.GetLatestTime(spotInfo, lastUpdatedTime);
			}
		}
	}
	
	private Vector3 getSpotPosition(ParseObject spotInfo){
		float x = spotInfo.Get<float>(SPOT_INFO_X_POS);
		float y = spotInfo.Get<float>(SPOT_INFO_Y_POS);
		return new UnityEngine.Vector3(x, y, Z_PLANE);
	}
	
	public static void updateSpot(string id, Color color){
		GameObject spot;
		spots.TryGetValue(id, out spot);
		spot.renderer.material.color = color;
		ParseObject spotInfo;
		parseSpots.TryGetValue(id, out spotInfo);
		spotInfo[SPOT_INFO_RED] = color.r;
		spotInfo[SPOT_INFO_GREEN] = color.g;
		spotInfo[SPOT_INFO_BLUE] = color.b;
		spotInfo.SaveAsync();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
