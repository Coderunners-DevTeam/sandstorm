/*
*	HARUN DASTEKIN 2025
*/

using Godot;
using System;


public partial class World : Node2D {
	private int WIDTH;
	private int HEIGHT;

	private Image image;
	private ImageTexture texture;
	private Sprite2D sprite;

	private Color[,] worldData;
	private MaterialType[,] materialData;

	private Camera2D camera;
	private bool needsUpdate = true;

	private MaterialType currentMaterial = MaterialType.Sand;

	//MainMenu20 menu20 = new MainMenu20();




	public override void _Ready() {
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		int blockSize = 4;

		camera = GetNode<Camera2D>("../Camera2D");

		WIDTH = (int)(screenSize.X / blockSize);
		HEIGHT = (int)(screenSize.Y / blockSize);

		worldData = new Color[WIDTH, HEIGHT];
		materialData = new MaterialType[WIDTH, HEIGHT];

		image = Image.CreateEmpty(WIDTH, HEIGHT, false, Image.Format.Rgba8);
		image.Fill(MaterialRegistry.Materials[MaterialType.Empty].Color);
		texture = ImageTexture.CreateFromImage(image);

		sprite = new Sprite2D {
			Texture = texture,
			Scale = Vector2.One,
			Centered = false, // ✨ wichtig!
			Position = Vector2.Zero
		};
		AddChild(sprite);

		ClearWorld();

		GD.Print($"Screen: {screenSize}, Grid: {WIDTH}x{HEIGHT}, BlockSize: {blockSize}");
		GD.Print("Sprite Pos: " + sprite.Position);
		GD.Print("Camera Pos: " + camera.Position);

		camera.Position = new Vector2(WIDTH / 2, HEIGHT / 2);


	}

	public override void _EnterTree() {
		var uiLayer = new CanvasLayer();
		var margin = new MarginContainer {
			AnchorLeft = 0,
			AnchorTop = 0,
			OffsetLeft = 20,
			OffsetTop = 20
		};

		var box = new VBoxContainer { Name = "UI" };
		margin.AddChild(box);
		uiLayer.AddChild(margin);

		var fpsLabel = new Label { Name = "FPSLabel" };
		box.AddChild(fpsLabel);

		foreach (var keyValuePair in MaterialRegistry.Materials) {
			var type = keyValuePair.Key;
			var name = keyValuePair.Value.Name;

			var button = new Buttons(name, () => currentMaterial = type);
			box.AddChild(button.MakeUIButton());
		}


		AddChild(uiLayer);

	}


	public override void _Process(double delta) {
		HandleInput();
		UpdateSimulation();

		if (needsUpdate) {
			UpdateImage();
			texture.Update(image);
			needsUpdate = false;
		}

		Label fpsLabel = GetNodeOrNull<Label>("CanvasLayer/MarginContainer/UI/FPSLabel");
		if (fpsLabel != null) {
			fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
		}
	}

	private void ClearWorld() {
		for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++) {
				worldData[x, y] = MaterialRegistry.Materials[MaterialType.Empty].Color;
				materialData[x, y] = MaterialType.Empty;
			}
		needsUpdate = true;
	}

	private void HandleInput() {
		//if (Input.IsActionPressed("ui_touch") || Input.IsMouseButtonPressed(MouseButton.Left)) {
		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
			Vector2 pos = GetLocalMousePosition();
			int x = (int)(pos.X / sprite.Scale.X);
			int y = (int)(pos.Y / sprite.Scale.Y);

			if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT) {
				PlaceMaterial(x, y);
				needsUpdate = true;
			}
		}


		if (Input.IsKeyPressed(Key.Key1)) currentMaterial = MaterialType.Sand;
		if (Input.IsKeyPressed(Key.Key2)) currentMaterial = MaterialType.Water;
		if (Input.IsKeyPressed(Key.Key3)) currentMaterial = MaterialType.Stone;
		if (Input.IsKeyPressed(Key.E)) currentMaterial = MaterialType.Empty;
	}

	private void PlaceMaterial(int x, int y) {
		materialData[x, y] = currentMaterial;
		worldData[x, y] = MaterialRegistry.Materials[currentMaterial].Color;
	}

	private void UpdateSimulation() {
		bool changed = false;

		for (int y = HEIGHT - 2; y >= 0; y--)
			for (int x = 1; x < WIDTH - 1; x++) {
				MaterialType mat = materialData[x, y];

				if (mat == MaterialType.Sand) {
					if (materialData[x, y + 1] == MaterialType.Empty) {
						Swap(x, y, x, y + 1);
						changed = true;
					}
					else if (materialData[x - 1, y + 1] == MaterialType.Empty) {
						Swap(x, y, x - 1, y + 1);
						changed = true;
					}
					else if (materialData[x + 1, y + 1] == MaterialType.Empty) {
						Swap(x, y, x + 1, y + 1);
						changed = true;
					}
				}
				else if (mat == MaterialType.Water) {
					if (materialData[x, y + 1] == MaterialType.Empty) {
						Swap(x, y, x, y + 1);
						changed = true;
					}
					else if (materialData[x - 1, y] == MaterialType.Empty) {
						Swap(x, y, x - 1, y);
						changed = true;
					}
					else if (materialData[x + 1, y] == MaterialType.Empty) {
						Swap(x, y, x + 1, y);
						changed = true;
					}
				}
			}

		if (changed)
			needsUpdate = true;
	}

	private void Swap(int x1, int y1, int x2, int y2) {
		(materialData[x2, y2], materialData[x1, y1]) = (materialData[x1, y1], materialData[x2, y2]);
		(worldData[x2, y2], worldData[x1, y1]) = (worldData[x1, y1], worldData[x2, y2]);
	}

	private void UpdateImage() {
		for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
				image.SetPixel(x, y, worldData[x, y]);
	}
}

