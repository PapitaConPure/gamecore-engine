using System;
using System.Globalization;
using System.Collections.Generic;

//PENDIENTE: volver más general el dibujado de interfaz
namespace GameCore {
    /// <summary>
    /// Representa una interfaz gráfica de consola
    /// </summary>
	public static class CGUI {
        static public int UIRight { get => 79; }
        static public int GameLeft { get => 1; }
        static public int GameCenter { get => Convert.ToInt32(Math.Ceiling((GameLeft + GameRight) / 2.0)); }
        static public int GameRight { get => 59; }
        static public int GameTop { get => 1; }
        static public int GameMiddle { get => Convert.ToInt32(Math.Ceiling((GameTop + GameBottom) / 2.0)); }
        static public int GameBottom { get => 23; }
        static public Vec2 GameTopLeft { get => new Vec2(GameLeft, GameTop); }
        static public Vec2 GameBottomRight { get => new Vec2(GameRight, GameBottom); }

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

        private static void DrawVerticalLine(int cursorTop, int cursorLeft, int height, char c) {
            int tempCursorTop = Console.CursorTop;
            int tempCursorLeft = Console.CursorLeft;

            for (int i = 0; i < height; i++) {
                Console.CursorTop = cursorTop + i;
                Console.CursorLeft = cursorLeft;
                Console.Write(c);
            }

            Console.CursorTop = tempCursorTop;
            Console.CursorLeft = tempCursorLeft;
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

        public static void EmptyGameFrame() {
            string emptyLine = new string(' ', GameRight - GameLeft + 1);
            for(int l = GameTop; l <= GameBottom; l++) {
                Console.CursorLeft = 1;
                Console.CursorTop = l;
                Console.Write(emptyLine);
            }
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
    }
}
