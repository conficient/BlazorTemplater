using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTemplater.Library
{
    public class TestService : ITestService
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
