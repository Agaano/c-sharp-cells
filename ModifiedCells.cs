using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Cells {
	class DevouringCell: Cell {
		public DevouringCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		override public void Render() {
			base.Render();
			DrawingMath.Shape asd = new(x,y,PixelSize, DrawLine);
			asd.makeShape(DrawingMath.Shape.Square, Color.White);
		}

		override public void Step(ref List<ICell> Cells) {
			List<int> neighbours = FindNeighbours(Cells);
			if (neighbours.Count == 0) return;
			if (this.energy >= 100) return;
			foreach (int neighbourIndex in neighbours) {
				this.energy += Cells[neighbourIndex].giveEnergy(12/neighbours.Count);
			}
		}
	}

	class UndevouringCell: Cell {
		public UndevouringCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		override public void Render() {
			base.Render();
			DrawingMath.Shape asd = new(x,y,PixelSize,DrawLine);
			asd.makeShape(DrawingMath.Shape.BigRhombus, Color.White);
		}

		public override void Step(ref List<ICell> Cells) {
			List<int> neighbours = FindNeighbours(Cells);
			if (neighbours.Count == 0) return;
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex] is DevouringCell) {
					this.energy += Cells[neighbourIndex].giveEnergy(12 / neighbours.Count);
				} else if (this.energy - 12 / neighbours.Count >= 0) {
					this.energy -= Cells[neighbourIndex].getEnergy(12 / neighbours.Count, this.x, this.y);
				} else {
					this.energy -= Cells[neighbourIndex].getEnergy(this.energy, this.x, this.y);
				}
			}
		}

		public override int giveEnergy(int energy) {
			return 0;
		}

	}

	abstract class DirectedCell : Cell {
		public DirectedCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		private protected ICell from = null;
		private protected ICell to = null;
		private protected bool fromExist = false;
		private protected bool toExist = false;

		private protected void Step() {
			if (!fromExist || !toExist) return;
			to.getEnergy(from.giveEnergy(12), this.x, this.y);
		}

		public override int getEnergy(int energy, int senderX, int senderY)
		{
			return 0;
		}
		public override int giveEnergy(int energy)
		{
			return 0;
		}
	}

	class UpDirectedCell: DirectedCell {
		public UpDirectedCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize,DrawLine);
			asd.makeShape(DrawingMath.Shape.ArrowUp, Color.White);
		}

		public override void Step(ref List<ICell> Cells) {
			fromExist = false;
			toExist = false;
			List<int> neighbours = FindNeighbours(Cells);
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex].y > this.y) {from = Cells[neighbourIndex]; fromExist = true;}
				if (Cells[neighbourIndex].y < this.y) {to = Cells[neighbourIndex]; toExist = true;}
			}
			this.Step();
		}
	}

	class DownDirectedCell: DirectedCell {
		public DownDirectedCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize,DrawLine);
			asd.makeShape(DrawingMath.Shape.ArrowDown, Color.White);
		}

		public override void Step(ref List<ICell> Cells) {
			fromExist = false;
			toExist = false;
			List<int> neighbours = FindNeighbours(Cells);
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex].y < this.y) {from = Cells[neighbourIndex]; fromExist = true;}
				if (Cells[neighbourIndex].y > this.y) {to = Cells[neighbourIndex]; toExist = true;}
			}
			this.Step();
		}
	}

	class RightDirectedCell: DirectedCell {
		public RightDirectedCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize,DrawLine);
			asd.makeShape(DrawingMath.Shape.ArrowRight, Color.White);
		}

		public override void Step(ref List<ICell> Cells) {
			fromExist = false;
			toExist = false;
			List<int> neighbours = FindNeighbours(Cells);
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex].x < this.x) {from = Cells[neighbourIndex]; fromExist = true;}
				if (Cells[neighbourIndex].x > this.x) {to = Cells[neighbourIndex]; toExist = true;}
			}
			this.Step();
		}
	}

	class LeftDirectedCell: DirectedCell {
		public LeftDirectedCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize,DrawLine);
			asd.makeShape(DrawingMath.Shape.ArrowLeft, Color.White);
		}

		public override void Step(ref List<ICell> Cells) {
			fromExist = false;
			toExist = false;
			List<int> neighbours = FindNeighbours(Cells);
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex].x > this.x) {from = Cells[neighbourIndex]; fromExist = true;}
				if (Cells[neighbourIndex].x < this.x) {to = Cells[neighbourIndex]; toExist = true;}
			}
			this.Step();
		}
	}

	class InfinityCell: Cell {
		public InfinityCell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}

		public override void Render() {
			this.DrawPixel(this.x, this.y, Color.Red);
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize, DrawLine);
			asd.makeShape(DrawingMath.Shape.Circle, Color.White);
		}
		public override int giveEnergy(int energy) {
			return energy;
		}

		public override int getEnergy(int energy, int senderX, int senderY) {
			return 0;
		}

		public override void Step(ref List<ICell> Cells) {
			List<int> neighbours = FindNeighbours(Cells);
			if (neighbours.Count == 0) return;
			foreach (int neighbourIndex in neighbours) {
				Cells[neighbourIndex].getEnergy(1, this.x, this.y);
			}
		}
	}

	class NTypeTransistor: Cell {
		public NTypeTransistor(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			DrawPixel(this.x,this.y,Color.Black);
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize, DrawLine);
			asd.makeShape(DrawingMath.Shape.Square, Color.White);
			asd.makeShape(new int[,] {
				{-1,-1},
				{1,1}
			}, Color.White);
			asd.makeShape(new int[,] {
				{1,-1},
				{-1,1}
			}, Color.White);
		}
		public override void Step(ref List<ICell> Cells) {
			List<int> neighbours = FindNeighbours(Cells);
			if (neighbours.Count == 0) return;
			int outputIndex = -1;
			int baseIndex = -1;
			int collectorIndex = -1;
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex] is TransistorOutput) outputIndex = neighbourIndex;
				if (Cells[neighbourIndex] is TransistorBase) baseIndex = neighbourIndex;
				if (Cells[neighbourIndex] is TransistorCollector) collectorIndex = neighbourIndex;
			}
			if (outputIndex == -1 || baseIndex == -1 || collectorIndex == -1) return;
			TransistorOutput output = (TransistorOutput) Cells[outputIndex];
			TransistorBase @base = (TransistorBase) Cells[baseIndex];
			TransistorCollector collector = (TransistorCollector) Cells[collectorIndex];
			if (output.energy >= 100 || @base.energy <= 0) return;
			output.getEnergy(collector.giveEnergy(1), this.x, this.y);
		}
		public override int getEnergy(int energy, int senderX, int senderY) { 
			return 0;
		}

		public override int giveEnergy(int energy) {
			return 0;
		}
	}

	class PTypeTransistor: Cell {
		public PTypeTransistor(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			DrawPixel(this.x,this.y,Color.Black);
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize, DrawLine);
			asd.makeShape(DrawingMath.Shape.Square, Color.White);
			asd.makeShape(DrawingMath.Shape.Rhombus, Color.White);
		}
		public override void Step(ref List<ICell> Cells) {
			List<int> neighbours = FindNeighbours(Cells);
			if (neighbours.Count == 0) return;
			int outputIndex = -1;
			int baseIndex = -1;
			int collectorIndex = -1;
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex] is TransistorOutput) outputIndex = neighbourIndex;
				if (Cells[neighbourIndex] is TransistorBase) baseIndex = neighbourIndex;
				if (Cells[neighbourIndex] is TransistorCollector) collectorIndex = neighbourIndex;
			}
			if (outputIndex == -1 || baseIndex == -1 || collectorIndex == -1) return;
			TransistorOutput output = (TransistorOutput) Cells[outputIndex];
			TransistorBase @base = (TransistorBase) Cells[baseIndex];
			TransistorCollector collector = (TransistorCollector) Cells[collectorIndex];
			if (output.energy >= 100 || @base.energy > 0) return;
			output.getEnergy(collector.giveEnergy(1), this.x, this.y);
		}
		public override int getEnergy(int energy, int senderX, int senderY) { 
			return 0;
		}

		public override int giveEnergy(int energy) {
			return 0;
		}
	}

	class TransistorBase: Cell {
		public TransistorBase(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize,DrawLine);
			asd.makeShape(DrawingMath.Shape.Rhombus, Color.White);
		}
	}

	class TransistorOutput: Cell {
		public TransistorOutput(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new DrawingMath.Shape(this.x,this.y,PixelSize,DrawLine);
			asd.makeShape(new int[,] {
				{-1,-1},
				{1,1}
			}, Color.White);
			asd.makeShape(new int[,] {
				{1,-1},
				{-1,1}
			}, Color.White);
		}
	}

	class TransistorCollector: Cell {
		private string direction = "";
		public TransistorCollector(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize): base(x, y, energy, drawPixelFunc, drawLineFunc, PixelSize) {}
		public override void Render() {
			base.Render();
			DrawingMath.Shape asd = new(this.x,this.y,PixelSize,DrawLine);
			switch (direction) {
				case "right":
					asd.makeShape(DrawingMath.Shape.ArrowRight, Color.White);
					break;
				case "left":
					asd.makeShape(DrawingMath.Shape.ArrowLeft, Color.White);
					break;
				case "up":
					asd.makeShape(DrawingMath.Shape.ArrowUp, Color.White);
					break;
				case "down":
					asd.makeShape(DrawingMath.Shape.ArrowDown, Color.White);
					break;
			}
			asd.makeShape(DrawingMath.Shape.Square,Color.White);
		}

		public override void Step(ref List<ICell> Cells) {
			base.Step(ref Cells);
			List<int> neighbours = FindNeighbours(Cells);
			int transistorIndex = -1;
			foreach (int neighbourIndex in neighbours) {
				if (Cells[neighbourIndex] is NTypeTransistor || Cells[neighbourIndex] is PTypeTransistor) transistorIndex = neighbourIndex;
			}
			if (transistorIndex == -1) {
				direction = "";
				return;
			}
			ICell transistor = Cells[transistorIndex];
			if (transistor.x > this.x) direction = "right";
			if (transistor.x < this.x) direction = "left";
			if (transistor.y > this.y) direction = "down";
			if (transistor.y < this.y) direction = "up";
		}

	}
}