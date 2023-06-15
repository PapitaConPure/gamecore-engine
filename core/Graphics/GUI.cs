using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

//PENDIENTE: volver más general el dibujado de interfaz
namespace GameCore {
	/// <summary>
	/// Representa una interfaz gráfica de usuario
	/// </summary>
	public static class GUI {
		private static IRenderer drawer;

		#region Dimensiones de UI y Escena
		private static Rect uiArea   = new Rect(new Vec2(1, 1), new Vec2(79, 23));
		private static Rect gameArea = new Rect(new Vec2(1, 1), new Vec2(79, 23));

		public static int UITop    => (int)uiArea.Top;
		public static int UIBottom => (int)uiArea.Bottom;
		public static int UILeft   => (int)uiArea.Left;
		public static int UIRight  => (int)uiArea.Right;
		public static int UICenter => MathX.Round(uiArea.Center.X);
		public static int UIMiddle => MathX.Round(uiArea.Center.Y);

		public static int GameTop    => (int)gameArea.Top;
		public static int GameBottom => (int)gameArea.Bottom;
		public static int GameLeft   => (int)gameArea.Left;
		public static int GameRight  => (int)gameArea.Right;
		public static int GameCenter => MathX.Round(gameArea.Center.X);
		public static int GameMiddle => MathX.Round(gameArea.Center.Y);

		public static Vec2 UITopLeft       => uiArea.IV1;
		public static Vec2 UIBottomRight   => uiArea.IV2;
		public static Vec2 GameTopLeft     => gameArea.IV1;
		public static Vec2 GameBottomRight => gameArea.IV2;

		public static Rect UIArea {
			get => uiArea;
			set => uiArea = value;
		}
		public static Rect GameArea {
			get => gameArea;
			set => gameArea = value;
		}
		#endregion

		#region Acceso a Drawer
		public static ConsoleRenderer ConsoleDrawer {
			get {
				if(Game.Target != GameTarget.Console)
					throw new NullReferenceException("No existe ningún ConsoleDrawer porque el juego no es para consolas");

				return (ConsoleRenderer)drawer;
			}
		}
		public static byte FormsDrawer {
			get {
				if(Game.Target != GameTarget.Forms)
					throw new NullReferenceException("No existe ningún FormsDrawer porque el juego no es para Windows Forms");

				return 0; //Pendiente
			}
		}
		#endregion

		#region Preparado de Dibujadores
		public static void PrepareConsole(string title, short windowWidth = 80, short windowHeight = 25) {
			if(Game.Target != GameTarget.Console)
				throw new InvalidOperationException("No se puede preparar la consola porque el juego no apunta a consolas");

			Console.Title = title;

			Console.Clear();
			Console.ResetColor();
			Console.WindowWidth = windowWidth;
			Console.WindowHeight = windowHeight;
			Console.BufferWidth = windowWidth + 1;
			Console.BufferHeight = windowHeight + 1;

			Console.OutputEncoding = System.Text.Encoding.Unicode;
			Console.CursorVisible = false;
			Console.TreatControlCAsInput = true;

			drawer = new ConsoleRenderer();
		}
		/// <summary>
		/// Crea un nuevo <see cref="Form"/> y lo prepara para correr y renderizar un juego.
		/// Así mismo, se configura a <see cref="GUI.FormsDrawer"/> para renderizar cosas específicas de Forms
		/// </summary>
		/// <remarks>
		/// Requiere ejecutarse en una aplicación de Windows Forms
		/// Al finalizar la preparación, ejecuta inmediatamente <see cref="Application.Run"/> con la ventana creada
		/// </remarks>
		/// <param name="title"></param>
		/// <param name="windowWidth"></param>
		/// <param name="windowHeight"></param>
		public static void PrepareForms(string title, short windowWidth = 320, short windowHeight = 240) {
			if(Game.Target != GameTarget.Forms)
				throw new InvalidOperationException("No se puede preparar el formulario porque el juego no apunta a Windows Forms");

			#region Crear y preparar Formulario
			GameWindow gameWindow = new GameWindow();

			gameWindow.Text = title;
			gameWindow.FormBorderStyle = FormBorderStyle.FixedSingle;
			gameWindow.MaximizeBox = false;
			gameWindow.MinimizeBox = false;
			gameWindow.Size = new Size(windowWidth, windowHeight);
			#endregion

			#region Añadir PictureBox para renderizar
			PictureBox pbViewport = new PictureBox();
			pbViewport.Name = "pbViewport";
			pbViewport.Dock = DockStyle.Fill;
			pbViewport.BackColor = Color.Black;
			Bitmap bitmap = new Bitmap(windowWidth, windowHeight);
			pbViewport.Image = bitmap;
			gameWindow.Controls.Add(pbViewport);
			#endregion

			drawer = new FormsRenderer(pbViewport);

			Game.Controller.SubscribeToWindow(gameWindow);

			Application.Run(gameWindow);
		}
		#endregion

		#region Dibujado Cíclico General
		public static void EmptyGameFrame() => drawer.EmptyGameFrame();

		public static void DrawGameFrame(List<GameObject> instances) => drawer.RenderGameFrame(instances);

		public static void DrawSurface() => drawer.DrawSurface();

		public static void DrawTPS(double deltaTime) => drawer.DrawTPS(deltaTime);

		public static void DrawText(Vec2 pos, string text) => drawer.DrawText(pos, text);
		#endregion

		#region Cálculos de Utilidad Generales
		public static int NumberCharacters(int n) {
			return Convert.ToString(n).Length;
		}
		public static int NumberCharacters(long n) {
			return Convert.ToString(n).Length;
		}
		public static int NumberCharacters(double n) {
			return Convert.ToString(n).Length;
		}

		public static int CenteredStringOffset(int left, int right, string str) {
			double center = (left + right) / 2.0;
			double halfString = str.Length / 2.0;
			double offset = center - halfString;
			return (int)Math.Ceiling(offset);
		}

		public static void DrawVerticalLine(Vec2 pos, double height, char c) {
			int tempCursorTop = Console.CursorTop;
			int tempCursorLeft = Console.CursorLeft;

			for(int i = 0; i < height; i++) {
				Console.CursorTop = pos.IY + i;
				Console.CursorLeft = pos.IX;
				Console.Write(c);
			}

			Console.CursorTop = tempCursorTop;
			Console.CursorLeft = tempCursorLeft;
		}
		#endregion
	}

	public interface IRenderer {
		void EmptyGameFrame();
		void RenderGameFrame(List<GameObject> instances);
		void DrawTPS(double deltaTime);
		void DrawSurface();
		void DrawText(Vec2 pos, string text);
	}
}
