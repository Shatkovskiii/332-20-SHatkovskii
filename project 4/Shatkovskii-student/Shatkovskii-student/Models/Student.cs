using System;
using System.Text.RegularExpressions;

namespace Shatkovskii_student.Models
{
    public class Student
    {
        private string _email;
        private DateTime _birthDate;

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (value < new DateTime(1992, 1, 1))
                    throw new ArgumentException("Дата рождения не может быть ранее 01.01.1992");
                if (value > DateTime.Now)
                    throw new ArgumentException("Дата рождения не может быть позже текущей даты");
                _birthDate = value;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (!IsValidEmail(value))
                    throw new ArgumentException("Неверный формат email или недопустимый домен. Допустимые домены: yandex.ru, gmail.com, icloud.com");
                _email = value;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var pattern = @"^[a-zA-Z0-9._%+-]{3,}@(yandex\.ru|gmail\.com|icloud\.com)$";
            return Regex.IsMatch(email, pattern);
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(MiddleName) &&
                   Course >= 1 && Course <= 6 &&
                   !string.IsNullOrWhiteSpace(Group) &&
                   !string.IsNullOrWhiteSpace(Email);
        }
    }
} 