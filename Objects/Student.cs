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

    public Student(string Name, DateTime EnrollmentDate, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _dueDate = EnrollmentDate;
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
          bool dueEquality = this.GetEnrollmentDate() == newStudent.GetEnrollmentDate();
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
    public DateTime GetEnrollmentDate()
    {
      return _dueDate;
    }
    public void SetEnrollment(DateTime newEnrollmentDate)
    {
      _dueDate = newEnrollmentDate;
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
      DateTime studentEnrollmentDate = rdr.GetDateTime(2);
      Student newStudent = new Student(studentName, studentEnrollmentDate, studentId);
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

    SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@StudentName, @StudentEnrollmentDate);", conn);

    SqlParameter nameParameter = new SqlParameter();
    nameParameter.ParameterName = "@StudentName";
    nameParameter.Value = this.GetName();

    SqlParameter dueDateParameter = new SqlParameter();
    dueDateParameter.ParameterName = "@StudentEnrollmentDate";
    dueDateParameter.Value = this.GetEnrollmentDate();

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
    DateTime foundStudentEnrollmentDate = new DateTime(0);

    while(rdr.Read())
    {
      foundStudentId = rdr.GetInt32(0);
      foundStudentName = rdr.GetString(1);
      foundStudentEnrollmentDate = rdr.GetDateTime(2);
    }
    Student foundStudent = new Student(foundStudentName, foundStudentEnrollmentDate, foundStudentId);

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

      SqlCommand cmd = new SqlCommand("SELECT courses.* FROM students JOIN courses_students ON(students.id = courses_students.student_id) JOIN courses ON(courses_students.course_id = courses.id) WHERE students.id = @StudentId;", conn);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();

      cmd.Parameters.Add(studentIdParameter);

      rdr = cmd.ExecuteReader();

      List<Course> courses = new List<Course> {};

      while (rdr.Read())
      {
        int thisCourseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        Course foundCourse = new Course(courseName, courseNumber, thisCourseId);
        courses.Add(foundCourse);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return courses;
    }
    public void Update()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE students SET name = @StudentName WHERE id = @QueryId; UPDATE students SET enrollment_date = @StudentEnrollmentDate WHERE id = @QueryId;", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@StudentName";
      nameParameter.Value = this.GetName();

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@StudentEnrollmentDate";
      dueDateParameter.Value = this.GetEnrollmentDate();

      SqlParameter queryIdParameter = new SqlParameter();
      queryIdParameter.ParameterName = "@QueryId";
      queryIdParameter.Value = this.GetId();

      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(dueDateParameter);
      cmd.Parameters.Add(queryIdParameter);

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
    public static void Delete(int QueryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE id = @StudentId;", conn);
      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = QueryId.ToString();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
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
