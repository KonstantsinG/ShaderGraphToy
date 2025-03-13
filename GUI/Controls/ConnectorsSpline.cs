using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Net;
using System.Windows.Controls;

namespace GUI.Controls
{
    internal class ConnectorsSpline
    {
        private double _prevDir = 1;

        public Path? Path { get; set; }
        public BezierSegment? Bezier { get; set; }
        public int InitDirection { get; set; } = 1;
        public bool IsFixed { get; set; } = false;

        public NodesConnector? StartConnector { get; set; }
        public NodesConnector? EndConnector { get; set; }


        public void Define(Point startPoint, Color color1, Color color2, bool direction)
        {
            Path = new()
            {
                StrokeThickness = 5,
                Stroke = new LinearGradientBrush(color1, color2, new Point(0, 0), new Point(1, 0))
            };
            Panel.SetZIndex(Path, 2);
            PathGeometry geometry = new();
            PathFigure figure = new() { StartPoint = startPoint };
            Bezier = new(startPoint, startPoint, startPoint, true);
            figure.Segments.Add(Bezier);
            geometry.Figures.Add(figure);
            Path.Data = geometry;

            if (direction) ReverseSplineGradient();
            InitDirection = direction ? 1 : -1;
            _prevDir = InitDirection;
        }

        public void Update(Point startPoint, Point newPoint)
        {
            double len = newPoint.X - startPoint.X;
            Point p1 = new((startPoint.X + newPoint.X) / 2 + (len / 5), startPoint.Y);
            Point p2 = new((startPoint.X + newPoint.X) / 2 - (len / 5), newPoint.Y);
            
            Bezier!.Point1 = p1;
            Bezier!.Point2 = p2;
            Bezier!.Point3 = newPoint;

            if ((len > 0) != (_prevDir > 0)) ReverseSplineGradient();
            _prevDir = len;
        }

        public void UpdateColor(Color newColor, bool first)
        {
            if (first) ((LinearGradientBrush)Path!.Stroke).GradientStops[0].Color = newColor;
            else ((LinearGradientBrush)Path!.Stroke).GradientStops[1].Color = newColor;
        }

        private void ReverseSplineGradient()
        {
            ((LinearGradientBrush)Path!.Stroke).StartPoint = new(Math.Abs(((LinearGradientBrush)Path!.Stroke).StartPoint.X - 1), 0);
            ((LinearGradientBrush)Path!.Stroke).EndPoint = new(Math.Abs(((LinearGradientBrush)Path!.Stroke).EndPoint.X - 1), 0);
        }
    }
}
