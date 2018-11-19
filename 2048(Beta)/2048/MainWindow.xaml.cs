using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly SolidColorBrush Tile = new SolidColorBrush(Color.FromRgb(38, 38, 63));
        public readonly SolidColorBrush Tile2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile4 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile8 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile16 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile32 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile64 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile128 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile256 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile512 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile1024 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush Tile2048 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));
        public readonly SolidColorBrush TileSuper = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A5ACD"));

        public readonly SolidColorBrush Below4 = new SolidColorBrush(Colors.White);
        public readonly SolidColorBrush Above4 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        public const int FontSize2To64 = 55;
        public const int FontSize128To512 = 45;
        public const int FontSize1024To2048 = 35;
        public const int FontSizeSuper = 30;

        public readonly int TileSize;
        public readonly int BoardSize;
        public GameManager GameManager { get; set; }

        public DispatcherTimer Timer;
        public DateTime StartTime;

        public TextBlock[,] TextBlocks;
        public Border[,] Borders;

        public MainWindow()
        {
            InitializeComponent();

            Timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, Timer_Callback, Dispatcher);
            Timer.Stop();

            TileSize = Properties.Settings.Default.TileSize;
            BoardSize = Properties.Settings.Default.BoardSize;

            GameManager = new GameManager(BoardSize, 2);
            GameManager.GridModified += GameManagerOnGridModified;

            TextBlocks = new TextBlock[BoardSize,BoardSize];
            Borders = new Border[BoardSize,BoardSize];

            for(int i = 0; i < BoardSize; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(TileSize)
                    });
            for (int i = 0; i < BoardSize; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(TileSize)
                });
            for(int y = 0; y < BoardSize; y++)
                for(int x = 0; x < BoardSize; x++)
                {
                    TextBlock textBlock = new TextBlock
                        {
                            FontWeight = FontWeights.Bold,
                            FontSize = 14,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    Border border = new Border
                        {
                            Background = Tile,
                            BorderBrush = new SolidColorBrush(Colors.Black),
                            BorderThickness = new Thickness(2),
                            Margin = new Thickness(1),
                            Child = textBlock
                        };
                    System.Windows.Controls.Grid.SetRow(border, y);
                    System.Windows.Controls.Grid.SetColumn(border, x);
                    MainGrid.Children.Add(border);

                    Borders[x, y] = border;
                    TextBlocks[x, y] = textBlock;
                }
        }

        private void GameManagerOnGridModified(object sender, EventArgs eventArgs)
        {
            Refresh();
            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                    System.Diagnostics.Debug.Write(GameManager.Grid.Cells[x, y].ToString(CultureInfo.InvariantCulture).PadLeft(4));
                System.Diagnostics.Debug.WriteLine(String.Empty);
            }
            System.Diagnostics.Debug.WriteLine("===============================");
        }

        private void Refresh()
        {
            if (GameManager == null || GameManager.Grid == null)
                return;
            for (int y = 0; y < BoardSize; y++)
                for (int x = 0; x < BoardSize; x++)
                {
                    int value = GameManager.Grid.Cells[x, y];
                    TextBlocks[x, y].Text = value > 0 ? value.ToString(CultureInfo.InvariantCulture) : null;
                    switch(value)
                    {
                        case 0:
                            Borders[x, y].Background = Tile;
                            break;
                        case 2:
                            Borders[x, y].Background = Tile2;
                            TextBlocks[x, y].Foreground = Below4;
                            TextBlocks[x, y].FontSize = FontSize2To64;
                            break;
                        case 4:
                            Borders[x, y].Background = Tile4;
                            TextBlocks[x, y].Foreground = Below4;
                            TextBlocks[x, y].FontSize = FontSize2To64;
                            break;
                        case 8:
                            Borders[x, y].Background = Tile8;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize2To64;
                            break;
                        case 16:
                            Borders[x, y].Background = Tile16;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize2To64;
                            break;
                        case 32:
                            Borders[x, y].Background = Tile32;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize2To64;
                            break;
                        case 64:
                            Borders[x, y].Background = Tile64;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize2To64;
                            break;
                        case 128:
                            Borders[x, y].Background = Tile128;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize128To512;
                            break;
                        case 256:
                            Borders[x, y].Background = Tile256;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize128To512;
                            break;
                        case 512:
                            Borders[x, y].Background = Tile512;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize128To512;
                            break;
                        case 1024:
                            Borders[x, y].Background = Tile1024;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize1024To2048;
                            break;
                        case 2048:
                            Borders[x, y].Background = Tile2048;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSize1024To2048;
                            break;
                        default:
                            Borders[x, y].Background = TileSuper;
                            TextBlocks[x, y].Foreground = Above4;
                            TextBlocks[x, y].FontSize = FontSizeSuper;
                            break;
                    }
                }
            UpdateTitle();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Down:
                    GameManager.Move(Directions.Down);
                    break;
                case Key.Left:
                    GameManager.Move(Directions.Left);
                    break;
                case Key.Right:
                    GameManager.Move(Directions.Right);
                    break;
                case Key.Up:
                    GameManager.Move(Directions.Up);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartTime = DateTime.Now;
            Timer.Start();
            GameManager.Start();
        }

        private void Timer_Callback(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(UpdateTitle);
        }

        private void UpdateTitle()
        {
            TimeSpan ts = DateTime.Now - StartTime;
            MainWindowInstance.Title = String.Format("Очки: {0} - Нажатия: {1} - Время: {2:0.00} Секунды.", GameManager.Grid.Score, GameManager.Moves, ts.TotalSeconds);
        }

        public void Send(Key key)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    };
                    InputManager.Current.ProcessInput(e);
                }
            }
        }
    }
}
