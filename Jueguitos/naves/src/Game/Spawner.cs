using System;
using GameCore;

namespace naves {
    class Spawner {
        private readonly long streamStart;
        private readonly long streamEnd;
        private readonly Vec2 pos;
        private readonly ISpawnType spawn;
        private int amount;
        private readonly Sequence sequence;
        private readonly int interval;

        public Spawner(long start, long duration, Vec2 pos, ISpawnType enemy, int amount, Sequence sequence) {
            if(start < 0)
                throw new ArgumentOutOfRangeException("El inicio no puede ser menor que 0");
            if(duration < 0)
                throw new ArgumentOutOfRangeException("La duración debe ser positiva");
            if(amount < 1 || amount > duration)
                throw new ArgumentOutOfRangeException("La cantidad debe ser mayor que 0 y menor que la duración del spawn");

            this.streamStart = start;
            this.streamEnd = start + duration;
            this.pos = pos;
            this.spawn = enemy;
            this.amount = amount;
            this.sequence = sequence;
            this.interval = Convert.ToInt32((this.streamEnd - this.streamStart) / Math.Max(1, this.amount - 1));
        }

        public long StreamStart { get => this.streamStart; }
        public long StreamEnd { get => this.streamEnd; }
        public Vec2 Pos { get => this.pos; }
        public int Amount { get => this.amount; }
        public ISpawnType Enemy { get => this.spawn; }
        public Sequence Path { get => this.sequence; }

        public void Update() {
            if(Game.Ticks < this.streamStart || Game.Ticks > this.streamEnd)
                return;

            if((Game.Ticks - this.streamStart) % this.interval != 0)
                return;

            if(this.amount <= 0)
                return;

            GameObject instance = this.CreateEnemy();
            Game.AddInstance(instance);
            this.amount -= 1;
        }

        private GameObject CreateEnemy() {
            return new Enemy(this.pos, this.sequence, this.spawn);
        }
    }

    interface ISpawnType {
        int HP();
        int PowerItems();
        int PointItems();
        Display Display();
        Collider Collider(GameObject self);
    }
}