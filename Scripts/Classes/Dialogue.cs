namespace NewDialogueTest.Scripts.Classes;

public class Dialogue
{
    public string Id { get; set; }
    public DialoguePiece[] Pieces { get; set; }
    public DialoguePriority Priority { get; set;  }
}