using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
  public class Course
  {
    private int _id;
    private string _name;
    private string _courseNumber;

    public Course(string Name, string CourseNumber, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _courseNumber = CourseNumber;
    }

    public override bool Equals(System.Object otherCourse)
    {
        if (!(otherCourse is Course))
        {
          return false;
        }
        else
        {
          Course newCourse = (Course) otherCourse;
          bool idEquality = this.GetId() == newCourse.GetId();
          bool nameEquality = this.GetName() == newCourse.GetName();
          bool courseNumberEquality = this.GetCourseNumber() == newCourse.GetCourseNumber();
          return (idEquality && nameEquality && courseNumberEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public string GetCourseNumber()
    {
      return _courseNumber;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, course_number) OUTPUT INSERTED.id VALUES (@CourseName, @CourseNumber);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@CourseName";
      nameParameter.Value = this.GetName();

      SqlParameter courseNumberParameter = new SqlParameter();
      courseNumberParameter.ParameterName = "@CourseNumber";
      courseNumberParameter.Value = this.GetCourseNumber();


      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(courseNumberParameter);
      rdr = cmd.ExecuteReader();


      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static Course Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);
      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = id.ToString();
      cmd.Parameters.Add(courseIdParameter);
      rdr = cmd.ExecuteReader();

      int foundCourseId = 0;
      string foundCourseDescription = null;
      string foundCourseNumber = null;

      while(rdr.Read())
      {
        foundCourseId = rdr.GetInt32(0);
        foundCourseDescription = rdr.GetString(1);
        foundCourseNumber = rdr.GetString(2);
      }
      Course foundCourse = new Course(foundCourseDescription, foundCourseNumber, foundCourseId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCourse;
    }

    public static List<Course> GetAll()
    {
      List<Course> allCourses = new List<Course>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        Course newCourse = new Course(courseName, courseNumber, courseId);
        allCourses.Add(newCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allCourses;
    }

    public void AddStudent(Student newStudent)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId)", conn);
      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = this.GetId();
      cmd.Parameters.Add(courseIdParameter);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = newStudent.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Student> GetStudents()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT student_id FROM courses_students WHERE course_id = @CourseId;", conn);
      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = this.GetId();
      cmd.Parameters.Add(courseIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> studentIds = new List<int> {};
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        studentIds.Add(studentId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Student> students = new List<Student> {};
      foreach (int studentId in studentIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand studentQuery = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);

        SqlParameter studentIdParameter = new SqlParameter();
        studentIdParameter.ParameterName = "@StudentId";
        studentIdParameter.Value = studentId;
        studentQuery.Parameters.Add(studentIdParameter);

        queryReader = studentQuery.ExecuteReader();
        while(queryReader.Read())
        {
              int thisStudentId = queryReader.GetInt32(0);
              string studentDescription = queryReader.GetString(1);
              DateTime studentDueDate = queryReader.GetDateTime(2);
              Student foundStudent = new Student(studentDescription, studentDueDate, thisStudentId);
              students.Add(foundStudent);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return students;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM courses;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}
