﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Editor.Tools
{
    public class NoneTool : ToolBase
    {
        public override string Name => "None";

        public NoneToolSettings Settings { get; set; }
    }
}
