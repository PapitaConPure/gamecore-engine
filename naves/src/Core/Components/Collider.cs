using System;
using System.Collections.Generic;

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

	/// <summary>
	/// Representa un componente colisionador vacío, incapaz de detectar colisiones
	/// </summary>
	public class EmptyCollider : Collider {
		/// <summary>
		/// Crea un componente colisionador vacío, incapaz de detectar colisiones
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		public EmptyCollider(GameObject owner) : base(owner) { }

		public override bool Inside(Vec2 pos) => false;
		public override bool Intersect(Collider collider) => false;
		public override bool RectIntersect(Rect r) => false;
		public override bool CircleIntersect(Vec2 o, double r) => false;
	}

	/// <summary>
	/// Representa un componente colisionador puntual
	/// </summary>
	public class DotCollider : Collider {
		/// <summary>
		/// El desplazamiento del colisionador respecto a la posición del dueño
		/// </summary>
		private readonly Vec2 offset;

		/// <summary>
		/// Crea un componente colisionador puntual
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		public DotCollider(GameObject owner) : base(owner) {
			this.offset = Vec2.Zero;
		}
		/// <summary>
		/// Crea un componente colisionador punto
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		/// <param name="offset">Desplazamiento relativo al dueño</param>
		public DotCollider(GameObject owner, Vec2 offset) : base(owner) {
			this.offset = offset;
		}

		/// <summary>
		/// Posición del colisionador en relación al dueño y el desplazamiento aplicado
		/// </summary>
		private Vec2 CalculatedPos { get => this.owner.Pos.Offset(this.offset).Rounded; }

		public override bool Inside(Vec2 pos) => this.CalculatedPos == pos.Rounded;
		public override bool Intersect(Collider collider) => collider.Inside(this.CalculatedPos);
		public override bool RectIntersect(Rect r) => this.CalculatedPos.InsideRect(r.IV1, r.IV2);
		public override bool CircleIntersect(Vec2 o, double r) => this.CalculatedPos.DistanceTo(o) < r;
	}

	/// <summary>
	/// Representa un componente colisionador rectangular
	/// </summary>
	public class RectCollider : Collider {
		/// <summary>
		/// Área rectangular del colisionador
		/// </summary>
		private readonly Rect r;

		/// <summary>
		/// Crea un componente colisionador rectangular
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		/// <param name="v1">Vector de la primera esquina del rectángulo</param>
		/// <param name="v2">Vector de la esquina opuesta del rectángulo</param>
		public RectCollider(GameObject owner, Rect r) : base(owner) {
			this.r = r;
		}

		/// <summary>
		/// Posición del centro del área del colisionador en relación al dueño
		/// </summary>
		private Vec2 CalculatedPos { get => ((this.V1 + this.V2) / 2).Rounded; }
		/// <summary>
		/// Área del colisionador en relación al dueño
		/// </summary>
		private Rect CalculatedArea { get => new Rect(this.V1, this.V2); }
		/// <summary>
		/// Vector de la primera esquina del rectángulo en relación al dueño del colisionador
		/// </summary>
		public Vec2 V1 { get => this.r.V1 + this.owner.Pos; }
		/// <summary>
		/// Vector de la esquina opuesta del rectángulo en relación al dueño del colisionador
		/// </summary>
		public Vec2 V2 { get => this.r.V2 + this.owner.Pos; }

		public override bool Inside(Vec2 pos) => pos.InsideRect(this.V1.Rounded, this.V2.Rounded);
		public override bool Intersect(Collider collider) => collider.RectIntersect(this.CalculatedArea);

		public override bool RectIntersect(Rect r) {
			return this.r.Rounded.Intersects(r.Rounded);
		}

		public override bool CircleIntersect(Vec2 o, double r) {
			Vec2 bestCandidate = o.DirectionTo(this.CalculatedPos, r);
			return this.Inside(bestCandidate.Rounded);
		}
	}

	/// <summary>
	/// Representa un componente colisionador circular
	/// </summary>
	public class CircleCollider: Collider {
		/// <summary>
		/// Radio del círculo del colisionador
		/// </summary>
		private readonly double r;

		/// <summary>
		/// Crea un componente colisionador circular
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		/// <param name="r">Radio del círculo</param>
		public CircleCollider(GameObject owner, double r) : base(owner) {
			this.r = r;
		}

		public override bool Inside(Vec2 pos) => this.Pos.Rounded.DistanceTo(pos) <= r;
		public override bool Intersect(Collider collider) => collider.CircleIntersect(this.Pos, this.r);

		public override bool RectIntersect(Rect r) {
			Vec2 bestCandidate = this.Pos.Rounded;
			bestCandidate.Clamp(r.IV1, r.IV2);

			return this.Inside(bestCandidate);
		}

		public override bool CircleIntersect(Vec2 o, double r) {
			Vec2 bestCandidate = o.DirectionTo(this.Pos);
			return this.Inside(bestCandidate);
		}
	}

	/// <summary>
	/// Representa un componente colisionador poligonal
	/// </summary>
	public class PolygonCollider: Collider {
		private readonly List<Vec2> polygon;
		private Rect bbox;

		public PolygonCollider(GameObject owner, List<Vec2> polygon) : base(owner) {
			this.polygon = polygon;
			this.CalculateBoundingBox();
		}

		public override bool Inside(Vec2 pos) {
			if(!bbox.Rounded.Inside(pos.Rounded))
				return false;

			int vertices = polygon.Count;
			bool inside = false;

			for(int i = 0, j = vertices - 1; i < vertices; j = i++) {
				if((polygon[i].Y < pos.Y && polygon[j].Y >= pos.Y
				|| polygon[j].Y < pos.Y && polygon[i].Y >= pos.Y)
				&& (polygon[i].X <= pos.X || polygon[j].X <= pos.X)) {
					if(polygon[i].X + (pos.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < pos.X)
						inside = !inside;
				}
			}

			return inside;
		}

		//*No tengo ni idea de cómo voy a hacer esto*
		public override bool Intersect(Collider collider) => false;

		public override bool RectIntersect(Rect r) => false;

		public override bool CircleIntersect(Vec2 o, double r) => false;

		private void CalculateBoundingBox() {
			double minX = double.MaxValue;
			double minY = double.MaxValue;
			double maxX = double.MinValue;
			double maxY = double.MinValue;

			foreach(Vec2 vertex in polygon) {
				if(vertex.X < minX)
					minX = vertex.X;
				if(vertex.Y < minY)
					minY = vertex.Y;
				if(vertex.X > maxX)
					maxX = vertex.X;
				if(vertex.Y > maxY)
					maxY = vertex.Y;
			}

			this.bbox = new Rect(minX, minY, maxX, maxY);
		}
	}
}
