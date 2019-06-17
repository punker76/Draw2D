﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class HitTest : ViewModelBase, IHitTest
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, double scale)
        {
            return shape.Bounds?.TryToGetPoint(shape, target, radius / scale, this);
        }

        public IPointShape TryToGetPoint(IEnumerable<IBaseShape> shapes, Point2 target, double radius, double scale, IPointShape exclude)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = TryToGetPoint(shape, target, radius, scale);
                if (result != null && result != exclude)
                {
                    return result;
                }
            }
            return null;
        }

        public IBaseShape TryToGetShape(IEnumerable<IBaseShape> shapes, Point2 target, double radius, double scale)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = shape.Bounds?.Contains(shape, target, radius / scale, this);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public ISet<IBaseShape> TryToGetShapes(IEnumerable<IBaseShape> shapes, Rect2 target, double radius, double scale)
        {
            var selected = new HashSet<IBaseShape>();
            foreach (var shape in shapes.Reverse())
            {
                var result = shape.Bounds?.Overlaps(shape, target, radius / scale, this);
                if (result != null)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
