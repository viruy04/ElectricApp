using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElectricApp;

namespace ElectricAppUnitTest
{
    //----------------------------//
   /* МОДУЛЬ 1 - ФОРМУЛА 1 I=q/t */
  //----------------------------//
    [TestClass]
    public class AmperageFormulaTest
    {
        // Проверка корректного присвоения параметров при валидных значениях
        [TestMethod]
        public void InputParameters_ValidValues_AssignsCorrectly()
        {
            var formula = new AmperageFormula();
            formula.InputParameters(new double[] { 101.7, 8 });

            Assert.AreEqual(101.7, formula.q);
            Assert.AreEqual(8, formula.t);
        }

        // Проверка вычисления силы тока при валидных параметрах
        [TestMethod]
        public void Calculate_ValidInput_ReturnsCorrectCurrent()
        {
            var formula = new AmperageFormula();
            formula.InputParameters(new double[] { 10.0, 2.0 });

            formula.Calculate();
            double result = formula.GetResult();

            Assert.AreEqual(5.0, result, 0.001);
        }

        // Проверка, что выбрасывается исключение при неверном количестве параметров
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InputParameters_InvalidLength_ThrowsArgumentException()
        {
            var formula = new AmperageFormula();
            formula.InputParameters(new double[] { 92.5 });// Здесь вызывается строка:
            // if (values == null || values.Length != 2) throw new ArgumentException(...)
            // В остальных тестах (модулях) также проверка на исключения
        }

        // Проверка, что выбрасывается исключение при отрицательном заряде
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InputParameters_NegativeCharge_ThrowsArgumentException()
        {
            var formula = new AmperageFormula();
            formula.InputParameters(new double[] { -18, 3.4 });
        }
    }

    // ---------------------------//
   /* МОДУЛЬ 2 - ФОРМУЛА 2 I=U/R */
  // ---------------------------//
    [TestClass]
    public class OhmsLawFormulaTest
    {
        // Проверка вычисления тока при валидных значениях
        [TestMethod]
        public void Calculate_ValidInput_ReturnsCorrectCurrent()
        {
            var formula = new OhmsLawFormula();
            formula.InputParameters(new double[] { 12.1, 11.0 });

            formula.Calculate();
            double result = formula.GetResult();

            Assert.AreEqual(1.1, result, 0.001);
        }

        // Проверка: отрицательное напряжение — выбрасывается ArgumentException
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InputParameters_NegativeVoltage_ThrowsArgumentException()
        {
            var formula = new OhmsLawFormula();
            formula.InputParameters(new double[] { -5.0, 10.0 });
        }

        // Проверка: сопротивление 0 — выбрасывается DivideByZeroException
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Calculate_ZeroResistance_ThrowsDivideByZeroException()
        {
            var formula = new OhmsLawFormula();
            formula.InputParameters(new double[] { 5.0, 0.0 });
            formula.Calculate(); // исключение будет выброшено здесь
        }
    }

    // ----------------------------------//
   /* МОДУЛЬ 3 - ФОРМУЛА 3 I=√(Q/(R*t)) */
  // ----------------------------------//
    [TestClass]
    public class JoulesLawFormulaTest
    {
        // Валидные значения 
        [TestMethod]
        public void JoulesLawFormula_ValidInput_CalculatesCorrectly()
        {
            var formula = new JoulesLawFormula();
            formula.InputParameters(new double[] { 36.0, 3.0, 2.0 }); // Q = 36, R = 3, t = 2
            formula.Calculate();
            double result = formula.GetResult();
            Assert.AreEqual(2.4494, result, 0.0001); // √(36 / (3*2)) = √6 ≈ 2.4494
        }

        // Один из параметров — не число, вызов исключения
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void JoulesLawFormula_InputContainsNaN_ThrowsArgumentException()
        {
            var formula = new JoulesLawFormula();
            formula.InputParameters(new double[] { double.NaN, 3.0, 2.0 });
        }

        //Введено недостаточно параметров, вызов исключения
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void JoulesLawFormula_MissingParameters_ThrowsArgumentException()
        {
            var formula = new JoulesLawFormula();
            formula.InputParameters(new double[] { 10.0, 2.0 }); // не хватает времени
        }
    }
}