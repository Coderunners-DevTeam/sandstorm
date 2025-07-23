/*
*	HARUN DASTEKIN 2025
*/

using Godot;
using System;
public partial class Buttons : Node2D, IButton {

    public string buttontext;
    public Action onButtonClickAction;

    public Buttons(string buttontext, Action onButtonClickAction) {
        this.buttontext = buttontext;
        this.onButtonClickAction = onButtonClickAction;
    }

    public Button MakeUIButton() {
        var button = new Button { Text = buttontext };
        button.Pressed += onButtonClickAction;
        return button;
    }
}