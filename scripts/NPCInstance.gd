@tool
extends Node2D

const DialogueSystem = preload("res://scripts/DialogueSystem.gd");

@export var NPC_id: String = "";

var interacted := false;

func interact() -> void:
	var system = get_node("/root/DialogueSystem") as DialogueSystem;
	var dialogue = system.get_top_dialogue(NPC_id);
	if (dialogue != null):
		# TODO: display the dialogue in a text box
		system.dialogue_viewed(NPC_id, dialogue.id);
		interacted = true;
