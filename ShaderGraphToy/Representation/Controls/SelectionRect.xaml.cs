using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShaderGraphToy.Representation.Controls
{
    /// <summary>
    /// Логика взаимодействия для SelectionRect.xaml
    /// </summary>
    public partial class SelectionRect : UserControl
    {
        public Rectangle Rect
        {
            get => selectionRect;
        }

        public TranslateTransform Transform
        {
            get => transform;
        }

        public Point Offset { get; set; }


        public SelectionRect()
        {
            InitializeComponent();
        }


        public void Reset()
        {
            Rect.Width = 0;
            Rect.Height = 0;

            Transform.X = 0;
            Transform.Y = 0;
        }

        public void Translate(Point p)
        {
            Transform.X = p.X;
            Transform.Y = p.Y;
        }

        public void Update(double x, double y, double width, double height)
        {
            Rect.Width = width;
            Rect.Height = height;

            Transform.X = x;
            Transform.Y = y;
        }

        public Rect GetAreaRect() => new(Transform.X, Transform.Y, Rect.Width, Rect.Height);
    }
}
