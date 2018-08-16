using System;

namespace Dash.Domain
{
    public class Signature
    {
        public Signature(string name, string email, DateTime date)
        {
            Name = name;
            Email = email;
            Date = date;
        }

        public string Name { get; }
        public string Email { get; }
        public DateTime Date { get; }
    }

    public class SignatureBuilder
    {
        private string _name;

        public SignatureBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        private string _email;

        public SignatureBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        private DateTime _date;

        public SignatureBuilder WithDate(DateTime date)
        {
            _date = date;
            return this;
        }

        public Signature Build()
        {
            return new Signature(_name, _email, _date);
        }

        public static implicit operator Signature(SignatureBuilder builder)
        {
            return builder.Build();
        }
    }
}