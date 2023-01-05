class_name DialogueLine;

extends Resource;

@export var text: String;
@export var display_name: String;
@export var portrait_name: String;
@export var sfx_name: String;


func _init(p_text = "", p_display_name = "", p_portrait_name = "", p_sfx_name = ""):
	text = p_text;
	display_name = p_display_name;
	portrait_name = p_portrait_name;
	sfx_name = p_sfx_name;
