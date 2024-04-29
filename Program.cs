using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cells;
using System.Net;
using System.Reflection;
using UserInterface;
using System.Net.Mail;
using System.IO;
using System.Text.Json;
using System.Text;
namespace c_sharp_cells
{
    enum Tool {
        Place,
        Info,
        Delete
    }

    delegate ICell CreateCellDelegate(int x, int y, int energy);

    struct NameAction {
        public string name;
        public string type;
        public CreateCellDelegate createCell;
    }

    class Game {
        private readonly RenderWindow window;
        private readonly uint BoardHeight;
        private readonly uint BoardWidth;
        private readonly uint PixelSize;
        private int offsetX;
        private int offsetY;
        private Font font = new("Roboto-Medium.ttf");
        private Interface @interface;
        private Vector2i selectedCellPosition;
        private Tool selectedTool = Tool.Place;
        private List<ICell> Cells = new();
        private List<NameAction> cellCreateActions = new();
        private int selectedCellCreateActionIndex = 0;
        private float FPS;
        private int nextCellEnergy = 0;
        public Game(uint pixelSize, string title) {
            BoardHeight = VideoMode.DesktopMode.Height;
            BoardWidth = VideoMode.DesktopMode.Width;
            window = new RenderWindow(VideoMode.DesktopMode, title, Styles.Fullscreen);
            window.SetTitle("Cells");
            SFML.Graphics.Image image = new("Icon.jpg");
            window.SetIcon(image.Size.X, image.Size.Y, image.Pixels);
            @interface = new(font, this.window.Draw);
            ConfigureInventoryContent();
            ConfigureWindow();
            PixelSize = pixelSize;
        }//

        private bool middleButtonPressed = false;
        private Vector2i middleButtonPressedPosition = new();
        private Vector2i prevOffset = new();
        private bool isInventoryOpen = false;
        private bool isSavesPanelOpen = false;

        private void ConfigureWindow() {
            window.SetFramerateLimit(150);
            window.Closed += (e,w) => window.Close();
            window.Resized += (e,w) => window.Size = new Vector2u(BoardWidth,BoardHeight);
            window.MouseButtonReleased += OnMouseButtonReleased;
            window.MouseButtonPressed += OnMouseButtonPressed;
            window.MouseMoved += OnMouseMove;
            window.KeyPressed += OnKeyPressed;
            @interface.AddNode(new Block(10,10,300,200,"right-top menu", null, 1, Color.Black));
            @interface.AddNode(new Block(320,10,210,70,"tools menu", null, 1, Color.Black));
            @interface.AddNode(new Block(320,90,290,120, "next cell energy", null, 1, Color.Black));
            @interface.GetNode<Block>("next cell energy").AddNode(new P("Place cell energy: ", 10, 10, 24, "next cell energy label", null));
            @interface.GetNode("right-top menu").AddNode(new P("", 10, 10, 24, "text", null));
            @interface.GetNode("tools menu").AddNode(new UserInterface.Image(10, 10,"image1","PlaceIcon.png"));
            @interface.GetNode("tools menu").AddNode(new UserInterface.Image(150, 10,"image2","DeleteIcon.png"));
            @interface.GetNode("tools menu").AddNode(new UserInterface.Image(80, 10,"image3","InfoIcon.png"));
            @interface.GetNode("tools menu").AddNode(new P("1",5, 5,12,"image1Label",null));
            @interface.GetNode("tools menu").AddNode(new P("3", 145, 5,12,"image2Label",null));
            @interface.GetNode("tools menu").AddNode(new P("2",75, 5, 12,"image3Label", null));
            // @interface.AddNode(new Button(540,10,70,70,"inventory button", new UserInterface.Image(10, 10, "inventory button icon", "InventoryIcon.png"), () => {Console.WriteLine("Clicked the Button!");}, null));
            @interface.AddNode(new Block(540,10,70,70,"inventory button wrapper", null, 1, Color.Black));
            @interface.GetNode("inventory button wrapper").AddNode(new Button(10,10,50,50,"inventory button", new UserInterface.Image(0,0,"inventory button image", "InventoryIcon.png"), () => {Console.WriteLine("Open the inventory");}, null));
            @interface.GetNode("inventory button wrapper").GetNode("inventory button").GetNode("inventory button image").ChangeColor(new(255,255,255,50));
            @interface.GetNode("inventory button wrapper").AddNode(new P("Tab", 5,5, 12, "inventory button label", null));
            // @interface.AddNode(new Block(10, 10, (int) BoardWidth - 20, (int) BoardHeight - 20, "inventory", null, 1, Color.Black));
            window.Clear(Color.White);
        }

