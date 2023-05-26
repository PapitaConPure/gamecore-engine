using System;

namespace GameCore {
    public class GameEventArgs: EventArgs {
        private readonly Game.EventReason reason;

        public GameEventArgs(Game.EventReason reason) {
            this.reason = reason;
        }

        public Game.EventReason Reason => this.reason;
    }

    public class GameKeyEventArgs: EventArgs {
        private readonly ConsoleKey key;

        public GameKeyEventArgs(ConsoleKey key) {
            this.key = key;
        }

        public ConsoleKey Key => this.key;
    }

    public class GameInstanceEventArgs: EventArgs {
        private readonly Game.InstanceEventReason reason;

        public GameInstanceEventArgs(Game.InstanceEventReason reason) {
            this.reason = reason;
        }

        public Game.InstanceEventReason Reason => this.reason;
    }

    public class GameInstanceDeletedEventArgs: EventArgs {
        private readonly Game.InstanceDeletedEventReason reason;

        public GameInstanceDeletedEventArgs(Game.InstanceDeletedEventReason reason) {
            this.reason = reason;
        }

        public Game.InstanceDeletedEventReason Reason => this.reason;
    }
}
