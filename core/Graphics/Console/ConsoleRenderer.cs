using System;
using System.Globalization;
using System.Collections.Generic;

namespace GameCore {
	public class ConsoleRenderer: IRenderer {
		private readonly List<int> linesToCleanUp = new List<int>();
		private readonly List<ConsoleWriteRequest> writeRequests = new List<ConsoleWriteRequest>();

		public void EmptyGameFrame() {
			string emptyLine = new string(' ', GUI.GameRight - GUI.GameLeft + 1);
			foreach(int line in linesToCleanUp) {
				Console.CursorLeft = GUI.GameLeft;
				Console.CursorTop = line;
				Console.Write(emptyLine);
			}
			linesToCleanUp.Clear();
		}

		public void RenderGameFrame(List<GameObject> instances) {
			foreach(GameObject instance in instances)
				instance.Draw();
		}

		public void DrawSurface() {
			foreach(ConsoleWriteRequest request in writeRequests)
				request.Draw();
			writeRequests.Clear();
		}

		public void DrawTPS(double deltaTime) {
			double tps = 1000 / MathX.Clamp(Clock.BaseTick + Math.Max(0, deltaTime - Clock.BaseTick), 1, 1000);
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
		public void DrawText(Vec2 pos, string text, ConsoleColor color) {
			ConsoleWriteRequest request = new ConsoleWriteRequest(pos, text, color);
			writeRequests.Add(request);
		}
		public void DrawText(Vec2 pos, string text, ConsoleColor fgColor, ConsoleColor bgColor) {
			ConsoleWriteRequest request = new ConsoleWriteRequest(pos, text, fgColor, bgColor);
			writeRequests.Add(request);
		}
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
