@tool
class_name NPC;

extends Node2D

const Dialogue = preload("res://scripts/classes/Dialogue.gd");
const DialogueQueue = preload("res://scripts/classes/DialogueQueue.gd");

@export var id: String = "";
@export var dialogue_file: Resource;

var queue: DialogueQueue;

# Called when the node enters the scene tree for the first time.
func _ready():
	queue = DialogueQueue.new();
	if (!dialogue_file):
		print("No dialogue provided for NPC ", id);
	elif (Engine.is_editor_hint()):
		print("NPC ", id, " loaded with ", len(dialogue_file.dialogues), " dialogues");

func init():
	if (!dialogue_file.dialogues.has("default")):
		print("No default dialogue found for NPC ", id);
		return;
	queue.enqueue(dialogue_file.dialogues["default"]);
	
func init_with_data(queued_ids: Array[String]):
	for d_id in queued_ids:
		if (dialogue_file.dialogues.has(d_id)):
			queue.enqueue(dialogue_file.dialogues[d_id]);
			
func save() -> Array[String]:
	return queue.get_contents().map(func (d: Dialogue): return d.id);
	
func get_top_dialogue() -> Dialogue:
	return queue.dequeue();
	
func unlock_dialogue(d_id: String) -> void:
	if (dialogue_file.dialogues.has(d_id)):
		queue.enqueue(dialogue_file.dialogues[d_id]);
		
func remove_dialogue(d_id: String) -> void:
	queue.remove(d_id);
