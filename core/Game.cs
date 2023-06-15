using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace GameCore {
	public enum GameTarget {
		Console,
		Forms,
	}

	/// <summary>
	/// Representa un sistema general básico de juegos
	/// </summary>
	public static class Game {
		#region Atributos Básicos
		/// <summary>
		/// Plataforma del juego
		/// </summary>
		private static GameTarget target;
		/// <summary>
		/// Contador de ticks transcurridos en el escenario actual
		/// </summary>
		private static long ticks = 0;
		/// <summary>
		/// Indica si el juego terminó
		/// </summary>
		private static bool ended = false;
		/// <summary>
		/// Generador de números aleatorios
		/// </summary>
		private static readonly Random rng = new Random();
		/// <summary>
		/// Método de entrada/control del jugador
		/// </summary>
		private static Controller controller;
		#endregion

		#region Colecciones de Instancias
		/// <summary>
		/// Lista de instancias <see cref="GameObject"/> activas en la escena actual
		/// </summary>
		private static readonly List<GameObject> instances = new List<GameObject>();
		/// <summary>
		/// Lista de instancias <see cref="GameObject"/> activas en la escena actual
		/// </summary>
		private static readonly List<GameObject> disabledInstances = new List<GameObject>();
		#endregion

		#region Eventos de Juego
		#region Delegados
		public delegate void GameEventHandler(GameEventArgs e);
		public delegate void GameInstanceEventHandler(GameObject instance, GameInstanceEventArgs e);
		public delegate void GameInstanceDeletedEventHandler(GameObject instance, GameInstanceDeletedEventArgs e);
		#endregion

		#region Handlers
		/// <summary>
		/// Evento general del juego.
		/// Se acciona a medida que se va procesando un tick, o por cambios generales en la escena
		/// </summary>
		/// <remarks>Utiliza el enumerador <see cref="EventReason"/></remarks>
		public static event GameEventHandler GameEvent;
		/// <summary>
		/// Evento de instancia en la escena actual.
		/// Se acciona cuando se agregan o modifican instancias <see cref="GameObject"/> dentro de la escena
		/// </summary>
		/// <remarks>Utiliza el enumerador <see cref="InstanceEventReason"/></remarks>
		public static event GameInstanceEventHandler GameInstanceEvent;
		/// <summary>
		/// Evento de instancia en la escena actual.
		/// Se acciona cuando se eliminan instancias <see cref="GameObject"/> de la escena
		/// </summary>
		/// <remarks>Utiliza el enumerador <see cref="InstanceDeletedEventReason"/></remarks>
		public static event GameInstanceDeletedEventHandler GameInstanceDeletedEvent;
		#endregion

		#region Eventos
		private static void OnGameEvent(GameEventArgs e) {
			if(GameEvent != null)
				GameEvent.Invoke(e);
		}

		private static void OnGameInstanceEvent(GameObject instance, GameInstanceEventArgs e) {
			if(GameInstanceEvent != null)
				GameInstanceEvent.Invoke(instance, e);
		}

		private static void OnGameInstanceDeletedEvent(GameObject instance, GameInstanceDeletedEventArgs e) {
			if(GameInstanceDeletedEvent != null)
				GameInstanceDeletedEvent.Invoke(instance, e);
		}
		#endregion

		#region Suscripciones y Mensajes
		/// <summary>
		/// Suscribe una instancia <see cref="GameObject"/> al evento indicado por <paramref name="eventType"/> en el tick actual
		/// </summary>
		/// <remarks>La suscripción toma efecto desde el preciso instante en el que se realiza</remarks>
		/// <param name="self">Instancia a suscribir al evento</param>
		/// <param name="eventType">Tipo de evento al cual suscribir</param>
		public static void Subscribe(GameObject self, EventType eventType) {
			switch(eventType) {
			case EventType.General:
				GameEvent += self.OnGameEvent;
				break;
			case EventType.Instance:
				GameInstanceEvent += self.OnGameInstanceEvent;
				break;
			case EventType.InstanceDeleted:
				GameInstanceDeletedEvent += self.OnGameInstanceDeletedEvent;
				break;
			}
		}

		/// <inheritdoc cref="Subscribe(GameObject, EventType)"/>
		public static void SubscribeTo(this GameObject self, EventType eventType) {
			Subscribe(self, eventType);
		}

		/// <summary>
		/// Tipo de evento del juego.
		/// Usado para indicar a qué eventos del juego suscribir determinadas instancias <see cref="GameObject"/>
		/// </summary>
		/// <remarks>Véase <see cref="Subscribe(GameObject, EventType)"/> y <see cref="SubscribeTo(GameObject, EventType)"/></remarks>
		public enum EventType {
			/// <summary>Eventos generales</summary>
			General,
			/// <summary>Eventos de actualización de instancia</summary>
			Instance,
			/// <summary>Eventos de eliminación de instancia</summary>
			InstanceDeleted,
		}

		/// <summary>
		/// Motivo de evento general del juego
		/// </summary>
		public enum EventReason {
			/// <summary>Inicio de tick, antes de procesar las instancias</summary>
			TickStarted,
			/// <summary>Punto en un tick luego de procesar las instancias, pero antes de renderizarlo y aplicar cambios a las mismas</summary>
			TickProcessed,
			/// <summary>Punto en un tick luego de ser renderizado, pero antes de aplicar cambios de instancias y ser contado</summary>
			TickRendered,
			/// <summary>Final de tick, luego de aplicar cambios de instancias pendientes y ser contado</summary>
			TickCounted,

			/// <summary>Finalización de nivel</summary>
			StageFinished,
		}

		/// <summary>
		/// Motivo de evento de actualización de instancia
		/// </summary>
		public enum InstanceEventReason {
			None,
			/// <summary>Nueva sub-instancia hija de una instancia padre</summary>
			Related,
			/// <summary>Nueva sub-instancia disparo</summary>
			Shoot,
		}

		/// <summary>
		/// Motivo de evento de eliminación de instancia
		/// </summary>
		public enum InstanceDeletedEventReason {
			None,
			/// <summary>Instancia eliminada por efecto secundario o como sub-producto de otra acción</summary>
			Dropped,
			/// <summary>Instancia fallecida por causas determinadas naturales</summary>
			Died,
			/// <summary>Instancia asesinada por otra instancia</summary>
			Killed,
		}
		#endregion
		#endregion

		#region Propiedades de Juego
		/// <inheritdoc cref="ticks"/>
		public static long Ticks { get => ticks; }

		public static bool Debug { get; set; }

		/// <inheritdoc cref="ended"/>
		public static bool Ended {
			get => ended;
			set {
				if(!ended)
					ended = value;
			}
		}

		/// <inheritdoc cref="rng"/>
		public static Random RNG { get => rng; }

		/// <inheritdoc cref="controller"/>
		public static Controller Controller => controller;
		/// <inheritdoc cref="Controller.previousButton"/>
		public static GameInput PreviousButton => controller.PreviousButton;
		/// <inheritdoc cref="Controller.currentButton"/>
		public static GameInput CurrentButton => controller.CurrentButton;
		/// <inheritdoc cref="Controller.pressedButton"/>
		public static GameInput PressedButton => controller.PressedButton;
		/// <inheritdoc cref="Controller.releasedButton"/>
		public static GameInput ReleasedButton => controller.ReleasedButton;

		public static GameTarget Target {
			set {
				switch(value) {
				case GameTarget.Console:
					controller = new ConsoleController();
					break;
				case GameTarget.Forms:
					controller = new FormsController();
					break;
				}
				target = value;
			}
			get => target;
		}
		#endregion

		#region Ciclo de Juego y Control de Escena
		/// <summary>
		/// Puerta de acceso al bucle principal del juego.
		/// Procesa el juego tick por tick hasta que algo cause su finalización
		/// </summary>
		public static void Loop() {
			double deltaTime;
			double lastUpdate = 0;
			double currentDate;
			double refreshRate = Clock.BaseTick;
			while(!ended) {
				currentDate = DateTime.Now.Ticks / (double)TimeSpan.TicksPerMillisecond;
				deltaTime = currentDate - lastUpdate;
				if(deltaTime >= refreshRate) {
					Console.WriteLine(deltaTime);
					lastUpdate = DateTime.Now.Ticks / (double)TimeSpan.TicksPerMillisecond;
					UpdateGameState(deltaTime);
				}
			}
		}

		/// <summary>
		/// Procesa un tick del juego
		/// </summary>
		/// <param name="deltaTime">La cantidad de milisegundos transcurridos entre este tick y el anterior</param>
		public static void UpdateGameState(double deltaTime) {
			OnGameEvent(new GameEventArgs(EventReason.TickStarted));

			controller.Update(ticks);

			foreach(GameObject instance in instances.ToList())
				instance.Update(deltaTime);
			OnGameEvent(new GameEventArgs(EventReason.TickProcessed));

			GUI.EmptyGameFrame();
			GUI.DrawGameFrame(instances);
			GUI.DrawTPS(deltaTime);
			if(Debug) {
				string activeInstanceCount = Convert.ToString(instances.Count);
				string disabledInstanceCount = Convert.ToString(disabledInstances.Count);

				if(Target == GameTarget.Console) {
					GUI.ConsoleDrawer.DrawText(GUI.UITopLeft.Offset(1, 1), activeInstanceCount, ConsoleColor.Yellow);
					GUI.ConsoleDrawer.DrawText(GUI.UITopLeft.Offset(2 + activeInstanceCount.Length, 1), disabledInstanceCount, ConsoleColor.DarkCyan);
				}
			}
			GUI.DrawSurface();
			OnGameEvent(new GameEventArgs(EventReason.TickRendered));

			ticks += 1;
			OnGameEvent(new GameEventArgs(EventReason.TickCounted));
		}

		/// <summary>
		/// Reinicia el contador de ticks y la lista de instancias
		/// </summary>
		public static void ClearState() {
			ticks = 0;
			instances.Clear();
		}
		#endregion

		#region Control de Instancias
		#region Manipulación de Instancias
		/// <summary>
		/// Añade una instancia <see cref="GameObject"/> al juego y dispara un <see cref="GameInstanceEvent"/> para todas las instancias actuales
		/// 
		/// </summary>
		/// <remarks>
		/// Asegúrate de no suscribir la instancia recién creada antes de añadirla si no quieres que esta también reciba el evento de su propia adición
		/// </remarks>
		/// <param name="instance">Nueva instancia del juego</param>
		/// <param name="reason">Razón de adición</param>
		public static void AddInstance(GameObject instance, InstanceEventReason reason = InstanceEventReason.None) {
			instances.Add(instance);

			OnGameInstanceEvent(instance, new GameInstanceEventArgs(reason));
		}

		/// <summary>
		/// Elimina una instancia <see cref="GameObject"/> del juego y dispara un <see cref="GameInstanceDeletedEvent"/> para todas las instancias actuales (sin incluir la recién removida).
		/// Si la instancia especificada ya estaba marcada para eliminar en este tick, no pasa nada
		/// </summary>
		/// <remarks>
		/// El <see cref="GameInstanceEventArgs.Reason"/> disparado corresponde a <see cref="GameInstanceEventArgs.GameEventType.InstanceDeleted"/>.
		/// </remarks>
		/// <param name="instance">Instancia a eliminar del juego</param>
		/// <param name="reason">Razón de eliminación</param>
		public static void DeleteInstance(GameObject instance, InstanceDeletedEventReason reason = InstanceDeletedEventReason.None) {
			if(!instances.Contains(instance))
				return;

			instances.Remove(instance);

			GameEvent -= instance.OnGameEvent;
			GameInstanceEvent -= instance.OnGameInstanceEvent;
			GameInstanceDeletedEvent -= instance.OnGameInstanceDeletedEvent;
			OnGameInstanceDeletedEvent(instance, new GameInstanceDeletedEventArgs(reason));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		public static void DisableInstance(GameObject instance, InstanceEventReason reason = InstanceEventReason.None) {
			if(instances.Remove(instance))
				disabledInstances.Add(instance);

			GameEvent -= instance.OnGameEvent;
			GameInstanceEvent -= instance.OnGameInstanceEvent;
			GameInstanceDeletedEvent -= instance.OnGameInstanceDeletedEvent;
			OnGameInstanceEvent(instance, new GameInstanceEventArgs(reason));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		public static void EnableInstance(GameObject instance, InstanceEventReason reason = InstanceEventReason.None) {
			if(disabledInstances.Remove(instance))
				instances.Add(instance);

			GameEvent -= instance.OnGameEvent;
			GameInstanceEvent -= instance.OnGameInstanceEvent;
			GameInstanceDeletedEvent -= instance.OnGameInstanceDeletedEvent;
			OnGameInstanceEvent(instance, new GameInstanceEventArgs(reason));
		}
		#endregion

		#region Comprobación y Búsqueda de Instancias
		/// <summary>
		/// Determina si existe una instancia de tipo <typeparamref name="InstanceType"/> en la escena actual
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia a comprobar</typeparam>
		/// <returns>Si existe al menos una instancia <see cref="GameObject"/> bajo el criterio, o <see langword="null"/> si no se encuentra ninguna</returns>
		public static bool InstanceExists<InstanceType>() where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.Any();
		}

		public static InstanceType GetInstance<InstanceType>(int id) where InstanceType : GameObject {
			bool HasSameId(GameObject instance) => instance.Id == id;
			GameObject found = instances.Find(HasSameId);

			if(found is InstanceType type)
				return type;

			return null;
		}

		/// <summary>
		/// Devuelve la primer instancia de tipo <typeparamref name="InstanceType"/> encontrada en la escena actual, o <see langword="null"/> si no se encuentra ninguna
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia a buscar</typeparam>
		/// <returns>La primer instancia <see cref="GameObject"/> encontrada bajo el criterio, o <see langword="null"/> si no se encuentra ninguna</returns>
		public static InstanceType FindInstance<InstanceType>() where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.FirstOrDefault();
		}
		/// <summary>
		/// Devuelve la primer instancia de tipo <typeparamref name="InstanceType"/> encontrada en la escena actual, o <see langword="null"/> si no se encuentra ninguna
		/// </summary>
		/// <remarks>Si <paramref name="self"/> fuese a ser del tipo buscado, se omite en la búsqueda</remarks>
		/// <typeparam name="InstanceType">Tipo de instancia a buscar</typeparam>
		/// <param name="self">Una instancia a ignorar. Generalmente la instancia que invoca este método</param>
		/// <returns>La primer instancia <see cref="GameObject"/> encontrada bajo el criterio, o <see langword="null"/> si no se encuentra ninguna</returns>
		public static InstanceType FindInstance<InstanceType>(GameObject self) where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.Where(instance => !ReferenceEquals(self, instance))
				.FirstOrDefault();
		}
		/// <summary>
		/// Devuelve la primer instancia de tipo <typeparamref name="InstanceType"/> encontrada en la escena actual
		/// que satisfaga la condición dada por <paramref name="predicate"/>, o <see langword="null"/> si no se encuentra ninguna
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia a buscar</typeparam>
		/// <param name="predicate">Función para probar cada instancia con una condición</param>
		/// <returns>La primer instancia <see cref="GameObject"/> encontrada bajo el criterio, o <see langword="null"/> si no se encuentra ninguna</returns>
		public static InstanceType FindInstance<InstanceType>(Func<InstanceType, bool> predicate) where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.FirstOrDefault(predicate);
		}

		/// <summary>
		/// Devuelve una lista de todas las instancias de tipo <typeparamref name="InstanceType"/> encontradas en la escena actual
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia a buscar</typeparam>
		/// <returns>Una <see cref="List{InstanceType}"/> conteniendo los resultados de la búsqueda</returns>
		public static List<InstanceType> FindInstances<InstanceType>() where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.ToList();
		}
		/// <summary>
		/// Devuelve una lista de todas las instancias de tipo <typeparamref name="InstanceType"/> encontradas en la escena actual
		/// </summary>
		/// <remarks>Si <paramref name="self"/> fuese a ser del tipo buscado, se descarta de la lista</remarks>
		/// <typeparam name="InstanceType">Tipo de instancia a buscar</typeparam>
		/// <param name="self">Una instancia a ignorar. Generalmente la instancia que invoca este método</param>
		/// <returns>Una <see cref="List{InstanceType}"/> conteniendo los resultados de la búsqueda</returns>
		public static List<InstanceType> FindInstances<InstanceType>(GameObject self) where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.Where(instance => !ReferenceEquals(self, instance))
				.ToList();
		}
		/// <summary>
		/// Devuelve una lista de todas las instancias de tipo <typeparamref name="InstanceType"/> encontradas en la escena actual
		/// que satisfagan la condición dada por <paramref name="predicate"/>
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia a buscar</typeparam>
		/// <param name="predicate">Función para probar cada instancia con una condición</param>
		/// <returns>Una <see cref="List{InstanceType}"/> conteniendo los resultados de la búsqueda</returns>
		public static List<InstanceType> FindInstances<InstanceType>(Func<InstanceType, bool> predicate) where InstanceType : GameObject {
			return instances
				.OfType<InstanceType>()
				.Where(predicate)
				.ToList();
		}
		#endregion

		#region Comprobación de Colisiones
		/// <summary>
		/// Determina si la instancia <see cref="GameObject"/> especificada colisiona con alguna otra instancia del juego
		/// </summary>
		/// <param name="self">Instancia desde la cual detectar colisiones</param>
		/// <returns><see langword="true"/> si se detectan una o más colisiones</returns>
		public static bool Collides(GameObject self) {
			bool IsNotSelfAndCollidesWith(GameObject instance) => !ReferenceEquals(self, instance) && self.CollidesWith(instance);
			return instances.Any(IsNotSelfAndCollidesWith);
		}

		/// <summary>
		/// Determina si la instancia <see cref="GameObject"/> especificada colisiona con alguna otra instancia del juego de tipo <typeparamref name="InstanceType"/>
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia contra las cuales comprobar colisiones</typeparam>
		/// <param name="self">Instancia desde la cual detectar colisiones</param>
		/// <returns><see langword="true"/> si se detectan una o más colisiones</returns>
		public static bool CollidesWith<InstanceType>(GameObject self) where InstanceType: GameObject {
			bool IsDesiredTypeAndNotSelfAndCollidesWith(GameObject instance) => instance is InstanceType && !ReferenceEquals(self, instance) && self.CollidesWith(instance);
			return instances.Any(IsDesiredTypeAndNotSelfAndCollidesWith);
		}

		/// <summary>
		/// Busca colisiones entre la instancia <see cref="GameObject"/> especificada y alguna otra instancia del juego
		/// </summary>
		/// <param name="self">Instancia desde la cual detectar colisiones</param>
		/// <returns>Una <see cref="List{GameObject}"/> de las colisiones detectadas</returns>
		public static List<GameObject> Collisions(GameObject self) {
			bool IsNotSelfAndCollidesWith(GameObject instance) => !ReferenceEquals(self, instance) && self.CollidesWith(instance);
			return instances.FindAll(IsNotSelfAndCollidesWith);
		}

		/// <summary>
		/// Busca colisiones entre la instancia <see cref="GameObject"/> especificada y alguna otra instancia del juego de tipo <typeparamref name="InstanceType"/>
		/// </summary>
		/// <typeparam name="InstanceType">Tipo de instancia contra las cuales comprobar colisiones</typeparam>
		/// <param name="self">Instancia desde la cual detectar colisiones</param>
		/// <returns>Una <see cref="List{T}"/> cuyo tipo genérico es <typeparamref name="InstanceType"/> de las colisiones detectadas</returns>
		public static List<InstanceType> Collisions<InstanceType>(GameObject self) where InstanceType: GameObject {
			bool IsNotSelfAndCollidesWith(InstanceType instance) => !ReferenceEquals(self, instance) && self.CollidesWith(instance);

			return instances
				.OfType<InstanceType>()
				.Where(IsNotSelfAndCollidesWith)
				.ToList();
		}
		#endregion
		#endregion
	}

	/// <summary>
	/// Representa un reloj de juego
	/// </summary>
	public class Clock {
		private static double tps = 60;

		#region Conversiones básicas
		/// <summary>
		/// Ticks por segundo del juego.
		/// Entre 1 y 1000, por defecto 60
		/// </summary>
		/// <remarks>Por cada tick, se procesan y dibujan los elementos de la escena actual del juego</remarks>
		public static double TicksPerSecond { get => tps; set => tps = MathX.Clamp(value, 1, 1000); }

		/// <summary>
		/// Duración de un tick en milisegundos
		/// </summary>
		/// <remarks>Por cada tick, se procesan y dibujan los elementos de la escena actual del juego</remarks>
		public static double BaseTick { get => 1000d / TicksPerSecond; }

		/// <summary>
		/// Duración de un segundo en ticks
		/// </summary>
		/// <remarks>Por cada tick, se procesan y dibujan los elementos de la escena actual del juego</remarks>
		public static double Second { get => 1000d / BaseTick; }
		#endregion

		#region Traducción de Tiempo a Ticks
		/// <summary>
		/// Traduce los segundos ingresados a ticks del juego
		/// </summary>
		/// <remarks>No tiene en cuenta las variaciones de tiempo que puedan ocurrir por tick, así que es solo un estimado</remarks>
		/// <param name="s">Segundos</param>
		/// <returns>La cantidad de ticks que ocurren en el periodo especificado</returns>
		public static long Seconds(double s) {
			return Convert.ToInt64(s * Second);
		}
		/// <summary>
		/// Traduce los minutos ingresados a ticks del juego
		/// </summary>
		/// <remarks>No tiene en cuenta las variaciones de tiempo que puedan ocurrir por tick, así que es solo un estimado</remarks>
		/// <param name="s">Segundos</param>
		/// <returns>La cantidad de ticks que ocurren en el periodo especificado</returns>
		public static long Minutes(double m) {
			return Convert.ToInt64(m * 60 * Second);
		}
		/// <summary>
		/// Traduce las horas ingresadas a ticks del juego
		/// </summary>
		/// <remarks>No tiene en cuenta las variaciones de tiempo que puedan ocurrir por tick, así que es solo un estimado</remarks>
		/// <param name="s">Segundos</param>
		/// <returns>La cantidad de ticks que ocurren en el periodo especificado</returns>
		public static long Hours(double m) {
			return Convert.ToInt64(m * 3600 * Second);
		}
		#endregion

		#region Traducción de Ticks a Tiempo
		public static double ToSeconds(long ticks) {
			return ticks / TicksPerSecond;
		}
		public static double ToMinutes(long ticks) {
			return ticks / (TicksPerSecond * 60);
		}
		public static double ToHours(long ticks) {
			return ticks / (TicksPerSecond * 3600);
		}
		#endregion
	}
}
