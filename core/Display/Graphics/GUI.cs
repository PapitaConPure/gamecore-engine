using System;
using System.Collections.Generic;

//PENDIENTE: volver más general el dibujado de interfaz
namespace GameCore {
	/// <summary>
	/// Representa una interfaz gráfica de usuario
	/// </summary>
	public static class GUI {
		private static IDrawer drawer;

		#region Dimensiones de UI y Escena
		private static Rect uiArea   = new Rect(new Vec2(1, 1), new Vec2(79, 23));
		private static Rect gameArea = new Rect(new Vec2(1, 1), new Vec2(79, 23));

		public static int UITop    => (int)uiArea.Top;
		public static int UIBottom => (int)uiArea.Bottom;
		public static int UILeft   => (int)uiArea.Left;
		public static int UIRight  => (int)uiArea.Right;
		public static int UICenter => MathUtils.Round(uiArea.Center.X);
		public static int UIMiddle => MathUtils.Round(uiArea.Center.Y);

		public static int GameTop    => (int)gameArea.Top;
		public static int GameBottom => (int)gameArea.Bottom;
		public static int GameLeft   => (int)gameArea.Left;
		public static int GameRight  => (int)gameArea.Right;
		public static int GameCenter => MathUtils.Round(gameArea.Center.X);
		public static int GameMiddle => MathUtils.Round(gameArea.Center.Y);

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
		public static ConsoleDrawer ConsoleDrawer {
			get {
				if(Game.Target != GameTarget.Console)
					throw new NullReferenceException("No existe ningún ConsoleDrawer porque el juego no es para consolas");

				return (ConsoleDrawer)drawer;
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

			drawer = new ConsoleDrawer();
		}
		public static void PrepareForms(string title, short windowWidth = 80, short windowHeight = 25) {
			if(Game.Target != GameTarget.Forms)
				throw new InvalidOperationException("No se puede preparar el formulario porque el juego no apunta a Windows Forms");

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
		}
		#endregion

		#region Dibujado Cíclico General
		public static void EmptyGameFrame() => drawer.EmptyGameFrame();

		public static void DrawGameFrame(List<GameObject> instances) => drawer.DrawGameFrame(instances);

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

	public interface IDrawer {
		void EmptyGameFrame();
		void DrawGameFrame(List<GameObject> instances);
		void DrawTPS(double deltaTime);
		void DrawSurface();
		void DrawText(Vec2 pos, string text);
	}

	public struct ConsoleWriteRequest {
		private readonly Vec2 pos;
		private readonly string text;
		private readonly ConsoleColor fgColor;
		private readonly ConsoleColor bgColor;

		public ConsoleWriteRequest(Vec2 pos, string text, ConsoleColor fgColor, ConsoleColor bgColor) {
			this.pos = pos;
			this.text = text;
			this.fgColor = fgColor;
			this.bgColor = bgColor;
		}
		public ConsoleWriteRequest(Vec2 pos, string text, ConsoleColor fgColor) {
			this.pos = pos;
			this.text = text;
			this.fgColor = fgColor;
			this.bgColor = ConsoleColor.Black;
		}
		public ConsoleWriteRequest(Vec2 pos, string text) {
			this.pos = pos;
			this.text = text;
			this.fgColor = ConsoleColor.White;
			this.bgColor = ConsoleColor.Black;
		}

		public void Draw() {
			Console.SetCursorPosition(pos.IX, pos.IY);
			Console.ForegroundColor = fgColor;
			Console.BackgroundColor = bgColor;
			Console.Write(text);
			GUI.ConsoleDrawer.AddLineToCleanUp(pos.IY);
		}
	}
}
