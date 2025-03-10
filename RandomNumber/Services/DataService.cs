using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RandomNumber.Services
{

    public static class DataService
    {
        private static string ClassesFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Classes.txt");

        public static string GetStudentsFilePath(string className)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Students_{className}.txt");
        }

        public static async Task<List<string>> GetClassesAsync()
        {
            if (!File.Exists(ClassesFilePath))
                return new List<string>();

            return (await File.ReadAllLinesAsync(ClassesFilePath)).ToList();
        }

        public static async Task AddClassAsync(string className)
        {
            var classes = await GetClassesAsync();
            if (!classes.Contains(className))
            {
                classes.Add(className);
                await File.WriteAllLinesAsync(ClassesFilePath, classes);
            }
        }

        public static async Task<List<string>> GetStudentsAsync(string className)
        {
            var filePath = GetStudentsFilePath(className); 

            if (!File.Exists(filePath))
                return new List<string>();

            return (await File.ReadAllLinesAsync(filePath)).ToList();
        }

        public static async Task AddStudentAsync(string className, string studentName)
        {
            var students = await GetStudentsAsync(className);
            if (!students.Contains(studentName))
            {
                students.Add(studentName);
                var filePath = GetStudentsFilePath(className);
                await File.WriteAllLinesAsync(filePath, students);
            }
        }

        public static async Task UpdateStudentAsync(string className, string oldName, string newName)
        {
            var students = await GetStudentsAsync(className);
            var index = students.IndexOf(oldName);
            if (index != -1)
            {
                students[index] = newName;
                var filePath = GetStudentsFilePath(className);
                await File.WriteAllLinesAsync(filePath, students);
            }
        }

        public static async Task DeleteStudentAsync(string className, string studentName)
        {
            var students = await GetStudentsAsync(className);
            if (students.Remove(studentName))
            {
                var filePath = GetStudentsFilePath(className);
                await File.WriteAllLinesAsync(filePath, students);
            }
        }
    }
}