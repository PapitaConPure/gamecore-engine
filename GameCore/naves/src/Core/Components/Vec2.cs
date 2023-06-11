using System;

namespace GameCore {
	/// <summary>
	/// Representa un vector de 2 dimensiones
	/// </summary>
	public class Vec2 {
		#region Atributos básicos
		/// <summary>
		/// Componente horizontal del <see cref="Vec2"/>
		/// </summary>
		private double x;
		/// <summary>
		/// Componente vertical del <see cref="Vec2"/>
		/// </summary>
		private double y;
		#endregion

		#region Operadores
		public static Vec2 operator +(Vec2 v) => v;
		public static Vec2 operator -(Vec2 v) => new Vec2(-v.X, -v.Y);
		public static Vec2 operator +(Vec2 v, double n) => v.Offset(n, n);
		public static Vec2 operator -(Vec2 v, double n) => v.Offset(-n, -n);
		public static Vec2 operator +(Vec2 a, Vec2 b) => a.Offset(b);
		public static Vec2 operator -(Vec2 a, Vec2 b) => a.Offset(-b);
		public static Vec2 operator *(double n, Vec2 v) => new Vec2(v.X * n, v.Y * n);
		public static Vec2 operator *(Vec2 v, double n) => new Vec2(v.X * n, v.Y * n);
		public static Vec2 operator /(Vec2 v, double n) => new Vec2(v.X / n, v.Y / n);
		public static Vec2 operator *(Vec2 a, Vec2 b) => new Vec2(a.X * b.X, a.Y * b.Y);
		public static Vec2 operator /(Vec2 a, Vec2 b) => new Vec2(a.X / b.X, a.Y / b.Y);
		public static bool operator ==(Vec2 a, Vec2 b) => a.Equals(b);
		public static bool operator !=(Vec2 a, Vec2 b) => a.X != b.X || a.Y != b.Y;
		#endregion

		#region Constructores, inicializadores y prefabricados
		/// <summary>
		/// <see cref="Vec2"/> con sus componentes en 0
		/// </summary>
		public static Vec2 Zero => new Vec2(0, 0);
		/// <summary>
		/// <see cref="Vec2"/> con sus componentes en 1
		/// </summary>
		public static Vec2 One => new Vec2(1, 1);
		/// <summary>
		/// <see cref="Vec2"/> unitario hacia arriba
		/// </summary>
		/// <remarks><code>Vec2(0, -1)</code></remarks>
		public static Vec2 Up => new Vec2(0, -1);
		/// <summary>
		/// <see cref="Vec2"/> unitario hacia abajo
		/// </summary>
		/// <remarks><code>Vec2(0, 1)</code></remarks>
		public static Vec2 Down => new Vec2(0, 1);
		/// <summary>
		/// <see cref="Vec2"/> unitario hacia la izquierda
		/// </summary>
		/// <remarks><code>Vec2(-1, 0)</code></remarks>
		public static Vec2 Left => new Vec2(-1, 0);
		/// <summary>
		/// <see cref="Vec2"/> unitario hacia la derecha
		/// </summary>
		/// <remarks><code>Vec2(1, 0)</code></remarks>
		public static Vec2 Right => new Vec2(1, 0);
		/// <summary>
		/// <see cref="Vec2"/> cuyos componentes tienen un valor aleatorio entre -1 y 1
		/// </summary>
		public static Vec2 Random {
			get {
				double rX = Game.RNG.NextDouble() * Math.Sign(Game.RNG.Next(2) - 0.5);
				double rY = Game.RNG.NextDouble() * Math.Sign(Game.RNG.Next(2) - 0.5);

				return new Vec2(rX, rY);
			}
		}
		/// <summary>
		/// <see cref="Vec2"/> unitario hacia la dirección indicada en radianes
		/// </summary>
		public static Vec2 FromAngle(double angle) {
			double xx = Math.Cos(angle);
			double yy = Math.Sin(angle);
			return new Vec2(xx, yy);
		}

