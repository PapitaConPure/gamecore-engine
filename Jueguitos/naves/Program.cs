using System;
using System.Collections.Generic;
using System.Threading;
using GameCore;

namespace naves {
    class Program {
        static void Main(string[] _) {
            Game.Target = GameTarget.Console;

            CGUI.PrepareConsole("Juego de naves");
            CGUI.DrawBasicUI();

            Player player = new Player(new Vec2(CGUI.GameCenter, CGUI.GameBottom - 6));
            Game.AddInstance(player);
            StageSequencer sequencer = new StageSequencer(new List<Spawner>());
            Game.AddInstance(sequencer);

            PromptGameStart();
            Game.Loop();
            ShowGameOver();
        }

        private static void PromptGameStart() {
            string bottomPrompt = "Comenzar...";
            int leftPos = CGUI.CenteredStringOffset(CGUI.GameRight + 1, CGUI.UIRight, bottomPrompt);
            Console.SetCursorPosition(leftPos, CGUI.GameBottom - 1);
            Console.Write(bottomPrompt);
            Console.ReadKey(true);
            bottomPrompt = "Esc. para salir ";
            leftPos = CGUI.CenteredStringOffset(CGUI.GameRight + 1, CGUI.UIRight, bottomPrompt);
            Console.CursorLeft = leftPos;
            Console.Write(bottomPrompt);
        }

        private static void ShowGameOver() {
            Console.ResetColor();
            CGUI.DrawBasicUI();
            CGUI.EmptyGameFrame();
            Console.SetCursorPosition(CGUI.GameCenter, CGUI.GameMiddle);
            Console.Write("- FIN DEL JUEGO -");
            Thread.Sleep(3000);
            Console.ReadKey(true);
        }
    }
}