using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakDemo.controller
{
    //[Authorize(Policy = "Administrator")]
    [Authorize]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        public String Index()
        {
            return "Hello Keycloak :)";
        }
    }
}
