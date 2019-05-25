﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Editor.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using SkiaSharp;

namespace Draw2D.Editor.Views
{
    public class ExportSkiaView : IDrawContainerView
    {
        private IToolContext _context;

        public ExportSkiaView(IToolContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
        }

        public void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            using (var renderer = new SkiaShapeRenderer(_context))
            using (var background = SkiaHelper.ToSKPaintBrush(view.PrintBackground))
            {
                var canvas = context as SKCanvas;
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, view.Width, view.Height), background);
                view.CurrentContainer.Draw(canvas, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            }
        }
    }
}
