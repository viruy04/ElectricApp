using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ElectricApp
{
    public partial class HelpWindow : Window
    {
        private readonly FormulaType formulaType; // Сохраняем формулу, чтобы передать обратно

        public HelpWindow(FormulaType formulaType)
        {
            InitializeComponent();

            this.formulaType = formulaType;
        }

        // Обработчик кнопки закрытия
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var calcWindow = new CalculationWindow(formulaType)
            {
                Opacity = 0,
                WindowState = WindowState.Maximized
            };

            // Сначала показываем новое окно
            calcWindow.Show();

            // Плавно появляется
            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            calcWindow.BeginAnimation(Window.OpacityProperty, fadeIn);

            // Плавно исчезает это окно
            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(200),
                FillBehavior = FillBehavior.Stop
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Close();
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }
    }
}