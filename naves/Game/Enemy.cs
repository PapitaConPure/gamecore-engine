using System;
using GameCore;

namespace naves {
	class Enemy: GameObject {
		private int hp;
		private readonly int powerItems;
		private readonly int pointItems;
		private readonly Sequence sequence; 

		public Enemy(Vec2 pos, Sequence sequence, ISpawnType enemyType) : base(pos) {
			this.sequence = sequence.Copy;
			this.hp = enemyType.HP();
			this.display = enemyType.Display();
			this.collider = enemyType.Collider(this);
			this.powerItems = enemyType.PowerItems();
			this.pointItems = enemyType.PointItems();
		}

		public void Damage(int damage) {
			if(damage < 0)
				damage *= -1;

			this.hp -= damage;

			if(this.hp > 0)
				return;

			Game.DeleteInstance(this, Game.InstanceDeletedEventReason.Killed);
			Game.FindInstance<Player>().AddScore(0.1, 10);
			this.BurstItems(new PowerItemContent(this.powerItems));
			this.BurstItems(new PointItemContent(this.pointItems));
		}

		protected override void MainUpdate(double deltaTime) {
			this.sequence.Follow(this);
		}

		private void BurstItems(ItemContent content) {
			int amount = content.Amount;
			int spread = 2 * amount / (amount + 1) + (amount - 1) / 8;

			do {
				int selected = Game.RNG.Next(amount) + 1;
				ItemContent selectedContent = content.Take(selected);
				Game.AddInstance(new Item(this.pos + Vec2.Random * spread, selectedContent));
			} while(content.Amount > 0);
		}
	}

	struct Enemy1: ISpawnType {
		public int HP() => 1;
		public int PowerItems() => 1;
		public int PointItems() => 1;
		public Display Display() => new Display(new ConsoleDotRender('X', ConsoleColor.Red));
		public Collider Collider(GameObject self) => new DotCollider(self);
	}

	struct Enemy2: ISpawnType {
		public int HP() => 10;
		public int PowerItems() => 32;
		public int PointItems() => 16;
		public Display Display() => new Display(new ConsoleRender2D("WWW\n(O)\n\\_/", ConsoleColor.Magenta, true));
		public Collider Collider(GameObject self) => new RectCollider(self, new Rect(new Vec2(-1, -1), new Vec2(1, 1)));
	}

	struct Enemy3: ISpawnType {
		public int HP() => 6;
		public int PowerItems() => 3;
		public int PointItems() => 4;
		public Display Display() {
			ConsoleDisplayComponent component = new ConsoleDisplayComponent(
				new ConsoleDisplayRow(
					new ConsoleDisplayCell('<', ConsoleColor.White),
					new ConsoleDisplayCell('O', ConsoleColor.Yellow),
					new ConsoleDisplayCell('>', ConsoleColor.White)
				),
				new ConsoleDisplayRow(
					new ConsoleDisplayCell('¡', ConsoleColor.Magenta),
					new ConsoleDisplayCell('¡', ConsoleColor.DarkMagenta),
					new ConsoleDisplayCell('¡', ConsoleColor.Magenta)
				)
			);

			return new Display(new ConsoleComplexRender2D(component, true));
		}
		public Collider Collider(GameObject self) => new RectCollider(self, new Rect(new Vec2(-1, -1), new Vec2(1, 0)));
	}

	class EnemyBullet: GameObject {
		public EnemyBullet(Vec2 pos, Vec2 vel) : base(pos, vel) {
			this.display = new Display(new ConsoleDotRender("*", ConsoleColor.Yellow));
			this.collider = new DotCollider(this);
		}
	}

}