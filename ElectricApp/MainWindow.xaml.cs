using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ElectricApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Opacity = 0; // Изначально прозрачное окно
            Loaded += MainWindow_Loaded; // Подписываемся на событие загрузки окна
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1700));
            this.BeginAnimation(Window.OpacityProperty, fadeIn); // Запускаем анимацию появления
        }

        // Метод для плавного исчезновения текущего окна и открытия нового
        private void FadeTransitionAndOpen(Func<Window> createNewWindow)
        {
            Window newWindow = null;

            if (createNewWindow != null)
            {
                newWindow = createNewWindow.Invoke();

                if (newWindow != null)
                {
                    newWindow.Opacity = 0;
                    newWindow.Show();

                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                    newWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
                }
            }

            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(500),
                FillBehavior = FillBehavior.Stop
            };

            fadeOut.Completed += (s, e) =>
            {
                this.Opacity = 0;
                this.Close();
            };

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.BeginAnimation(Window.OpacityProperty, fadeOut);
            };
            timer.Start();
        }

        // Обработчик кнопки открытия окна для формулы "Сила тока через заряд"
        private void OpenFormula1_Click(object sender, RoutedEventArgs e)
        {
            FadeTransitionAndOpen(() =>
            {
                var calcWindow = new CalculationWindow(FormulaType.AMPERAGE_FORMULA);
                return calcWindow;
            });
        }

        private void OpenFormula2_Click(object sender, RoutedEventArgs e)
        {
            FadeTransitionAndOpen(() =>
            {
                var calcWindow = new CalculationWindow(FormulaType.OHMS_LAW);
                return calcWindow;
            });
        }

        private void OpenFormula3_Click(object sender, RoutedEventArgs e)
        {
            FadeTransitionAndOpen(() =>
            {
                var calcWindow = new CalculationWindow(FormulaType.JOULES_LAW);
                return calcWindow;
            });
        }


        // Обработчик кнопки выхода из приложения с плавным исчезновением
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            FadeTransitionAndOpen(() => null); // Возвращаем null, чтобы закрыть только текущее окно
        }


        // Выбор формулы по клавишам 1, 2, 3
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1 || e.Key == Key.NumPad1)
            {
                OpenFormula1_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
            {
                OpenFormula2_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
            {
                OpenFormula3_Click(null, null);
                e.Handled = true;
            }
        }

    }
}