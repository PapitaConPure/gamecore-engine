using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore {
    public class Sequence {
        private readonly List<SequenceStep> steps;
        private long ticks;

        public Sequence() {
            this.steps = new List<SequenceStep>();
            this.ticks = 0;
        }
        public Sequence(Sequence sequence) {
            this.steps = new List<SequenceStep>(sequence.steps.Select(step => step.Copy));
            this.ticks = 0;
        }

        public Sequence Copy => new Sequence(this);
		public long Ticks { get => this.ticks; }

		public Sequence AddStep(SequenceStep step) {
            this.steps.Add(step);
            return this;
        }
        public Sequence AddSteps(params SequenceStep[] steps) {
            this.steps.AddRange(steps);
            return this;
        }
        public Sequence AddSteps(List<SequenceStep> steps) {
            this.steps.AddRange(steps);
            return this;
        }

        public void Follow(GameObject instance) {
            SequenceStep step = this.Step(this.ticks);
            this.ticks++;

            if(step is null)
                return;

            step.Follow(this, instance);
        }

        public SequenceStep Return(long steps = 1) {
            SequenceStep step = this.Step(this.ticks);
            this.ticks -= steps;

            if(this.ticks < 0)
                this.ticks = 0;
            
            return step;
        }
		public SequenceStep Skip(long steps = 1) {
            SequenceStep step = this.Step(this.ticks);
            this.ticks += steps;
            return step;
        }

		public SequenceStep Previous(long steps = 1) => this.Step(Math.Max(0, this.ticks - steps));
		public SequenceStep Next(long steps = 1) => this.Step(this.ticks + steps);
		public SequenceStep Current() => this.Step(this.ticks);
		public SequenceStep Step(long tick) => this.steps.Find(step => step.Tick == tick);
    }

    public abstract class SequenceStep {
        protected readonly long tick;
        public SequenceStep(long tick) => this.tick = tick;
        public long Tick => this.tick;
        public abstract SequenceStep Copy { get; }
        public abstract void Follow(Sequence parent, GameObject instance);
    }
    public class VelStep: SequenceStep {
        private readonly Vec2 vel;
        public VelStep(long tick, Vec2 vel) : base(tick) => this.vel = vel;
        public override SequenceStep Copy => new VelStep(this.tick, this.vel);
        public override void Follow(Sequence parent, GameObject instance) => instance.Vel = this.vel.Copy;
    }
    public class SpawnStep: SequenceStep {
        private readonly GameObject shoot;
        private readonly Game.InstanceEventReason reason;
        public SpawnStep(long tick, GameObject instance, Game.InstanceEventReason reason = Game.InstanceEventReason.Related) : base(tick) {
            this.shoot = instance;
            this.reason = reason;
        }
        public override SequenceStep Copy => new SpawnStep(this.tick, this.shoot);
        public override void Follow(Sequence parent, GameObject instance) => Game.AddInstance(instance, this.reason);
    }
    public class KillStep: SequenceStep {
        public KillStep(long tick): base(tick) { }

        public override SequenceStep Copy => new KillStep(this.tick);
        public override void Follow(Sequence parent, GameObject instance) => Game.DeleteInstance(instance);
    }
    public class RepeatStep: SequenceStep {
        private readonly long targetTick;
        private int times;

        public RepeatStep(long tick, long targetTick, int times = -1) : base(tick) {
            if(targetTick < 0)
                targetTick *= -1;
            this.targetTick = targetTick;
            this.times = times;
        }

        public override SequenceStep Copy => new RepeatStep(this.tick, this.targetTick, this.times);

		public override void Follow(Sequence parent, GameObject instance) {
            if(this.times == 0) return;
            this.times--;
            parent.Return(Math.Abs(parent.Ticks - this.targetTick) + 1);
		}
	}
}
