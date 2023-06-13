using System;
using System.Globalization;
using System.Collections.Generic;

namespace GameCore {
	public class ConsoleDrawer: IDrawer {
		private static readonly List<int> linesToCleanUp = new List<int>();
		private static readonly List<ConsoleWriteRequest> writeRequests = new List<ConsoleWriteRequest>();

		public void EmptyGameFrame() {
			string emptyLine = new string(' ', GUI.GameRight - GUI.GameLeft + 1);
			foreach(int line in linesToCleanUp) {
				Console.CursorLeft = 1;
				Console.CursorTop = line;
				Console.Write(emptyLine);
			}
			linesToCleanUp.Clear();
		}

		public void DrawGameFrame(List<GameObject> instances) {
			foreach(GameObject instance in instances)
				instance.Draw();
		}

		public void DrawSurface() {
			foreach(ConsoleWriteRequest request in writeRequests)
				request.Draw();
			writeRequests.Clear();
		}

		public void DrawTPS(double deltaTime) {
			double tps = 1000 / MathUtils.Clamp(Clock.BaseTick + Math.Max(0, deltaTime - Clock.BaseTick), 1, 1000);
			string ticksSuffix = " TPS";
			int ticksCharacters = GUI.NumberCharacters(Math.Round(tps)) + 3 + /*ticksPrefix.Length*/ +ticksSuffix.Length;
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			Console.SetCursorPosition(GUI.UIRight - ticksCharacters - 2, GUI.GameBottom);
			Console.Write(new string(' ', ticksCharacters + 2));
			Console.SetCursorPosition(GUI.UIRight - ticksCharacters, GUI.GameBottom);
			Console.Write(string.Format(new CultureInfo("en"), "{0:0.00}{1}", Math.Min(tps, 1000), ticksSuffix));
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.BackgroundColor = ConsoleColor.Black;
		}

		public void AddLineToCleanUp(int line) {
			if(!linesToCleanUp.Contains(line))
				linesToCleanUp.Add(line);
		}

		public void DrawText(Vec2 pos, string text) {
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
	}
}