		/// <summary>
		/// Crea un <see cref="Vec2"/> con los componentes especificados
		/// </summary>
		/// <param name="x">Componente X del vector</param>
		/// <param name="y">Componente Y del vector</param>
		public Vec2(double x, double y) {
			this.x = x;
			this.y = y;
		}
		/// <summary>
		/// Crea un <see cref="Vec2"/> a partir de otro <see cref="Vec2"/> modelo
		/// </summary>
		/// <param name="vec2">Vector modelo</param>
		public Vec2(Vec2 vec2) {
			this.x = vec2.X;
			this.y = vec2.Y;
		}
		#endregion

		#region Propiedades básicas
		/// <summary>
		/// Componente horizontal del <see cref="Vec2"/>
		/// </summary>
		public double X { get => this.x; set => this.x = value; }
		/// <summary>
		/// Componente vertical del <see cref="Vec2"/>
		/// </summary>
		public double Y { get => this.y; set => this.y = value; }

		/// <summary>
		/// Componente X (horizontal) del <see cref="Vec2"/> redondeado al entero más cercano
		/// </summary>
		public int IX => (int)Math.Round(this.x);
		/// <summary>
		/// Componente Y (vertical) del <see cref="Vec2"/> redondeado al entero más cercano
		/// </summary>
		public int IY => (int)Math.Round(this.y);
		#endregion

		#region Propiedades de vectores derivados
		/// <summary>
		/// Nueva copia idéntica del <see cref="Vec2"/>
		/// </summary>
		public Vec2 Copy => new Vec2(this);
		/// <summary>
		/// Versión del <see cref="Vec2"/> con sus componentes redondeados al entero más cercano
		/// </summary>
		public Vec2 Rounded => new Vec2(this.IX, this.IY);
		/// <summary>
		/// Nuevo <see cref="Vec2"/> con cada componente en 1 si es positivo, -1 si es negativo ó 0 si es cero
		/// </summary>
		public Vec2 Sign => new Vec2(Math.Sign(this.x), Math.Sign(this.y));
		/// <summary>
		/// Versión del <see cref="Vec2"/> con una magnitud (largo) de 1
		/// </summary>
		/// <seealso cref="Magnitude"/>
		public Vec2 Normalized {
			get {
				double magnitude = this.Magnitude;
				if(magnitude < 0.001)
					return Vec2.Zero;
				return new Vec2(this.x / magnitude, this.y / magnitude);
			}
		}
		#endregion

		#region Propiedades de cálculos relacionados
		/// <summary>
		/// Magnitud (o largo) del vector
		/// </summary>
		public double Magnitude => Math.Sqrt(this.x * this.x + this.y * this.y);

		/// <summary>
		/// Magnitud (o largo) del vector
		/// </summary>
		/// <seealso cref="Magnitude"/>
		public double Length => this.Magnitude;

		/// <summary>
		/// Ángulo (en radianes) del vector, desde el origen (0, 0) en relación al vector unitario (1, 0) hasta la posición que apunta
		/// </summary>
		public double Angle => Math.Acos(this.x);
		#endregion

		#region Métodos de cálculos relacionados
		/// <summary>
		/// Calcula el desplazamiento de este <see cref="Vec2"/> por la cantidad especificada para cada componente
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Vec2"/> original</remarks>
		/// <param name="x">Desplazamiento horizontal</param>
		/// <param name="y">Desplazamiento vertical</param>
		/// <returns>El <see cref="Vec2"/> resultante del desplazamiento</returns>
		/// <seealso cref="Offset(Vec2)"/>
		public Vec2 Offset(double x, double y) {
			return new Vec2(this.x + x, this.y + y);
		}
		/// <summary>
		/// Calcula el desplazamiento de este <see cref="Vec2"/> según el otro <see cref="Vec2"/> especificado
		/// </summary>
		/// <remarks>Este método no modifica el <see cref="Vec2"/> original</remarks>
		/// <param name="offset">Vector de desplazamiento</param>
		/// <returns>El <see cref="Vec2"/> resultante del desplazamiento</returns>
		/// <seealso cref="Offset(double, double)"/>
		public Vec2 Offset(Vec2 offset) {
			return new Vec2(this.x + offset.X, this.y + offset.Y);
		}

		/// <summary>
		/// Calcula el <see href="https://es.wikipedia.org/wiki/Producto_escalar">producto punto o escalar</see> de este <see cref="Vec2"/> por el otro especificado
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>El valor escalar resultante de la operación</returns>
		public double Dot(Vec2 other) {
			return this.x * other.X + this.y * other.Y;
		}

