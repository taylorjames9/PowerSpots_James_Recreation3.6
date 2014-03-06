Parse.Cloud.beforeSave("Color", function(request, response) {
	//first see if the save is a reset
	var team = request.object.get("team");
  	if (team === "reset") {
	    request.object.set("team", "available");
	    response.success();
	}
	
	//if it is a new save, see if the team is available
	var id = request.object.id;
	var Color = Parse.Object.extend("Color");
	var query = new Parse.Query(Color);
	query.get(id, {
		success: function(object) {
			var team = object.get("team");
			if(team === "available"){
				response.success();
			}
			else{
				response.error("Color has already been claimed");
			}
  		},
		error: function(object, error) {
			response.error("Couldn't claim color: " + error.message);
		}
	});
});