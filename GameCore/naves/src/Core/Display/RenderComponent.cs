using System;
using System.Collections.Generic;

namespace GameCore {
	public class RenderComponent {
        private readonly List<RenderRow> rows;
        private readonly int maxWidth;

        public RenderComponent(List<RenderRow> rows = null) {
            this.rows = rows ?? new List<RenderRow>();
            this.maxWidth = 0;
            
            foreach(RenderRow row in rows)
                if(this.maxWidth < row.Width)
                    this.maxWidth = row.Width;
        }
        public RenderComponent(params RenderRow[] rows) {
            this.rows = new List<RenderRow>(rows);
            this.maxWidth = 0;

            foreach(RenderRow row in rows)
                if(this.maxWidth < row.Width)
                    this.maxWidth = row.Width;
        }
        public RenderComponent(ConsoleColor color, params string[] symbolLines) {
            this.rows = new List<RenderRow>();
            this.maxWidth = 0;

            foreach(string symbolLine in symbolLines) {
                RenderRow renderRow = new RenderRow(color, symbolLine);
                this.rows.Add(renderRow);
                if(this.maxWidth < renderRow.Width)
                    this.maxWidth = renderRow.Width;
            }
        }

        public RenderRow this[int i] => this.rows[i];
        public List<RenderRow> Rows => this.rows;
        public int Width {
            get {
                int width = 0;
                foreach(RenderRow row in this.rows) {
                    if(row.Width > width)
                        width = row.Width;
                }
                return width;
            }
        }
        public int Height { get => this.rows.Count; }

        public void AddRows(params RenderRow[] rows) {
            foreach(RenderRow row in rows)
                this.rows.Add(row);
		}

        public RenderRow RowAt(int i) {
            return this.rows[i];
		}
    }

    public class RenderRow {
        private readonly List<RenderCell> cells;
		private ConsoleColor colorStream;
        private bool colorStreamEnabled;

		public RenderRow(List<RenderCell> cells = null) {
            this.cells = cells ?? new List<RenderCell>();
            this.colorStreamEnabled = false;
        }
        public RenderRow(params RenderCell[] cells) {
            this.cells = new List<RenderCell>(cells);
            this.colorStreamEnabled = false;
        }
        public RenderRow(ConsoleColor color, bool stream = true, List<RenderCell> cells = null) {
            this.cells = cells ?? new List<RenderCell>();
            this.colorStreamEnabled = stream;
            this.colorStream = color;
        }
        public RenderRow(ConsoleColor color, bool stream = true, params RenderCell[] cells) {
            this.cells = new List<RenderCell>(cells);
            this.colorStreamEnabled = stream;
            this.colorStream = color;
        }
        public RenderRow(ConsoleColor color, string cells) {
            this.cells = new List<RenderCell>();
            this.colorStreamEnabled = false;

            foreach(char cell in cells.ToCharArray())
                this.cells.Add(new RenderCell(cell, color));
        }

        public RenderCell this[int i] => this.cells[i];
        public int Width => this.cells.Count;

		public RenderRow ChangeColorStream(ConsoleColor color) {
			this.colorStream = color;
            this.colorStreamEnabled = true;
            return this;
        }

		public RenderRow ResetColorStream() {
			this.colorStreamEnabled = false;
            return this;
		}

		public RenderRow AddCells(List<RenderCell> cells) {
            foreach(RenderCell cell in cells) {
                if(colorStreamEnabled)
                    cell.Copy.Colorize(colorStream);
                this.cells.Add(cell);
            }   
            return this;
        }
        public RenderRow AddCells(params RenderCell[] cells) {
            foreach(RenderCell cell in cells) {
                if(colorStreamEnabled)
                    cell.Copy.Colorize(colorStream);
                this.cells.Add(cell);
            }
            return this;
        }
        public RenderRow AddCells(ConsoleColor color, params char[] symbols) {
            foreach(char symbol in symbols)
                this.cells.Add(new RenderCell(symbol, color));
            return this;
        }
        public RenderRow AddCells(ConsoleColor color, string symbols) {
            foreach(char symbol in symbols.ToCharArray())
                this.cells.Add(new RenderCell(symbol, color));
            return this;
        }

        public RenderCell CellAt(int i) {
            return this.cells[i];
        }
    }

    public class RenderCell {
        private readonly char symbol;
        private ConsoleColor color;

        public RenderCell(char symbol, ConsoleColor color = ConsoleColor.White) {
            this.symbol = symbol;
            this.color = color;
        }
        public RenderCell(char symbol) {
            this.symbol = symbol;
        }
        public RenderCell(RenderCell old) {
            this.symbol = old.symbol;
            this.color = old.color;
        }

        public RenderCell Copy => new RenderCell(this);
		public char Symbol => this.symbol;
		public ConsoleColor Color => this.color;

        public void Colorize(ConsoleColor color) => this.color = color;
    }
}
