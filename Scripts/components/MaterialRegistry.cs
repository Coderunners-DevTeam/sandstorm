using Godot;
using System.Collections.Generic;

public static class MaterialRegistry {
    public static readonly Dictionary<MaterialType, MaterialInfo> Materials = new() {
        {MaterialType.Sand, new MaterialInfo(new Color(1f, 1f, 0f),"Sand")},
        {MaterialType.Sand, new MaterialInfo(new Color(0f, 0.5f, 1f),"Wasser")},
        {MaterialType.Sand, new MaterialInfo(new Color(0.4f, 0.4f, 0.4f),"Stein")},
        {MaterialType.Sand, new MaterialInfo(new Color(0f, 0f, 0f),"Leer")},

    };
}