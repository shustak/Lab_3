using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicsEditor
{
    public partial class MainWindow : Window
    {
        private Brush _lineColor = Brushes.Black;
        private Brush _fillColor = Brushes.White;
        private double _lineThickness = 1.0;

        public string MouseCoordinates { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_CanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed, Open_CanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, Clear_Executed, Clear_CanExecute));
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(drawingArea);
            MouseCoordinates = $"X: {position.X}, Y: {position.Y}";
            OnPropertyChanged(nameof(MouseCoordinates));
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(drawingArea);
            Rectangle rectangle = new Rectangle
            {
                Stroke = _lineColor,
                Fill = _fillColor,
                StrokeThickness = _lineThickness,
                Width = 50,
                Height = 30
            };
            Canvas.SetLeft(rectangle, position.X);
            Canvas.SetTop(rectangle, position.Y);
            drawingArea.Children.Add(rectangle);
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Файлы (dat)|*.dat|Все файлы|*.*" };
            if (sfd.ShowDialog() == true)
            {
                StringBuilder sb = new StringBuilder();
                foreach (UIElement element in drawingArea.Children)
                {
                    if (element is Rectangle rect)
                    {
                        double left = Canvas.GetLeft(rect);
                        double top = Canvas.GetTop(rect);
                        sb.AppendLine($"{left},{top},{rect.Width},{rect.Height},{rect.Stroke},{rect.Fill},{rect.StrokeThickness}");
                    }
                }
                File.WriteAllText(sfd.FileName, sb.ToString());
                this.Title = sfd.FileName;
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = drawingArea != null && drawingArea.Children.Count != 0;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Файлы (dat)|*.dat|Все файлы|*.*" };
            if (ofd.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(ofd.FileName);
                drawingArea.Children.Clear();
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 7) // ensure correct number of parts
                    {
                        double left = double.Parse(parts[0]);
                        double top = double.Parse(parts[1]);
                        double width = double.Parse(parts[2]);
                        double height = double.Parse(parts[3]);
                        Brush stroke = (Brush)new BrushConverter().ConvertFromString(parts[4]);
                        Brush fill = (Brush)new BrushConverter().ConvertFromString(parts[5]);
                        double thickness = double.Parse(parts[6]);

                        Rectangle rect = new Rectangle
                        {
                            Stroke = stroke,
                            Fill = fill,
                            StrokeThickness = thickness,
                            Width = width,
                            Height = height
                        };
                        Canvas.SetLeft(rect, left);
                        Canvas.SetTop(rect, top);
                        drawingArea.Children.Add(rect);
                    }
                }
                this.Title = ofd.FileName;
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Clear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            drawingArea.Children.Clear();
        }

        private void Clear_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = drawingArea != null && drawingArea.Children.Count != 0;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(_lineColor, _fillColor, _lineThickness);
            if (settingsWindow.ShowDialog() == true)
            {
                _lineColor = settingsWindow.LineColor;
                _fillColor = settingsWindow.FillColor;
                _lineThickness = settingsWindow.LineThickness;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
