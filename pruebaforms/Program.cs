using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameCore;

namespace pruebaforms {
	static class Program {
		/// <summary>
		/// Punto de entrada principal para la aplicación.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Game.Target = GameTarget.Forms;
			Clock.TicksPerSecond = 60;
			GUI.PrepareForms("jueguito siiiiiii", 640, 480);
		}
	}
}
