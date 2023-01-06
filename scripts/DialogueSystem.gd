@tool
extends Node

const NPC = preload("res://scripts/NPC.gd");
const DialogueNPCId = preload("res://scripts/classes/DialogueNpcIds.gd");
const Dialogue = preload("res://scripts/classes/Dialogue.gd");

@export var dialogue_unlock_table: Resource;
@export var save_file_name: String = "";

var NPCs: Dictionary = {};


# Called when the node enters the scene tree for the first time.
func _ready():
	var init_time: int;
	if (Engine.is_editor_hint()):
		print("Loading Dialogue System");
		init_time = Time.get_ticks_usec();
	for child in get_children():
		if (child is NPC):
			NPCs[child.id] = child;
	
	var save_dict := {};
	if (FileAccess.file_exists(save_file_name)):
		var file = FileAccess.open(save_file_name, FileAccess.READ);
		var text = file.get_as_text();
		file = null;
		var json = JSON.new();
		var error = json.parse(text);
		if (error == OK):
			var data = json.data;
			if (typeof(data) == TYPE_DICTIONARY):
				save_dict = data as Dictionary;
			else:
				print("Malformed save data.");
		else:
			print("JSON Parse Error: ", json.get_error_message(), " in ", text, " at line ", json.get_error_line());
	for npc in NPCs.values():
		if (save_dict.has(npc.id)):
			npc.init_with_data(save_dict[npc.id] as Array[String]);
		else:
			npc.init();
			
	if (Engine.is_editor_hint()):
		print("Loaded Dialogue System in {time} usec.".format({"time": Time.get_ticks_usec() - init_time}));

func get_top_dialogue(npc_id: String) -> Dialogue:
	if (!NPCs.has(npc_id)):
		print("No NPC with id ", npc_id, " exists.");
		return null;
	return (NPCs[npc_id] as NPC).get_top_dialogue();

func unlock_dialogues(unlocked: Array[DialogueNPCIds]) -> void:
	for ids in unlocked:
		if (NPCs.has(ids.npc_id) && NPCs[ids.npc_id] is NPC):
			(NPCs[ids.npc_id] as NPC).unlock_dialogue(ids.dialogue_id);
			
func remove_dialogues(removed: Array[DialogueNPCIds]) -> void:
	for ids in removed:
		if (NPCs.has(ids.npc_id) && NPCs[ids.npc_id] is NPC):
			(NPCs[ids.npc_id] as NPC).remove_dialogue(ids.dialogue_id);
			
func _lookup_unlock_table(key: String) -> void:
	if (!dialogue_unlock_table.entries.has(key)):
		return;
	
	var unlocked_entry: UnlockTableEntry = dialogue_unlock_table.entries[key];
	if (len(unlocked_entry.unlocked_ids) > 0):
		unlock_dialogues(unlocked_entry.unlocked_ids);
	if (len(unlocked_entry.removed_ids)):
		remove_dialogues(unlocked_entry.removed_ids);

func dialogue_viewed(npc_id: String, dialogue_id: String) -> void:
	var key := "npc-{npc_id}-{dialogue_id}".format({"npc_id": npc_id, "dialogue_id": dialogue_id});
	_lookup_unlock_table(key);
		
func choice_selected(choice_id: String, option_selected: int) -> void:
	var key := "choice-{choice_id}-{option}".format({"choice_id": choice_id, "option": option_selected});
	_lookup_unlock_table(key);

func event_viewed(event_tag: String):
	var key := "world-{event_tag}".format({"event_tag": event_tag});
	_lookup_unlock_table(key);
	
func save() -> void:
	var save_dict := {};
	for npc in NPCs.values():
		save_dict[npc.id] = npc.save();
	var text = JSON.stringify(save_dict);
	var file = FileAccess.open(save_file_name, FileAccess.WRITE);
	file.store_line(text);
	file = null;

func debug_info() -> String:
	return "NPCs loaded: {npc_count}\nUnlock Table Entries: {table_count}".format({"npc_count": len(NPCs), "table_count": len(dialogue_unlock_table.entries.keys())});
