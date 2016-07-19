using System.Collections.Generic;
using System;
using Nancy;

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
