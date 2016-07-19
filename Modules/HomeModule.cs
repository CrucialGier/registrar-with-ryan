using System.Collections.Generic;
using System;
using Nancy;
using ToDoList.Objects;

namespace Registrar
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] =_=> View["index.cshtml"];
    }
  }
}
