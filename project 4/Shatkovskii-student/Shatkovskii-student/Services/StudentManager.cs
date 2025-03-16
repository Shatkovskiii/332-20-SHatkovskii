using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Shatkovskii_student.Models;

namespace Shatkovskii_student.Services
{
    public class StudentManager
    {
        private List<Student> _students;
        private string _currentFilePath;
        private bool _hasUnsavedChanges;

        public bool HasUnsavedChanges => _hasUnsavedChanges;

        public StudentManager()
        {
            _students = new List<Student>();
            _hasUnsavedChanges = false;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _students.ToList();
        }

        public void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (!student.IsValid())
                throw new ArgumentException("Данные студента некорректны");

            _students.Add(student);
            _hasUnsavedChanges = true;
        }

        public void UpdateStudent(int index, Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (!student.IsValid())
                throw new ArgumentException("Данные студента некорректны");

            if (index >= 0 && index < _students.Count)
            {
                _students[index] = student;
                _hasUnsavedChanges = true;
            }
        }

        public void RemoveStudent(int index)
        {
            if (index >= 0 && index < _students.Count)
            {
                _students.RemoveAt(index);
                _hasUnsavedChanges = true;
            }
        }

        public void SaveToJson(string filePath)
        {
            var json = JsonConvert.SerializeObject(_students, Formatting.Indented);
            File.WriteAllText(filePath, json);
            _currentFilePath = filePath;
            _hasUnsavedChanges = false;
        }

        public void LoadFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _students = JsonConvert.DeserializeObject<List<Student>>(json);
                _currentFilePath = filePath;
                _hasUnsavedChanges = false;
            }
        }

        public IEnumerable<Student> FilterStudents(int? course = null, string group = null, string lastName = null)
        {
            var query = _students.AsEnumerable();

            if (course.HasValue)
                query = query.Where(s => s.Course == course.Value);

            if (!string.IsNullOrWhiteSpace(group))
                query = query.Where(s => s.Group.Contains(group));

            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(s => s.LastName.Contains(lastName));

            return query.ToList();
        }

        public void ImportFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1)) // Пропускаем заголовок
            {
                var values = line.Split(',');
                if (values.Length >= 7)
                {
                    var student = new Student
                    {
                        LastName = values[0],
                        FirstName = values[1],
                        MiddleName = values[2],
                        Course = int.Parse(values[3]),
                        Group = values[4],
                        BirthDate = DateTime.Parse(values[5]),
                        Email = values[6]
                    };
                    AddStudent(student);
                }
            }
        }

        public void ExportToCsv(string filePath)
        {
            var lines = new List<string>
            {
                "Фамилия,Имя,Отчество,Курс,Группа,Дата рождения,Email"
            };

            foreach (var student in _students)
            {
                lines.Add($"{student.LastName},{student.FirstName},{student.MiddleName}," +
                         $"{student.Course},{student.Group},{student.BirthDate:dd.MM.yyyy},{student.Email}");
            }

            File.WriteAllLines(filePath, lines);
        }
    }
} 