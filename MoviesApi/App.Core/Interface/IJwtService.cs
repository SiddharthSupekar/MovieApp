using App.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface
{
    public interface IJwtService
    {
        public string GenerateToken(TokenDto dto, string apiKey);
    }
}
