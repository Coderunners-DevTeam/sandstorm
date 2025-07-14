using Godot;
using System;

public partial class World : Node2D
{
	private const int WIDTH = 256;
	private const int HEIGHT = 256;

	private Image image;
	private ImageTexture texture;
	private Sprite2D sprite;

	private Color[,] worldData = new Color[WIDTH, HEIGHT];
	private Color sandColor = Colors.Yellow;
	private Color emptyColor = Colors.Black;

	private bool needsUpdate = true;

	public override void _Ready()
	{
	image = Image.CreateEmpty(WIDTH, HEIGHT, false, Image.Format.Rgba8);
		image.Fill(emptyColor);

		texture = ImageTexture.CreateFromImage(image);
		sprite = new Sprite2D
		{
			Texture = texture,
			Position = Vector2.Zero,
			Scale = new Vector2(4, 4)
		};
		AddChild(sprite);

		ClearWorld();

		// Testpixel
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				worldData[x, y] = sandColor;
			}
		}
		needsUpdate = true;
	}

	private void ClearWorld()
	{
		for (int x = 0; x < WIDTH; x++)
			for (int y = 0; y < HEIGHT; y++)
				worldData[x, y] = emptyColor;
	}

	public override void _Process(double delta)
	{
		HandleMouseInput();
		UpdateSimulation();

		if (needsUpdate)
		{
			UpdateImage();
			texture.Update(image);
			needsUpdate = false;
		}
	}

	private void HandleMouseInput()
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 mousePos = GetLocalMousePosition();
			int x = (int)mousePos.X;
			int y = (int)mousePos.Y;

			if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
			{
				worldData[x, y] = sandColor;
				needsUpdate = true;
			}
		}
	}

	private void UpdateSimulation()
	{
		bool changed = false;

		for (int y = HEIGHT - 2; y >= 0; y--)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				if (worldData[x, y] == sandColor && worldData[x, y + 1] == emptyColor)
				{
					worldData[x, y + 1] = sandColor;
					worldData[x, y] = emptyColor;
					changed = true;
				}
			}
		}

		if (changed)
			needsUpdate = true;
	}

	private void UpdateImage()
	{
		for (int x = 0; x < WIDTH; x++)
		{
			for (int y = 0; y < HEIGHT; y++)
			{
				image.SetPixel(x, y, worldData[x, y]);
			}
		}
	}
}
