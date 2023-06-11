using System;
using System.Collections.Generic;
using GameCore;

namespace naves {
	class StageSequencer: GameObject {
		private readonly List<Spawner> spawns;
		private Sequence currentSequence;

		public StageSequencer(List<Spawner> stage) : base(Vec2.Zero) {
			this.spawns = new List<Spawner>();
			this.SetupStageSequence(stage);
			this.SubscribeTo(Game.EventType.General);
		}

		public override void OnGameEvent(GameEventArgs e) {
			switch(e.Reason) {
			case Game.EventReason.TickProcessed:
				this.UpdateSpawns();
				break;
			}
		}

		private void UpdateSpawns() {
			foreach(Spawner spawn in this.spawns)
				spawn.Update();
		}

		private void SetupStageSequence(List<Spawner> _) {
			this.Stage1Sequence();
		}

		private void AddSpawn(long start, long duration, Vec2 pos, ISpawnType enemy, int amount, Sequence sequence = null) {
			if(sequence is null)
				sequence = this.currentSequence;

			Spawner spawn = new Spawner(start, duration, pos, enemy, amount, sequence);
			this.spawns.Add(spawn);
		}

		private void Stage1Sequence() {
			this.currentSequence = new Sequence().AddSteps(
				new VelStep(0, Vec2.Left * 0.5),
				new VelStep(Clock.Seconds(0.25), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(0.25) + 2, Vec2.Right * 0.5),
				new VelStep(Clock.Seconds(0.75), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(0.75) + 2, Vec2.Left * 0.5),
				new VelStep(Clock.Seconds(1.25), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(1.25) + 2, Vec2.Right * 0.5),
				new VelStep(Clock.Seconds(1.75), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(1.75) + 2, Vec2.Left * 0.5),
				new KillStep(Clock.Seconds(4))
			);

			this.AddSpawn(Clock.Seconds(0), Clock.Seconds(1), new Vec2(CGUI.GameRight - 7, CGUI.GameTop), new Enemy1(), 8);

			this.currentSequence = new Sequence().AddSteps(
				new VelStep(0, Vec2.Left * 0.25),
				new VelStep(Clock.Seconds(0.5), Vec2.Down * 0.25),
				new VelStep(Clock.Seconds(0.5) + 4, Vec2.Right * 0.25),
				new VelStep(Clock.Seconds(1.5), Vec2.Down * 0.25),
				new VelStep(Clock.Seconds(1.5) + 4, Vec2.Left * 0.25),
				new VelStep(Clock.Seconds(2.5), Vec2.Down * 0.25),
				new VelStep(Clock.Seconds(2.5) + 4, Vec2.Right * 0.25),
				new VelStep(Clock.Seconds(3.5), Vec2.Down * 0.25),
				new VelStep(Clock.Seconds(3.5) + 4, Vec2.Left * 0.25),
				new KillStep(Clock.Seconds(8))
			);

			this.AddSpawn(Clock.Seconds(2), Clock.Seconds(1), new Vec2(CGUI.GameRight - 10, CGUI.GameTop - 2), new Enemy2(), 4);

			this.currentSequence = new Sequence().AddSteps(
				new VelStep(0, new Vec2(1, 1)),
				new VelStep(2, Vec2.Right * 0.5),
				new VelStep(Clock.Seconds(0.25), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(0.25) + 2, Vec2.Left * 0.5),
				new VelStep(Clock.Seconds(0.75), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(0.75) + 2, Vec2.Right * 0.5),
				new VelStep(Clock.Seconds(1.25), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(1.25) + 2, Vec2.Left * 0.5),
				new VelStep(Clock.Seconds(1.75), Vec2.Down * 0.5),
				new VelStep(Clock.Seconds(1.75) + 2, Vec2.Right * 0.5),
				new KillStep(Clock.Seconds(4))
			);

			this.AddSpawn(Clock.Seconds(8), Clock.Seconds(2), new Vec2(CGUI.GameLeft + 5, CGUI.GameTop), new Enemy1(), 16);
			this.AddSpawn(Clock.Seconds(12), Clock.Seconds(1), new Vec2(CGUI.GameRight - 8, CGUI.GameTop), new Enemy1(), 16);

			this.currentSequence = new Sequence().AddSteps(
				new VelStep(0 + 12 * 0, Vec2.Down * 0.5),
				new VelStep(2 + 12 * 0, Vec2.Right * 0.5),
				new VelStep(2 + 12 * 1, Vec2.Down * 0.5),
				new VelStep(2 + 12 * 2, Vec2.Left * 0.5),
				new VelStep(2 + 12 * 3, Vec2.Up * 0.5),
				new RepeatStep(2 + 12 * 4 - 1, 0, 2),
				new VelStep(2 + 12 * 4 + 4 * 0, Vec2.Right * 0.5),
				new VelStep(2 + 12 * 4 + 4 * 1, Vec2.Down * 0.5),
				new RepeatStep(2 + 12 * 4 + 4 * 2 - 1, 2 + 12 * 4 + 4 * 0, 3),
				new VelStep(2 + 12 * 4 + 4 * 2, Vec2.Right * 0.5),
				new VelStep(2 + 12 * 6 + 4 * 2, Vec2.Up * 0.5),
				new VelStep(2 + 12 * 8 + 4 * 2, Vec2.Left * 0.5),
				new KillStep(2 + 12 * 12 + 4 * 2)
			);

			this.AddSpawn(Clock.Seconds(16), Clock.Seconds(4), new Vec2(CGUI.GameLeft + 4, CGUI.GameTop), new Enemy3(), 4);
		}
	}
}