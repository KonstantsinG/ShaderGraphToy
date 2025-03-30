using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace GUI.Representation.GraphNodes
{
    internal class ConnectorsSpline
    {
        private double _prevDir = 1;

        public Path? Path { get; set; }
        public BezierSegment? Bezier { get; set; }
        public int InitDirection { get; set; } = 1;
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }

        public NodesConnector? InputConnector { get; set; }
        public NodesConnector? OutputConnector { get; set; }
        public NodesConnector? FreeConnector
        {
            get
            {
                if (InputConnector != null) return InputConnector;
                else if (OutputConnector != null) return OutputConnector;
                else return null;
            }
        }


        public void Define(Point startPoint, Color color1, Color color2, bool direction)
        {
            Color1 = color1;
            Color2 = color2;

            Path = new()
            {
                StrokeThickness = 5,
                Stroke = new LinearGradientBrush(color1, color2, new Point(0, 0), new Point(1, 0)),
                IsHitTestVisible = false
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

        public void Define(ConnectorsSpline sp, Color mouseColor)
        {
            Point stPoint = sp.InitDirection == 1 ? sp.Bezier!.Point3 : sp.GetStartPoint();
            Color stColor = sp.InitDirection == 1 ? sp.Color1 : sp.Color2;

            Define(stPoint, mouseColor, stColor, false);
            InputConnector = sp.InputConnector;
            OutputConnector = sp.OutputConnector;
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

        public void Break()
        {
            InputConnector?.Disconnect();
            OutputConnector?.Disconnect();
        }

        public Point GetStartPoint() => ((PathGeometry)Path!.Data).Figures[0].StartPoint;

        public void UpdateColor(Color newColor, bool first)
        {
            if (first)
            {
                Color1 = newColor;
                ((LinearGradientBrush)Path!.Stroke).GradientStops[0].Color = newColor;
            }
            else
            {
                Color2 = newColor;
                ((LinearGradientBrush)Path!.Stroke).GradientStops[1].Color = newColor;
            }
        }

        public void UpdatePoint(Point newPoint, bool isInput)
        {
            if (isInput)
            {
                if (InitDirection == 1) // if input conn is moved and connection started from input
                {
                    ((PathGeometry)Path!.Data).Figures[0].StartPoint = newPoint;
                    Update(newPoint, Bezier!.Point3);
                }
                else // if input conn is moved and connection started from output
                {
                    Bezier!.Point3 = newPoint;
                    Update(((PathGeometry)Path!.Data).Figures[0].StartPoint, newPoint);
                }
            }
            else
            {
                if (InitDirection == -1) // if output conn is moved and connection started from output
                {
                    ((PathGeometry)Path!.Data).Figures[0].StartPoint = newPoint;
                    Update(newPoint, Bezier!.Point3);
                }
                else // if output conn is moved and connection started from input
                {
                    Bezier!.Point3 = newPoint;
                    Update(((PathGeometry)Path!.Data).Figures[0].StartPoint, newPoint);
                }
            }
        }

        private void ReverseSplineGradient()
        {
            ((LinearGradientBrush)Path!.Stroke).StartPoint = new(Math.Abs(((LinearGradientBrush)Path!.Stroke).StartPoint.X - 1), 0);
            ((LinearGradientBrush)Path!.Stroke).EndPoint = new(Math.Abs(((LinearGradientBrush)Path!.Stroke).EndPoint.X - 1), 0);
        }
    }
}
