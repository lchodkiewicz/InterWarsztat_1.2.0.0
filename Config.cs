using System;

public static class Config
{

    dictionary<int, string> userType = new dictionary<int, string>()
        {
           {1, "admin"},
           {2, "customer"},
           {3, "employee"}

        };

}
