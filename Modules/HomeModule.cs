using System.Collections.Generic;
using System;
using Nancy;

namespace Registrar
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] =_=> View["index.cshtml", Course.GetAll()];
      Get["/course/new"] =_=> View["course_form.cshtml"];
      Post["course/added"] =_=> {
        Course newCourse = new Course(Request.Form["newName"], Request.Form["newNumber"]);
        newCourse.Save();
        return View["index.cshtml", Course.GetAll()];
      };
      Get["/course/{id}"] =parameters=> {
        Course Model = Course.Find(parameters.id);
        return View["course_detail.cshtml", Model];
      };
      Get["/student/edit"] =_=> View["student_edit.cshtml", Student.GetAll()];
      Get["/student/delete/{id}"] =parameters=> {
        Student.Delete(parameters.id);
        return View["student_edit.cshtml", Student.GetAll()];
      };
      Post["/student/new"] =_=> {
        Student newStudent = new Student(Request.Form["studentName"], Request.Form["studentEnrollment"]);
        newStudent.Save();
        return View["student_edit.cshtml", Student.GetAll()];
      };
      Post["/student/edit/name/{id}"] =parameters=> {
        Student ExistingStudent = Student.Find(parameters.id);
        ExistingStudent.SetName(Request.Form["newName"]);
        ExistingStudent.Update();
        return View["student_edit.cshtml", Student.GetAll()];
      };
      Post["/student/edit/enrollment/{id}"] =parameters=> {
        Student ExistingStudent = Student.Find(parameters.id);
        ExistingStudent.SetEnrollment(Request.Form["newEnrollment"]);
        ExistingStudent.Update();
        return View["student_edit.cshtml", Student.GetAll()];
      };
    }
  }
}
