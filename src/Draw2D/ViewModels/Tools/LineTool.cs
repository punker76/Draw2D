﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Tools
{
    public class LineToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private bool _splitIntersections;

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public bool SplitIntersections
        {
            get => _splitIntersections;
            set => Update(ref _splitIntersections, value);
        }
    }

    public static class LineHelper
    {
        public static IList<LineShape> SplitByIntersections(IToolContext context, IEnumerable<PointIntersection> intersections, LineShape target)
        {
            var points = intersections.SelectMany(i => i.Intersections).ToList();
            points.Insert(0, target.StartPoint);
            points.Insert(points.Count, target.Point);

            var unique = points
                .Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                .Select(p => new PointShape(p.X, p.Y, context.PointShape)).ToList();

            var lines = new List<LineShape>();
            for (int i = 0; i < unique.Count - 1; i++)
            {
                var line = new LineShape(unique[i], unique[i + 1]);
                line.Style = context.CurrentStyle;

                context.CurrentContainer.Shapes.Add(line);
                lines.Add(line);
            }

            return lines;
        }
    }

    public class LineTool : ToolBase
    {
        private LineShape _line = null;

        public enum State
        {
            StartPoint,
            Point
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public override string Title => "Line";

        public LineToolSettings Settings { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
            _line = new LineShape()
            {
                StartPoint = next,
                Point = (PointShape)next.Copy(null),
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_line);
            context.Renderer.Selection.Selected.Add(_line.StartPoint);
            context.Renderer.Selection.Selected.Add(_line.Point);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.Renderer.Selection.Selected.Remove(_line.StartPoint);
            context.Renderer.Selection.Selected.Remove(_line.Point);
            context.WorkingContainer.Shapes.Remove(_line);

            _line.Point = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            if ((Settings?.SplitIntersections ?? false) && (Intersections?.Any(i => i.Intersections.Count > 0) ?? false))
            {
                LineHelper.SplitByIntersections(context, Intersections, _line);
            }
            else
            {
                context.CurrentContainer.Shapes.Add(_line);
            }

            _line = null;

            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _line.Point.X = x;
            _line.Point.Y = y;

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                context.Renderer.Selection.Selected.Remove(_line.StartPoint);
                context.Renderer.Selection.Selected.Remove(_line.Point);
                _line = null;
            }

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Point:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
