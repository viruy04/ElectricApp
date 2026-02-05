using System;

namespace ElectricApp
{
    public class JoulesLawFormula : Formula
    {
        public double Q { get; private set; }  // Энергия в джоулях
        public double R { get; private set; }  // Сопротивление в омах
        public double t { get; private set; }  // Время в секундах
        public double I { get; private set; }  // Сила тока

        public void InputParameters(double[] values)
        {
            if (values == null || values.Length != 3)
                throw new ArgumentException("Ожидается ровно 3 параметра: Q, R и t");

            if (values[0] < 0)
                throw new ArgumentException("Значение Q не может быть отрицательным");

            if (values[1] < 0)
                throw new ArgumentException("Значение R не может быть отрицательным");

            if (values[2] < 0)
                throw new ArgumentException("Значение t не может быть отрицательным");

            if (double.IsNaN(values[0]) || double.IsNaN(values[1]) || double.IsNaN(values[2]))
                throw new ArgumentException("Параметры не могут быть NaN");

            if (double.IsNaN(values[0]) || double.IsNaN(values[1]) || double.IsNaN(values[2]))
                throw new ArgumentException("Параметры не могут быть NaN");

            Q = values[0];
            R = values[1];
            t = values[2];
        }

        public void Calculate()
        {
            if (R == 0 || t == 0)
                throw new DivideByZeroException("Значения R и t не могут быть равны нулю");

            double underRoot = Q / (R * t);

            if (underRoot < 0)
                throw new ArithmeticException("Подкоренное выражение не может быть отрицательным");

            I = Math.Sqrt(underRoot);
        }

        public double GetResult() => I;

        public string GetFormulaPresentation() => "I = √(Q / (R * t))";

        public string GetParameterPresentation() => "Q (Дж), R (Ом), t (с)";
    }
}