namespace GameCore {
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

		public override Rect? Bounds => r + this.owner.Pos;

		/// <summary>
		/// Posición del centro del área del colisionador en relación al dueño
		/// </summary>
		private Vec2 CalculatedPos { get => ((this.V1 + this.V2) / 2).Rounded; }
		/// <summary>
		/// Área del colisionador en relación al dueño
		/// </summary>
		private Rect CalculatedArea { get => r + this.owner.Pos; }
		/// <summary>
		/// Vector de la primera esquina del rectángulo en relación al dueño del colisionador
		/// </summary>
		public Vec2 V1 { get => this.r.V1 + this.owner.Pos; }
		/// <summary>
		/// Vector de la esquina opuesta del rectángulo en relación al dueño del colisionador
		/// </summary>
		public Vec2 V2 { get => this.r.V2 + this.owner.Pos; }

		public override bool Inside(Vec2 pos) => this.CalculatedArea.Inside(pos);
		public override bool Intersect(Collider collider) => collider.RectIntersect(this.CalculatedArea);

		public override bool RectIntersect(Rect r) {
			return this.r.Rounded.Intersects(r.Rounded);
		}

		public override bool CircleIntersect(Vec2 o, double r) {
			Vec2 bestCandidate = o.DirectionTo(this.CalculatedPos, r);
			return this.Inside(bestCandidate.Rounded);
		}
	}
}
