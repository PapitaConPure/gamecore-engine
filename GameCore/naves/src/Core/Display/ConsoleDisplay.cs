using System;

namespace GameCore {
	public class ConsoleDotRender: IDisplay {
		private readonly char symbol;
		private readonly ConsoleColor color;

		public ConsoleDotRender(char symbol, ConsoleColor color) {
			this.symbol = symbol;
			this.color = color;
		}
		public ConsoleDotRender(string symbol, ConsoleColor color) {
			this.symbol = Convert.ToChar(symbol);
			this.color = color;
		}

		public void Draw(Vec2 pos) {
			if(!pos.InsideRect(CGUI.GameTopLeft, CGUI.GameBottomRight))
				return;

			ConsoleColor lastColor = Console.ForegroundColor;
			//int lastLeft = Console.CursorLeft;
			//int lastTop = Console.CursorTop;

			Console.ForegroundColor = this.color;
			Console.CursorLeft = pos.IX;
			Console.CursorTop = pos.IY;

			Console.Write(this.symbol);

			Console.ForegroundColor = lastColor;
			//Console.CursorLeft = lastLeft;
			//Console.CursorTop = lastTop;
		}
	}

	public class ConsoleRender2D: IDisplay {
		private readonly string symbol;
		private readonly ConsoleColor color;
		private readonly bool centered;

		public ConsoleRender2D(string symbol, ConsoleColor color, bool centered = false) {
			this.symbol = symbol;
			this.color = color;
			this.centered = centered;
		}

		public void Draw(Vec2 pos) {
			Console.ForegroundColor = this.color;

			string[] symbolLines = this.symbol.Split('\n');
			int width = 0;
			foreach(string symbolLine in symbolLines) {
				if(symbolLine.Length > width)
					width = symbolLine.Length;
			}
			int height = symbolLines.Length;

			Vec2 topLeft = pos;
			if(this.centered)
				topLeft = pos.Offset(-width / 2, -height / 2);
			int cornerX = topLeft.IX;
			int cornerY = topLeft.IY;

			for(int yy = 0; yy < height; yy++) {
				int y = cornerY + yy;
				if(y < CGUI.GameTop || y > CGUI.GameBottom)
					continue;

				int x = cornerX;
				int minX = Math.Max(CGUI.GameLeft, x);
				int maxX = Math.Min(x + width, CGUI.GameRight + 1);
				string lineOutput = "";
				for(int xx = minX; xx < maxX; xx++)
					lineOutput += symbolLines[yy][xx - x];

				Console.CursorTop = y;
				Console.CursorLeft = minX;
				Console.Write(lineOutput);
			}
		}
	}

	public class ConsoleComplexRender2D: IDisplay {
		private readonly RenderComponent symbol;
		private readonly bool centered;

		public ConsoleComplexRender2D(RenderComponent symbol, bool centered = false) {
			this.symbol = symbol;
			this.centered = centered;
		}

		public void Draw(Vec2 pos) {
			int width = this.symbol.Width;
			int height = this.symbol.Height;

			Vec2 topLeft = pos;
			if(this.centered)
				topLeft = pos.Offset(-width / 2, -height / 2);
			int cornerX = topLeft.IX;
			int cornerY = topLeft.IY;

			for(int yy = 0; yy < height; yy++) {
				int y = cornerY + yy;
				if(y < CGUI.GameTop || y > CGUI.GameBottom)
					continue;

				int x = cornerX;
				int maxX = Math.Min(width, CGUI.GameRight + 1 - x);
				string outputBuffer = "";
				ConsoleColor outputColor = this.symbol.RowAt(0).CellAt(0).Color;
				Console.ForegroundColor = outputColor;
				Console.CursorLeft = Math.Max(CGUI.GameLeft, x);
				Console.CursorTop = y;

				for(int xx = Math.Max(0, CGUI.GameLeft - x); xx < maxX; xx++) {
					RenderCell symbol = this.symbol.RowAt(yy).CellAt(xx);

					if(outputColor == symbol.Color)
						outputBuffer += symbol.Symbol;
					else {
						Console.Write(outputBuffer);
						Console.ForegroundColor = symbol.Color;
						outputBuffer = symbol.Symbol.ToString();
						outputColor = symbol.Color;
					}
				}

				if(outputBuffer.Length > 0)
					Console.Write(outputBuffer);
			}
		}
	}
}
