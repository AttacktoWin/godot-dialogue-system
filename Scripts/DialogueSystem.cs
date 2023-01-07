using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using NewDialogueTest.Scripts.Classes;
using Newtonsoft.Json;

public partial class DialogueSystem : Node2D
{
	[Export(PropertyHint.File, "*.json")]
	private string DialogueUnlockTablePath = "";
	[Export]
	private string SaveFileName = "dialogueSystemSave.save";

	private List<NPC> NPCs = new List<NPC>();
	private Dictionary<string, UnlockTableEntry> UnlockTable;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var child in this.GetChildren())
		{
			if (child is NPC npc)
			{
				NPCs.Add(npc);
			}
		}

		var saveDict = new Dictionary<string, string[]>();
		// Load Dialogue Queues from save file
		if (File.Exists($"user://{SaveFileName}"))
		{
			using var sr = File.OpenText($"user://{SaveFileName}");
			var saveText = sr.ReadToEnd();
			saveDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(saveText);
		}
		foreach (var npc in NPCs)
		{
			if (saveDict.ContainsKey(npc.Id))
			{
				npc.Init(saveDict[npc.Id]);
			}
			else
			{
				npc.Init();
			}
		}
		
		// Initialize Unlock Table
		if (File.Exists(DialogueUnlockTablePath))
		{
			using var sr = File.OpenText(DialogueUnlockTablePath);
			var unlockTableText = sr.ReadToEnd();
			UnlockTable = JsonConvert.DeserializeObject<Dictionary<string, UnlockTableEntry>>(unlockTableText);
		}
		else
		{
			UnlockTable = new Dictionary<string, UnlockTableEntry>();
		}
	}

	public Dialogue GetTopDialogue(string NPCName)
	{
		var npc = NPCs.Find(n => n.Id == NPCName);
		if (npc == null)
		{
			throw new Exception($"No NPC with id \"{NPCName}\" exists");
		}

		return npc.GetTopDialogue();
	}

	private void UnlockDialogues(IEnumerable<DialogueNPCIds> unlockedDialogues)
	{
		foreach (var ids in unlockedDialogues)
		{
			var npc = NPCs.Find(n => n.Id == ids.NPCId);
			npc?.UnlockDialogue(ids.DialogueId);
		}
	}

	private void RemoveDialogues(IEnumerable<DialogueNPCIds> removedDialogues)
	{
		foreach (var ids in removedDialogues)
		{
			var npc = NPCs.Find(n => n.Id == ids.NPCId);
			npc?.RemoveDialogue(ids.DialogueId);
		}
	}
	
	public void DialogueViewed(string NPCId, string DialogueId)
	{
		var key = $"{NPCId}-{DialogueId}";
		if (!UnlockTable.ContainsKey(key))
		{
			return;
		}
		var unlockedDialogue = UnlockTable[key];

		if (unlockedDialogue.UnlockedIds.Length > 0)
		{
			UnlockDialogues(unlockedDialogue.UnlockedIds);
		}

		if (unlockedDialogue.RemovedIds.Length > 0)
		{
			RemoveDialogues(unlockedDialogue.RemovedIds);
		}
		// TODO: Add functionality with unlocking stuff in the world
	}

	public void ChoiceSelected(string ChoiceId, int OptionSelected)
	{
		var key = $"choice-{ChoiceId}-{OptionSelected}";
		if (!UnlockTable.ContainsKey(key))
		{
			return;
		}
		var unlockedDialogue = UnlockTable[key];

		if (unlockedDialogue.UnlockedIds.Length > 0)
		{
			UnlockDialogues(unlockedDialogue.UnlockedIds);
		}

		if (unlockedDialogue.RemovedIds.Length > 0)
		{
			RemoveDialogues(unlockedDialogue.RemovedIds);
		}
		// TODO: Add functionality with unlocking stuff in the world
	}

	public void EventViewed(string EventTag)
	{
		var key = $"world-{EventTag}";
		if (!UnlockTable.ContainsKey(key))
		{
			return;
		}
		var unlockedDialogue = UnlockTable[key];
		if (unlockedDialogue.UnlockedIds.Length > 0)
		{
			UnlockDialogues(unlockedDialogue.UnlockedIds);
		}

		if (unlockedDialogue.RemovedIds.Length > 0)
		{
			RemoveDialogues(unlockedDialogue.RemovedIds);
		}
	}
	
	public void Save()
	{
		var saveText = "{";
		var npcSaves = new string[NPCs.Count];
		for(var i = 0; i < NPCs.Count; i++)
		{
			var ids = NPCs[i].Save();
			npcSaves[i] = $"\"{NPCs[i].Id}\": [\"{string.Join("\",", ids)}\"]";
		}
		saveText += string.Join(",", npcSaves);
		saveText += "}";

		using var saveFile = File.CreateText($"user://{SaveFileName}");
		saveFile.WriteLine(saveText);
	}
}
