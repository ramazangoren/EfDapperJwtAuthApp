using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_cSharp.DTOs
{
    public partial class UserForLoginConfirmationDto
    {
        byte[] PasswordHash {get; set;}
        byte[] PasswordSalt {get; set;}

        public UserForLoginConfirmationDto()
        {
            // PasswordHash ??= new byte[0];
            if (PasswordHash == null)
            {
                PasswordHash = new byte[0];
            }
            if (PasswordSalt == null)
            {
                PasswordSalt = new byte[0];
            }
        }
    }
}