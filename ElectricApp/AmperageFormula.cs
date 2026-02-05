using System;

namespace ElectricApp
{
    public class AmperageFormula : Formula
    {
        public double q { get; private set; }  // Заряд в кулонах
        public double t { get; private set; }  // Время в секундах
        public double I { get; private set; }  // Сила тока (ампер)

        // Ввод параметров из массива значений
        public void InputParameters(double[] values)
        {
            if (values == null || values.Length != 2)
                throw new ArgumentException("Ожидается ровно 2 параметра: q и t");

            if (values[0] <= 0)
                throw new ArgumentException("Значение q должно быть положительным");

            if (values[1] < 0)
                throw new ArgumentException("Значение t не может быть отрицательным");

            if (double.IsNaN(values[0]) || double.IsNaN(values[1]))
                throw new ArgumentException("Параметры не могут быть NaN");

            q = values[0];
            t = values[1];
        }

        // Вычисляем силу тока по формуле I = q / t
        public void Calculate()
        {
            if (t == 0)
                throw new DivideByZeroException("Время t не может быть нулём");

            I = q / t;
        }

        // Возвращаем результат вычислений
        public double GetResult() => I;

        // Возвращаем формулу в виде строки для отображения
        public string GetFormulaPresentation() => "I = q / t";

        // Возвращаем описание параметров для UI
        public string GetParameterPresentation() => "q (Кл), t (с)";
    }
}
