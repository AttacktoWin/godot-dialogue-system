using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NewDialogueTest.Scripts.Classes;
using Newtonsoft.Json;

public partial class NPC : Node2D
{
	[Export]
	public string Id;
	[Export(PropertyHint.File, "*.json")] private string DialogueFilePath;
	private Dictionary<string, Dialogue> AvailableDialogue;
	private PriorityQueue<Dialogue, int> DialogueQueue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DialogueQueue = new PriorityQueue<Dialogue, int>();
		// Get dialogues from file
		if (!File.Exists(DialogueFilePath))
		{
			AvailableDialogue = new Dictionary<string, Dialogue>();
			return;
		}
		var text = "";
		using (var sr = File.OpenText(DialogueFilePath))
		{
			text = sr.ReadToEnd();
		}
		AvailableDialogue = text.Length > 2 ? JsonConvert.DeserializeObject<Dictionary<string, Dialogue>>(text, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) : new Dictionary<string, Dialogue>();
	}

	public void Init()
	{
		if (!AvailableDialogue.ContainsKey("default"))
		{
			throw new Exception($"No default dialogue provded for NPC \"{Id}\"");
		}
		DialogueQueue.Enqueue(AvailableDialogue["default"], (int)AvailableDialogue["default"].Priority);
	}
	
	public void Init(string[] queuedIds)
	{
		foreach (var id in queuedIds)
		{
			if (AvailableDialogue.ContainsKey(id))
			{
				DialogueQueue.Enqueue(AvailableDialogue[id], (int)AvailableDialogue[id].Priority);
			}
		}
	}

	public string[] Save()
	{
		var ids = new string[DialogueQueue.Count];
		var i = 0;
		foreach (var (d, p) in DialogueQueue.UnorderedItems)
		{
			ids[i++] = d.Id;
		}

		return ids;
	}

	public Dialogue GetTopDialogue()
	{
		return DialogueQueue.Dequeue();
	}

	public void UnlockDialogue(string dialogueId)
	{
		if (!AvailableDialogue.ContainsKey(dialogueId))
		{
			throw new Exception($"Unknown Dialogue Id \"{dialogueId}\" on NPC \"{this.Id}\"");
		}
		DialogueQueue.Enqueue(AvailableDialogue[dialogueId], (int)AvailableDialogue[dialogueId].Priority);
	}

	public void RemoveDialogue(string dialogueId)
	{
		if (!AvailableDialogue.ContainsKey(dialogueId)) return;
		var dialogue = AvailableDialogue[dialogueId];
		var items = DialogueQueue.UnorderedItems.ToList();
		items.Remove((dialogue, (int)dialogue.Priority));
		DialogueQueue.Clear();
		DialogueQueue.EnqueueRange(items);
	}
}
