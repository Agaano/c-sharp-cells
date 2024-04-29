using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using SFML.Graphics;
using SFML.System;


namespace UserInterface {

		class Button : Node {
        public int x {get; private set;}
        public int y {get; private set;}
        public int width {get; private set;}
        public int height {get; private set;}
        public string name {get; private set;}
				public Color color {get; private set;} = Color.White;

				private Node node;
				public Action onClick;
				
				public Button() {
					x = 0;
					y = 0;
					width = 0;
					height = 0;
					name = "";
					node = new P();
				}

				public Button(int x, int y, int width, int height, string name, Node node, Action onClick, Color? color) {
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					this.name = name;
					this.node = node;
					this.color = color ?? Color.White;
					this.onClick = onClick;
				}

				public void	Render(DrawDelegate DrawFunc, int x, int y) {
					DrawFunc(new RectangleShape(new Vector2f(width,height)) {
						Position = new(this.x+x,this.y+y),
						FillColor = color,
						OutlineColor = Color.Black,
						OutlineThickness = 1,
					});
					node.Render(DrawFunc, this.x + x,this.y + y);
				}

				public void changleOnClick(Action onClick) {
					this.onClick = onClick;
				}
				public bool isIntersecting(int x, int y, int originX, int originY) {
					bool isinter = !(x > this.x + originX + width || x < this.x + originX || y > this.y + originY + height || y < this.y + originY);
					if (isinter) {
						onClick();
					}
          return isinter;
				}
				public void AddNode(Node node) {}
				public void ChangeColor(Color color) {
					this.color = color;
				}

				public void OnClick(Action onClick) {
					this.onClick = onClick;
				}
				public Node GetNode(string name) {
					return node;
				}
				
				public T GetNode<T>(string name) where T : Node, new() {
					return new();
				}
		}
		class Image : Node {
        public int x {get; private set;}
        public int y {get; private set;}
        public int width {get; private set;}
        public int height {get; private set;}
        public string name {get; private set;}
				public Color color = Color.White;
				private Texture texture;
				public Image() {
					x = 0;
					y = 0;
					width = 0;
					height = 0;
					name = "";
				}
				public Image(int x, int y, string name, string imagePath) {
					this.x = x;
					this.y = y;
					// this.width = width;
					// this.height = height;
					this.name = name;
					SFML.Graphics.Image image = new(imagePath);
					width = (int) image.Size.X;
					height = (int) image.Size.Y;
					texture = new(image);
				}
				public void	Render(DrawDelegate DrawFunc, int x, int y) {
					DrawFunc(new Sprite(texture) {Position=new(x+this.x, y+this.y), Color = color});
				}
				public bool isIntersecting(int x, int y, int originX, int originY) {
          return !(x > this.x + width || x < this.x || y > this.y + height || y < this.y);
				}
				public void AddNode(Node node) {}
				public void ChangeColor(Color color) {
					this.color = color;
				}
				public Node GetNode(string name) {
					return this;
				}
				
				public T GetNode<T>(string name) where T : Node, new() {
					return new();
				}
		}
		class P : Node {
        public int x {get; private set;}
        public int y {get; private set;}
        public int width {get; private set;}
        public int height {get; private set;}
        public uint size {get; set;}
        public string name {get; private set;}
        public Color color {get; private set;}
        public string text {get; set;}
        private Font font = new("Roboto-Medium.ttf");
        public P() {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            name = "";
            color = new();
        }

        public P(string text, int x, int y, uint size, string name, Color? color) {
            this.x = x;
            this.y = y;
            this.size = size;
            this.name = name;
            this.color = color ?? Color.Black;
            this.text = text;
            height = 0;
            width = 0;
        }

        public void Render(DrawDelegate DrawFunc, int x, int y) {
            DrawFunc(new Text(text ,font) {
                Position = new(this.x + x, this.y + y),
                CharacterSize = size,
                FillColor = color,
            });
        }

        public bool isIntersecting(int x, int y, int originX, int originY) {
            return true;
        }

				public void ChangeColor(Color color) {
					this.color = color;
				}

        public void AddNode(Node node) {}
        public Node GetNode(string name) {
            return this;
        }
				
