using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace GameCore {
	class Wawa: GameObject {
		public Wawa(Vec2 pos): base(pos) {}
	}

	class Wawa2: GameObject {
		public Wawa2(Vec2 pos) : base(pos) { }

		protected override void MainUpdate(double deltaTime) {
			switch(Game.PressedButton.Button) {
			case GameButton.WKey: this.vel.Y = -4; break;
			case GameButton.AKey: this.vel.X = -4; break;
			case GameButton.SKey: this.vel.Y = +4; break;
			case GameButton.DKey: this.vel.X = +4; break;
			}

			switch(Game.ReleasedButton.Button) {
			case GameButton.AKey:
			case GameButton.DKey:
				this.vel.X = 0;
				break;
			case GameButton.WKey:
			case GameButton.SKey:
				this.vel.Y = 0;
				break;
			}
		}
	}

	class FormsRenderer: IRenderer {
		private readonly List<FormsWriteRequest> writeRequests = new List<FormsWriteRequest>();
		private readonly PictureBox pbViewport = new PictureBox();
		private readonly DirectBitmap bitmap;
		private RectCollider r;
		private CircleCollider c;

		public FormsRenderer(PictureBox viewport) {
			this.pbViewport = viewport;
			this.bitmap = new DirectBitmap(pbViewport.Width, pbViewport.Height);
			r = new RectCollider(new Wawa(new Vec2(200, 220)), new Rect(new Vec2(-40, -30), new Vec2(40, 30)));
			Wawa2 ww = new Wawa2(new Vec2(400, 240));
			Game.AddInstance(ww);
			c = new CircleCollider(ww, 60);
		}

		public void EmptyGameFrame() {
			Vec2 v;
			bool ri, ci;

			Color[] rColors;
			Color[] cColors;

			if(r.Intersect(c)) {
				rColors = new Color[] { Color.Red, Color.OrangeRed, Color.MediumVioletRed };
				cColors = new Color[] { Color.Yellow, Color.YellowGreen, Color.GreenYellow, Color.Gold };
			} else {
				rColors = new Color[] { Color.Aqua, Color.Cyan, Color.DarkCyan, Color.LightCyan };
				cColors = new Color[] { Color.BlueViolet, Color.Fuchsia, Color.Pink, Color.HotPink, Color.Lavender };
			}

			for(int y = 0; y < pbViewport.Height; y++)
				for(int x = 0; x < pbViewport.Width; x++)
					bitmap.SetPixel(x, y, Color.Black);

			#region Rectángulo de prueba
			Rect b = r.Bounds.Value;
			int x1 = MathX.Clamp(b.V1.IX, 0, pbViewport.Width);
			int x2 = MathX.Clamp(b.V2.IX, 0, pbViewport.Width);
			int y1 = MathX.Clamp(b.V1.IY, 0, pbViewport.Height);
			int y2 = MathX.Clamp(b.V2.IY, 0, pbViewport.Height);

			for(int y = y1; y <= y2; y++)
				for(int x = x1; x <= x2; x++) {
					v = new Vec2(x, y);

					ri = r.Inside(v);

					if(ri)
						bitmap.SetPixel(x, y, MathX.Choose(rColors));
				}
			#endregion

			#region Círculo de prueba
			b = c.Bounds.Value;
			x1 = MathX.Clamp(b.V1.IX, 0, pbViewport.Width - 1);
			x2 = MathX.Clamp(b.V2.IX, 0, pbViewport.Width - 1);
			y1 = MathX.Clamp(b.V1.IY, 0, pbViewport.Height - 1);
			y2 = MathX.Clamp(b.V2.IY, 0, pbViewport.Height - 1);

			for(int y = y1; y <= y2; y++)
				for(int x = x1; x <= x2; x++) {
					v = new Vec2(x, y);

					ci = c.Inside(v);

					if(ci)
						bitmap.SetPixel(x, y, MathX.Choose(cColors));
				}
			#endregion

			pbViewport.Image = bitmap.Bitmap;
		}

		public void RenderGameFrame(List<GameObject> instances) {
			foreach(GameObject instance in instances)
				instance.Draw();
		}

		public void DrawSurface() {

		}

		public void DrawTPS(double deltaTime) {
			//double tps = 1000 / MathUtils.Clamp(Clock.BaseTick + Math.Max(0, deltaTime - Clock.BaseTick), 1, 1000);
			//string ticksSuffix = " TPS";
			//int ticksCharacters = GUI.NumberCharacters(Math.Round(tps)) + 3 + /*ticksPrefix.Length*/ +ticksSuffix.Length;
			//return;
			//Console.ForegroundColor = ConsoleColor.White;
			//Console.BackgroundColor = ConsoleColor.DarkBlue;
			//Console.SetCursorPosition(GUI.UIRight - ticksCharacters - 2, GUI.GameBottom);
			//Console.Write(new string(' ', ticksCharacters + 2));
			//Console.SetCursorPosition(GUI.UIRight - ticksCharacters, GUI.GameBottom);
			//Console.Write(string.Format(new CultureInfo("en"), "{0:0.00}{1}", Math.Min(tps, 1000), ticksSuffix));
			//Console.ForegroundColor = ConsoleColor.Gray;
			//Console.BackgroundColor = ConsoleColor.Black;
		}

		public void DrawText(Vec2 pos, string text) {
			FormsWriteRequest request = new FormsWriteRequest(pos, text);
			writeRequests.Add(request);
		}
		public void DrawText(Vec2 pos, string text, ConsoleColor color) {
			FormsWriteRequest request = new FormsWriteRequest(pos, text, color);
			writeRequests.Add(request);
		}
		public void DrawText(Vec2 pos, string text, ConsoleColor fgColor, ConsoleColor bgColor) {
			FormsWriteRequest request = new FormsWriteRequest(pos, text, fgColor, bgColor);
			writeRequests.Add(request);
		}
	}

	public struct FormsWriteRequest {
		private readonly Vec2 pos;
		private readonly string text;
		private readonly ConsoleColor fgColor;
		private readonly ConsoleColor bgColor;

		public FormsWriteRequest(Vec2 pos, string text, ConsoleColor fgColor, ConsoleColor bgColor) {
			this.pos = pos;
			this.text = text;
			this.fgColor = fgColor;
			this.bgColor = bgColor;
		}
		public FormsWriteRequest(Vec2 pos, string text, ConsoleColor fgColor) {
			this.pos = pos;
			this.text = text;
			this.fgColor = fgColor;
			this.bgColor = ConsoleColor.Black;
		}
		public FormsWriteRequest(Vec2 pos, string text) {
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

	public class DirectBitmap: IDisposable {
		public Bitmap Bitmap { get; private set; }
		public int[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }

		public DirectBitmap(int width, int height) {
			this.Width = width;
			this.Height = height;
			this.Bits = new int[width * height];
			this.BitsHandle = GCHandle.Alloc(this.Bits, GCHandleType.Pinned);
			this.Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
		}

		public void SetPixel(int x, int y, Color colour) {
			int index = x + y * this.Width;
			int col = colour.ToArgb();

			this.Bits[index] = col;
		}

		public Color GetPixel(int x, int y) {
			int index = x + y * this.Width;
			int col = this.Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}

		public void Dispose() {
			if(this.Disposed) return;
			this.Disposed = true;
			this.Bitmap.Dispose();
			this.BitsHandle.Free();
		}
	}
}
