/*===================================================
* 类名称: DrawCanvas
* 类描述: 
* 创建人: musli
* 创建时间: 2020/10/20 9:22:43
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace WinD.Plug.SystemMonitoring
{
    /// <summary>
    /// 
    /// </summary>
    public class DrawCanvas : Canvas
    {
        private List<Visual> visuals = new List<Visual>();
        private StringBuilder Path = new StringBuilder();
        private List<Point> Points = new List<Point>();
        public double Tick = 10;

        public DrawCanvas()
        {
        }
        public double MaxRange
        {
            get { return (double)GetValue(MaxRangeProperty); }
            set { SetValue(MaxRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxRangeProperty =
            DependencyProperty.Register("MaxRange", typeof(double), typeof(DrawCanvas), new PropertyMetadata(100d));


        public double MinRange
        {
            get { return (double)GetValue(MinRangeProperty); }
            set { SetValue(MinRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinRangeProperty =
            DependencyProperty.Register("MinRange", typeof(double), typeof(DrawCanvas), new PropertyMetadata(0d));

        public double MaxYRange
        {
            get { return (double)GetValue(MaxYRangeProperty); }
            set { SetValue(MaxYRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxYRangeProperty =
            DependencyProperty.Register("MaxYRange", typeof(double), typeof(DrawCanvas), new PropertyMetadata(100d));


        public double MinYRange
        {
            get { return (double)GetValue(MinYRangeProperty); }
            set { SetValue(MinYRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinYRangeProperty =
            DependencyProperty.Register("MinYRange", typeof(double), typeof(DrawCanvas), new PropertyMetadata(0d));




        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);
            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }
        public string TName { get; set; }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var geometry = new PathGeometry();
            var group = new TransformGroup();
            var translate = new TranslateTransform() { Y = -this.Height };
            group.Children.Add(translate);
            var scal = new ScaleTransform() { ScaleY = -1 };
            group.Children.Add(scal);
            geometry.Transform = group;
            var figures = new PathFigure();
            //geometry.Figures.Add(figures);
            var point = new LineSegment(new Point(0, 0), false);
            var point2 = new LineSegment(new Point(10, 15), false);
            var point3 = new LineSegment(new Point(20, 35), false);
            var point4 = new LineSegment(new Point(30, 20), false);
            var point5 = new LineSegment(new Point(40, 40), false);
            figures.Segments.Add(point);
            figures.Segments.Add(point2);
            figures.Segments.Add(point3);
            figures.Segments.Add(point4);
            figures.Segments.Add(point5);

            RefreshData(null);
            string sData = Path.ToString();//"M0,0 L10,12 20,30 30,5 40,35 50,0" + TName;
            var converter = TypeDescriptor.GetConverter(typeof(PathFigureCollection));
            geometry.Figures = (PathFigureCollection)converter.ConvertFrom(sData);
            geometry.FillRule = FillRule.Nonzero;
            geometry.Freeze();

            var bursh = new SolidColorBrush(Color.FromArgb(20, 0, 255, 0));
            var bursh1 = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, (MaxRange - MinRange), Height)));
            drawingContext.DrawGeometry(bursh, new Pen(bursh1, 1), geometry);
            drawingContext.Pop();
            drawingContext.DrawRectangle(null, new Pen(bursh1, 1), new Rect(0, 0, (MaxRange - MinRange), Height));

        }
        private void RefreshData(List<Point> points)
        {
            if (points != null)
                Points = points;
            if (Points.Count == 0)
                return;
            Path.Clear();
            Path.Append("M");
            Path.Append(string.Join(" ", Points.Select(u => u.ToStringByYRate(this.Height / (MaxYRange - MinYRange)))));

            //Path.Append($" {Points.Last().X},0");
            //Path.Append(" 0,0");
        }
        public void Refresh(List<Point> points)
        {
            RefreshData(points == null ? new List<Point>() : points);
            this.InvalidateVisual();
        }

    }
    public static class PointExtensions
    {
        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static string ToStringByYRate(this Point value, double rate)
        {
            return $"{value.X},{ value.Y * rate}";
        }
    }
}
