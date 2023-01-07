using Godot;
using JsonSubTypes;
using NewDialogueTest.Scripts.Classes;
using Newtonsoft.Json;

[JsonConverter(typeof(JsonSubtypes), "PieceType")]
[JsonSubtypes.KnownSubType(typeof(DialogueLine), "Line")]
[JsonSubtypes.KnownSubType(typeof(DialogueChoice), "Choice")]
public class DialoguePiece
{
    public string DisplayName { get; set; }
    public string Contents { get; set; }
    public virtual string PieceType { get; }
}