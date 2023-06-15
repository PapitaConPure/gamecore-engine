using System;
using System.Collections.Generic;

namespace GameCore {
	public class Display {
		private bool visible;
		private readonly IDisplay display;

		public Display(IDisplay drawer, bool visible = true) {
			this.visible = visible;
			this.display = drawer;
		}

		public bool Visible { get => this.visible; set => this.visible = value; }

		public void Draw(Vec2 pos) {
			if(!this.visible) return;
			this.display.Draw(pos);
		}
	}

	public static class MultiLineSymbol {
		public static string Create(params string[] symbolLines) {
			return string.Join("\n", symbolLines);
		}
	}

	public interface IDisplay {
		void Draw(Vec2 pos);
	}
}
