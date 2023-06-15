using System.Windows.Forms;

namespace GameCore {
	/// <summary>
	/// Representa un método de entrada de juego para aplicaciones de Forms
	/// </summary>
	public class FormsController: Controller {
		GameButton buttonDown;
		GameButton buttonHold;
		GameButton buttonUp;

		public override void Update(long ticks) {
			this.pressedButton = new GameInput();
			this.currentButton = new GameInput();
			this.releasedButton = new GameInput();

			if(buttonDown != GameButton.None) {
				this.currentButton = new GameInput(buttonDown, ticks);
				this.previousButton = this.lastButton;
				this.lastButton = this.currentButton;
				this.pressedButton = this.currentButton;
				buttonHold = buttonDown;
				buttonDown = GameButton.None;
			}
			
			if(buttonUp != GameButton.None) {
				this.releasedButton = new GameInput(buttonUp, ticks);
				buttonHold = GameButton.None;
				buttonUp = GameButton.None;
			}

			if(buttonHold != GameButton.None)
				this.currentButton = new GameInput(buttonDown, ticks);
		}

		public override void SubscribeToWindow(GameWindow window) {
			window.KeyDownEvent += this.OnKeyDown;
			window.KeyPressEvent += this.OnKeyPress;
			window.KeyUpEvent += this.OnKeyUp;
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			this.buttonDown = this.DetermineKey(e.KeyCode);
		}

		private void OnKeyPress(object sender, KeyPressEventArgs e) {
			//PENDIENTE: ver qué hacer con esto
		}

		private void OnKeyUp(object sender, KeyEventArgs e) {
			this.buttonUp = this.DetermineKey(e.KeyCode);
		}

		private GameButton DetermineKey(Keys keys) {
			switch(keys) {
			case Keys.Up:         return GameButton.Up;
			case Keys.Down:       return GameButton.Down;
			case Keys.Left:       return GameButton.Left;
			case Keys.Right:      return GameButton.Right;
			case Keys.Q:          return GameButton.QKey;
			case Keys.W:          return GameButton.WKey;
			case Keys.E:          return GameButton.EKey;
			case Keys.R:          return GameButton.RKey;
			case Keys.T:          return GameButton.TKey;
			case Keys.Y:          return GameButton.YKey;
			case Keys.U:          return GameButton.UKey;
			case Keys.I:          return GameButton.IKey;
			case Keys.O:          return GameButton.OKey;
			case Keys.P:          return GameButton.PKey;
			case Keys.A:          return GameButton.AKey;
			case Keys.S:          return GameButton.SKey;
			case Keys.D:          return GameButton.DKey;
			case Keys.F:          return GameButton.FKey;
			case Keys.G:          return GameButton.GKey;
			case Keys.H:          return GameButton.HKey;
			case Keys.J:          return GameButton.JKey;
			case Keys.K:          return GameButton.KKey;
			case Keys.L:          return GameButton.LKey;
			case Keys.Z:          return GameButton.ZKey;
			case Keys.X:          return GameButton.XKey;
			case Keys.C:          return GameButton.CKey;
			case Keys.V:          return GameButton.VKey;
			case Keys.B:          return GameButton.BKey;
			case Keys.N:          return GameButton.NKey;
			case Keys.M:          return GameButton.MKey;
			case Keys.Space:      return GameButton.Space;
			case Keys.Enter:      return GameButton.Enter;
			case Keys.Escape:     return GameButton.Escape;
			case Keys.Tab:        return GameButton.Tab;
			case Keys.Control:    return GameButton.Control;
			case Keys.Shift:      return GameButton.Shift;
			case Keys.Alt:        return GameButton.Alt;
			default:              return GameButton.None;
			}
		}
	}
}
