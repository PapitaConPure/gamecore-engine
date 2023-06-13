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

		private static void DrawBasicUI() {
			//Líneas horizontales
			string hbar1 = new string('═', GUI.GameRight);
			string hbar2 = new string('═', GUI.UIRight - GUI.GameRight - 2);
			Console.WriteLine("╔" + hbar1 + "╦" + hbar2 + "╗");
			Console.CursorTop = 24;
			Console.WriteLine("╚" + hbar1 + "╩" + hbar2 + "╝");
			Console.CursorTop = 0; //Evita scrollear hacia abajo

			//Líneas verticales
			GUI.DrawVerticalLine(new Vec2(0, 1), GUI.GameBottom, '║');
			GUI.DrawVerticalLine(new Vec2(GUI.GameRight + 1, 1), GUI.GameBottom, '║');
			GUI.DrawVerticalLine(new Vec2(GUI.UIRight, 1), GUI.GameBottom, '║');

			//Panel vertical
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			int panelWidth = GUI.UIRight - GUI.GameRight - 2;
			string filler = new string(' ', panelWidth);
			for(int i = 1; i < GUI.GameBottom - GUI.GameTop + 2; i++) {
				Console.SetCursorPosition(GUI.GameRight + 2, i);
				Console.Write(filler);
			}

			string diffName = "~NORMAL~";
			Console.CursorTop = 2;
			Console.CursorLeft = GUI.CenteredStringOffset(GUI.GameRight + 1, GUI.UIRight, diffName);
			Console.BackgroundColor = ConsoleColor.Cyan;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(diffName);
			Console.ResetColor();
		}
	}
}