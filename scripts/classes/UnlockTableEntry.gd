class_name UnlockTableEntry;

extends Resource;

const DialogueNPCIds = preload("res://scripts/classes/DialogueNpcIds.gd");

@export var unlocked_ids: Array[DialogueNPCIds];
@export var removed_ids: Array[DialogueNPCIds];

func _init(p_unlocked_ids = [], p_removed_ids = []):
	unlocked_ids = p_unlocked_ids;
	removed_ids = p_removed_ids;
