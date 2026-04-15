using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

/*
continueOn:
    Input
    Attack
    EnemyFailedDeath
    EnemyDeath
    SecondRoomEntered
    TriedDashing
    LastRoomEntered
    ResponseWindow

*/
public partial class DialogueManager : Control
{
    private Dictionary<string, DialogEntry> tutorialData = new();
    private int currentIndex = 0;
    public RichTextLabel DialogueLabel;
    private bool skipTyping = false;
    private bool isTyping = false;
    private bool canBeSkipped = true;


    public override void _Ready()
    {
        DialogueLabel = GetNode<RichTextLabel>("DialogLabel");
        LoadJson("res://customResources/tutorialDialogue.json");
        DisplayDialogue();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else if (canBeSkipped)
            {
                DisplayDialogue();
            }
        }
    }

    public async void DisplayDialogue()
    {
        if (isTyping) return;

        var entry = LoadEntry("res://customResources/tutorialDialogue.json", currentIndex);

        if (entry != null)
        {
            isTyping = true;
            canBeSkipped = entry.continueOn == "Input";

            string text = ReplaceKeys(entry.text);

            await TypeText(text, entry.speed);

            isTyping = false;
            currentIndex++;
        }
    }

    private async Task TypeText(string text, string speed)
    {
        DialogueLabel.Text = "";
        skipTyping = false;

        double delay = GetDialogEntrySpeed(speed);

        foreach (char c in text)
        {
            if (skipTyping)
            {
                DialogueLabel.Text = text;
                return;
            }

            DialogueLabel.Text += c;
            await ToSignal(GetTree().CreateTimer(delay), "timeout");
        }
    }
    private string ReplaceKeys(string text)
    {
        return text
            .Replace("{upKey}", GetKeyForAction("plr_up"))
            .Replace("{downKey}", GetKeyForAction("plr_down"))
            .Replace("{leftKey}", GetKeyForAction("plr_left"))
            .Replace("{rightKey}", GetKeyForAction("plr_right"))
            .Replace("{attackKey}", GetKeyForAction("plr_attack"))
            .Replace("{chargeKey}", GetKeyForAction("plr_charge"));
    }

    private string GetKeyForAction(string action)
    {
        var events = InputMap.ActionGetEvents(action);

        foreach (var ev in events)
        {
            if (ev is InputEventKey keyEvent)
            {
                if (keyEvent.PhysicalKeycode != 0)
                    return OS.GetKeycodeString(keyEvent.PhysicalKeycode);

                if (keyEvent.Keycode != 0)
                    return OS.GetKeycodeString(keyEvent.Keycode);
            }
        }

        return "?";
    }

    public DialogEntry LoadEntry(string file, int index)
    {
        if (tutorialData == null || tutorialData.Count == 0)
            LoadJson(file);

        string key = "tutorial_" + index;

        if (tutorialData.ContainsKey(key))
            return tutorialData[key];

        return null;
    }

    public void LoadJson(string path)
    {
        if (!FileAccess.FileExists(path))
        {
            GD.PrintErr("File not found: " + path);
            return;
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string content = file.GetAsText();

        tutorialData = JsonSerializer.Deserialize<Dictionary<string, DialogEntry>>(content);
    }

    private double GetDialogEntrySpeed(string speed)
    {
        return speed switch
        {
            "slow" => 0.08,
            "normal" => 0.04,
            "fast" => 0.01,
            _ => 0.04
        };
    }


}
