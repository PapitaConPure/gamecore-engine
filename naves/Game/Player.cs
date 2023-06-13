using System;
using System.Collections.Generic;
using System.Globalization;
using GameCore;
using System.Threading.Tasks;

namespace naves {
	class Player: GameObject {
		private readonly PlayerBulletPool bulletPool;
		private long score;
		private int lives;
		private int bombs;
		private double power;
		private long value;
		private int graze;
		private int iTicks;
		private bool focused;
		private Display focusedDisplay;

		//PENDIENTE: Extraer a clase Tutorial
		private readonly List<InputTutorial> availableTutorials;
		private enum InputTutorial {
			MoveUp,
			MoveDown,
			MoveLeft,
			MoveRight,
			Stop,
			Bomb,
			Focus,
		}

		public Player(Vec2 pos) : base(pos) {
			RenderComponent renderComponent = new RenderComponent(
				new RenderRow(ConsoleColor.DarkMagenta, " ^^^ "),
				new RenderRow()
					.AddCells(ConsoleColor.Cyan, '<')
					.AddCells(ConsoleColor.White, ')')
					.AddCells(ConsoleColor.Cyan, 'O')
					.AddCells(ConsoleColor.White, '(')
					.AddCells(ConsoleColor.Cyan, '>'),
				new RenderRow(ConsoleColor.Gray, "/W#W\\")
			);
			this.display = new Display(new ConsoleComplexRender2D(renderComponent, true));
			renderComponent = new RenderComponent(
				new RenderRow(ConsoleColor.DarkMagenta, " ^^^ "),
				new RenderRow()
					.AddCells(ConsoleColor.Red, '<')
					.AddCells(ConsoleColor.White, ')')
					.AddCells(ConsoleColor.Red, 'O')
					.AddCells(ConsoleColor.White, '(')
					.AddCells(ConsoleColor.Red, '>'),
				new RenderRow(ConsoleColor.Gray, "/V#V\\")
			);
			this.focusedDisplay = new Display(new ConsoleComplexRender2D(renderComponent, true));
			this.collider = new DotCollider(this);

			this.score = 0;
			this.lives = 3;
			this.bombs = 3;
			this.power = 1;
			this.value = 1000;
			this.graze = 0;
			this.iTicks = (int)Clock.Seconds(1);
			this.bulletPool = new PlayerBulletPool();

			//PENDIENTE: Extraer a clase Tutorial
			this.availableTutorials = new List<InputTutorial>();
			this.availableTutorials.Add(InputTutorial.MoveUp);
			this.availableTutorials.Add(InputTutorial.MoveDown);
			this.availableTutorials.Add(InputTutorial.MoveLeft);
			this.availableTutorials.Add(InputTutorial.MoveRight);
			this.availableTutorials.Add(InputTutorial.Stop);
			this.availableTutorials.Add(InputTutorial.Bomb);
			this.availableTutorials.Add(InputTutorial.Focus);

			this.SubscribeTo(Game.EventType.General);
			this.SubscribeTo(Game.EventType.InstanceDeleted);
		}

		public long Score { get => this.score * 10; }
		public int Lives { get => this.lives; }
		public int Bombs { get => this.bombs; }
		public double Power { get => this.power; }
		public long Value { get => this.value * 10; }
		public int Graze { get => this.graze; }

		protected override void MainUpdate(double deltaTime) {
			this.power += (Game.CurrentButton.Button == GameButton.HKey ? 1 : 0) - (Game.CurrentButton.Button == GameButton.JKey ? 1 : 0);

			this.ShowTutorials();
			this.ClampPosition();
			this.ProcessShoot();
			this.CheckItemPick();
			this.ProcessITicks();
			this.CheckDamage();
			this.CheckInputs();
		}

		public override void OnGameEvent(GameEventArgs e) {
			switch(e.Reason) {
			case Game.EventReason.TickRendered:
				this.DrawPlayerGUI();
				break;
			}
		}

		private void DrawPlayerGUI() {
			ConsoleColor previousForegroundColor = Console.ForegroundColor;
			ConsoleColor previousBackgroundColor = Console.BackgroundColor;
			int uiLeft = GUI.GameRight + 3;
			int uiTop = GUI.GameTop + 3;
			int uiWidth = GUI.UIRight - (GUI.GameRight + 1);

			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.SetCursorPosition(uiLeft - 1, uiTop);
			Console.Write(" P.M. {0,11} ", this.Score);
			uiTop += 1;
			Console.SetCursorPosition(uiLeft - 1, uiTop);
			Console.Write(" Pts. {0,11} ", this.Score);

			uiTop += 2;
			this.DrawResourceBar(this.Lives - 1, uiLeft, uiTop, uiWidth, "Vidas", "♥", ConsoleColor.Magenta);
			uiTop += 1;
			this.DrawResourceBar(this.Bombs, uiLeft, uiTop, uiWidth, "Bombas", "*", ConsoleColor.Green);

			Console.BackgroundColor = ConsoleColor.DarkBlue;
			Console.ForegroundColor = ConsoleColor.White;

			uiTop += 2;
			Console.SetCursorPosition(uiLeft, uiTop);
			Console.Write(string.Format(new CultureInfo("en"), "Poder: {0:0.00}/4.00", this.Power));
			uiTop += 2;
			Console.SetCursorPosition(uiLeft, uiTop);
			Console.Write(String.Format(new CultureInfo("en"), "Valor: {0:n0}", this.Value));
			uiTop += 2;
			Console.SetCursorPosition(uiLeft, uiTop);
			Console.Write("Roce: {0:0}", this.Graze);

			Console.ForegroundColor = previousForegroundColor;
			Console.BackgroundColor = previousBackgroundColor;
		}

