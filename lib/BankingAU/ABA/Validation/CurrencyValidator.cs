﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Banking.AU.ABA.Validation
{
    public class CurrencyValidator<T> : IValidator<T>
    {
        private int _fieldLength;
        private GetValue<T, decimal> _get;
        private SetValue<T, decimal> _set;
        public CurrencyValidator(int fieldLength, GetValue<T, decimal> get)
        {
            if (fieldLength < 2)
                throw new ArgumentException("fieldLength must be 2 or greater.");
            _fieldLength = fieldLength;
            _get = get;
        }

        public CurrencyValidator(int fieldLength, GetValue<T, decimal> get, SetValue<T, decimal> set)
            : this(fieldLength, get)
        {
            _set = set;
        }

        private IEnumerable<IError> Validate(decimal value)
        {
            if (value < 0)
                yield return new Error("Value must be greater than zero.");

            // 10 chars in cents = $99,999,999.99 (8 digits dollars, 2 digits cents)
            var max = new decimal(Math.Pow(10, _fieldLength - 2)); 
            if (value >= max)
                yield return new Error(String.Format("Value must be less than {0:C2}", max));
        }

        public void Clean(T item)
        {
            if (_set == null)
                throw new MissingMethodException("Cleaning currency requires a set method.");
            var value = Math.Abs(_get(item));
            foreach (var e in Validate(value))
                throw new ArgumentOutOfRangeException(e.Message, (Exception)null);
            _set(item, value);
        }

        public IEnumerable<IError> Validate(T item)
        {
            return Validate(_get(item));
        }
    }
}