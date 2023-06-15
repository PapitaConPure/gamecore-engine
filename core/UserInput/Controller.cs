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
		/// <see cref="GameInput"/> del botón anterior al último presionado
		/// </summary>
		protected GameInput previousButton;
		/// <summary>
		/// <see cref="GameInput"/> del último botón presionado
		/// </summary>
		protected GameInput lastButton;
		/// <summary>
		/// <see cref="GameInput"/> del botón actualmente presionado
		/// </summary>
		protected GameInput currentButton;
		/// <summary>
		/// <see cref="GameInput"/> del botón recién presionado
		/// </summary>
		/// <remarks>No implementado en aplicaciones de consola</remarks>
		protected GameInput pressedButton;
		/// <summary>
		/// <see cref="GameInput"/> del botón recién soltado
		/// </summary>
		/// <remarks>No implementado en aplicaciones de consola</remarks>
		protected GameInput releasedButton;

		/// <inheritdoc cref="this.previousButton"/>
		public GameInput PreviousButton => this.previousButton;
		/// <inheritdoc cref="this.lastButton"/>
		public GameInput LastButton => this.lastButton;
		/// <inheritdoc cref="this.currentButton"/>
		public GameInput CurrentButton => this.currentButton;
		/// <inheritdoc cref="this.pressedButton"/>
		public GameInput PressedButton => this.pressedButton;
		/// <inheritdoc cref="this.releasedButton"/>
		public GameInput ReleasedButton => this.releasedButton;

		/// <summary>
		/// Detecta nuevas interacciones del usuario y actualiza <see cref="PreviousButton"/>, <see cref="LastButton"/> y <see cref="CurrentButton"/>
		/// </summary>
		/// <remarks><see cref="CurrentButton"/> corresponderá a la tecla detectada en el tick que se la detectó</remarks>
		/// <param name="ticks"></param>
		public abstract void Update(long ticks);

		public bool ButtonPressed(GameButton button) {
			return this.currentButton.Button == button;
		}

		public bool ButtonReleased(GameButton button) {
			if(this.lastButton.Button == this.currentButton.Button)
				return true;

			return this.lastButton.Button == button;
		}

		public abstract void SubscribeToWindow(GameWindow window);
	}
}
