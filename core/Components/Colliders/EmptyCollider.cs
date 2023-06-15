namespace GameCore {
	/// <summary>
	/// Representa un componente colisionador vacío, incapaz de detectar colisiones
	/// </summary>
	public class EmptyCollider : Collider {
		/// <summary>
		/// Crea un componente colisionador vacío, incapaz de detectar colisiones
		/// </summary>
		/// <param name="owner">Dueño del componente</param>
		public EmptyCollider(GameObject owner) : base(owner) {}

		public override Rect? Bounds => null;

		public override bool Inside(Vec2 pos) => false;
		public override bool Intersect(Collider collider) => false;
		public override bool RectIntersect(Rect r) => false;
		public override bool CircleIntersect(Vec2 o, double r) => false;
	}
}