        private void ConfigureInventoryContent() {
            this.cellCreateActions.Add(new() {type = "Cell", name = "Regular Cell", createCell = (int x, int y, int energy) => new Cell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "DevouringCell", name = "Devouring Cell", createCell = (int x, int y, int energy) => new DevouringCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "UndevouringCell", name = "Undevouring Cell", createCell = (int x, int y, int energy) => new UndevouringCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "UpDirectedCell", name = "Up Directed Cell", createCell = (int x, int y, int energy) => new UpDirectedCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "DownDirectedCell", name = "Down Directed Cell", createCell = (int x, int y, int energy) => new DownDirectedCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "LeftDirectedCell", name = "Left Directed Cell", createCell = (int x, int y, int energy) => new LeftDirectedCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "RightDirectedCell", name = "Right Directed Cell", createCell = (int x, int y, int energy) => new RightDirectedCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "InfinityCell", name = "Infinity Cell", createCell = (int x, int y, int energy) => new InfinityCell(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "NTypeTransistor", name = "N Type Transistor", createCell = (int x, int y, int energy) => new NTypeTransistor(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "PTypeTransistor", name = "P Type Transistor", createCell = (int x, int y, int energy) => new PTypeTransistor(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "TransistorBase", name = "Transistor Base", createCell = (int x, int y, int energy) => new TransistorBase(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "TransistorOutput", name = "Transistor Output", createCell = (int x, int y, int energy) => new TransistorOutput(x,y,energy, DrawPixel, DrawLine, PixelSize)});
            this.cellCreateActions.Add(new() {type = "TransistorCollector", name = "Transistor Collector", createCell = (int x, int y, int energy) => new TransistorCollector(x,y,energy, DrawPixel, DrawLine, PixelSize)});
        }

        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs ev) {
            if (@interface.isIntersecting(ev.X,ev.Y)) {
                return;
            }
            if (ev.Button == Mouse.Button.Middle) {
                    middleButtonPressed = true;
                    middleButtonPressedPosition = new(ev.X,ev.Y);
                    prevOffset = new(offsetX,offsetY);
                    window.SetMouseCursorVisible(false);
                }
        }
        private void OnMouseButtonReleased(object? sender, MouseButtonEventArgs ev) {
            if (@interface.isIntersecting(ev.X,ev.Y)) {
                return;
            }
            int x = (int)(ev.X - offsetX) - (int) (ev.X-offsetX < 0 ? PixelSize : 0); 
            int y = (int)(ev.Y - offsetY) - (int) (ev.Y-offsetY < 0 ? PixelSize : 0);
            if (ev.Button == Mouse.Button.Left) {
                if (selectedTool == Tool.Place) CreateNewCell(x, y);
                if (selectedTool == Tool.Info) SubscribeToCell(x,y);
                if (selectedTool == Tool.Delete) {
                    int index = Cells.FindIndex((cell) => cell.x == x / PixelSize && cell.y == y / PixelSize);
                    if (index == -1) return;
                    Cells.RemoveAt(index);
                }
                DrawBoard();
            }
            if (ev.Button == Mouse.Button.Right) CreateNewCellWEnergy(x,y, 100);
            if (ev.Button == Mouse.Button.Middle) {
                window.SetMouseCursorVisible(true);
                middleButtonPressed = false;
            }
        }
        private void OnMouseMove(object? sender, MouseMoveEventArgs ev) {
            if (middleButtonPressed) setOffset(ev.X, ev.Y);
        }

        private void OnKeyPressed(object? sender, KeyEventArgs ev) {
            switch (ev.Code) {
                    case Keyboard.Key.Num1:
                        selectedTool = Tool.Place;
                        break;
                    case Keyboard.Key.Num2:
                        selectedTool = Tool.Info;
                        break;
                    case Keyboard.Key.Num3:
                        selectedTool = Tool.Delete;
                        break;
                    case Keyboard.Key.Tab:
                        if (isInventoryOpen) {
                            @interface.RemoveNode("inventory");
                        } else {
                            if (isSavesPanelOpen) {@interface.RemoveNode("saves panel"); isSavesPanelOpen = false;}
                            Inventory();
                        }
                        isInventoryOpen = !isInventoryOpen;
                        break;
                    case Keyboard.Key.F1:
                        if (isSavesPanelOpen) {
                            @interface.RemoveNode("saves panel");
                        } else {
                            if (isInventoryOpen) {@interface.RemoveNode("inventory"); isInventoryOpen = false;}
                            SavesPanel();
                        }
                        isSavesPanelOpen = !isSavesPanelOpen;
                        break;
                    case Keyboard.Key.Add: 
                        nextCellEnergy= Math.Clamp(nextCellEnergy + 20,0,200);
                        break;
                    case Keyboard.Key.Hyphen: 
                        nextCellEnergy = Math.Clamp(nextCellEnergy - 20, 0, 200);
                        break;
                    case Keyboard.Key.Equal:
                        nextCellEnergy= Math.Clamp(nextCellEnergy + 20,0,200);
                        break;
                    case Keyboard.Key.Subtract:
                        nextCellEnergy = Math.Clamp(nextCellEnergy - 20, 0, 200);
                        break;
                    case Keyboard.Key.Escape: 
                        Save();
                        break;
                }
        }

        private void Inventory() {
            int inventoryWidth = 300;
            @interface.AddNode(new Block((int) BoardWidth - inventoryWidth - 10, 10, inventoryWidth, (int) BoardHeight - 20, "inventory", null, 1, Color.Black));
            Block inventory = @interface.GetNode<Block>("inventory");
            inventory.AddNode(new P("Cells",10,30,36,"h1",null));
            inventory.AddNode(new Block(10,110,inventory.width - 20, inventory.height - 140,"inventory cells",null, 1, Color.Black));
            Block inventoryCells = inventory.GetNode<Block>("inventory cells");
            int index = 0;
            foreach (NameAction action in cellCreateActions) {
                P p = new(action.name,10,10,24,$"{action.name} label",null);
                Button button = new(0,index*50,inventoryCells.width,50,$"{action.name} button",p,() => {}, null);
                int currIndex = index;
                button.onClick = () => {
                    selectedCellCreateActionIndex = currIndex;
                };
                inventoryCells.AddNode(button);
                index++;
            }
        }

        private void SavesPanel() {
            @interface.AddNode(new Block((int) BoardWidth - 300 - 10,10,300, (int) BoardHeight - 20, "saves panel", null, 1, Color.Black));
            Block panel = @interface.GetNode<Block>("saves panel");
            panel.AddNode(new P("Saves", 10, 30, 36, "h1", null));
            panel.AddNode(new Block(10,110, panel.width - 20, panel.height - 140, "panel saves", null, 1, Color.Black));
            Block panelSaves = panel.GetNode<Block>("panel saves");
            int index = 0;
            string[] files = GetAllSaves();
            foreach (string file in files) {
                string fileName = Path.GetFileName(file);
                P p = new(fileName,10,10,24,$"{fileName} label",null);
				Button button = new(0, index * 50, panelSaves.width, 50, $"{fileName} button", p, () => {}, null);
                button.onClick = () => {
                    Load(fileName);
                    Console.WriteLine(fileName);
                };
				panelSaves.AddNode(button);
                index++;
            }//
        }
        private string[] GetAllSaves() {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cells");
            string[] files = Directory.GetFiles(path);
            return files;
        }

        struct CellMarkdown {
            public int x {get; set;}
            public int y {get; set;}
            public int energy {get; set;}
            public string type {get; set;}
        }

        private void Save() {
            List<CellMarkdown> cellsToSave = new();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cells");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Cells.ForEach((cell) => {
                cellsToSave.Add(new() {x = cell.x, y = cell.y, energy = cell.energy, type = cell.GetType().Name});
            });
            string text = JsonSerializer.Serialize(cellsToSave, cellsToSave.GetType());
            string filePath = Path.Combine(path, $"{DateTime.Now.Millisecond}.json");
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            FileStream fs = File.Create(filePath);
            fs.Write(bytes);
        }

        private void Load(string fileName) {
            List<CellMarkdown> cellsToLoad = new();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cells");
            string filePath = Path.Combine(path, fileName);
            string text = File.ReadAllText(filePath);
            cellsToLoad = (List<CellMarkdown>) JsonSerializer.Deserialize(text, cellsToLoad.GetType());
            Cells = new();
            cellsToLoad.ForEach((cellToLoad) => {
                // cellCreateActions.Find((cellCreateAction) => cellCreateAction.type == cellToLoad.type).createCell(cellToLoad.x, cellToLoad.y, cellToLoad.energy);
                int index = cellCreateActions.FindIndex((cell) => cell.type == cellToLoad.type);
                if (index == -1) {
                    Console.WriteLine($"{cellToLoad.type} is not exist");
                    return;
                }
                Cells.Add(cellCreateActions[index].createCell(cellToLoad.x, cellToLoad.y, cellToLoad.energy));
            });
            // cellCreateActions[0].createCell(0,0,0);
        }

        private void SubscribeToCell(int x, int y) {
            selectedCellPosition = new((int) (x/PixelSize),(int) (y / PixelSize));
        }
        private void setOffset(int x, int y) {
            offsetX = prevOffset.X - ((int) middleButtonPressedPosition.X - x);
            offsetY = prevOffset.Y - ((int) middleButtonPressedPosition.Y - y);
        }

        public void Step() {
            Cells.ForEach((Cell) => {
                Cell.Step(ref Cells);
            });
        }

        private void CreateNewCell(int x, int y) {
            if (Cells.Exists((Cell) => Cell.x == x/PixelSize && Cell.y == y /PixelSize)) return;
            Cells.Add(cellCreateActions[selectedCellCreateActionIndex].createCell((int) (x/PixelSize),(int) (y/PixelSize), nextCellEnergy));
        }
        private void CreateNewCellWEnergy(int x, int y,int energy) {
            if (Cells.Exists((Cell) => Cell.x == x/PixelSize && Cell.y == y /PixelSize)) return;
            Cells.Add(new Cell((int) (x/PixelSize),(int) (y/PixelSize), energy, DrawPixel, DrawLine, PixelSize));
        }

        public void DrawBoard() {
            window.Clear(Color.White);
            Cells.ForEach((Cell) => {
                Cell.Render();
            });
            for (uint i = 0; i <= BoardHeight; i+= PixelSize) { //горизонтальные линии
                DrawLine(0 - offsetX + offsetX % PixelSize - PixelSize,i - offsetY + offsetY % PixelSize,BoardWidth - offsetX + offsetX % PixelSize + PixelSize, i - offsetY + offsetY % PixelSize, Color.Black);
            }
            for (uint i = 0; i <= BoardWidth; i+=PixelSize) { // вертикальные линии
                DrawLine(i - offsetX + offsetX % PixelSize,0 - offsetY + offsetY % PixelSize - PixelSize,i - offsetX + offsetX % PixelSize,BoardHeight - offsetY + offsetY % PixelSize + PixelSize, Color.Black);
            }
            DrawSelection(selectedCellPosition.X,selectedCellPosition.Y);
            string s;
            if (Cells.Exists((Cell) => Cell.x == selectedCellPosition.X && Cell.y == selectedCellPosition.Y)) {
                ICell cell = Cells.Find((Cell) => Cell.x == selectedCellPosition.X && Cell.y==selectedCellPosition.Y);
                s = $"X: {cell.x}\nY: {cell.y}\nEnergy: {cell.energy}\nType: {cell.GetType().Name}";
            } else s = $"X: {selectedCellPosition.X}\nY: {selectedCellPosition.Y}";
            @interface.GetNode("right-top menu").GetNode<P>("text").text = s + $"\nFPS: {FPS.ToString("F")}";
            UserInterface.Image image1 = @interface.GetNode("tools menu").GetNode<UserInterface.Image>("image1"); 
            UserInterface.Image image2 = @interface.GetNode("tools menu").GetNode<UserInterface.Image>("image2");
            UserInterface.Image image3 = @interface.GetNode("tools menu").GetNode<UserInterface.Image>("image3");
            Color activeColor = new(255,255,255);
            Color disabledColor = new(255,255,255,50);
            switch (selectedTool) {
                case Tool.Place:
                    image1.ChangeColor(activeColor);
                    image2.ChangeColor(disabledColor);
                    image3.ChangeColor(disabledColor);
                    break;
                case Tool.Delete:
                    image1.ChangeColor(disabledColor);  
                    image2.ChangeColor(activeColor);
                    image3.ChangeColor(disabledColor);
                    break;
                case Tool.Info: 
                    image1.ChangeColor(disabledColor);
                    image2.ChangeColor(disabledColor);
                    image3.ChangeColor(activeColor);
                    break;
            }
            if (@interface.GetNode<Block>("inventory").name == "inventory") {
                Block inventory = @interface.GetNode<Block>("inventory").GetNode<Block>("inventory cells");
                foreach (NameAction action in cellCreateActions) {
                    Button button = inventory.GetNode<Button>($"{action.name} button");
                    button.ChangeColor(cellCreateActions[selectedCellCreateActionIndex].name == action.name ? new Color(200,200,200) : Color.White);
                }
            }
            @interface.GetNode<Block>("next cell energy").GetNode<P>("next cell energy label").text = $"Place cell energy: {nextCellEnergy}\nPlace cell type: \n{cellCreateActions[selectedCellCreateActionIndex].name}";
            @interface.Render();
            window.Display();
        }

        public void SetFPS(float fps) {
            this.FPS = fps;
        }

		public bool IsOpen {
            get {return window.IsOpen;}
        }

        public void DispatchEvents() {
            window.DispatchEvents();
        }
        
        private void DrawSelection(int x, int y) {
            DrawLine(x*PixelSize, y*PixelSize, x*PixelSize+PixelSize/4, y*PixelSize, Color.Green);
            DrawLine(x*PixelSize, y*PixelSize, x*PixelSize,y*PixelSize+PixelSize/4,Color.Green);
            DrawLine(x*PixelSize+PixelSize,y*PixelSize,x*PixelSize+PixelSize-PixelSize/4,y*PixelSize,Color.Green);
            DrawLine(x*PixelSize+PixelSize,y*PixelSize,x*PixelSize+PixelSize,y*PixelSize+PixelSize/4,Color.Green);
            DrawLine(x*PixelSize,y*PixelSize+PixelSize,x*PixelSize,y*PixelSize+PixelSize-PixelSize/4,Color.Green);
            DrawLine(x*PixelSize,y*PixelSize+PixelSize,x*PixelSize+PixelSize/4,y*PixelSize+PixelSize,Color.Green);
            DrawLine(x*PixelSize+PixelSize,y*PixelSize+PixelSize,x*PixelSize+PixelSize-PixelSize/4, y*PixelSize+PixelSize, Color.Green);
            DrawLine(x*PixelSize+PixelSize,y*PixelSize+PixelSize,x*PixelSize+PixelSize,y*PixelSize+PixelSize-PixelSize/4,Color.Green);
        }

        private void DrawLine(float fromX, float fromY, float toX, float toY, Color color) {
            Vertex[] line = {
                new(new Vector2f(fromX + offsetX, fromY + offsetY), color),
                new(new Vector2f(toX + offsetX,toY + offsetY), color)
            };
            window.Draw(line,PrimitiveType.Lines);
        }

        private void DrawPixel(float X, float Y, Color color) {
            RectangleShape rect = new(new Vector2f(PixelSize,PixelSize)) {
                FillColor=color,
                Position=new((X * PixelSize) + offsetX, (Y* PixelSize) + offsetY)
            };
            window.Draw(rect);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new(50,"Window");
            DateTime FrameTime = DateTime.Now;
            game.SetFPS(0);
            int StepMilliseconds = 0;
            int fpsMilliseconds = 0;
            int frames = 0;
            while(game.IsOpen) {
                int deltaTime = (DateTime.Now - FrameTime).Milliseconds;
                if (StepMilliseconds >= 100) {
                    game.Step();
                    StepMilliseconds = 0;
                } else StepMilliseconds += deltaTime;
                if (fpsMilliseconds >= 1000) {
                    game.SetFPS(frames);
                    frames = 0;
                    fpsMilliseconds = 0;
                } else fpsMilliseconds += deltaTime;
                // if ((DateTime.Now - FrameTime).Milliseconds > 0) 
                    // game.SetFPS(1000 / (DateTime.Now - FrameTime).Milliseconds);
                FrameTime = DateTime.Now;
                game.DrawBoard();
                frames++;
                game.DispatchEvents();
                // Thread.Sleep(30);
            }
            Console.ReadKey();
            return;
        }
    }
}
