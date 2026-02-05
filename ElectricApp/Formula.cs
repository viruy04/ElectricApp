// Интерфейс для всех формул
public interface Formula
{
    // Метод для ввода параметров (через массив чисел)
    void InputParameters(double[] values);

    // Метод для расчёта результата по формуле
    void Calculate();

    // Метод для получения результата расчёта
    double GetResult();

    // Метод для получения строки с самой формулой (например, "I = q / t")
    string GetFormulaPresentation();

    // Метод для получения описания параметров формулы (например, "q (Кл), t (с)")
    string GetParameterPresentation();
}