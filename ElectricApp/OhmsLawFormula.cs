using System;

namespace ElectricApp
{
    public class OhmsLawFormula : Formula
    {
        public double U { get; private set; }  // Напряжение
        public double R { get; private set; }  // Сопротивление
        public double I { get; private set; }  // Ток (результат)

        public void InputParameters(double[] values)
        {
            if (values == null || values.Length != 2)
                throw new ArgumentException("Ожидается ровно 2 параметра: U и R");

            if (values[0] < 0)
                throw new ArgumentException("Значение U не может быть отрицательным");

            if (values[1] < 0)
                throw new ArgumentException("Значение R не может быть отрицательным");

            if (double.IsNaN(values[0]) || double.IsNaN(values[1]))
                throw new ArgumentException("Параметры не могут быть NaN");

            U = values[0];
            R = values[1];
        }

        public void Calculate()
        {
            if (R == 0)
                throw new DivideByZeroException("Сопротивление R не может быть нулём");

            I = U / R;
        }

        public double GetResult() => I;

        public string GetFormulaPresentation() => "I = U / R";

        public string GetParameterPresentation() => "U (В), R (Ом)";
    }
}

