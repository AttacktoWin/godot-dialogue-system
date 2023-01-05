@tool
extends Node2D

@onready
var system = $DialogueSystem;

@onready
var text_edit = $TextEdit;

func _ready():
	text_edit.text = system.debug_info();
