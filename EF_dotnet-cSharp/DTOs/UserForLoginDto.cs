using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_cSharp.DTOs
{
    public partial class UserForLoginDto
    {
        string Email {get; set;} = "";
        string Password {get; set;} = "";
    }
}