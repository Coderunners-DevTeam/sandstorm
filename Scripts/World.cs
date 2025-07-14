/*
*	Mein SandStrom Game
*	Harun Dastekin 2025
*/


using Godot;
using System;

public partial class World : Node2D
{
	////////////////////////////////////////////////////////////////////////////

	private int WIDTH;
	private int HEIGHT;

	private Image image;
	private ImageTexture texture;
	private Sprite2D sprite;

	private Color[,] worldData;
	private Color sandColor = Colors.Yellow;
	private Color emptyColor = Colors.Black;

	private bool needsUpdate = true;


	////////////////////////////////////////////////////////////////////////////
	//////              _Ready                                         ///////
	/////////////////////////////////////////////////////////////////////////

	public override void _Ready()
	{
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		WIDTH = (int)(screenSize.X / 5);  //  Zoom-Faktor
		HEIGHT = (int)(screenSize.Y / 5);

		// Welt vorbereiten
	worldData = new Color[WIDTH, HEIGHT];

	image = Image.CreateEmpty(WIDTH, HEIGHT, false, Image.Format.Rgba8);
	image.Fill(emptyColor);

	texture = ImageTexture.CreateFromImage(image);

		sprite = new Sprite2D
		{
			Texture = texture,
			Scale = new Vector2(5, 5) // Zoomfaktor (sichtbar machen)
		};
		AddChild(sprite);

		// Zentrieren
		sprite.Position = (GetViewport().GetVisibleRect().Size / 2f) - ((new Vector2(WIDTH, HEIGHT) * sprite.Scale) / 2f);

		ClearWorld();

		// Testpixel (Sandblock oben links)
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				worldData[x, y] = sandColor;
			}
		}
		needsUpdate = true;
	}

	////////////////////////////////////////////////////////////////////////////
	//////              ClearWorld                                     ///////
	/////////////////////////////////////////////////////////////////////////

	private void ClearWorld()
	{
		for (int x = 0; x < WIDTH; x++)
		{
			for (int y = 0; y < HEIGHT; y++)
			{
				worldData[x, y] = emptyColor;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////
	//////              _Process                                     ///////
	/////////////////////////////////////////////////////////////////////////

	public override void _Process(double delta)
	{
		HandleTouchInput();
		UpdateSimulation();

		if (needsUpdate)
		{
			UpdateImage();
			texture.Update(image);
			needsUpdate = false;
		}
	}

   ////////////////////////////////////////////////////////////////////////////
   //////              HandleTouchInput                                     ///////
   /////////////////////////////////////////////////////////////////////////

	private void HandleTouchInput()
	{
		if (Input.IsActionPressed("ui_touch") || Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 touchPos = GetLocalMousePosition();
			int x = (int)(touchPos.X / sprite.Scale.X);
			int y = (int)(touchPos.Y / sprite.Scale.Y);

			if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
			{
				worldData[x, y] = sandColor;
				needsUpdate = true;
			}
		}
	}

   ////////////////////////////////////////////////////////////////////////////
   //////              UpdateSimulation                                ///////
   /////////////////////////////////////////////////////////////////////////
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
		{
			needsUpdate = true;
		}
	}

	   ////////////////////////////////////////////////////////////////////////////
	   //////              UpdateImage                                     ///////
	   /////////////////////////////////////////////////////////////////////////

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
