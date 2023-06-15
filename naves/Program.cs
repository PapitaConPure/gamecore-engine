using System;
using System.Collections.Generic;
using System.Threading;
using GameCore;

namespace naves {
	class Program {
		static void Main(string[] _) {
			Game.Target = GameTarget.Console;
			Game.Debug = true;

			GUI.PrepareConsole("Juego de naves");
			GUI.GameArea = new Rect(1, 1, 59, 23);
			DrawBasicUI();

			Player player = new Player(new Vec2(GUI.GameCenter, GUI.GameBottom - 6));
			Game.AddInstance(player);

			StageSequencer sequencer = new StageSequencer(new List<Spawner>());
			Game.AddInstance(sequencer);

			PromptGameStart();
			Game.Loop();
			ShowGameOver();
		}

		private static void PromptGameStart() {
			string bottomPrompt = "Comenzar...";
			int leftPos = GUI.CenteredStringOffset(GUI.GameRight + 1, GUI.UIRight, bottomPrompt);
			Console.SetCursorPosition(leftPos, GUI.GameBottom - 1);
			Console.Write(bottomPrompt);
			Console.ReadKey(true);
			bottomPrompt = "Esc. para salir ";
			leftPos = GUI.CenteredStringOffset(GUI.GameRight + 1, GUI.UIRight, bottomPrompt);
			Console.CursorLeft = leftPos;
			Console.Write(bottomPrompt);
		}

		private static void ShowGameOver() {
			Console.ResetColor();
			DrawBasicUI();
			GUI.EmptyGameFrame();
			Console.SetCursorPosition(GUI.GameCenter, GUI.GameMiddle);
			Console.Write("- FIN DEL JUEGO -");
			Thread.Sleep(3000);
			Console.ReadKey(true);
		}

		public static void DrawBasicUI() {
			//Líneas horizontales
			string hbar1 = new string('═', GUI.GameRight - GUI.GameLeft + 1);
			string hbar2 = new string('═', GUI.UIRight - GUI.GameRight - 2);

			int gbLeft = GUI.GameLeft - 1;
			int gbRight = GUI.GameRight + 1;
			int gbTop = GUI.GameTop - 1;
			int gbBottom = GUI.GameBottom + 1;

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.CursorTop = gbTop;
			Console.CursorLeft = gbLeft;
			Console.WriteLine("╔" + hbar1 + "╦" + hbar2 + "╗");
			Console.CursorTop = gbBottom;
			Console.CursorLeft = gbLeft;
			Console.WriteLine("╚" + hbar1 + "╩" + hbar2 + "╝");
			Console.CursorTop = 0; //Evita scrollear hacia abajo

			//Líneas verticales
			int uiHeight = GUI.GameBottom - GUI.GameTop + 1;
			GUI.DrawVerticalLine(new Vec2(gbLeft, GUI.GameTop), uiHeight, '║');
			GUI.DrawVerticalLine(new Vec2(gbRight, GUI.GameTop), uiHeight, '║');
			GUI.DrawVerticalLine(new Vec2(GUI.UIRight, GUI.GameTop), uiHeight, '║');

			//Panel vertical
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			int panelWidth = GUI.UIRight - GUI.GameRight - 2;
			string filler = new string(' ', panelWidth);
			for(int i = GUI.GameTop; i < gbBottom; i++) {
				Console.SetCursorPosition(GUI.GameRight + 2, i);
				Console.Write(filler);
			}

			string diffName = "~NORMAL~";
			Console.CursorTop = GUI.UITop + 1;
			Console.CursorLeft = GUI.CenteredStringOffset(gbRight, GUI.UIRight, diffName);
			Console.BackgroundColor = ConsoleColor.Cyan;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(diffName);
			Console.ResetColor();
		}
	}
}