				public T GetNode<T>(string name) where T : Node, new() {
					return new();
				}
    }
    class Block : Node {
        public int x {get; private set;}
        public int y {get; private set;}
        public int width {get; private set;}
        public int height {get; private set;}
        public string name {get; private set;}
        private List<Node> content = new();
        private Color color;
        private int outlineThickness;
        private Color outlineColor;
        public Block() {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
            color = new();
            outlineThickness = 0;
            outlineColor = new();
            name = "";
        }
        public Block(int x, int y, int width, int height, string name, Color? color, int? outlineThickness, Color? outlineColor) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.color = color ?? Color.White;
            this.outlineThickness = outlineThickness ?? 0;
            this.outlineColor = outlineColor ?? color ?? Color.White;
            this.name = name;
        }

        public void Render(DrawDelegate DrawFunc, int x, int y) {
            DrawFunc(new RectangleShape(new Vector2f(width, height)) {
                Position = new(x + this.x, y + this.y),
                FillColor = color,
                OutlineThickness = outlineThickness,
                OutlineColor = outlineColor,
            });
            content.ForEach((node) => {
                node.Render(DrawFunc, x + this.x, y + this.y);
            });
        }

				public void ChangeColor(Color color) {
					this.color = color;
				} 

        public void AddNode(Node node) {
            content.Add(node);
        }
        public Node GetNode(string name) {
            int index = content.FindIndex((node) => node.name == name);
            if (index == -1) return this;
            return content[index];
        }

        public bool isIntersecting(int x, int y, int originX, int originY) {
						content.ForEach((node) => {
							node.isIntersecting(x,y,this.x + originX, this.y + originY);
						});
            return !(x > this.x + originX + width || x < this.x + originX || y > this.y + originY + height || y < this.y + originY);
        }

				public T GetNode<T>(string name) where T : Node, new() {
					int index = content.FindIndex((node) => node.name == name && node is T);
					return (T) content[index];
				}
				
				// public Block GetBlock(string name) {
				// 	int index = content.FindIndex((node) => node.name == name && node is Block);
				// 	if (index == -1) return new();
				// 	return (Block) content[index];
				// }
				// public Image GetImage(string name) {
				// 	int index = content.FindIndex((node) => node.name == name && node is Image);
				// 	if (index == -1) return new();
				// 	return (Image) content[index];
				// }
				// public P GetText(string name) {
				// 	int index = content.FindIndex((node) => node.name == name && node is P);
				// 	if (index == -1) return new();
				// 	return (P) content[index];
				// }
				// public Button GetButton(string name) {
				// 	int index = content.FindIndex((node) => node.name == name && node is Button);
				// 	if (index == -1) return new();
				// 	return (Button) content[index];
				// }
    }

    interface Node {
        public int x {get;}
        public int y {get;}
        public int width {get;}
        public int height {get;}
        public string name {get;}
        // public string text {get; set;}
        public void Render(DrawDelegate DrawFunc, int x, int y);
        public bool isIntersecting(int x, int y, int originX, int originY);
        public void AddNode(Node node);
        public Node GetNode(string name);
				public void ChangeColor(Color color);
				public T GetNode<T>(string name) where T : Node, new();
    }

    delegate void DrawDelegate(Drawable drawable);

    class Interface {
        private List<Node> content = new();
        private Font font;
        private DrawDelegate DrawFunc;
        public Interface(Font font, DrawDelegate DrawFunc) {
            this.font = font;
            this.DrawFunc = DrawFunc;
        }

        public void AddNode(Node node) {
            // if (blocks.Exists((block) => block.name == name)) throw new("Block name must be unique!");
            content.Add(node);
        }

        // public void SetText(string text, int blockIndex) {
        //     if (blockIndex >= blocks.Count) return;
        //     blocks[blockIndex].SetText(text);
        // }

        public Node GetNode(string name) {
            int index = content.FindIndex((node) => node.name == name);
            if (index == -1) return new Block();
            return content[index];
        }

				public T GetNode<T>(string name) where T : Node, new() {
					int index = content.FindIndex((node) => node.name == name && node is T);
					if (index == -1) return new();
					return (T) content[index];
				}

        public void Render() {
            content.ForEach((block) => {
                block.Render(DrawFunc, 0, 0);
                // DrawFunc(block.GetText(font));
            });
        }
        public bool isIntersecting(int x, int y) {
            bool isinter = false;
            new List<Node>(content).ForEach((block) => {
                if (isinter) return;
                isinter = block.isIntersecting(x,y, 0,0);
            });
            return isinter;
        }
				public void RemoveNode(string name) {
					int index = content.FindIndex((node) => node.name == name);
					if (index == -1) return;
					content.RemoveAt(index);
				}
    }
}