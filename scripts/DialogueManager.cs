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
    private string continueCondition;
    private bool skipTyping = false;
    private bool isTyping = false;
    private bool canContinueWithInput = true;

    private bool enemyDied = false;
    private bool enemyShouldveDied = false;
    private bool secondRoomEntered = false;
    private bool lastRoomEntered=false;
    public void init()
    {
        DialogueLabel = GetNode<RichTextLabel>("DialogLabel");
        LoadJson("res://customResources/tutorialDialogue.json");
    }

    public override void _Input(InputEvent @event)
    {
        if(!Visible) return;
        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (isTyping)
            {
                skipTyping = true;
                return;
            }

            if (canContinueWithInput)
            {
                DisplayDialogue();
            }
        }
    }

    public override void _Process(double delta)
    {

        if(ContinueEventTriggered(continueCondition))
        {
            DisplayDialogue();
        }

        
    }


    private bool ContinueEventTriggered(string condition)
    {       
            switch (condition)
            {
                case "Attack": return Input.IsActionJustPressed("plr_attack");
                case "EnemyFailedDeath": return enemyShouldveDied;
                case "EnemyDeath": return enemyDied;
                case "SecondRoomEntered": return secondRoomEntered;
                case "TriedDashing": return Input.IsActionJustReleased("plr_charge");
                case "LastRoomEntered": return lastRoomEntered;
                case "ResponseWindow": return false;
            }
            return false;

    }


    public async void DisplayDialogue()
    {
        if (isTyping) return;
        if(currentIndex == 33) {
            GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
        }

        var entry = LoadEntry("res://customResources/tutorialDialogue.json", currentIndex);

        if (entry != null)
        {
            isTyping = true;
            canContinueWithInput = entry.continueOn == "Input";
            continueCondition = entry.continueOn;
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



    //signal recieving
    public void _on_training_dummy_on_false_death()
    {
        enemyShouldveDied = true;
    }

    public void _on_training_dummy_on_death()
    {
        enemyDied = true;
    }

    public void _on_area_2d_body_entered(Node2D body)
    {
        secondRoomEntered = true;
    }

}
