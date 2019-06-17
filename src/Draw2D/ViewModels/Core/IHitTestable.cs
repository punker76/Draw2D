﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels
{
    public interface IHitTestable
    {
        IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius, double scale);
    }
}