		private void DrawResourceBar(int resource, int left, int top, int width, string name, string symbol, ConsoleColor topicColor) {
			Console.SetCursorPosition(left - 1, top);
			Console.BackgroundColor = topicColor;
			Console.ForegroundColor = ConsoleColor.Black;
			string bar = (" " + name).PadRight(width - 1);
			Console.Write(bar);

			Console.SetCursorPosition(GUI.UIRight - 10, top);
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = topicColor;
			Console.Write(" ");

			string resBar = "";
			for(int i = 0; i < 8; i++) {
				if(i < resource)
					resBar += symbol;
				else
					resBar += " ";
			}
			Console.Write(resBar);
		}

		//PENDIENTE: Extraer a clase Tutorial
		private void ShowTutorials() {
			if(availableTutorials.Count == 0)
				return;

			Vec2 center = new Vec2(GUI.GameCenter, GUI.GameMiddle);
			string tutorialTitle = "<{~ Controles básicos ~}>";

			GUI.ConsoleDrawer.DrawText(center.Offset(-tutorialTitle.Length / 2, -2), tutorialTitle, ConsoleColor.DarkYellow);

			if(availableTutorials.Contains(InputTutorial.MoveUp))
				GUI.DrawText(center.Offset(5, 0), "↑");
			if(availableTutorials.Contains(InputTutorial.MoveLeft))
				GUI.DrawText(center.Offset(3, 1), "←");
			if(availableTutorials.Contains(InputTutorial.MoveDown))
				GUI.DrawText(center.Offset(5, 1), "↓");
			if(availableTutorials.Contains(InputTutorial.MoveRight))
				GUI.DrawText(center.Offset(7, 1), "→");
			if(availableTutorials.Contains(InputTutorial.Stop))
				GUI.ConsoleDrawer.DrawText(center.Offset(-7, 1), "Z", ConsoleColor.Red);
			if(availableTutorials.Contains(InputTutorial.Bomb))
				GUI.ConsoleDrawer.DrawText(center.Offset(-5, 1), "X", ConsoleColor.Green);
			if(availableTutorials.Contains(InputTutorial.Focus))
				GUI.ConsoleDrawer.DrawText(center.Offset(-3, 1), "C", ConsoleColor.Blue);
		}

		private void ClampPosition() {
			this.pos.Clamp(GUI.GameTopLeft, GUI.GameBottomRight);
		}

		private void ProcessShoot() {
			if(this.iTicks > Clock.Seconds(5 - 1))
				return;

			if((Game.Ticks % 4) == 0) {
				double angle;
				int pw = (int)Math.Floor(this.power);
				Vec2 pos = this.pos.Rounded;
				int hOffset = pw - 1;
				double wideFactor = Math.PI * (pw - Math.PI / 2) / (2 * (pw + 1));

				for(int shoot = 0; shoot < pw; shoot++) {
					angle = -Math.PI / 2;
					if(pw > 1) {
						angle -= wideFactor * 0.5;
						angle += wideFactor * shoot / (pw - 1);
					}

					bulletPool.EnableShoot(pos.Offset(-hOffset / 2.0 + shoot, -2), Vec2.FromAngle(angle)/*, Game.InstanceEventReason.Shoot*/);
				}
			}
		}

		private void CheckItemPick() {
			List<Item> points = Game.Collisions<Item>(this);

			foreach(Item point in points) {
				switch(point.ItemType) {
				case Item.ContentType.Power:
					this.power = Math.Min(this.power + point.Amount * 0.01, 4);
					break;
				case Item.ContentType.Point:
					this.score += point.Amount * this.value;
					break;
				case Item.ContentType.Value:
					this.score += point.Amount;
					this.value += point.Amount;
					break;
				}
				Game.DeleteInstance(point);
			}
		}

		private void ProcessITicks() {
			if(this.iTicks <= 0)
				return;

			this.iTicks -= 1;
			this.display.Visible = (iTicks / 2 % 2) == 0;

			if(this.iTicks > Clock.Seconds(5 - 1.5) && this.iTicks % 20 == 0)
				this.pos.Y -= 1;
		}

		private void CheckDamage() {
			if(!(Game.CollidesWith<Enemy>(this) || Game.CollidesWith<EnemyBullet>(this))) {
				this.CheckGraze();
				return;
			}

			this.lives -= 1;
			this.bombs = 3;
			this.power = Math.Max(1, this.power - 1);

			this.pos = new Vec2(GUI.GameCenter, GUI.GameBottom);
			this.vel = Vec2.Zero;
			this.SetFocused(false);

			this.iTicks = Convert.ToInt32(Clock.Seconds(5));
			if(this.lives <= 0)
				Game.Ended = true;
		}

