using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Line {lineNumber}: Missing field(s). Expected 3 fields but found {parts.Length}.");

                string idStr = parts[0].Trim();
                string name = parts[1].Trim();
                string scoreStr = parts[2].Trim();

                if (!int.TryParse(idStr, out int id))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID '{idStr}'.");

                if (!int.TryParse(scoreStr, out int score))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score '{scoreStr}'.");

                students.Add(new Student(id, name, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                string line = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                writer.WriteLine(line);
            }
        }
    }
}

class Program
{
    static void Main()
    {
        string inputFilePath = "students.txt";
        string outputFilePath = "students_report.txt";

        var processor = new StudentResultProcessor();

        try
        {
            var students = processor.ReadStudentsFromFile(inputFilePath);

            processor.WriteReportToFile(students, outputFilePath);

            Console.WriteLine($"Report successfully written to {outputFilePath}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: Input file not found. {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}