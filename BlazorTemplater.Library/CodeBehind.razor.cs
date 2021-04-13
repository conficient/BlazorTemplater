using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTemplater.Library
{
    public partial class CodeBehind : ComponentBase
    {
        [Parameter] public string Name { get; set; }
        [Parameter] public string App { get; set; }
    }
}
