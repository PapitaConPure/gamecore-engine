using System;
using System.Collections.Generic;

namespace GameCore {
	public class ConsoleDisplayComponent {
		private readonly List<ConsoleDisplayRow> rows;
		private readonly int maxWidth;

		public ConsoleDisplayComponent(List<ConsoleDisplayRow> rows = null) {
			this.rows = rows ?? new List<ConsoleDisplayRow>();
			this.maxWidth = 0;
			
			foreach(ConsoleDisplayRow row in rows)
				if(this.maxWidth < row.Width)
					this.maxWidth = row.Width;
		}
		public ConsoleDisplayComponent(params ConsoleDisplayRow[] rows) {
			this.rows = new List<ConsoleDisplayRow>(rows);
			this.maxWidth = 0;

			foreach(ConsoleDisplayRow row in rows)
				if(this.maxWidth < row.Width)
					this.maxWidth = row.Width;
		}
		public ConsoleDisplayComponent(ConsoleColor color, params string[] symbolLines) {
			this.rows = new List<ConsoleDisplayRow>();
			this.maxWidth = 0;

			foreach(string symbolLine in symbolLines) {
				ConsoleDisplayRow renderRow = new ConsoleDisplayRow(color, symbolLine);
				this.rows.Add(renderRow);
				if(this.maxWidth < renderRow.Width)
					this.maxWidth = renderRow.Width;
			}
		}

		public ConsoleDisplayRow this[int i] => this.rows[i];
		public List<ConsoleDisplayRow> Rows => this.rows;
		public int Width {
			get {
				int width = 0;
				foreach(ConsoleDisplayRow row in this.rows) {
					if(row.Width > width)
						width = row.Width;
				}
				return width;
			}
		}
		public int Height { get => this.rows.Count; }

		public void AddRows(params ConsoleDisplayRow[] rows) {
			foreach(ConsoleDisplayRow row in rows)
				this.rows.Add(row);
		}

		public ConsoleDisplayRow RowAt(int i) {
			return this.rows[i];
		}
	}

	public class ConsoleDisplayRow {
		private readonly List<ConsoleDisplayCell> cells;
		private ConsoleColor colorStream;
		private bool colorStreamEnabled;

		public ConsoleDisplayRow(List<ConsoleDisplayCell> cells = null) {
			this.cells = cells ?? new List<ConsoleDisplayCell>();
			this.colorStreamEnabled = false;
		}
		public ConsoleDisplayRow(params ConsoleDisplayCell[] cells) {
			this.cells = new List<ConsoleDisplayCell>(cells);
			this.colorStreamEnabled = false;
		}
		public ConsoleDisplayRow(ConsoleColor color, bool stream = true, List<ConsoleDisplayCell> cells = null) {
			this.cells = cells ?? new List<ConsoleDisplayCell>();
			this.colorStreamEnabled = stream;
			this.colorStream = color;
		}
		public ConsoleDisplayRow(ConsoleColor color, bool stream = true, params ConsoleDisplayCell[] cells) {
			this.cells = new List<ConsoleDisplayCell>(cells);
			this.colorStreamEnabled = stream;
			this.colorStream = color;
		}
		public ConsoleDisplayRow(ConsoleColor color, string cells) {
			this.cells = new List<ConsoleDisplayCell>();
			this.colorStreamEnabled = false;

			foreach(char cell in cells.ToCharArray())
				this.cells.Add(new ConsoleDisplayCell(cell, color));
		}

		public ConsoleDisplayCell this[int i] => this.cells[i];
		public int Width => this.cells.Count;

		public ConsoleDisplayRow ChangeColorStream(ConsoleColor color) {
			this.colorStream = color;
			this.colorStreamEnabled = true;
			return this;
		}

		public ConsoleDisplayRow ResetColorStream() {
			this.colorStreamEnabled = false;
			return this;
		}

		public ConsoleDisplayRow AddCells(List<ConsoleDisplayCell> cells) {
			foreach(ConsoleDisplayCell cell in cells) {
				if(colorStreamEnabled)
					cell.Copy.Colorize(colorStream);
				this.cells.Add(cell);
			}   
			return this;
		}
		public ConsoleDisplayRow AddCells(params ConsoleDisplayCell[] cells) {
			foreach(ConsoleDisplayCell cell in cells) {
				if(colorStreamEnabled)
					cell.Copy.Colorize(colorStream);
				this.cells.Add(cell);
			}
			return this;
		}
		public ConsoleDisplayRow AddCells(ConsoleColor color, params char[] symbols) {
			foreach(char symbol in symbols)
				this.cells.Add(new ConsoleDisplayCell(symbol, color));
			return this;
		}
		public ConsoleDisplayRow AddCells(ConsoleColor color, string symbols) {
			foreach(char symbol in symbols.ToCharArray())
				this.cells.Add(new ConsoleDisplayCell(symbol, color));
			return this;
		}

		public ConsoleDisplayCell CellAt(int i) {
			return this.cells[i];
		}
	}

	public class ConsoleDisplayCell {
		private readonly char symbol;
		private ConsoleColor color;

		public ConsoleDisplayCell(char symbol, ConsoleColor color = ConsoleColor.White) {
			this.symbol = symbol;
			this.color = color;
		}
		public ConsoleDisplayCell(char symbol) {
			this.symbol = symbol;
		}
		public ConsoleDisplayCell(ConsoleDisplayCell old) {
			this.symbol = old.symbol;
			this.color = old.color;
		}

		public ConsoleDisplayCell Copy => new ConsoleDisplayCell(this);
		public char Symbol => this.symbol;
		public ConsoleColor Color => this.color;

		public void Colorize(ConsoleColor color) => this.color = color;
	}
}
