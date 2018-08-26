using System;
using System.Text;
using Xunit.Sdk;

namespace Dash.Tests.TestHelpers
{
    public abstract class AssertHelper<T>
    {
        private static readonly string DefaultName = typeof(T).Name;
       
        private T _expected;
        private T _actual;
        private StringBuilder _expectedText;
        private StringBuilder _actualText;

        private bool _notEqual;

        private StringBuilder CreateStringBuilder()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Name} with");
            return sb;
        }

        protected virtual string Name => DefaultName;

        public void Equal(T expected, T actual)
        {
            _expected = expected;
            _actual = actual;
            _expectedText = CreateStringBuilder();
            _actualText = CreateStringBuilder();

            AssertObject(_expected, _actual);

            if (_notEqual)
            {
                throw new EqualException(_expectedText, _actualText);
            }
        }

        protected abstract void AssertObject(T expected, T actual);

        protected void AssertProperty<TValue>(string format, Func<T, TValue> selector, Func<bool> equal = null, Func<TValue, string> print = null)
        {
            var expectedValue = selector(_expected);
            var actualValue = selector(_actual);

            equal = equal ?? (() => Equals(expectedValue, actualValue));
            print = print ?? (o => o.ToString());

            if (!equal())
            {
                _notEqual = true;
                _expectedText.AppendLine(string.Format(format, print(expectedValue)));
                _actualText.AppendLine(string.Format(format, print(actualValue)));
            }
        }
    }
}