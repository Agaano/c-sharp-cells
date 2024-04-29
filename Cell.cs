using System.Collections.Generic;
using SFML.Graphics;

namespace Cells {
	delegate void DrawPixelDelegate(float X, float Y, Color color);
	delegate void DrawLineDelegate(float fromX, float fromY, float toX, float toY, Color color);
	delegate void OnChangeDelegate(ICell cell); 
	
	interface ICell {
        public int x {get;}
        public int y {get;}
        public int energy {get;}
        public void Render();
        public int getEnergy(int energy, int senderX, int senderY);
        public int giveEnergy(int energy);
        public void Step(ref List<ICell> neighbours);

    }

	class Cell: ICell {
        public int x { get; }
        public int y { get; }
        public int energy {get; private protected set;}

        private protected readonly DrawPixelDelegate DrawPixel;
        private protected readonly DrawLineDelegate DrawLine;
        private protected uint PixelSize;
        private int prevEnergy;
        private int energyLifeTime;
        private int prevEnergySenderX;
        private int prevEnergySenderY;
        public Cell(int x, int y, int energy, DrawPixelDelegate drawPixelFunc, DrawLineDelegate drawLineFunc, uint PixelSize) {
            this.x = x;
            this.y = y;
            this.energy = energy;
            DrawPixel = drawPixelFunc;
            DrawLine = drawLineFunc;
            this.PixelSize = PixelSize;
        }

        virtual public void Render() {//
            DrawPixel(x,y, energy <= 0 ? Color.Black : Color.Red);
        }

        virtual public void Step(ref List<ICell> Cells) {
            List<int> neighbours = FindNeighbours(Cells);
            if (energy != prevEnergy) energyLifeTime = 24;
            else if (energy != 0 && prevEnergy != 0) energyLifeTime--;
            if (energyLifeTime <= 0) energy = 0;
            
            if (neighbours.Count == 0 || energy < 12) {
                prevEnergy = energy;
                return;
            }
            int Energy = energy;
            float s = 12 / neighbours.Count;
            foreach (int index in neighbours) {
                if (Cells[index].energy > energy) continue;
                if (Cells[index].x == prevEnergySenderX && Cells[index].y == prevEnergySenderY) continue;
                energy -= Cells[index].getEnergy((int) s, x, y);
            }
            if (Energy == energy) energy--;
            prevEnergy = energy;
        }

        private protected List<int> FindNeighbours(List<ICell> Cells) {
            List<int> list = new() {
                Cells.FindIndex((Cell) => Cell.x == x + 1 && Cell.y == y),
                Cells.FindIndex((Cell) => Cell.x == x - 1 && Cell.y == y),
                Cells.FindIndex((Cell) => Cell.x == x && Cell.y == y - 1),
                Cells.FindIndex((Cell) => Cell.x == x && Cell.y == y + 1),
            };
            return list.FindAll((index) => index != -1);
        }

        virtual public int getEnergy(int energy, int senderX, int senderY) {
            prevEnergySenderX = senderX;
            prevEnergySenderY = senderY;
            this.energy += energy;
            return energy;
        }

        virtual public int giveEnergy(int energy) {
            if (this.energy < energy) {
                int e = this.energy;
                this.energy = 0;
                return e;
            }
            this.energy -= energy;
            return energy;
        }
    }
}