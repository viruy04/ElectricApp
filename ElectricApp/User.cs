using System;
using System.Windows.Controls;

namespace ElectricApp
{
    public class User
    {
        private Animation_electrons animation;  // Анимация электронов
        private Formula selectedFormula;         // Выбранная формула
        public UnitType selectedUnit;            // Выбранная единица измерения
        private readonly ResultSaver resultSaver = new ResultSaver(); // Класс для сохранения результатов

        public void SelectFormula(FormulaType type)
        {
            selectedFormula = GetFormulaByType(type); // Выбор формулы по типу
        }

        public Formula GetFormulaByType(FormulaType type)
        {
            // Создаем нужный объект формулы в зависимости от типа
            switch (type)
            {
                case FormulaType.AMPERAGE_FORMULA:
                    return new AmperageFormula();
                case FormulaType.OHMS_LAW:
                    return new OhmsLawFormula();
                case FormulaType.JOULES_LAW:
                    return new JoulesLawFormula();
                default:
                    throw new NotImplementedException();
            }
        }

        // Выбор формулы
        public FormulaType FormulaType
        {
            get
            {
                if (selectedFormula is AmperageFormula)
                    return FormulaType.AMPERAGE_FORMULA;
                else if (selectedFormula is OhmsLawFormula)
                    return FormulaType.OHMS_LAW;
                else if (selectedFormula is JoulesLawFormula)
                    return FormulaType.JOULES_LAW;
                else
                    throw new InvalidOperationException("Неизвестный тип формулы");
            }
        }


        public void SelectUnit(UnitType unit)
        {
            selectedUnit = unit; // Выбираем единицу измерения
        }

        public double GetUnit()
        {
            // Возвращаем множитель для выбранной единицы
            switch (selectedUnit)
            {
                case UnitType.Гигаампер:
                    return 1e9;
                case UnitType.Мегаампер:
                    return 1e6;
                case UnitType.Килоампер:
                    return 1e3;
                case UnitType.Ампер:
                    return 1;
                case UnitType.Миллиампер:
                    return 1e-3;
                case UnitType.Микроампер:
                    return 1e-6;
                case UnitType.Наноампер:
                    return 1e-9;
                default:
                    throw new NotImplementedException("Единица измерения не реализована.");
            }
        }

        public void HandleFormula(double[] inputValues)
        {
            selectedFormula.InputParameters(inputValues); // Вводим параметры
            selectedFormula.Calculate();                  // Считаем результат
        }

        public double DisplayResult()
        {
            return selectedFormula.GetResult();           // Получаем результат вычисления
        }

        public string DisplayFormula()
        {
            return selectedFormula.GetFormulaPresentation(); // Получаем строку с формулой
        }

        public string DisplayParameters()
        {
            return selectedFormula.GetParameterPresentation(); // Получаем строку с параметрами
        }

        public void SaveResult()
        {
            if (selectedFormula == null)
                throw new InvalidOperationException("Формула не выбрана");

            // Сохраняем результат в зависимости от типа формулы
            switch (selectedFormula)
            {
                case OhmsLawFormula ohmsLaw:
                    resultSaver.SaveOhmsLaw(
                        ohmsLaw.U,
                        ohmsLaw.R,
                        ohmsLaw.I
                    );
                    break;

                case JoulesLawFormula joulesLaw:
                    resultSaver.SaveJoulesLaw(
                        joulesLaw.Q,
                        joulesLaw.R,
                        joulesLaw.t,
                        joulesLaw.I
                    );
                    break;

                case AmperageFormula amperage:
                    resultSaver.SaveAmperage(
                        amperage.q,
                        amperage.t,
                        amperage.I
                    );
                    break;

                default:
                    throw new NotImplementedException("Сохранение для данной формулы не реализовано");
            }
        }

        public void DisplayElectrons(Canvas canvas, double result)
        {
            // Запускаем анимацию движения электронов на канве с учетом результата
            animation = new Animation_electrons(canvas, result);
            animation?.Start();
        }
    }
}