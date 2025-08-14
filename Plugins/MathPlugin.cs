using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace DataAICapstone.Plugins
{
    /// <summary>
    /// Math plugin that provides mathematical functions to Semantic Kernel agents
    /// </summary>
    public class MathPlugin
    {
        [KernelFunction]
        [Description("Add two numbers together")]
        public double Add(
            [Description("The first number")] double a,
            [Description("The second number")] double b)
        {
            return a + b;
        }

        [KernelFunction]
        [Description("Subtract the second number from the first")]
        public double Subtract(
            [Description("The first number")] double a,
            [Description("The second number")] double b)
        {
            return a - b;
        }

        [KernelFunction]
        [Description("Multiply two numbers")]
        public double Multiply(
            [Description("The first number")] double a,
            [Description("The second number")] double b)
        {
            return a * b;
        }

        [KernelFunction]
        [Description("Divide the first number by the second")]
        public double Divide(
            [Description("The dividend")] double a,
            [Description("The divisor")] double b)
        {
            if (b == 0)
                throw new ArgumentException("Cannot divide by zero");
            return a / b;
        }

        [KernelFunction]
        [Description("Calculate the square root of a number")]
        public double SquareRoot([Description("The number to calculate square root of")] double number)
        {
            if (number < 0)
                throw new ArgumentException("Cannot calculate square root of negative number");
            return Math.Sqrt(number);
        }

        [KernelFunction]
        [Description("Calculate the power of a number")]
        public double Power(
            [Description("The base number")] double baseNumber,
            [Description("The exponent")] double exponent)
        {
            return Math.Pow(baseNumber, exponent);
        }

        [KernelFunction]
        [Description("Calculate the absolute value of a number")]
        public double Absolute([Description("The number")] double number)
        {
            return Math.Abs(number);
        }
    }
}
