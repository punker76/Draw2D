﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views
{
    public class MatrixView : UserControl
    {
        public MatrixView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
