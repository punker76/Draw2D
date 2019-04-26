﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels
{
    public interface IToolContext : IInputTarget
    {
        IShapeRenderer Renderer { get; set; }
        IHitTest HitTest { get; set; }
        CanvasContainer CurrentContainer { get; set; }
        CanvasContainer WorkingContainer { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointShape { get; set; }
        IList<ITool> Tools { get; set; }
        ITool CurrentTool { get; set; }
        EditMode Mode { get; set; }
        ICanvasPresenter Presenter { get; set; }
        ISelection Selection { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
        void SetTool(string name);
    }
}
