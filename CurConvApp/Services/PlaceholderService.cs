using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace CurConvApp.Services
{
    public static class PlaceholderService
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(PlaceholderService),
                new PropertyMetadata(null, OnPlaceholderChanged));

        public static void SetPlaceholder(UIElement element, string value) =>
            element.SetValue(PlaceholderProperty, value);

        public static string GetPlaceholder(UIElement element) =>
            (string)element.GetValue(PlaceholderProperty);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            textBox.Loaded += (s, _) => AddAdorner(textBox);
            textBox.TextChanged += (s, _) => UpdateAdornerVisibility(textBox);
        }

        private static void AddAdorner(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            var adorners = layer.GetAdorners(textBox);
            if (adorners?.OfType<PlaceholderAdorner>().Any() == true) return;

            var adorner = new PlaceholderAdorner(textBox, GetPlaceholder(textBox));
            layer.Add(adorner);
            UpdateAdornerVisibility(textBox);
        }

        private static void UpdateAdornerVisibility(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            var adorners = layer.GetAdorners(textBox);
            if (adorners == null) return;

            foreach (var adorner in adorners.OfType<PlaceholderAdorner>())
            {
                adorner.Visibility = string.IsNullOrEmpty(textBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private class PlaceholderAdorner : Adorner
        {
            private readonly TextBlock _placeholder;

            public PlaceholderAdorner(UIElement adornedElement, string placeholderText)
                : base(adornedElement)
            {
                IsHitTestVisible = false;

                _placeholder = new TextBlock
                {
                    Text = placeholderText,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(4, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                AddVisualChild(_placeholder);
            }

            protected override int VisualChildrenCount => 1;

            protected override Visual GetVisualChild(int index) => _placeholder;

            protected override Size ArrangeOverride(Size finalSize)
            {
                _placeholder.Arrange(new Rect(finalSize));
                return finalSize;
            }
        }
    }
}