		/// <summary>
		/// Calcula el análogo bidimensional del <see href="https://es.wikipedia.org/wiki/Producto_vectorial">producto vectorial o cruzado</see> de este <see cref="Vec2"/> por el otro especificado
		/// </summary>
		/// <remarks>
		/// Nota: el producto cruzado entre vectores de 2 dimensiones no está matemáticamente definido.
		/// Este método es el resultado del componente Z en el producto cruzado de los vectores en un espacio 3D
		/// </remarks>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>El valor escalar resultante de la operación</returns>
		public double Cross(Vec2 other) {
			return this.x * other.y - other.x * this.y;
		}

		/// <summary>
		/// Calcula el <see cref="Vec2"/> resultante de una interpolación linear entre este <see cref="Vec2"/> y <paramref name="end"/>, con una proporción especificada por <paramref name="proportion"/>
		/// </summary>
		/// <param name="end">Punto en el cual la proporción es de 100%</param>
		/// <param name="proportion">Proporción de la interpolación, siendo 0 el inicio y 1 el final</param>
		/// <returns>El <see cref="Vec2"/> resultante de la interpolación</returns>
		public Vec2 Lerp(Vec2 end, double proportion) {
			double xx = MathUtils.Lerp(this.x, end.x, proportion);
			double yy = MathUtils.Lerp(this.y, end.y, proportion);

			return new Vec2(xx, yy);
		}

		/// <summary>
		/// Calcula el ángulo entre la dirección de este <see cref="Vec2"/> y la recta formada desde el origen (0, 0) hasta el punto especificado
		/// </summary>
		/// <returns>Ángulo calculado (en radianes)</returns>
		public double AngleTowards(double x, double y) {
			Vec2 other = new Vec2(x, y);
			return this.AngleTowards(other);
		}
		/// <summary>
		/// Calcula el ángulo entre la dirección de este <see cref="Vec2"/> y la del otro especificado
		/// </summary>
		/// <returns>Ángulo calculado (en radianes)</returns>
		public double AngleTowards(Vec2 other) {
			return Math.Acos(this.Normalized.Dot(other.Normalized));
		}

		/// <summary>
		/// Calcula la distancia desde este <see cref="Vec2"/> hasta el otro especificado
		/// </summary>
		/// <remarks>
		/// Esto equivale a la magnitud de la diferencia de ambos <see cref="Vec2"/>
		/// </remarks>
		/// <param name="other"><see cref="Vec2"/> objetivo</param>
		/// <returns>Distancia entre ambos <see cref="Vec2"/></returns>
		public double DistanceTo(Vec2 other) {
			return (other - this).Magnitude;
		}

		/// <summary>
		/// Calcula la dirección normalizada desde este <see cref="Vec2"/> hasta el otro especificado
		/// </summary>
		/// <remarks>
		/// Esto equivale al <see cref="Vec2"/> normalizado de la diferencia entre ambos
		/// </remarks>
		/// <param name="other">Vector objetivo</param>
		/// <returns>Dirección desde este <see cref="Vec2"/> al otro</returns>
		public Vec2 DirectionTo(Vec2 other) {
			return (other - this).Normalized;
		}
		/// <summary>
		/// Calcula la dirección desde este <see cref="Vec2"/> hasta el otro especificado
		/// </summary>
		/// <remarks>
		/// Esto equivale al <see cref="Vec2"/> normalizado de la diferencia entre ambos, multiplicado por el largo
		/// </remarks>
		/// <param name="other"><see cref="Vec2"/> objetivo</param>
		/// <param name="length">Largo del <see cref="Vec2"/> resultante</param>
		/// <returns>Dirección desde este <see cref="Vec2"/> al otro, con un largo fijo</returns>
		public Vec2 DirectionTo(Vec2 other, double length) {
			return (other - this).Normalized * length;
		}

		/// <summary>
		/// Determina si este <see cref="Vec2"/> es igual al otro especificado (tienen los mismos valores de componente)
		/// </summary>
		/// <param name="other"><see cref="Vec2"/> objetivo</param>
		/// <returns><c>true</c> si los <see cref="Vec2"/> son iguales</returns>
		public bool Equals(Vec2 other) {
			return this.x == other.X && this.y == other.Y;
		}

