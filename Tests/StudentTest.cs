using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Registrar
{
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Student.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueIfDescriptionsAreTheSame()
    {
      //Arrange, Act
      Student firstStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      Student secondStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));

      //Assert
      Assert.Equal(firstStudent, secondStudent);
    }

    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));

      //Act
      testStudent.Save();
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));

      //Act
      testStudent.Save();
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsStudentInDatabase()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      testStudent.Save();

      //Act
      Student foundStudent = Student.Find(testStudent.GetId());

      //Assert
      Assert.Equal(testStudent, foundStudent);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Student firstStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      Student secondStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));

      //Assert
      Assert.Equal(firstStudent, secondStudent);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      testStudent.Save();

      //Act
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      testStudent.Save();

      //Act
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsStudentInDatabase()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      testStudent.Save();

      //Act
      Student foundStudent = Student.Find(testStudent.GetId());

      //Assert
      Assert.Equal(testStudent, foundStudent);
    }

    [Fact]
    public void Test_AddCourse_AddsCourseToStudent()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      testStudent.Save();

      Course testCourse = new Course("History" , "HIST101");
      testCourse.Save();

      //Act
      testStudent.AddCourse(testCourse);

      List<Course> result = testStudent.GetCourses();
      List<Course> testList = new List<Course>{testCourse};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetCourses_ReturnsAllStudentCourses()
    {
      //Arrange
      Student testStudent = new Student("Mow the lawn", new DateTime(2015, 1, 18));
      testStudent.Save();

      Course testCourse1 = new Course("History" , "HIST101");
      testCourse1.Save();

      Course testCourse2 = new Course("Work stuff", "Career101");
      testCourse2.Save();

      //Act
      testStudent.AddCourse(testCourse1);
      List<Course> result = testStudent.GetCourses();
      List<Course> testList = new List<Course> {testCourse1};

      //Assert
      Assert.Equal(testList, result);
    }

    public void Dispose()
    {
      Student.DeleteAll();
    }
  }
}
