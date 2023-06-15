using System;

namespace GameCore {
	/// <summary>
	/// Representa un componente colisionador
	/// </summary>
	public abstract class Collider {
		/// <summary>
		/// Dueño del colisionador
		/// </summary>
		protected readonly GameObject owner;

		/// <summary>
		/// Crea un componente colisionador en base al dueño especificado
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		public Collider(GameObject owner) => this.owner = owner;

		/// <summary>
		/// Posición del dueño del colisionador
		/// </summary>
		public Vec2 Pos => this.owner.Pos;

		/// <summary>
		/// Caja delimitante del colisionador
		/// </summary>
		public abstract Rect? Bounds { get; }

		/// <summary>
		/// Determina si un vector se encuentra dentro del colisionador
		/// </summary>
		/// <param name="pos">Vector a comprobar</param>
		/// <returns></returns>
		public abstract bool Inside(Vec2 pos);
		/// <summary>
		/// Determina si este colisionador colisiona con el otro especificado
		/// </summary>
		/// <param name="collider">Otro colisionador a comprobar</param>
		/// <returns></returns>
		public abstract bool Intersect(Collider collider);
		/// <summary>
		/// Determina si este colisionador se encuentra dentro del área rectangular descrita
		/// </summary>
		/// <param name="v1">Vector de la primera esquina del rectángulo</param>
		/// <param name="v2">Dector de la esquina opuesta del rectángulo</param>
		/// <returns></returns>
		public abstract bool RectIntersect(Rect r);
		/// <summary>
		/// Determina si este colisionador se encuentra dentro del área circular descrita
		/// </summary>
		/// <param name="o">Centro del círculo</param>
		/// <param name="r">Radio del círculo</param>
		/// <returns></returns>
		public abstract bool CircleIntersect(Vec2 o, double r);
	}
}
