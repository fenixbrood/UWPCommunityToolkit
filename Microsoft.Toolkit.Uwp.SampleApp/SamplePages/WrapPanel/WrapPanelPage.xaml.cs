﻿using Microsoft.Toolkit.Uwp.UI.Controls.WrapPanel;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// WrapPanel sample page
    /// </summary>
    public sealed partial class WrapPanelPage : Page
    {
        private static Button GenerateButton()
        {
            var button = new Button { Content = "Button" };
            button.Click += (sender, args) =>
            {
                var currentButton = sender as Button;
                var parent = currentButton?.Parent as WrapPanel;
                parent?.Children.Remove(button);
            };
            return button;
        }

        public WrapPanelPage()
        {
            InitializeComponent();
        }

        private void HorizontalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            HorizontalWrapPanel.Children.Add(GenerateButton());
        }

        private void VerticalButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VerticalWrapPanel.Children.Add(GenerateButton());
        }
    }
}
