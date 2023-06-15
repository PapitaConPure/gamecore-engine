using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore {
	/// <summary>
	/// Representa un rectángulo
	/// </summary>
	public struct Rect {
		#region Atributos básicos
		/// <summary>
		/// <see cref="Vec2"/> correspondiente a la esquina superior izquierda
		/// </summary>
		private Vec2 v1;
		/// <summary>
		/// <see cref="Vec2"/> correspondiente a la esquina inferior derecha
		/// </summary>
		private Vec2 v2;
		#endregion

		#region Operadores
		public static Rect operator +(Rect rect) => rect;
		public static Rect operator -(Rect rect) => new Rect(-rect.V1, -rect.V2);
		public static Rect operator +(Rect rect, Vec2 vector) => rect.Offset(vector);
		public static Rect operator -(Rect rect, Vec2 vector) => rect.Offset(-vector);
		public static Rect operator +(Rect left, Rect right) => left.Transform(right);
		public static Rect operator -(Rect left, Rect right) => left.Transform(-right);
		public static Rect operator *(double scalar, Rect rect) => new Rect(rect.v1 * scalar, rect.v2 * scalar);
		public static Rect operator *(Rect rect, double scalar) => new Rect(rect.v1 * scalar, rect.v2 * scalar);
		public static Rect operator /(Rect rect, double scalar) => new Rect(rect.v1 / scalar, rect.v2 / scalar);
		public static Rect operator *(Rect left, Rect right) => new Rect(left.V1 * right.V1, left.V2 * right.V2);
		public static Rect operator /(Rect left, Rect right) => new Rect(left.V1 / right.V1, left.V2 / right.V2);
		public static bool operator ==(Rect left, Rect right) => left.Equals(right);
		public static bool operator !=(Rect left, Rect right) => left.V1 != right.V1 || left.V2 != right.V2;
		#endregion

		#region Constructores, inicializadores y prefabricados
		/// <summary>
		/// Crea un rectángulo según las coordenadas especificadas
		/// </summary>
		/// <param name="x1">Coordenada X del primer vértice del rectángulo</param>
		/// <param name="y1">Coordenada Y del primer vértice del rectángulo</param>
		/// <param name="x2">Coordenada X del vértice opuesto del rectángulo</param>
		/// <param name="y2">Coordenada Y del vértice opuesto del rectángulo</param>
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
			this.v1 = v1;
			this.v2 = v2;

			this.OrderVecs();
		}
		/// <summary>
		/// Crea un <see cref="Rect"/> a partir de otro 
		/// </summary>
		/// <param name="rect"><see cref="Rect"/> modelo</param>
		public Rect(Rect rect) {
			this.v1 = rect.V1;
			this.v2 = rect.V2;
		}
		#endregion

		#region Propiedades básicas
		/// <inheritdoc cref="v1"/>
		public Vec2 V1 { get => this.v1; set => this.v1 = value; }
		/// <inheritdoc cref="v2"/>
		public Vec2 V2 { get => this.v2; set => this.v2 = value; }
		/// <summary>
		/// Primer vértice del <see cref="Rect"/> con sus componentes redondeados al entero más cercano
		/// </summary>
		public Vec2 IV1 => this.v1.Rounded;
		/// <summary>
		/// Vértice opuesto del <see cref="Rect"/> con sus componentes redondeados al entero más cercano
		/// </summary>
		public Vec2 IV2 => this.v2.Rounded;
		/// <summary>
		/// Ancho del <see cref="Rect"/> basado en sus vértices
		/// </summary>
		public double W => this.v2.X - this.v1.X;
		/// <summary>
		/// Alto del <see cref="Rect"/> basado en sus vértices
		/// </summary>
		public double H => this.v2.Y - this.v1.Y;
		/// <summary>
		/// Ancho entero del <see cref="Rect"/> basado en sus vértices redondeados al entero más cercano
		/// </summary>
		public int IW => this.v2.IX - this.v1.IX;
		/// <summary>
		/// Alto entero del <see cref="Rect"/> basado en sus vértices redondeados al entero más cercano
		/// </summary>
		public int IH => this.v2.IY - this.v1.IY;
		/// <summary>
		/// Área del <see cref="Rect"/> basada en sus vértices
		/// </summary>
		public double A => this.W * this.H;
		/// <summary>
		/// Área entera del <see cref="Rect"/> basada en sus vértices redondeados al entero más cercano
		/// </summary>
		public int IA => this.IW * this.IH;
		#endregion

		#region Propiedades de rectángulos derivados
		/// <summary>
		/// <see cref="Rect"/> con ambos vértices en la posición (0,0)
		/// </summary>
		public static Rect Zero => new Rect();
		/// <summary>
		/// <see cref="Rect"/> con ambos vértices en la posición (1,1)
		/// </summary>
		public static Rect One => new Rect(Vec2.One, Vec2.One);
		/// <summary>
		/// <see cref="Rect"/> ubicado en el cuadrante superior derecho
		/// </summary>
		public static Rect QuadrantI => new Rect(Vec2.Up, Vec2.Right);
		/// <summary>
		/// <see cref="Rect"/> ubicado en el cuadrante superior izquierdo
		/// </summary>
		public static Rect QuadrantII => new Rect(-Vec2.One, Vec2.Zero);
		/// <summary>
		/// <see cref="Rect"/> ubicado en el cuadrante inferior izquierdo
		/// </summary>
		public static Rect QuadrantIII => new Rect(Vec2.Left, Vec2.Down);
		/// <summary>
		/// <see cref="Rect"/> ubicado en el cuadrante inferior derecho
		/// </summary>
		public static Rect QuadrantIV => new Rect(Vec2.Zero, Vec2.One);
		/// <summary>
		/// <see cref="Rect"/> con su primer vértice en la posición (-1, -1) y su vértice opuesto en la posición (1,1)
		/// </summary>
		public static Rect Full => new Rect(-Vec2.One, Vec2.One);

		/// <summary>
		/// Versión del <see cref="Rect"/> con su primer vértice y vértice opuesto redondeados al entero más cercano
		/// </summary>
		public Rect Rounded => new Rect(this.IV1, this.IV2);
		#endregion

		#region Propiedades de cálculos relacionados
		public double Left => this.v1.X;
		public double Right => this.v2.X;
		public double Top => this.v1.Y;
		public double Bottom => this.v2.Y;

		public Vec2 Center => (this.V1 + this.V2) / 2;

		public Vec2 RoundedCenter => ((this.V1 + this.V2) / 2).Rounded;
		#endregion

		#region Métodos de cálculos relacionados
		/// <summary>
		/// Calcula el desplazamiento de este <see cref="Rect"/> por la cantidad especificada para ambas esquinas
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="x">Desplazamiento horizontal</param>
		/// <param name="y">Desplazamiento vertical</param>
		/// <returns>El <see cref="Rect"/> resultante del desplazamiento</returns>
		/// <seealso cref="Offset(Vec2)"/>
		public Rect Offset(double x, double y) {
			return new Rect(this.v1.Offset(x, y), this.v2.Offset(x, y));
		}
		/// <summary>
		/// Calcula el desplazamiento de este <see cref="Rect"/> según el <see cref="Vec2"/> especificado para ambas esquinas
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="offset">Vector de desplazamiento</param>
		/// <returns>El <see cref="Rect"/> resultante del desplazamiento</returns>
		/// <seealso cref="Offset(double, double)"/>
		public Rect Offset(Vec2 v) {
			return new Rect(this.v1 + v, this.v2 + v);
		}

		/// <summary>
		/// Calcula la transformación de este <see cref="Rect"/> según las cantidades especificadas para cada esquina
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="x1">Desplazamiento horizontal de la primer esquina</param>
		/// <param name="y1">Desplazamiento vertical de la primer esquina</param>
		/// <param name="x2">Desplazamiento horizontal de la esquina opuesta</param>
		/// <param name="y2">Desplazamiento vertical de la esquina opuesta</param>
		/// <returns>El <see cref="Rect"/> resultante de la transformación</returns>
		/// <seealso cref="Transform(Vec2, Vec2)"/>
		/// <seealso cref="Transform(Rect)"/>
		public Rect Transform(double x1, double y1, double x2, double y2) {
			return new Rect(this.v1.Offset(x1, y1), this.v2.Offset(x2, y2));
		}
		/// <summary>
		/// Calcula la transformación de este <see cref="Rect"/> según las cantidades especificadas para cada esquina
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="v1">Vector de desplazamiento de la primer esquina</param>
		/// <param name="v2">Vector de desplazamiento de la esquina opuesta</param>
		/// <returns>El <see cref="Rect"/> resultante de la transformación</returns>
		/// <seealso cref="Transform(double, double, double, double)"/>
		/// <seealso cref="Transform(Rect)"/>
		public Rect Transform(Vec2 v1, Vec2 v2) {
			return new Rect(this.v1 + v1, this.v2 + v2);
		}
		/// <summary>
		/// Calcula la transformación de este <see cref="Rect"/> según el otro <see cref="Rect"/> especificado.
		/// Se desplaza la primer esquina de este con la primer esquina del otro y la esquina opuesta de este con esquina opuesta del otro
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="offset">Rectángulo de transformación</param>
		/// <returns>El <see cref="Rect"/> resultante del desplazamiento</returns>
		/// <seealso cref="Transform(double, double, double, double)"/>
		/// <seealso cref="Transform(Vec2, Vec2)"/>
		public Rect Transform(Rect r) {
			return new Rect(this.v1 + r.V1, this.v2 + r.V2);
		}

		/// <summary>
		/// Calcula un <see cref="Rect"/> que mide <paramref name="scale"/> lo que el original.
		/// El escalado se realiza desde el centro del <see cref="Rect"/> hacia afuera
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="scale">Escalado del <see cref="Rect"/> (1 = original)</param>
		/// <returns>El <see cref="Rect"/> resultante del escalado</returns>
		public Rect ScaleOutwards(double scale) {
			Vec2 outPush = new Vec2(this.W, this.H) * scale / 2;
			return new Rect(this.Center - outPush, this.Center + outPush);
		}

		/// <summary>
		/// Calcula un <see cref="Rect"/> que mide <paramref name="factor"/> veces lo que el original.
		/// El escalado se realiza desde (0,0) hacia afuera
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Rect"/> original</remarks>
		/// <param name="factor">Factor de escalado del <see cref="Rect"/> (1 = original)</param>
		/// <returns>El <see cref="Rect"/> resultante del escalado</returns>
		public Rect ScaleGlobal(double factor) {
			return new Rect(this.V1, this.V2) * factor;
		}

		/// <summary>
		/// Calcula el <see cref="Rect"/> resultante de una interpolación bilinear entre este <see cref="Rect"/> y <paramref name="end"/>, con una proporción especificada por <paramref name="proportion"/>
		/// </summary>
		/// <param name="end">Recta resultante de una proporción de 100%</param>
		/// <param name="proportion">Proporción de la interpolación, siendo 0 el inicio y 1 el final</param>
		/// <returns>El <see cref="Rect"/> resultante de la interpolación</returns>
		public Rect Berp(Rect end, double proportion) {
			Vec2 vv1 = this.v1.Lerp(end.V1, proportion);
			Vec2 vv2 = this.v2.Lerp(end.V2, proportion);

			return new Rect(vv1, vv2);
		}
		/// <summary>
		/// Calcula el <see cref="Rect"/> resultante de una interpolación bilinear entre este <see cref="Rect"/> y <paramref name="end"/>, con una proporción especificada por <paramref name="proportion"/>
		/// </summary>
		/// <param name="end">Recta resultante de una proporción de (100%, 100%)</param>
		/// <param name="proportion">Proporción de la interpolación para cada componente de las esquina, siendo 0 el inicio y 1 el final</param>
		/// <returns>El <see cref="Rect"/> resultante de la interpolación</returns>
		public Rect Berp(Rect end, Vec2 proportion) {
			Vec2 vv1 = this.v1.Lerp(end.V1, proportion.X);
			Vec2 vv2 = this.v2.Lerp(end.V2, proportion.Y);

			return new Rect(vv1, vv2);
		}

		/// <summary>
		/// Calcula la unión entre dos <see cref="Rect"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Rect? Union(Rect other) {
			if(!this.Intersects(other))
				return null;

			Rect r = new Rect(this.V1, this.V2);
			r.Clamp(other);
			return r;
		}

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
		#endregion

		#region Métodos de manipulación de vectores
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		public void Clamp(Rect other) {
			this.V1.Clamp(other.V1, other.V2);
			this.V2.Clamp(other.V1, other.V2);
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
		#endregion

		#region Métodos generales de clase
		/// <summary>
		/// Devuelve una cadena que muestra los componentes de este <see cref="Vec2"/> como "(X, Y)"
		/// </summary>
		/// <returns>Una cadena que representa el <see cref="Vec2"/> actual</returns>
		public override string ToString() => "{ " + $"{this.v1}, {this.v2}" + " }";
		public override bool Equals(object obj) => base.Equals(obj);
		public override int GetHashCode() => base.GetHashCode();
		#endregion
	}
}
