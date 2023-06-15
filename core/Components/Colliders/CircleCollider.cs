namespace GameCore {
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

		public override Rect? Bounds => new Rect(this.Pos - this.r, this.Pos + this.r);

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
}
