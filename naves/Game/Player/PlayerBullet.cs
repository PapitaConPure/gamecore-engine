using System;
using System.Collections.Generic;
using GameCore;
using System.Threading.Tasks;

namespace naves {
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