using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore {
	/// <summary>
	/// Representa un rectángulo
	/// </summary>
	public class Rect {
		/// <summary>
		/// <see cref="Vec2"/> correspondiente a la esquina superior izquierda
		/// </summary>
		private Vec2 v1;
		/// <summary>
		/// <see cref="Vec2"/> correspondiente a la esquina inferior derecha
		/// </summary>
		private Vec2 v2;

		/// <summary>
		/// Crea un rectángulo según las coordenadas especificadas
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public Rect(double x1, double y1, double x2, double y2) {
			this.v1 = new Vec2(x1, y1);
			this.v2 = new Vec2(x2, y2);

			this.OrderVecs();
		}
		/// <summary>
		/// Crea un rectángulo a partir de dos <see cref="Vec2"/> especificados
		/// </summary>
		/// <param name="v1">Primer <see cref="Vec2"/> del rectángulo</param>
		/// <param name="v2"><see cref="Vec2"/> opuesto del rectángulo</param>
		public Rect(Vec2 v1, Vec2 v2) {
			this.v1 = v1.Copy;
			this.v2 = v2.Copy;

			this.OrderVecs();
		}
		/// <summary>
		/// Crea un <see cref="Rect"/> a partir de otro 
		/// </summary>
		/// <param name="rect"><see cref="Rect"/> modelo</param>
		public Rect(Rect rect) {
			this.v1 = rect.V1.Copy;
			this.v2 = rect.V2.Copy;
		}

		/// <inheritdoc cref="v1"/>
		public Vec2 V1 => this.v1;
		/// <inheritdoc cref="v2"/>
		public Vec2 V2 => this.v2;
		/// <summary>
		/// Primer vértice del <see cref="Rect"/> con sus componentes redondeados al entero más cercano
		/// </summary>
		public Vec2 IV1 => this.v1.Rounded;
		/// <summary>
		/// Vértice opuesto del <see cref="Rect"/> con sus componentes redondeados al entero más cercano
		/// </summary>
		public Vec2 IV2 => this.v2.Rounded;
		/// <summary>
		/// Nueva copia idéntica del <see cref="Rect"/>
		/// </summary>
		public Rect Copy => new Rect(this);
		/// <summary>
		/// Versión del <see cref="Rect"/> con su primer vértice y vértice opuesto redondeados al entero más cercano
		/// </summary>
		public Rect Rounded => new Rect(this.IV1, this.IV2);
		/// <summary>
		/// Nuevo <see cref="Vec2"/> con cada componente en 1 si es positivo, -1 si es negativo ó 0 si es cero
		/// </summary>

		/// <summary>
		/// Determina si un punto se encuentra dentro de este <see cref="Rect"/>
		/// </summary>
		/// <param name="x">Coordenada X del punto a evaluar</param>
		/// <param name="y">Coordenada Y del punto a evaluar</param>
		/// <returns><see langword="true"/> si el punto está dentro</returns>
		public bool Inside(double x, double y) {
			return x >= this.v1.X && x <= this.V2.X
				&& y >= this.v1.Y && y <= this.v2.Y;
		}
		/// <summary>
		/// Determina si un <see cref="Vec2"/> se encuentra dentro de este <see cref="Rect"/>
		/// </summary>
		/// <param name="x">Coordenada X del punto a evaluar</param>
		/// <param name="y">Coordenada Y del punto a evaluar</param>
		/// <returns><see langword="true"/> si el <see cref="Vec2"/> está dentro</returns>
		public bool Inside(Vec2 other) {
			return this.Inside(other.X, other.Y);
		}
		/// <summary>
		/// Determina si ocurre una intersección entre este <see cref="Rect"/> y otro
		/// </summary>
		/// <param name="other">Objetivo a comprobar</param>
		/// <returns><see langword="true"/> si los <see cref="Rect"/> se superponen</returns>
		public bool Intersects(Rect other) {
			if(other.V1.X > this.v2.X || other.V2.X < this.v1.X)
				return false;

			if(other.V1.Y > this.v2.Y || other.V2.Y < this.v1.Y)
				return false;

			return true;
		}
		/// <summary>
		/// Determina si ocurre una intersección entre este <see cref="Rect"/> y otro rectángulo
		/// </summary>
		/// <param name="v1">Primer <see cref="Vec2"/> del rectángulo a evaluar</param>
		/// <param name="v2"><see cref="Vec2"/> opuesto del rectángulo a evaluar</param>
		/// <returns><see langword="true"/> si se verifica una superposición</returns>
		public bool Intersects(Vec2 v1, Vec2 v2) {
			Rect rect = new Rect(v1, v2);
			return this.Intersects(rect);
		}

		/// <summary>
		/// Ordena los <see cref="Vec2"/> asociados a este <see cref="Rect"/> de forma tal que el primero represente la esquina superior izquierda y el segundo la inferior derecha
		/// </summary>
		private void OrderVecs() {
			if(this.v1.X > this.v2.X) {
				double xt = this.v1.X;
				this.v1.X = this.v2.X;
				this.v2.X = xt;
			}

			if(this.v1.Y > this.v2.Y) {
				double yt = this.v1.Y;
				this.v1.Y = this.v2.Y;
				this.v2.Y = yt;
			}
		}
	}
}
