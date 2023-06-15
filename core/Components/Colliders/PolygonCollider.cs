using System.Collections.Generic;

namespace GameCore {
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

		public override Rect? Bounds => this.bbox;

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
