namespace GameCore {
	public abstract class GameObject {
        protected Vec2 pos;
        protected Vec2 vel;
        protected Display display;
        protected Collider collider;
        protected byte layer;

        public GameObject() => this.Initialize(Vec2.Zero, Vec2.Zero);
        public GameObject(Vec2 pos) => this.Initialize(pos.Copy, Vec2.Zero);
        public GameObject(Vec2 pos, Vec2 vel) => this.Initialize(pos.Copy, vel.Copy);

        private void Initialize(Vec2 pos, Vec2 vel) {
            this.pos = pos;
            this.vel = vel;
            this.collider = new EmptyCollider(this);
            this.layer = 127;
        }

        public Vec2 Pos { get => this.pos; set => this.pos = value; }
        public Vec2 Vel { get => this.vel; set => this.vel = value; }
        public Collider Collider { get => this.collider; }

        public void Draw() {
            if(this.display is null) return;
            this.display.Draw(this.pos);
        }

        public bool CollidesWith(GameObject instance) {
            return this.collider.Intersect(instance.Collider);
        }

        public void Update() {
            this.PositionUpdate();
            this.MainUpdate();
        }

        /// <summary>
        /// Se ejecuta cuando el juego dispara un evento general
        /// </summary>
        /// <remarks>Requiere que la instancia esté suscrita a los eventos del juego (véase <see cref="Game.SubscribeTo(GameObject, Game.EventType)"/>)</remarks>
        /// <param name="e">Argumentos del evento disparado</param>
        public virtual void OnGameEvent(GameEventArgs e) {}

        /// <summary>
        /// Se ejecuta cuando el juego dispara un evento de adición de instancias
        /// </summary>
        /// <remarks>Requiere que la instancia esté suscrita a los eventos del juego (véase <see cref="Game.SubscribeTo(GameObject, Game.EventType)"/>)</remarks>
        /// <param name="sender">La instancia del juego que ocasionó el evento</param>
        /// <param name="e">Argumentos del evento disparado</param>
        public virtual void OnGameInstanceEvent(GameObject sender, GameInstanceEventArgs e) {}

        /// <summary>
        /// Se ejecuta cuando el juego dispara un evento de eliminación de instancias
        /// </summary>
        /// <remarks>Requiere que la instancia esté suscrita a los eventos del juego (véase <see cref="Game.SubscribeTo(GameObject, Game.EventType)"/>)</remarks>
        /// <param name="sender">La instancia del juego que ocasionó el evento</param>
        /// <param name="e">Argumentos del evento disparado</param>
        public virtual void OnGameInstanceDeletedEvent(GameObject sender, GameInstanceDeletedEventArgs e) {}

        protected virtual void PositionUpdate() {
            this.pos += this.vel;
            //this.pos.Clamp(GUI.GameTopLeft, GUI.GameBottomRight);
        }

        protected virtual void MainUpdate() {
            //No hace nada por defecto
        }
    }
}
