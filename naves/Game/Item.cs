using System;
using GameCore;

namespace naves {
	class Item: GameObject {
		private bool homing;
		private readonly ItemContent content;

		public Item(Vec2 pos, ItemContent itemContent): base(pos, Vec2.Up * 0.2) {
			this.display = itemContent.Display;
			this.collider = new DotCollider(this);

			this.content = itemContent;
			this.homing = false;
		}
		public enum ContentType {
			Power,
			Point,
			Value,
		}

		public int Amount => this.content.Amount;
		public ContentType ItemType => this.content.Type;

		protected override void MainUpdate(double deltaTime) {
			if(this.Pos.Y > GUI.GameBottom) {
				Game.DeleteInstance(this);
				return;
			}

			this.Move();
		}

		private void Move() {
			this.pos.X = MathX.Clamp(this.Pos.X, GUI.GameLeft, GUI.GameRight);

			Player player = Game.FindInstance<Player>();

			if(this.pos.DistanceTo(player.Pos) < 6)
				this.homing = true;

			if(this.homing)
				this.HomePlayer(player);
			else
				this.vel.Y = Math.Min(this.vel.Y + 0.005, 0.2);
		}

		private void HomePlayer(Player player) {
			double distance = pos.DistanceTo(player.Pos);

			this.vel = pos.DirectionTo(player.Pos, Math.Min(1.2, distance));
		}
	}

	abstract class ItemContent {
		protected int amount;

		public ItemContent(int amount) {
			this.amount = amount;
		}

		public int Amount => this.amount;
		public abstract ItemContent Take(int amount);
		public abstract Display Display { get; }
		public abstract Item.ContentType Type { get; }
		protected int Subtract(int amount) {
			this.amount -= amount;
			if(this.amount < 0) {
				amount += this.amount;
				this.amount = 0;
			}
			return amount;
		}
	}

	class PowerItemContent: ItemContent {
		public PowerItemContent(int amount): base(amount) {}
		public override ItemContent Take(int amount) => new PowerItemContent(this.Subtract(amount));
		public override Display Display => new Display(new ConsoleDotRender('P', ConsoleColor.DarkRed));
		public override Item.ContentType Type => Item.ContentType.Power;
	}

	class PointItemContent: ItemContent {
		public PointItemContent(int amount): base(amount) {}
		public override ItemContent Take(int amount) => new PointItemContent(this.Subtract(amount));
		public override Display Display => new Display(new ConsoleDotRender('*', ConsoleColor.DarkBlue));
		public override Item.ContentType Type => Item.ContentType.Point;
	}

	class ValueItemContent: ItemContent {
		public ValueItemContent(int amount) : base(amount) {}
		public override ItemContent Take(int amount) => new ValueItemContent(this.Subtract(amount));
		public override Display Display => new Display(new ConsoleDotRender('•', ConsoleColor.DarkCyan));
		public override Item.ContentType Type => Item.ContentType.Value;
	}
}
