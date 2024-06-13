using System;
using ClassLibrary_for_3;

namespace ValidatorDemo
{
    class Program
    {
        static void Main()
        {
            var stringValidator = new DataValidatorBuilder<string>()
                .AddRule(data => !string.IsNullOrEmpty(data))
                .AddRule(data => data.Length > 3)
                .Build();

            try
            {
                string input = "Hello";
                stringValidator.Validate(input);
                Console.WriteLine("String data is valid.");
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Console.WriteLine("Validation error: " + innerEx.Message);
                }
            }

            var intValidator = new DataValidatorBuilder<int>()
                .AddRule(data => data > 0)
                .AddRule(data => data % 2 == 0)
                .Build();

            try
            {
                int number = 3;
                intValidator.Validate(number);
                Console.WriteLine("Integer data is valid.");
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Console.WriteLine("Validation error: " + innerEx.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
