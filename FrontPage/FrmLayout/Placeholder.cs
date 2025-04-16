// repos/FrontPage/FrmLayout/Placeholder.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrontPage.FrmLayout
{
    public static class Placeholder
    {
        #region Propriedade Anexada para Texto do Placeholder

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof(string),
                typeof(Placeholder),
                new FrameworkPropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        #endregion

        #region Comportamento do Placeholder

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            // Adiciona ou remove os handlers de eventos
            if (e.OldValue == null && e.NewValue != null)
            {
                textBox.Loaded += TextBox_Loaded;
                textBox.GotFocus += RemovePlaceholder;
                textBox.LostFocus += ShowPlaceholder;
                textBox.TextChanged += TextBox_TextChanged;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                textBox.Loaded -= TextBox_Loaded;
                textBox.GotFocus -= RemovePlaceholder;
                textBox.LostFocus -= ShowPlaceholder;
                textBox.TextChanged -= TextBox_TextChanged;
            }

            // Aplica o placeholder inicial
            SetPlaceholder(textBox, GetText(textBox));
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SetPlaceholder(textBox, GetText(textBox));
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsFocused && string.IsNullOrEmpty(textBox.Text))
                {
                    SetPlaceholder(textBox, GetText(textBox));
                }
            }
        }

        private static void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && GetText(textBox) == textBox.Text)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = SystemColors.WindowTextBrush;
            }
        }

        private static void ShowPlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                SetPlaceholder(textBox, GetText(textBox));
            }
        }

        private static void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = placeholderText;
                textBox.Foreground = Brushes.Gray;
            }
        }

        #endregion
    }
}