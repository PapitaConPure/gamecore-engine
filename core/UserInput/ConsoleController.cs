using System;

namespace GameCore {
	/// <summary>
	/// Representa un método de entrada de juego para aplicaciones de consola
	/// </summary>
	public class ConsoleController: Controller {
		public override void Update(long ticks) {
			this.currentButton = new GameInput();

			GameButton tempKey = this.GetGameButton();

			if(tempKey == GameButton.None)
				return;

			this.previousButton = lastButton;
			this.lastButton = new GameInput(tempKey, ticks);
			this.currentButton = lastButton;
		}

		private GameButton GetGameButton() {
			if(!Console.KeyAvailable)
				return GameButton.None;

			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			
			switch(keyInfo.Key) {
			case ConsoleKey.UpArrow:	return GameButton.Up;
			case ConsoleKey.DownArrow:  return GameButton.Down;
			case ConsoleKey.LeftArrow:  return GameButton.Left;
			case ConsoleKey.RightArrow: return GameButton.Right;
			case ConsoleKey.Q:		    return GameButton.QKey;
			case ConsoleKey.W:		    return GameButton.WKey;
			case ConsoleKey.E:		    return GameButton.EKey;
			case ConsoleKey.R:		    return GameButton.RKey;
			case ConsoleKey.T:		    return GameButton.TKey;
			case ConsoleKey.Y:		    return GameButton.YKey;
			case ConsoleKey.U:		    return GameButton.UKey;
			case ConsoleKey.I:		    return GameButton.IKey;
			case ConsoleKey.O:		    return GameButton.OKey;
			case ConsoleKey.P:		    return GameButton.PKey;
			case ConsoleKey.A:		    return GameButton.AKey;
			case ConsoleKey.S:		    return GameButton.SKey;
			case ConsoleKey.D:		    return GameButton.DKey;
			case ConsoleKey.F:		    return GameButton.FKey;
			case ConsoleKey.G:		    return GameButton.GKey;
			case ConsoleKey.H:		    return GameButton.HKey;
			case ConsoleKey.J:		    return GameButton.JKey;
			case ConsoleKey.K:		    return GameButton.KKey;
			case ConsoleKey.L:		    return GameButton.LKey;
			case ConsoleKey.Z:		    return GameButton.ZKey;
			case ConsoleKey.X:		    return GameButton.XKey;
			case ConsoleKey.C:		    return GameButton.CKey;
			case ConsoleKey.V:		    return GameButton.VKey;
			case ConsoleKey.B:		    return GameButton.BKey;
			case ConsoleKey.N:		    return GameButton.NKey;
			case ConsoleKey.M:		    return GameButton.MKey;
			case ConsoleKey.Spacebar:   return GameButton.Space;
			case ConsoleKey.Enter:	    return GameButton.Enter;
			case ConsoleKey.Escape:	    return GameButton.Escape;
			case ConsoleKey.Tab:		return GameButton.Tab;
			}

			if(keyInfo.Modifiers == 0)
				return GameButton.None;
			if((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
				return GameButton.Control;
			if((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
				return GameButton.Shift;
			if((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
				return GameButton.Alt;

			return GameButton.None;
		}

		public override void SubscribeToWindow(GameWindow _) {
			throw new InvalidOperationException("No se puede escuchar a la ventana principal en una aplicación de consola");
		}
	}
}
