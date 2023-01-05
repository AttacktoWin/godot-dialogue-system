class_name Dialogue;

extends Resource;

@export var id: String;
@export var priority: int;
@export var parts: Array[Resource];

func _init(p_id = "", p_priority = 0, p_parts = []):
	id = p_id;
	priority = p_priority;
	parts = p_parts;
