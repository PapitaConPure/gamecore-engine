namespace GameCore {
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

		public override Rect? Bounds => Rect.Full + (owner.Pos + this.offset);

		/// <summary>
		/// Posición del colisionador en relación al dueño y el desplazamiento aplicado
		/// </summary>
		private Vec2 CalculatedPos { get => (this.owner.Pos + this.offset).Rounded; }

		public override bool Inside(Vec2 pos) => this.CalculatedPos == pos.Rounded;
		public override bool Intersect(Collider collider) => collider.Inside(this.CalculatedPos);
		public override bool RectIntersect(Rect r) => this.CalculatedPos.InsideRect(r.IV1, r.IV2);
		public override bool CircleIntersect(Vec2 o, double r) => this.CalculatedPos.DistanceTo(o) < r;
	}
}
