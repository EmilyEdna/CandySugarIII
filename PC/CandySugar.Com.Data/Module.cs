using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data
{
    public class Module
    {
        public static List<Type> Services => new List<Type>
        {
            typeof(AxgleService)
        };
    }
}
