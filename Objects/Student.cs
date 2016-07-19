using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Registrar
{
  public class Student
  {
    private int _id;
    private string _name;
    private DateTime _dueDate = new DateTime();

    public Student(string Name, DateTime DueDate, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _dueDate = DueDate;
    }

    public override bool Equals(System.Object otherStudent)
    {
        if (!(otherStudent is Student))
        {
          return false;
        }
        else
        {
          Student newStudent = (Student) otherStudent;
          bool idEquality = this.GetId() == newStudent.GetId();
          bool nameEquality = this.GetName() == newStudent.GetName();
          bool dueEquality = this.GetDueDate() == newStudent.GetDueDate();
          return (idEquality && nameEquality && dueEquality);
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
    public DateTime GetDueDate()
    {
      return _dueDate;
    }
    public void SetDueDate(DateTime newDueDate)
    {
      _dueDate = newDueDate;
    }
  public static List<Student> GetAll()
  {
    List<Student> AllStudents = new List<Student>{};

    SqlConnection conn = DB.Connection();
    SqlDataReader rdr = null;
    conn.Open();

    SqlCommand cmd = new SqlCommand("SELECT * FROM students ORDER BY enrollment_date;", conn);
    rdr = cmd.ExecuteReader();

    while(rdr.Read())
    {
      int studentId = rdr.GetInt32(0);
      string studentName = rdr.GetString(1);
      DateTime studentDueDate = rdr.GetDateTime(2);
      Student newStudent = new Student(studentName, studentDueDate, studentId);
      AllStudents.Add(newStudent);
    }
    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
    return AllStudents;
  }
  public void Save()
  {
    SqlConnection conn = DB.Connection();
    SqlDataReader rdr;
    conn.Open();

    SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@StudentName, @StudentDueDate);", conn);

    SqlParameter nameParameter = new SqlParameter();
    nameParameter.ParameterName = "@StudentName";
    nameParameter.Value = this.GetName();

    SqlParameter dueDateParameter = new SqlParameter();
    dueDateParameter.ParameterName = "@StudentDueDate";
    dueDateParameter.Value = this.GetDueDate();

    cmd.Parameters.Add(nameParameter);
    cmd.Parameters.Add(dueDateParameter);

    rdr = cmd.ExecuteReader();

    while(rdr.Read())
    {
      this._id = rdr.GetInt32(0);
    }
    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
  }

  public static Student Find(int id)
  {
    SqlConnection conn = DB.Connection();
    SqlDataReader rdr = null;
    conn.Open();

    SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);
    SqlParameter studentIdParameter = new SqlParameter();
    studentIdParameter.ParameterName = "@StudentId";
    studentIdParameter.Value = id.ToString();
    cmd.Parameters.Add(studentIdParameter);
    rdr = cmd.ExecuteReader();

    int foundStudentId = 0;
    string foundStudentName = null;
    DateTime foundStudentDueDate = new DateTime(0);

    while(rdr.Read())
    {
      foundStudentId = rdr.GetInt32(0);
      foundStudentName = rdr.GetString(1);
      foundStudentDueDate = rdr.GetDateTime(2);
    }
    Student foundStudent = new Student(foundStudentName, foundStudentDueDate, foundStudentId);

    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
    return foundStudent;
  }

  public void AddCourse(Course newCourse)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId);", conn);

      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = newCourse.GetId();
      cmd.Parameters.Add(courseIdParameter);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Course> GetCourses()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT course_id FROM courses_students WHERE student_id = @StudentId;", conn);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> courseIds = new List<int> {};

      while (rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        courseIds.Add(courseId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Course> courses = new List<Course> {};

      foreach (int courseId in courseIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand courseQuery = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);

        SqlParameter courseIdParameter = new SqlParameter();
        courseIdParameter.ParameterName = "@CourseId";
        courseIdParameter.Value = courseId;
        courseQuery.Parameters.Add(courseIdParameter);

        queryReader = courseQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisCourseId = queryReader.GetInt32(0);
          string courseName = queryReader.GetString(1);
          string courseNumber = queryReader.GetString(2);
          Course foundCourse = new Course(courseName, courseNumber, thisCourseId);
          courses.Add(foundCourse);
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
      return courses;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students;", conn);
      cmd.ExecuteNonQuery();
    }


  }
}
