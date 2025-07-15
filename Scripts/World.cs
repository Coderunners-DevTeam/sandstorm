using Godot;
using System;

public partial class World : Node2D
{
	private int WIDTH;
	private int HEIGHT;

	private Image image;
	private ImageTexture texture;
	private Sprite2D sprite;

	private Color[,] worldData;
	private MaterialType[,] materialData;

	private Camera2D camera;
	private bool needsUpdate = true;

	private enum MaterialType { Empty, Sand, Water, Stone }
	private MaterialType currentMaterial = MaterialType.Sand;

	private readonly Color sandColor = new Color(1f, 1f, 0f);        // Gelb
	private readonly Color waterColor = new Color(0f, 0.5f, 1f);     // Blau
	private readonly Color stoneColor = new Color(0.4f, 0.4f, 0.4f); // Grau
	private readonly Color emptyColor = new Color(0f, 0f, 0f);       // Schwarz

	public override void _Ready()
	{
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		int blockSize = 4;

		camera = GetNode<Camera2D>("../Camera2D");

		WIDTH = (int)(screenSize.X / blockSize);
		HEIGHT = (int)(screenSize.Y / blockSize);

		worldData = new Color[WIDTH, HEIGHT];
		materialData = new MaterialType[WIDTH, HEIGHT];

		image = Image.CreateEmpty(WIDTH, HEIGHT, false, Image.Format.Rgba8);
		image.Fill(emptyColor);
		texture = ImageTexture.CreateFromImage(image);

		sprite = new Sprite2D
{
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

	public override void _EnterTree()
	{
		var uiLayer = new CanvasLayer();
		var margin = new MarginContainer
		{
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

		box.AddChild(MakeUIButton("Reset", () => ClearWorld()));
		box.AddChild(MakeUIButton("Sand", () => currentMaterial = MaterialType.Sand));
		box.AddChild(MakeUIButton("Wasser", () => currentMaterial = MaterialType.Water));
		box.AddChild(MakeUIButton("Stein", () => currentMaterial = MaterialType.Stone));
		box.AddChild(MakeUIButton("Löschen", () => currentMaterial = MaterialType.Empty));


		AddChild(uiLayer);
	}

	private Button MakeUIButton(string text, Action onClick)
	{
		var button = new Button { Text = text };
		button.Pressed += onClick;
		return button;
	}

	public override void _Process(double delta)
	{
		HandleInput();
		UpdateSimulation();

		if (needsUpdate)
		{
			UpdateImage();
			texture.Update(image);
			needsUpdate = false;
		}

		Label fpsLabel = GetNode<Label>("UI/FPSLabel");
		fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
	}

	private void ClearWorld()
	{
		for (int x = 0; x < WIDTH; x++)
		for (int y = 0; y < HEIGHT; y++)
		{
			worldData[x, y] = emptyColor;
			materialData[x, y] = MaterialType.Empty;
		}
		needsUpdate = true;
	}

	private void HandleInput()
	{
		if (Input.IsActionPressed("ui_touch") || Input.IsMouseButtonPressed(MouseButton.Left))
	{
		Vector2 pos = GetLocalMousePosition();
		int x = (int)(pos.X / sprite.Scale.X);
		int y = (int)(pos.Y / sprite.Scale.Y);

		if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
		{
			PlaceMaterial(x, y);
			needsUpdate = true;
		}
	}


		if (Input.IsKeyPressed(Key.Key1)) currentMaterial = MaterialType.Sand;
		if (Input.IsKeyPressed(Key.Key2)) currentMaterial = MaterialType.Water;
		if (Input.IsKeyPressed(Key.Key3)) currentMaterial = MaterialType.Stone;
		if (Input.IsKeyPressed(Key.E)) currentMaterial = MaterialType.Empty;
	}

	private void PlaceMaterial(int x, int y)
	{
		switch (currentMaterial)
		{
			case MaterialType.Sand:
				materialData[x, y] = MaterialType.Sand;
				worldData[x, y] = sandColor;
				break;
			case MaterialType.Water:
				materialData[x, y] = MaterialType.Water;
				worldData[x, y] = waterColor;
				break;
			case MaterialType.Stone:
				materialData[x, y] = MaterialType.Stone;
				worldData[x, y] = stoneColor;
				break;
			case MaterialType.Empty:
				materialData[x, y] = MaterialType.Empty;
				worldData[x, y] = emptyColor;
				break;
		}
	}

	private void UpdateSimulation()
	{
		bool changed = false;

		for (int y = HEIGHT - 2; y >= 0; y--)
		for (int x = 1; x < WIDTH - 1; x++)
		{
			MaterialType mat = materialData[x, y];

			if (mat == MaterialType.Sand)
			{
				if (materialData[x, y + 1] == MaterialType.Empty)
				{
					Swap(x, y, x, y + 1);
					changed = true;
				}
				else if (materialData[x - 1, y + 1] == MaterialType.Empty)
				{
					Swap(x, y, x - 1, y + 1);
					changed = true;
				}
				else if (materialData[x + 1, y + 1] == MaterialType.Empty)
				{
					Swap(x, y, x + 1, y + 1);
					changed = true;
				}
			}
			else if (mat == MaterialType.Water)
			{
				if (materialData[x, y + 1] == MaterialType.Empty)
				{
					Swap(x, y, x, y + 1);
					changed = true;
				}
				else if (materialData[x - 1, y] == MaterialType.Empty)
				{
					Swap(x, y, x - 1, y);
					changed = true;
				}
				else if (materialData[x + 1, y] == MaterialType.Empty)
				{
					Swap(x, y, x + 1, y);
					changed = true;
				}
			}
		}

		if (changed)
			needsUpdate = true;
	}

	private void Swap(int x1, int y1, int x2, int y2)
	{
		(materialData[x2, y2], materialData[x1, y1]) = (materialData[x1, y1], materialData[x2, y2]);
		(worldData[x2, y2], worldData[x1, y1]) = (worldData[x1, y1], worldData[x2, y2]);
	}

	private void UpdateImage()
	{
		for (int x = 0; x < WIDTH; x++)
		for (int y = 0; y < HEIGHT; y++)
			image.SetPixel(x, y, worldData[x, y]);
	}
}
