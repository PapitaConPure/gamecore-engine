using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameCore;

namespace pruebaforms {
	public partial class Form1: Form {
		public Form1() {
			this.InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			Game.Target = GameTarget.Forms;
			Game.Debug = true;

			GUI.PrepareConsole("Juego de naves");
			GUI.GameArea = new Rect(1, 1, 59, 23);

			Game.Loop();
		}
	}
}
