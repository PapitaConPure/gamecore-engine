using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using GameCore;

namespace GameCore {
	public partial class GameWindow: Form {
		private System.Threading.Timer timer;
		private double refreshRate = Clock.BaseTick / 2; //PENDIENTE: No tengo ni idea de por qué esto funciona
		private double deltaTime;
		private double lastUpdate = 0;
		private double currentDate;

		public event KeyEventHandler KeyDownEvent;
		public event KeyPressEventHandler KeyPressEvent;
		public event KeyEventHandler KeyUpEvent;

		public GameWindow() {
			this.InitializeComponent();
			//Application.Idle += this.HandleApplicationIdle;
		}

		private void GameWindow_Load(object sender, EventArgs e) {
			this.timer = new System.Threading.Timer(HandleApplicationIdle, null, 0, 1);
		}

		private void HandleApplicationIdle(object state) {
			lock(this) {
				if(!Game.Ended) {
					currentDate = DateTime.Now.Ticks / (double)TimeSpan.TicksPerMillisecond;
					deltaTime = currentDate - lastUpdate;
					if(deltaTime >= refreshRate) {
						Debug.WriteLine(deltaTime);
						lastUpdate = DateTime.Now.Ticks / (double)TimeSpan.TicksPerMillisecond;
						Game.UpdateGameState(deltaTime);
					}
				}
			}
		}

		private void GameWindow_KeyDown(object sender, KeyEventArgs e) {
			this.OnKeyDown(sender, e);
		}

		private void GameWindow_KeyPress(object sender, KeyPressEventArgs e) {
			this.OnKeyPress(sender, e);
		}

		private void GameWindow_KeyUp(object sender, KeyEventArgs e) {
			this.OnKeyUp(sender, e);
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			if(KeyDownEvent != null)
				KeyDownEvent.Invoke(sender, e);
		}

		private void OnKeyPress(object sender, KeyPressEventArgs e) {
			if(KeyPressEvent != null)
				KeyPressEvent.Invoke(sender, e);
		}

		private void OnKeyUp(object sender, KeyEventArgs e) {
			if(KeyUpEvent != null)
				KeyUpEvent.Invoke(sender, e);
		}
	}
}
