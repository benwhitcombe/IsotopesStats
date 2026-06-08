using System;
using System.Collections.Generic;
using Supabase.Functions;

class Program
{
    static void Main()
    {
        Client c = null;
        c.Invoke("abc", 123, 456, 789);
    }
}
