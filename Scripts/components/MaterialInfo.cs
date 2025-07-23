/*
* MaterialInfoklasse - verknüpft die Farbe mit dem Namen für die Farbe
*/

using Godot;


public class MaterialInfo {
    public Color Color { get; }
    public string Name { get; }

    public MaterialInfo(Color color, string name) {
        this.Color = color;
        this.Name = name;
    }

}