		/// <summary>
		/// Determina si el <see cref="Vec2"/> se encuentra dentro de un rectángulo
		/// </summary>
		/// <param name="x1">La posición X de la primer esquina del rectángulo</param>
		/// <param name="y1">La posición Y de la primer esquina del rectángulo</param>
		/// <param name="x2">La posición X de la esquina opuesta del rectángulo</param>
		/// <param name="y2">La posición Y de la esquina opuesta del rectángulo</param>
		/// <returns><c>true</c> si el <see cref="Vec2"/> está dentro de la recta</returns>
		public bool InsideRect(double x1, double y1, double x2, double y2) {
			if(x1 > x2) {
				double t = x1;
				x1 = x2;
				x2 = t;
			}

			if (y1 > y2) {
				double t = y1;
				y1 = y2;
				y2 = t;
			}

			return this.x >= x1 && this.x <= x2 && this.y >= y1 && this.y <= y2;
		}
		/// <summary>
		/// Determina si este <see cref="Vec2"/> se encuentra dentro de un rectángulo
		/// </summary>
		/// <param name="v1">La posición de la primer esquina del rectángulo</param>
		/// <param name="v2">La posición de la esquina opuesta del rectángulo</param>
		/// <returns><c>true</c> si el <see cref="Vec2"/> está dentro de la recta</returns>
		public bool InsideRect(Vec2 v1, Vec2 v2) {
			double x1 = v1.X,
				   y1 = v1.Y,
				   x2 = v2.X,
				   y2 = v2.Y;

			return this.InsideRect(x1, y1, x2, y2);
		}
		#endregion

		#region Métodos de manipulación de vectores
		/// <summary>
		/// Gira el <see cref="Vec2"/> según el ángulo especificado
		/// </summary>
		/// <remarks>Este método modifica el <see cref="Vec2"/> original</remarks>
		/// <param name="angle">Ángulo (en radianes)</param>
		public void Rotate(double angle) {
			double sinAngle = Math.Sin(angle);
			double cosAngle = Math.Cos(angle);
			this.x = x * cosAngle - y * sinAngle;
			this.y = x * sinAngle + y * cosAngle;
		}

		/// <summary>
		/// Restringe este vector entre los valores especificados para cada componente
		/// </summary>
		/// <param name="x1">El primer extremo de X</param>
		/// <param name="y1">El primer extremo de Y</param>
		/// <param name="x2">El segundo extremo de X</param>
		/// <param name="y2">El segundo extremo de Y</param>
		/// <remarks>Este método modifica el <see cref="Vec2"/> original</remarks>
		public void Clamp(double x1, double y1, double x2, double y2) {
			if (x1 > x2) {
				double t = x1;
				x1 = x2;
				x2 = t;
			}

			if (y1 > y2) {
				double t = y1;
				y1 = y2;
				y2 = t;
			}

			this.x = MathUtils.Clamp(this.x, x1, x2);
			this.y = MathUtils.Clamp(this.y, y1, y2);
		}
		/// <summary>
		/// Restringe este vector dentro del área de los dos vectores especificados
		/// </summary>
		/// <param name="v1">El primer vector del área</param>
		/// <param name="v2">El vector opuesto del área</param>
		/// <remarks>Este método modifica el <see cref="Vec2"/> original</remarks>
		public void Clamp(Vec2 v1, Vec2 v2) {
			double x1 = v1.X,
				   y1 = v1.Y,
				   x2 = v2.X,
				   y2 = v2.Y;

			this.Clamp(x1, y1, x2, y2);
		}
		#endregion

		#region Métodos generales de clase
		/// <summary>
		/// Devuelve una cadena que muestra los componentes de este <see cref="Vec2"/> como "(X, Y)"
		/// </summary>
		/// <returns>Una cadena que representa el <see cref="Vec2"/> actual</returns>
		public override string ToString() => $"({this.x:F2}, {this.y:F2})";
		public override bool Equals(object obj) => base.Equals(obj);
		public override int GetHashCode() => base.GetHashCode();
		#endregion
	}
}