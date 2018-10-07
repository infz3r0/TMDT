using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace TMDT.Controllers
{
    public class Manager
    {
        public static bool LoggedAsAdmin()
        {
            HttpSessionState session = HttpContext.Current.Session;
            if (session != null)
            {
                if (session["Account"] != null && session["Role"] != null)
                {
                    if (((string)session["Role"]).Equals("Admin"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}