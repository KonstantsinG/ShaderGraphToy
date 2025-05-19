using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes
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

        /// <summary>
        /// Set ConnectedNode and ConnectedConnector properties for spline points
        /// </summary>
        public void SetConnectionIds()
        {
            if (InputConnector == null || OutputConnector == null) return;

            InputConnector!.ConnectionsCount++;
            OutputConnector!.ConnectionsCount++;

            InputConnector!.ConnectedNodesIds.Add(OutputConnector!.NodeId);
            InputConnector.ConnectedConnectorsIds.Add(OutputConnector!.ConnectorId);

            OutputConnector!.ConnectedNodesIds.Add(InputConnector!.NodeId);
            OutputConnector.ConnectedConnectorsIds.Add(InputConnector!.ConnectorId);
        }

        /// <summary>
        /// Define spline connection
        /// </summary>
        /// <param name="startPoint">first spline point</param>
        /// <param name="color1">first point color</param>
        /// <param name="color2">second point color</param>
        /// <param name="direction">spline direction (true - left to right (output to input), false - right to left (input to output))</param>
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

        /// <summary>
        /// Redefine spline from connected state
        /// </summary>
        /// <param name="sp">New spline</param>
        /// <param name="mouseColor">Color for "searching for connection" state</param>
        public void Define(ConnectorsSpline sp, Color mouseColor)
        {
            Point stPoint = sp.InitDirection == 1 ? sp.Bezier!.Point3 : sp.GetStartPoint();
            Color stColor = sp.InitDirection == 1 ? sp.Color1 : sp.Color2;

            Define(stPoint, mouseColor, stColor, false);
            InputConnector = sp.InputConnector;
            OutputConnector = sp.OutputConnector;
        }

        /// <summary>
        /// Recalculate start and middle points for new endpoint
        /// </summary>
        /// <param name="startPoint">Spline start point</param>
        /// <param name="newPoint">New spline endpoint</param>
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

        /// <summary>
        /// Break connectors connection
        /// </summary>
        public void Break()
        {
            InputConnector?.Disconnect(OutputConnector!.NodeId, OutputConnector.ConnectorId);
            OutputConnector?.Disconnect(InputConnector!.NodeId, InputConnector.ConnectorId);
        }

        /// <summary>
        /// Get spline start point
        /// </summary>
        /// <returns>Start point</returns>
        public Point GetStartPoint() => ((PathGeometry)Path!.Data).Figures[0].StartPoint;

        /// <summary>
        /// Set new color for spline start or end
        /// </summary>
        /// <param name="newColor">New color</param>
        /// <param name="first">Spline start or end</param>
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

        /// <summary>
        /// Recalculate spline point position
        /// </summary>
        /// <param name="newPoint">New position</param>
        /// <param name="isInput">Recalculate position for input or output connector</param>
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

        /// <summary>
        /// Reverse start and end colors for spline when it is flipped
        /// </summary>
        private void ReverseSplineGradient()
        {
            ((LinearGradientBrush)Path!.Stroke).StartPoint = new(Math.Abs(((LinearGradientBrush)Path!.Stroke).StartPoint.X - 1), 0);
            ((LinearGradientBrush)Path!.Stroke).EndPoint = new(Math.Abs(((LinearGradientBrush)Path!.Stroke).EndPoint.X - 1), 0);
        }
    }
}
