using System;
using System.IO;
using System.Text;

namespace ElectricApp
{
    public class ResultSaver
    {
        private readonly string _filename; // Имя файла для сохранения результатов

        public ResultSaver(string filename = "results.txt")
        {
            _filename = filename; // Запоминаем имя файла
        }

        public void SaveOhmsLaw(double? voltage = null, double? resistance = null, double? current = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Закон Ома ==="); // Заголовок записи
            if (voltage.HasValue) sb.AppendLine($"Напряжение (U): {voltage} В"); // Записываем напряжение
            if (resistance.HasValue) sb.AppendLine($"Сопротивление (R): {resistance} Ом"); // Записываем сопротивление
            if (current.HasValue) sb.AppendLine($"Сила тока (I): {current} А"); // Записываем ток
            sb.AppendLine($"Время записи: {DateTime.Now}"); // Время записи
            sb.AppendLine();

            File.AppendAllText(_filename, sb.ToString()); // Добавляем запись в файл
        }

        public void SaveJoulesLaw(double? heat = null, double? resistance = null, double? time = null, double? current = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Закон Джоуля-Ленца ==="); // Заголовок записи
            if (heat.HasValue) sb.AppendLine($"Теплота (Q): {heat} Дж"); // Записываем теплоту
            if (resistance.HasValue) sb.AppendLine($"Сопротивление (R): {resistance} Ом"); // Записываем сопротивление
            if (time.HasValue) sb.AppendLine($"Время (t): {time} с"); // Записываем время
            if (current.HasValue) sb.AppendLine($"Сила тока (I): {current} А"); // Записываем ток
            sb.AppendLine($"Время записи: {DateTime.Now}"); // Время записи
            sb.AppendLine();

            File.AppendAllText(_filename, sb.ToString()); // Добавляем запись в файл
        }

        public void SaveAmperage(double? charge = null, double? time = null, double? current = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Формула силы тока ==="); // Заголовок записи
            if (charge.HasValue) sb.AppendLine($"Заряд (q): {charge} Кл"); // Записываем заряд
            if (time.HasValue) sb.AppendLine($"Время (t): {time} с"); // Записываем время
            if (current.HasValue) sb.AppendLine($"Сила тока (I): {current} А"); // Записываем ток
            sb.AppendLine($"Время записи: {DateTime.Now}"); // Время записи
            sb.AppendLine();

            File.AppendAllText(_filename, sb.ToString()); // Добавляем запись в файл
        }

        public void ClearFile()
        {
            File.WriteAllText(_filename, string.Empty); // Очищаем содержимое файла
        }
    }
}
