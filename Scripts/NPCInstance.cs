using System;
using Godot;

namespace NewDialogueTest.Scripts.Classes;

public partial class NPCInstance : Area2D
{
    [Export] private string NPCId = "";

    private bool Interacted = false;

    public void Interact()
    {
        if (Interacted) return;

        var system = GetNode<DialogueSystem>("/root/DialogueSystem");
        if (system == null)
        {
            throw new Exception("No instance of Dialogue System found");
        }

        var dialogue = system.GetTopDialogue(NPCId);
        // TODO: Display dialogue
        system.DialogueViewed(NPCId, dialogue.Id);
        Interacted = true;
    }
}