using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Registrar
{
  public class CourseTest : IDisposable
  {
    public CourseTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=registrar_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_CoursesEmptyAtFirst()
    {
      //Arrange, Act
      int result = Course.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Course firstCourse = new Course("History" , "HIST101");
      Course secondCourse = new Course("History" , "HIST101");

      //Assert
      Assert.Equal(firstCourse, secondCourse);
    }

    [Fact]
    public void Test_Save_SavesCourseToDatabase()
    {
      //Arrange
      Course testCourse = new Course("History" , "HIST101");
      testCourse.Save();

      //Act
      List<Course> result = Course.GetAll();
      List<Course> testList = new List<Course>{testCourse};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToCourseObject()
    {
      //Arrange
      Course testCourse = new Course("History" , "HIST101");
      testCourse.Save();

      //Act
      Course savedCourse = Course.GetAll()[0];

      int result = savedCourse.GetId();
      int testId = testCourse.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_AddStudent_AddsStudentToCourse()
    {
      Course testCourse = new Course("History" , "HIST101");
      testCourse.Save();

      Student testStudent = new Student("Mow the Lawn", new DateTime(2014, 4, 21));
      testStudent.Save();

      Student testStudent2 = new Student("Water the garden", new DateTime(2014, 4, 21));
      testStudent2.Save();

      testCourse.AddStudent(testStudent);
      testCourse.AddStudent(testStudent2);

      List<Student> result = testCourse.GetStudents();
      List<Student> testList = new List<Student>{testStudent, testStudent2};

      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetStudents_RetrievesAllCourseStudents()
    {
      Course testCourse = new Course("History" , "HIST101");
      testCourse.Save();

      Student testStudent1 = new Student("Mow the lawn", new DateTime(2014, 4, 21));
      testStudent1.Save();

      Student testStudent2 = new Student("Buy plane ticket", new DateTime(2014, 4, 21));
      testStudent2.Save();

      testCourse.AddStudent(testStudent1);
      List<Student> savedStudents = testCourse.GetStudents();
      List<Student> testList = new List<Student> {testStudent1};

      Assert.Equal(testList, savedStudents);
    }

    [Fact]
    public void Test_Find_FindsCourseInDatabase()
    {
      //Arrange
      Course testCourse = new Course("History" , "HIST101");
      testCourse.Save();

      //Act
      Course foundCourse = Course.Find(testCourse.GetId());

      //Assert
      Assert.Equal(testCourse, foundCourse);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();
    }
  }
}
