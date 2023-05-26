using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore {
	public enum GameButton {
		None = 0,

		Up,
		Down,
		Left,
		Right,

		AButton,
		BButton,
		XButton,
		YButton,
		LB,
		RB,
		ZB,
		LT,
		RT,
		LeftStick,
		RightStick,
		Select,
		Menu,
		DPadUp,
		DPadDown,
		DPadLeft,
		DPadRight,

		QKey,
		WKey,
		EKey,
		RKey,
		TKey,
		YKey,
		UKey,
		IKey,
		OKey,
		PKey,
		AKey,
		SKey,
		DKey,
		FKey,
		GKey,
		HKey,
		JKey,
		KKey,
		LKey,
		ZKey,
		XKey,
		CKey,
		VKey,
		BKey,
		NKey,
		MKey,

		Space,
		Enter,
		Escape,
		Tab,
		Shift,
		Control,
		Alt,

		M1,
		M2,
		M3,
		M4,
		M5
	}

	/// <summary>
	/// Representa un presionado de botón en el juego
	/// </summary>
	public struct GameInput {
		/// <summary>
		/// El botón presionado
		/// </summary>
		private readonly GameButton button;
		/// <summary>
		/// El tick en el cual se presionó el botón
		/// </summary>
		private readonly long tick;

		/// <summary>
		/// Crea una instancia de presionado de botón
		/// </summary>
		/// <param name="button">El botón presionado, <see cref="GameButton.None"/> indica que ninguno se presionó en ese momento</param>
		/// <param name="tick">El tick en el cual se presionó el botón</param>
		public GameInput(GameButton button, long tick) {
			this.button = button;
			this.tick = tick;
		}

		/// <inheritdoc cref="button"/>
		public GameButton Button => button;
		/// <inheritdoc cref="tick"/>
		public long Tick => tick;
	}

	/// <summary>
	/// Representa un método de entrada de usuario, o control de juego
	/// </summary>
	public abstract class Controller {
		/// <summary>
		/// <see cref="GameInput"/> de la tecla anterior a la última presionada
		/// </summary>
		private GameInput previousKey;
		/// <summary>
		/// <see cref="GameInput"/> de la última tecla presionada
		/// </summary>
		private GameInput lastKey;
		/// <summary>
		/// <see cref="GameInput"/> de la tecla actualmente presionada
		/// </summary>
		private GameInput currentKey;

		/// <inheritdoc cref="previousKey"/>
		public GameInput PreviousKey => previousKey;
		/// <inheritdoc cref="lastKey"/>
		public GameInput LastKey => lastKey;
		/// <inheritdoc cref="currentKey"/>
		public GameInput CurrentKey => currentKey;

		/// <summary>
		/// Detecta nuevas interacciones del usuario y actualiza <see cref="PreviousKey"/>, <see cref="LastKey"/> y <see cref="CurrentKey"/>
		/// </summary>
		/// <remarks><see cref="CurrentKey"/> corresponderá a la tecla detectada en el tick que se la detectó</remarks>
		/// <param name="ticks"></param>
		public void Update(long ticks) {
			currentKey = new GameInput();

			GameButton tempKey = this.GetGameButton();

			if(tempKey == GameButton.None)
				return;

			previousKey = lastKey;
			lastKey = new GameInput(tempKey, ticks);
			currentKey = lastKey;
		}

		protected abstract GameButton GetGameButton();
	}

	/// <summary>
	/// Representa un método de entrada de juego para aplicaciones de consola
	/// </summary>
	public class ConsoleController: Controller {
		protected override GameButton GetGameButton() {
			if(!Console.KeyAvailable)
				return GameButton.None;

			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			
			switch(keyInfo.Key) {
			case ConsoleKey.UpArrow:    return GameButton.Up;
			case ConsoleKey.DownArrow:  return GameButton.Down;
			case ConsoleKey.LeftArrow:  return GameButton.Left;
			case ConsoleKey.RightArrow: return GameButton.Right;
			case ConsoleKey.Q:          return GameButton.QKey;
			case ConsoleKey.W:          return GameButton.WKey;
			case ConsoleKey.E:          return GameButton.EKey;
			case ConsoleKey.R:          return GameButton.RKey;
			case ConsoleKey.T:          return GameButton.TKey;
			case ConsoleKey.Y:          return GameButton.YKey;
			case ConsoleKey.U:          return GameButton.UKey;
			case ConsoleKey.I:          return GameButton.IKey;
			case ConsoleKey.O:          return GameButton.OKey;
			case ConsoleKey.P:          return GameButton.PKey;
			case ConsoleKey.A:          return GameButton.AKey;
			case ConsoleKey.S:          return GameButton.SKey;
			case ConsoleKey.D:          return GameButton.DKey;
			case ConsoleKey.F:          return GameButton.FKey;
			case ConsoleKey.G:          return GameButton.GKey;
			case ConsoleKey.H:          return GameButton.HKey;
			case ConsoleKey.J:          return GameButton.JKey;
			case ConsoleKey.K:          return GameButton.KKey;
			case ConsoleKey.L:          return GameButton.LKey;
			case ConsoleKey.Z:          return GameButton.ZKey;
			case ConsoleKey.X:          return GameButton.XKey;
			case ConsoleKey.C:          return GameButton.CKey;
			case ConsoleKey.V:          return GameButton.VKey;
			case ConsoleKey.B:          return GameButton.BKey;
			case ConsoleKey.N:          return GameButton.NKey;
			case ConsoleKey.M:          return GameButton.MKey;
			case ConsoleKey.Spacebar:   return GameButton.Space;
			case ConsoleKey.Enter:      return GameButton.Enter;
			case ConsoleKey.Escape:     return GameButton.Escape;
			case ConsoleKey.Tab:        return GameButton.Tab;
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
	}

	/// <summary>
	/// Representa un método de entrada de juego para aplicaciones de Forms
	/// </summary>
	public class FormsController: Controller {
		protected override GameButton GetGameButton() {
			return GameButton.None;
		}
	}
}
