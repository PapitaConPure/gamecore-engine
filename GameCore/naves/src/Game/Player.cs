using System;
using System.Collections.Generic;
using System.Globalization;
using GameCore;

namespace naves {
    class Player: GameObject {
        private long score;
        private int lives;
        private int bombs;
        private double power;
        private long value;
        private int graze;
        private int iTicks;
        private bool focused;
        private Display focusedDisplay;

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
            this.iTicks = (int)Game.Seconds(1);

            this.SubscribeTo(Game.EventType.General);
            this.SubscribeTo(Game.EventType.InstanceDeleted);
        }

        public long Score { get => this.score * 10; }
        public int Lives { get => this.lives; }
        public int Bombs { get => this.bombs; }
        public double Power { get => this.power; }
        public long Value { get => this.value * 10; }
        public int Graze { get => this.graze; }

        protected override void MainUpdate() {
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

        public override void OnGameInstanceDeletedEvent(GameObject sender, GameInstanceDeletedEventArgs e) {
            if(ReferenceEquals(this, sender) || e.Reason != Game.InstanceDeletedEventReason.Killed)
                return;

            if(!(sender is Enemy))
                return;

            this.AddScore(0.1, 10);
        }

        private void DrawPlayerGUI() {
            ConsoleColor previousForegroundColor = Console.ForegroundColor;
            ConsoleColor previousBackgroundColor = Console.BackgroundColor;
            int uiLeft = CGUI.GameRight + 3;
            int uiTop = CGUI.GameTop + 3;
            int uiWidth = CGUI.UIRight - (CGUI.GameRight + 1);

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

            Console.SetCursorPosition(CGUI.UIRight - 10, top);
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

        private void ClampPosition() {
            this.pos.Clamp(CGUI.GameTopLeft, CGUI.GameBottomRight);
        }

        private void ProcessShoot() {
            if(this.iTicks > Game.Seconds(5 - 1))
                return;

            if(Game.Ticks % 4 == 0) {
                PlayerBullet bullet;
                double angle;
                int actualPower = (int)Math.Floor(this.power);
                int hOffset = actualPower - 1;
                double wideFactor = Math.PI / 32 * actualPower;

                for(int shoot = 0; shoot < actualPower; shoot++) {
                    angle = -Math.PI / 2;
                    if(actualPower > 1) {
                        angle -= wideFactor * 0.5 * (actualPower - 1);
                        angle += wideFactor * shoot;
                    }

                    bullet = new PlayerBullet(this.pos.Offset(-hOffset / 2.0 + shoot, -2), Vec2.FromAngle(angle));
                    Game.AddInstance(bullet, Game.InstanceEventReason.Shoot);
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

            if(this.iTicks > Game.Seconds(5 - 1.5) && this.iTicks % 20 == 0)
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

            this.pos = new Vec2(CGUI.GameCenter, CGUI.GameBottom);
            this.vel = Vec2.Zero;
            this.SetFocused(false);

            this.iTicks = Convert.ToInt32(Game.Seconds(5));
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
            if(iTicks > Game.Seconds(5 - 1.5))
                return;

            int multikeyTicks = 6;

            GameButton pressedKey = Game.CurrentKey.Button;
            long pressedTick = Game.CurrentKey.Tick;

            if(pressedKey == 0)
                return;

            long lastTick = Game.PreviousKey.Tick;
            bool isMultiKey = (pressedTick - lastTick) < multikeyTicks;
            Vec2 dir = Vec2.Zero;
            Vec2 velFactor = new Vec2(1, 0.5);
            if(focused)
                velFactor *= 0.5;

            switch(pressedKey) {
            case GameButton.Up:    dir = Vec2.Up;        break;
            case GameButton.Down:  dir = Vec2.Down;      break;
            case GameButton.Left:  dir = Vec2.Left;      break;
            case GameButton.Right: dir = Vec2.Right;     break;
            case GameButton.ZKey:  this.vel = Vec2.Zero; break;

            case GameButton.XKey:
                if(this.bombs < 0) break;

                this.bombs -= 1;

                break;

            case GameButton.CKey:
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

        protected override void MainUpdate() {
            if(this.pos.Y < 0) {
                Game.DeleteInstance(this);
                return;
            }

            List<Enemy> collidedEnemies = Game.Collisions<Enemy>(this);
            if(collidedEnemies.Count == 0)
                return;

            foreach(Enemy collidedEnemy in collidedEnemies)
                collidedEnemy.Damage(1);

            Game.DeleteInstance(this);
        }
    }
}