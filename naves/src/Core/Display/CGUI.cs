using System;
using System.Globalization;
using System.Collections.Generic;

//PENDIENTE: volver más general el dibujado de interfaz
namespace GameCore {
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
			CGUI.AddLineToCleanUp(pos.IY);
		}
	}

	/// <summary>
	/// Representa una interfaz gráfica de consola
	/// </summary>
	public static class CGUI {
		private static readonly List<int> linesToCleanUp = new List<int>();
		private static readonly List<ConsoleWriteRequest> writeRequests = new List<ConsoleWriteRequest>();

		private static int uiTop = 1;
		private static int uiBottom = 23;
		private static int uiLeft = 1;
		private static int uiRight = 79;

		private static int gameTop = 1;
		private static int gameBottom = 23;
		private static int gameLeft = 1;
		private static int gameRight = 79;

		public static int UITop => uiTop;
		public static int UIBottom => uiBottom;
		public static int UILeft => uiLeft;
		public static int UIRight => uiRight;
		public static int UICenter => (uiTop + uiBottom) / 2;
		public static int UIMiddle => (uiLeft + uiRight) / 2;

		//PENDIENTE: volver esto independiente del juego
		public static int GameTop => gameTop;
		public static int GameBottom => gameBottom; // 79
		public static int GameLeft => gameLeft;
		public static int GameRight => gameRight; //59
		public static int GameCenter => (GameLeft + GameRight) / 2;
		public static int GameMiddle => (GameTop + GameBottom) / 2;
		public static Vec2 UITopLeft => new Vec2(UILeft, UITop);
		public static Vec2 UIBottomRight => new Vec2(UIRight, UIBottom);
		public static Vec2 GameTopLeft => new Vec2(GameLeft, GameTop);
		public static Vec2 GameBottomRight => new Vec2(GameRight, GameBottom);
		public static Rect UIArea {
			get => new Rect(UITopLeft, UIBottomRight);
			set {
				uiLeft   = value.V1.IX;
				uiTop    = value.V1.IY;
				uiRight  = value.V2.IX;
				uiBottom = value.V2.IY;
			}
		}
		public static Rect GameArea {
			get => new Rect(GameTopLeft, GameBottomRight);
			set {
				gameLeft   = value.V1.IX;
				gameTop    = value.V1.IY;
				gameRight  = value.V2.IX;
				gameBottom = value.V2.IY;
			}
		}

		public static void PrepareConsole(string title, short windowWidth = 80, short windowHeight = 25) {
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

		public static void DrawBasicUI() {
			//Líneas horizontales
			string hbar1 = new String('═', GameRight);
			string hbar2 = new String('═', UIRight - GameRight - 2);
			Console.WriteLine("╔" + hbar1 + "╦" + hbar2 + "╗");
			Console.CursorTop = 24;
			Console.WriteLine("╚" + hbar1 + "╩" + hbar2 + "╝");
			Console.CursorTop = 0; //Evita scrollear hacia abajo

			//Líneas verticales
			DrawVerticalLine(1, 0, GameBottom, '║');
			DrawVerticalLine(1, GameRight + 1, GameBottom, '║');
			DrawVerticalLine(1, UIRight, GameBottom, '║');

			//Panel vertical
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			int panelWidth = UIRight - GameRight - 2;
			string filler = new String(' ', panelWidth);
			for (int i = 1; i < GameBottom - GameTop + 2; i++) {
				Console.SetCursorPosition(GameRight + 2, i);
				Console.Write(filler);
			}

			string diffName = "~NORMAL~";
			Console.CursorTop = 2;
			Console.CursorLeft = CenteredStringOffset(GameRight + 1, UIRight, diffName);
			Console.BackgroundColor = ConsoleColor.Cyan;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(diffName);
			Console.ResetColor();
		}

		public static void DrawSurface() {
			foreach(ConsoleWriteRequest request in writeRequests)
				request.Draw();
			writeRequests.Clear();
		}

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
			return Convert.ToInt32(Math.Ceiling(offset));
		}

		public static void AddLineToCleanUp(int line) {
			if(!linesToCleanUp.Contains(line))
				linesToCleanUp.Add(line);
		}

		public static void EmptyGameFrame() {
			string emptyLine = new string(' ', GameRight - GameLeft + 1);
			foreach(int line in linesToCleanUp) {
				Console.CursorLeft = 1;
				Console.CursorTop = line;
				Console.Write(emptyLine);
			}
			linesToCleanUp.Clear();
		}

		public static void DrawGameFrame(List<GameObject> instances) {
			foreach(GameObject instance in instances)
				instance.Draw();
		}

		public static void DrawTPS(double deltaTime) {
			double tps = 1000 / MathUtils.Clamp(Clock.BaseTick + Math.Max(0, deltaTime - Clock.BaseTick), 1, 1000);
			string ticksSuffix = " TPS";
			int ticksCharacters = NumberCharacters(Math.Round(tps)) + 3 + /*ticksPrefix.Length*/ +ticksSuffix.Length;
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			Console.SetCursorPosition(UIRight - ticksCharacters - 2, GameBottom);
			Console.Write(new string(' ', ticksCharacters + 2));
			Console.SetCursorPosition(UIRight - ticksCharacters, GameBottom);
			Console.Write(String.Format(new CultureInfo("en"), "{0:0.00}{1}", Math.Min(tps, 1000), ticksSuffix));
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.BackgroundColor = ConsoleColor.Black;
		}

		public static void DrawText(Vec2 pos, string text) {
			ConsoleWriteRequest request = new ConsoleWriteRequest(pos, text);
			writeRequests.Add(request);
		}
		public static void DrawText(Vec2 pos, string text, ConsoleColor color) {
			ConsoleWriteRequest request = new ConsoleWriteRequest(pos, text, color);
			writeRequests.Add(request);
		}
		public static void DrawText(Vec2 pos, string text, ConsoleColor fgColor, ConsoleColor bgColor) {
			ConsoleWriteRequest request = new ConsoleWriteRequest(pos, text, fgColor, bgColor);
			writeRequests.Add(request);
		}

		private static void DrawVerticalLine(int cursorTop, int cursorLeft, int height, char c) {
			int tempCursorTop = Console.CursorTop;
			int tempCursorLeft = Console.CursorLeft;

			for(int i = 0; i < height; i++) {
				Console.CursorTop = cursorTop + i;
				Console.CursorLeft = cursorLeft;
				Console.Write(c);
			}

			Console.CursorTop = tempCursorTop;
			Console.CursorLeft = tempCursorLeft;
		}
	}
}
