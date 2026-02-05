using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ElectricApp
{
    public partial class CalculationWindow : Window
    {
        private User user;
        private List<TextBox> inputBoxes = new List<TextBox>(); // Список для полей ввода
        private readonly ResultSaver resultSaver = new ResultSaver();

        private Animation_electrons electronsAnimation; // Для анимации электронов

        // Конструктор окна
        public CalculationWindow(FormulaType type)
        {
            try
            {
                InitializeComponent(); // Загрузка XAML

                user = new User();
                user.SelectFormula(type);

                InitializeUI();
                CreateParameterInputs();

                // Рисуем нижний слой - провод
                DrawWire(Animation_canvas);
                // Рисуем компоненты (батарея, резистор) поверх провода
                DrawComponents(Animation_canvas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        // Настраивает текстовые поля и список единиц измерения
        private void InitializeUI()
        {
            FormulaText.Text = "Формула: " + user.DisplayFormula();
            ParameterHintText.Text = "Параметры: " + user.DisplayParameters();
            UnitComboBox.ItemsSource = Enum.GetValues(typeof(UnitType));
            UnitComboBox.SelectedIndex = 3; // По умолчанию Ампер
        }

        // Создаёт поля ввода под каждый параметр формулы
        private void CreateParameterInputs()
        {
            try
            {
                if (InputFieldsPanel == null)
                    throw new InvalidOperationException("Панель для полей ввода не найдена");

                InputFieldsPanel.Children.Clear();
                inputBoxes.Clear();

                string[] paramNames = user.DisplayParameters().Split(',');

                foreach (var param in paramNames)
                {
                    string paramName = param.Trim();

                    var inputRow = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    var label = new TextBlock
                    {
                        Text = paramName,
                        Style = (Style)FindResource("ParameterLabelStyle"),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var input = new TextBox
                    {
                        Style = (Style)FindResource("InputFieldStyle"),
                        Tag = paramName
                    };

                    inputRow.Children.Add(label);
                    inputRow.Children.Add(input);
                    InputFieldsPanel.Children.Add(inputRow);
                    inputBoxes.Add(input);

                    Debug.WriteLine($"Добавлено поле ввода для: {paramName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании полей ввода: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        //Рисует только провод (чёрная толстая линия) — самый нижний слой
        public static void DrawWire(Canvas canvas)
        {
            double scale = 2.5;
            double offsetX = -25; // Смещение влево для всей схемы

            var wire = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 5,
                Points = new PointCollection {
                    new Point(offsetX + 30 * scale, 40 * scale),
                    new Point(offsetX + 300 * scale, 40 * scale),
                    new Point(offsetX + 300 * scale, 150 * scale),
                    new Point(offsetX + (220 + 20) * scale, 150 * scale),
                    new Point(offsetX + (220 + 20) * scale, 200 * scale),
                    new Point(offsetX + 30 * scale, 200 * scale),
                    new Point(offsetX + 30 * scale, 80 * scale),
                    new Point(offsetX + 30 * scale, 40 * scale)
                }
            };
            canvas.Children.Add(wire);
        }

        // Рисует батарею и резистор, амперметр
        public static void DrawComponents(Canvas canvas)
        {
            double scale = 1.5; // размер
            double offsetX = -25; // координаты
            double shiftX = 15;   // сдвиг вправо
            double shiftY = 10;   // сдвиг вниз

            var battery = new Rectangle
            {
                Width = 20 * scale,
                Height = 40 * scale,
                Fill = Brushes.Blue
            };
            Canvas.SetLeft(battery, offsetX + 20 * 2.5 + shiftX);
            Canvas.SetTop(battery, 40 * 2.5 + shiftY);
            canvas.Children.Add(battery);

            var batteryLabel = new TextBlock
            {
                Text = "Батарея",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            // Подпись справа от блока батареи, по центру по вертикали
            Canvas.SetLeft(batteryLabel, offsetX + 20 * 2.5 + shiftX + 20 * scale + 5); // справа на 5 пикселей от батареи
            Canvas.SetTop(batteryLabel, 40 * 2.5 + shiftY + (40 * scale) / 2 - 10); // центр по высоте минус поправка на высоту текста
            canvas.Children.Add(batteryLabel);

            var resistor = new Rectangle
            {
                Width = 60 * scale,
                Height = 20 * scale,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(resistor, offsetX + (220 + 20) * 2.5 + shiftX);
            Canvas.SetTop(resistor, 140 * 2.5 + shiftY);
            canvas.Children.Add(resistor);

            var resistorLabel = new TextBlock
            {
                Text = "Резистор",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12
            };

            // Подпись под резистором
            Canvas.SetLeft(resistorLabel, offsetX + (220 + 20) * 2.5 + shiftX);
            Canvas.SetTop(resistorLabel, 140 * 2.5 + shiftY + 20 * scale + 5);
            canvas.Children.Add(resistorLabel);

            // Добавление амперметра (круг с буквой А)
            var ammeter = new Ellipse
            {
                Width = 30 * scale,
                Height = 30 * scale,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.White
            };

            // Центр нижней линии: середина между X = 30 и X = 240 (примерно)
            double wireStartX = offsetX + 30 * 2.5;
            double wireEndX = offsetX + (220 + 20) * 2.5;
            double centerX = (wireStartX + wireEndX) / 2 - (30 * scale) / 2;
            double centerY = 200 * 2.5 - (30 * scale) / 2; // выравниваем по проводу

            Canvas.SetLeft(ammeter, centerX);
            Canvas.SetTop(ammeter, centerY);
            canvas.Children.Add(ammeter);

            var ammeterLetter = new TextBlock
            {
                Text = "A",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(ammeterLetter, centerX + (30 * scale) / 2 - 8);
            Canvas.SetTop(ammeterLetter, centerY + (30 * scale) / 2 - 10);
            canvas.Children.Add(ammeterLetter);

            var ammeterLabel = new TextBlock
            {
                Text = "Амперметр",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 12
            };
            Canvas.SetLeft(ammeterLabel, centerX - 10);
            Canvas.SetTop(ammeterLabel, centerY + 30 * scale + 5);
            canvas.Children.Add(ammeterLabel);
        }



        // Перерисовка
        private void RedrawAll(double current)
        {
            // Очищаем Canvas от всего
            Animation_canvas.Children.Clear();

            // 1) Рисуем провод (нижний слой)
            DrawWire(Animation_canvas);

            // 2) Создаем и запускаем анимацию электронов поверх провода
            electronsAnimation?.Stop();
            electronsAnimation = new Animation_electrons(Animation_canvas, current);
            electronsAnimation.Start();

            // 3) Рисуем компоненты поверх электронов
            DrawComponents(Animation_canvas);
        }

        // Вычисление результата
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryGetValidatedValues(out double[] values, out string error))
                {
                    ShowError(error);
                    return;
                }

                user.HandleFormula(values);
                DisplayResult();

                double current = user.DisplayResult();

                // Перерисовываем 
                RedrawAll(current);
            }
            catch (Exception ex)
            {
                ShowError("Произошла непредвиденная ошибка: " + ex.Message);
            }
        }

        // Метод для проверки ошибок
        public bool TryGetValidatedValues(out double[] values, out string error)
        {
            values = new double[inputBoxes.Count];
            error = "";

            for (int i = 0; i < inputBoxes.Count; i++)
            {
                string text = inputBoxes[i].Text.Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    error = "Все поля должны быть заполнены";
                    return false;
                }

                text = text.Replace(',', '.');

                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    error = $"Неверное значение в поле {i + 1}: \"{inputBoxes[i].Text}\"";
                    return false;
                }

                if (value <= 0)
                {
                    error = $"Неположительное значение в поле {i + 1}: \"{inputBoxes[i].Text}\"";
                    return false;
                }

                values[i] = value;
            }

            return true;
        }


        // Отображает результат вычисления с учетом выбранной единицы измерения
        private void DisplayResult()
        {
            double result = user.DisplayResult();
            double multiplier = user.GetUnit();
            double converted = result / multiplier;

            string unitSymbol;
            switch (user.selectedUnit)
            {
                case UnitType.Ампер: unitSymbol = "А"; break;
                case UnitType.Килоампер: unitSymbol = "кА"; break;
                case UnitType.Мегаампер: unitSymbol = "МА"; break;
                case UnitType.Гигаампер: unitSymbol = "ГА"; break;
                case UnitType.Миллиампер: unitSymbol = "мА"; break;
                case UnitType.Микроампер: unitSymbol = "мкА"; break;
                case UnitType.Наноампер: unitSymbol = "нА"; break;
                default: throw new NotImplementedException();
            }

            ResultText.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            ResultText.Text = $"Сила тока (I): {converted} {unitSymbol}";
        }

        // Показывает сообщение об ошибке и удаляет электроны с канваса
        private void ShowError(string message)
        {

            ResultText.Text = message;
            ResultText.Foreground = new SolidColorBrush(Color.FromRgb(255, 24, 78));
            DeleteElectrons(Animation_canvas);
        }

        // Удаkяет все эллипсы (электроны) с канваса
        private void DeleteElectrons(Canvas canvas)
        {
            var elementsToRemove = new List<UIElement>();
            foreach (UIElement element in canvas.Children)
            {
                if (element is Ellipse ellipse && ellipse.Tag?.ToString() == "electron")
                    elementsToRemove.Add(element);
            }

            foreach (var element in elementsToRemove)
            {
                canvas.Children.Remove(element);
            }
        }

        // Открывает окно справки
        private async void Help_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new HelpWindow(user.FormulaType)
            {
                Opacity = 0,
                WindowState = WindowState.Maximized
            };

            helpWindow.Show();

            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            helpWindow.BeginAnimation(Window.OpacityProperty, fadeIn);

            await Task.Delay(150);

            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300),
                FillBehavior = FillBehavior.Stop
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Opacity = 0;
                this.Close();
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        // Возвращается на главное окно с плавным затемнением
        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            // 1. Открываем главное окно (пока с прозрачностью 0)
            var mainWindow = new MainWindow
            {
                Opacity = 0
            };
            mainWindow.Show();

            // 2. Анимация появления главного окна
            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            mainWindow.BeginAnimation(Window.OpacityProperty, fadeIn);

            // 3. Делаем паузу, чтобы новое окно появилось перед закрытием текущего
            await Task.Delay(150);

            // 4. Анимация исчезновения текущего окна
            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300),
                FillBehavior = FillBehavior.Stop
            };
            fadeOut.Completed += (s, args) =>
            {
                this.Opacity = 0;
                this.Close();
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }


        // Обработчик изменения выбранной единицы измерения — обновляет результат с новой единицей
        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitComboBox.SelectedItem is UnitType selected)
            {
                user.SelectUnit(selected);
                if (ResultText.Text.Contains("Сила тока"))
                {
                    DisplayResult();
                }
            }
        }

        // Сохраняет текущий результат в файл
        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                user.SaveResult();
                MessageBox.Show("Результаты успешно сохранены!", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Показывает сохранённые результаты из файла в всплывающем окне
        private void ShowResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists("results.txt"))
                {
                    ResultsTextBox.Text = File.ReadAllText("results.txt");
                    ResultsPopup.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Файл с результатами не найден.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Очищает все поля ввода, результат и удаляет электроны с канваса
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            foreach (var box in inputBoxes)
                box.Text = string.Empty;

            ResultText.Text = string.Empty;
            ResultText.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));

            DeleteElectrons(Animation_canvas);
        }

        // Закрывает окно с показом сохранённых результатов
        private void CloseResults_Click(object sender, RoutedEventArgs e)
        {
            ResultsPopup.Visibility = Visibility.Collapsed;
        }


        // Очистка по del и возврат по esc
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ClearFields_Click(null, null);
                e.Handled = true;
            }
            if (e.Key == Key.Escape)
            {
                Back_Click(sender, e);
                e.Handled = true;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1100)
            {
                VisualStateManager.GoToState(this, "Narrow", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Wide", true);
            }
        }


    }
}