		private void CheckGraze() {
			foreach(Enemy enemy in Game.FindInstances<Enemy>()) {
				if(this.Pos.DistanceTo(enemy.Pos) > 2.5)
					continue;
				this.value += 1;
				this.graze += 1;
			}
		}

		private void CheckInputs() {
			if(iTicks > Clock.Seconds(5 - 1.5))
				return;

			int multikeyTicks = 6;

			GameButton pressedKey = Game.CurrentButton.Button;
			long pressedTick = Game.CurrentButton.Tick;

			if(pressedKey == 0)
				return;

			long lastTick = Game.PreviousButton.Tick;
			bool isMultiKey = (pressedTick - lastTick) < multikeyTicks;
			Vec2 dir = Vec2.Zero;
			Vec2 velFactor = new Vec2(1, 0.5);
			if(focused)
				velFactor *= 0.5;

			switch(pressedKey) {
			case GameButton.Up:
				availableTutorials.Remove(InputTutorial.MoveUp);
				dir = Vec2.Up;
				break;

			case GameButton.Down:
				availableTutorials.Remove(InputTutorial.MoveDown);
				dir = Vec2.Down;
				break;

			case GameButton.Left:
				availableTutorials.Remove(InputTutorial.MoveLeft);
				dir = Vec2.Left;
				break;

			case GameButton.Right:
				availableTutorials.Remove(InputTutorial.MoveRight);
				dir = Vec2.Right;
				break;

			case GameButton.ZKey:
				availableTutorials.Remove(InputTutorial.Stop);
				this.vel = Vec2.Zero;
				break;

			case GameButton.XKey:
				availableTutorials.Remove(InputTutorial.Bomb);
				if(this.bombs < 0) break;

				this.bombs -= 1;

				break;

			case GameButton.CKey:
				availableTutorials.Remove(InputTutorial.Focus);
				this.ToggleFocused();
				break;

			case GameButton.Escape:
				Game.Ended = true;
				break;
			}

			if(dir != Vec2.Zero) {
				if(isMultiKey && this.vel.Dot(dir) == 0)
					this.vel = (dir + vel.Sign) * velFactor;
				else
					this.vel = dir * velFactor;
			}
		}

		public void AddScore(double valueMultiplier) {
			this.score += Convert.ToInt64(this.value * valueMultiplier);
			this.value += (int)(valueMultiplier * 10);
		}
		public void AddScore(double valueMultiplier, int flatAmount) {
			this.score += Convert.ToInt64(this.value * valueMultiplier) + flatAmount;
			this.value += (int)(valueMultiplier * 10);
		}

		private void ToggleFocused() {
			if(this.focused) {
				this.vel *= 2;
				this.focused = false;
			} else {
				this.vel *= 0.5;
				this.focused = true;
			}
			Display tempDisplay = this.display;
			this.display = this.focusedDisplay;
			this.focusedDisplay = tempDisplay;
		}

		private void SetFocused(bool newFocused) {
			if(this.focused != newFocused)
				this.ToggleFocused();
		}
	}

	class PlayerBullet: GameObject {
		public PlayerBullet(Vec2 pos, Vec2 vel) : base(pos, vel) {
			this.display = new Display(new ConsoleDotRender('o', ConsoleColor.DarkGray));
			this.collider = new DotCollider(this);
		}

		protected override void MainUpdate(double deltaTime) {
			if(this.pos.Y < 0) {
				Game.DisableInstance(this);
				return;
			}

			List<Enemy> collidedEnemies = Game.Collisions<Enemy>(this);

			if(collidedEnemies.Count == 0)
				return;

			Parallel.ForEach(collidedEnemies, collidedEnemy => collidedEnemy.Damage(1));

			Game.DisableInstance(this);
		}
	}
	class PlayerBulletPool {
		private readonly List<PlayerBullet> bullets;
		private readonly int bulletLimit;
		private int bulletIndex;

		public PlayerBulletPool(int bulletLimit = 500) {
			this.bulletIndex = 0;
			this.bulletLimit = bulletLimit;
			this.bullets = new List<PlayerBullet>();

			Vec2 zeroVec = Vec2.Zero;
			for(int i = 0; i < bulletLimit; i++) {
				PlayerBullet bullet = new PlayerBullet(zeroVec, zeroVec);
				this.bullets.Add(bullet);
				Game.AddInstance(bullet);
				Game.DisableInstance(bullet);
			}
				
		}

		private PlayerBullet CurrentBullet {
			get {
				if(this.bulletIndex < this.bulletLimit)
					return this.bullets[this.bulletIndex++];

				this.bulletIndex = 1;
				return this.bullets[0];
			}
		}

		public void EnableShoot(Vec2 pos, Vec2 vel) {
			PlayerBullet currentBullet = this.CurrentBullet;
			currentBullet.Pos = pos;
			currentBullet.Vel = vel;
			Game.EnableInstance(currentBullet);
		}
	}
}