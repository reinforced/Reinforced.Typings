using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Denotes parameter name and constant value for constructor's :base call
    ///     We need this attribute because it is programmatically impossible to determine :base call parameters
    ///     via reflection. So in this case we need some help from user's side
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class TsBaseParamAttribute : Attribute
    {
        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="value">TypeScript expression to be supplied for super() call</param>
        public TsBaseParamAttribute(string value)
        {
            Values = new []{ value };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        public TsBaseParamAttribute(string firstValue, string secondValue)
        {
            Values = new[] { firstValue, secondValue };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        /// <param name="thirdValue">TypeScript expression to be supplied for super() call at position 3</param>
        public TsBaseParamAttribute(string firstValue, string secondValue, string thirdValue)
        {
            Values = new[] { firstValue, secondValue, thirdValue };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        /// <param name="thirdValue">TypeScript expression to be supplied for super() call at position 3</param>
        /// <param name="fourthValue">TypeScript expression to be supplied for super() call at position 4</param>
        public TsBaseParamAttribute(string firstValue, string secondValue, string thirdValue, string fourthValue)
        {
            Values = new[] { firstValue, secondValue, thirdValue, fourthValue };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        /// <param name="thirdValue">TypeScript expression to be supplied for super() call at position 3</param>
        /// <param name="fourthValue">TypeScript expression to be supplied for super() call at position 4</param>
        /// <param name="fifthValue">TypeScript expression to be supplied for super() call at position 5</param>
        public TsBaseParamAttribute(string firstValue, string secondValue, string thirdValue, string fourthValue, string fifthValue)
        {
            Values = new[] { firstValue, secondValue, thirdValue, fourthValue, fifthValue };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        /// <param name="thirdValue">TypeScript expression to be supplied for super() call at position 3</param>
        /// <param name="fourthValue">TypeScript expression to be supplied for super() call at position 4</param>
        /// <param name="fifthValue">TypeScript expression to be supplied for super() call at position 5</param>
        /// <param name="sixthValue">TypeScript expression to be supplied for super() call at position 6</param>
        public TsBaseParamAttribute(string firstValue, string secondValue, string thirdValue, string fourthValue, string fifthValue, string sixthValue)
        {
            Values = new[] { firstValue, secondValue, thirdValue, fourthValue, fifthValue, sixthValue };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        /// <param name="thirdValue">TypeScript expression to be supplied for super() call at position 3</param>
        /// <param name="fourthValue">TypeScript expression to be supplied for super() call at position 4</param>
        /// <param name="fifthValue">TypeScript expression to be supplied for super() call at position 5</param>
        /// <param name="sixthValue">TypeScript expression to be supplied for super() call at position 6</param>
        /// <param name="seventhValue">TypeScript expression to be supplied for super() call at position 7</param>
        public TsBaseParamAttribute(string firstValue, string secondValue, string thirdValue, string fourthValue, string fifthValue, string sixthValue, string seventhValue)
        {
            Values = new[] { firstValue, secondValue, thirdValue, fourthValue, fifthValue, sixthValue, seventhValue };
        }

        /// <summary>
        ///     Creates instance of TsBaseParamAttribute
        /// </summary>
        /// <param name="firstValue">TypeScript expression to be supplied for super() call at position 1</param>
        /// <param name="secondValue">TypeScript expression to be supplied for super() call at position 2</param>
        /// <param name="thirdValue">TypeScript expression to be supplied for super() call at position 3</param>
        /// <param name="fourthValue">TypeScript expression to be supplied for super() call at position 4</param>
        /// <param name="fifthValue">TypeScript expression to be supplied for super() call at position 5</param>
        /// <param name="sixthValue">TypeScript expression to be supplied for super() call at position 6</param>
        /// <param name="seventhValue">TypeScript expression to be supplied for super() call at position 7</param>
        /// <param name="eighthValue">TypeScript expression to be supplied for super() call at position 8</param>
        public TsBaseParamAttribute(string firstValue, string secondValue, string thirdValue, string fourthValue, string fifthValue, string sixthValue, string seventhValue, string eighthValue)
        {
            Values = new[] { firstValue, secondValue, thirdValue, fourthValue, fifthValue, sixthValue, seventhValue, eighthValue };
        }

        /// <summary>
        ///     Parameters for super() call
        ///     Here should be stored TypeScript expressions
        /// </summary>
        public string[] Values { get; set; }
    }
}