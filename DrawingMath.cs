using System.Linq;
using System.Numerics;
using Cells;
using SFML.Graphics;

namespace DrawingMath {

	struct Vector3x3i {
		public readonly int x; 
		public readonly int y; 
		public Vector3x3i(int x, int y) {
			if (x > 2 && x < -2 && y > 2 && y < -2) throw new("Vector3x3i accepts only integer values between 2 and -2");
			// Console.WriteLine($"x > 2 = {x > 2}; x < -2 = {x < -2}; 2 > x > -2 = {x > 2 && x < -2}; y > 2 = {y > 2}; y < -2 = {y < -2}; 2 > y > -2 = {y > 2 && y < -2}; 2 > y,x > -2 = {}");
			this.x = x;
			this.y = y;
		}
	}
	class Dots {
		private protected int x;
		private protected int y;
		private protected uint PixelSize;
		private protected float Quarter;
		public Dots(int x, int y, uint PixelSize) {
			this.x = x;
			this.y = y;
			this.PixelSize = PixelSize;
			Quarter = PixelSize/4;
		}
		private Vector3x3i prevMatrix = new(0,0);
		public Vector2 getDot(Vector3x3i matrix) {
			float x = this.x*PixelSize + Quarter*(matrix.x + 2);
			float y = this.y*PixelSize + Quarter*(matrix.y + 2);
			prevMatrix = matrix;
			return new(x,y);
		}
		public Vector2 getDot() {
			float x = this.x*PixelSize + Quarter*(prevMatrix.x + 2);
			float y = this.y*PixelSize + Quarter*(prevMatrix.y + 2);
			return new(x,y);
		}
	}

	class Shape: Dots {
		public static int[,] ArrowUp = new int[,] {{-1, 1},{0, -1},{1,1}};
		public static int[,] ArrowDown = new int[,] {{-1, -1},{0, 1},{1,-1}};
		public static int[,] ArrowRight = new int[,] {{-1, -1},{1, 0},{-1,1}};
		public static int[,] ArrowLeft = new int[,] {{1, -1},{-1, 0},{1,1}};
		public static int[,] Square = new int[,] {{-1,-1},{-1,1},{1,1},{1,-1},{-1,-1}};
		public static int[,] Circle = new int[,] {{-2,-1},{-1,-2},{1,-2},{2,-1},{2,1},{1,2},{-1,2},{-2,1},{-2,-1}};
		public static int[,] Rhombus = new int[,] {{0,-1},{1,0},{0, 1},{-1,0},{0,-1}};
		
		public static int[,] BigRhombus = new int[,] {{0,-2},{2,0},{0, 2},{-2,0},{0,-2}};
		private DrawLineDelegate drawLineFunc;
		public Shape(int x, int y, uint PixelSize, DrawLineDelegate drawLineFunc): base(x,y,PixelSize) {
			this.drawLineFunc = drawLineFunc;
		}
		int clamp(int x) => x > 2 ? 2 : x < -2 ? -2 : x;

		public void makeShape(int[,] numbers, Color color) {
			if (numbers.GetLength(0) < 2 || numbers.GetLength(1) < 2) return;
			for (int i = 0; i < numbers.GetLength(0) - 1; i++) {
					drawLineFunc(
						getDot(new(clamp(numbers[i,0]), clamp(numbers[i,1]))).X,
						getDot().Y,
						getDot(new(clamp(numbers[i+1,0]), clamp(numbers[i+1, 1]))).X,
						getDot().Y,
						color
					);
			}
		} 
	}
}