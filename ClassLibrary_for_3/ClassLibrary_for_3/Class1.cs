using System;
using System.Collections.Generic;

namespace ClassLibrary_for_3
{
    public class DataValidator<T>
    {
        public List<Func<T, bool>> validationRules = new List<Func<T, bool>>();

        public void Validate(T data)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (var rule in validationRules)
            {
                if (!rule(data))
                {
                    exceptions.Add(new ValidationException("Validation failed for the data."));
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public class ValidationException : Exception
        {
            public ValidationException(string message) : base(message) { }
        }
    }

    public class DataValidatorBuilder<T>
    {
        private DataValidator<T> validator = new DataValidator<T>();

        public DataValidatorBuilder<T> AddRule(Func<T, bool> validationRule)
        {
            validator.validationRules.Add(validationRule);
            return this;
        }

        public DataValidator<T> Build()
        {
            if (validator.validationRules.Count == 0)
            {
                throw new NoRulesException("No validation rules added to the validator.");
            }
            //
            var result = validator;
            validator = new DataValidator<T>();
            return result;
        }

        public class NoRulesException : Exception
        {
            public NoRulesException(string message) : base(message) { }
        }
    }
}
