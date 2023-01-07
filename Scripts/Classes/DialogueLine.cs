using Godot;
namespace NewDialogueTest.Scripts.Classes;

public class DialogueLine : DialoguePiece
{
    public string PortraitName { get; set; }
    public string SoundEffectName { get; set; }
    public override string PieceType { get; } = "Line"; 
}