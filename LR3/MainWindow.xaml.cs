using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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

namespace LR3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color lineColor = Colors.Black;
        private Color backgroundColor = Colors.White;
        private double lineThickness = 2;

        public MainWindow()
        {
            InitializeComponent();
            CommandBinding saveBinding = new CommandBinding(ApplicationCommands.Save);
            saveBinding.Executed += Save_Executed;
            saveBinding.CanExecute += Save_CanExecute;
            this.CommandBindings.Add(saveBinding);
            CommandBinding openBinding = new CommandBinding(ApplicationCommands.Open);
            openBinding.Executed += Open_Executed;
            this.CommandBindings.Add(openBinding);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(drawingArea);
            DrawStar(position);
        }

        private void DrawStar(Point position, Color? currentLineColor = null, double? currentLineThickness = null, Color? currentBackgroundColor = null)
        {
            //Color lineColorToUse = currentLineColor ?? lineColor;
            //double lineThicknessToUse = currentLineThickness ?? lineThickness;
            //Color backgroundColorToUse = currentBackgroundColor ?? backgroundColor;

            //System.Windows.Shapes.Path star = new System.Windows.Shapes.Path
            //{
            //    Stroke = new SolidColorBrush(lineColorToUse),
            //    StrokeThickness = lineThicknessToUse,
            //    Fill = new SolidColorBrush(backgroundColorToUse)
            //};

            System.Windows.Shapes.Path star = new System.Windows.Shapes.Path
            {
                Stroke = new SolidColorBrush(lineColor),
                StrokeThickness = lineThickness,
                Fill = new SolidColorBrush(backgroundColor)
            };

            PathGeometry starGeometry = new PathGeometry();
            PathFigure starFigure = new PathFigure();
            starFigure.StartPoint = new Point(position.X, position.Y - 20);

            double outerRadius = 20;
            double innerRadius = 5;
            int points = 4;
            double angle = -Math.PI / 2;

            for (int i = 0; i < points * 2; i++)
            {
                double radius = i % 2 == 0 ? outerRadius : innerRadius;
                double x = position.X + radius * Math.Cos(angle);
                double y = position.Y + radius * Math.Sin(angle);
                starFigure.Segments.Add(new LineSegment(new Point(x, y), true));
                angle += Math.PI / points;
            }

            starFigure.IsClosed = true;
            starGeometry.Figures.Add(starFigure);
            star.Data = starGeometry;

            drawingArea.Children.Add(star);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = drawingArea.Children.Count != 0;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Файлы (bin)|*.bin|Все файлы|*.*"
            };

            if (sfd.ShowDialog() == true)
            {
                SaveShapesAsBinary(sfd.FileName);
                this.Title = sfd.FileName;
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Файлы (bin)|*.bin|Все файлы|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                LoadShapesFromBinary(ofd.FileName);
                this.Title = ofd.FileName;
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow(lineThickness, lineColor, backgroundColor);
            if (settings.ShowDialog() == true)
            {
                lineThickness = settings.LineThickness;
                lineColor = settings.LineColor;
                backgroundColor = settings.BackgroundColor;
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Графический редактор для ЛР №3. \nРазработано Альшевский А.Д.", "О программе");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(drawingArea);
            MouseCoordinates.Text = $"X: {position.X}, Y: {position.Y}";
        }

        private void SaveShapesAsBinary(string filePath)
        {
            List<ShapeData> shapes = new List<ShapeData>();

            foreach (UIElement element in drawingArea.Children)
            {
                if (element is System.Windows.Shapes.Path path && path.Data is PathGeometry geometry)
                {
                    PathFigure figure = geometry.Figures[0];
                    List<Point> points = new List<Point> { figure.StartPoint };

                    foreach (LineSegment segment in figure.Segments.OfType<LineSegment>())
                    {
                        points.Add(segment.Point);
                    }

                    shapes.Add(new ShapeData
                    {
                        Left = Canvas.GetLeft(path),
                        Top = Canvas.GetTop(path),
                        Points = points,
                        StrokeColor = (path.Stroke as SolidColorBrush).Color,
                        FillColor = (path.Fill as SolidColorBrush).Color,
                        StrokeThickness = path.StrokeThickness
                    });
                }
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(List<ShapeData>));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.WriteObject(fs, shapes);
            }
        }

        private void LoadShapesFromBinary(string filePath)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<ShapeData>));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                List<ShapeData> shapes = (List<ShapeData>)serializer.ReadObject(fs);
                drawingArea.Children.Clear();

                foreach (ShapeData shapeData in shapes)
                {
                    System.Windows.Shapes.Path star = new System.Windows.Shapes.Path
                    {
                        Stroke = new SolidColorBrush(shapeData.StrokeColor),
                        StrokeThickness = shapeData.StrokeThickness,
                        Fill = new SolidColorBrush(shapeData.FillColor)
                    };

                    PathGeometry starGeometry = new PathGeometry();
                    PathFigure starFigure = new PathFigure
                    {
                        StartPoint = shapeData.Points[0]
                    };

                    for (int i = 1; i < shapeData.Points.Count; i++)
                    {
                        starFigure.Segments.Add(new LineSegment(shapeData.Points[i], true));
                    }

                    starFigure.IsClosed = true;
                    starGeometry.Figures.Add(starFigure);
                    star.Data = starGeometry;

                    Canvas.SetLeft(star, shapeData.Left);
                    Canvas.SetTop(star, shapeData.Top);
                    drawingArea.Children.Add(star);
                }
            }
        }
    }
}
