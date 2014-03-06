using System.Collections;
using System;
using UnityEngine;
using Parse;

public class ParseUtil {
	public static Color DEFAULT_COLOR = new Color(0.5f, 0.5f, 0.5f);
	
	public static Color GetColor(ParseObject hasColorFields){
		float red = hasColorFields.Get<float>("red");
		float blue = hasColorFields.Get<float>("blue");
		float green = hasColorFields.Get<float>("green");
		return new Color(red, green, blue);
	}
	
	public static DateTime? GetLatestTime(ParseObject po, DateTime? dateTime){
		return po.UpdatedAt > dateTime || dateTime == null ? po.UpdatedAt : dateTime;
	}
}
