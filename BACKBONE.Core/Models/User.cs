﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string EmpCode { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
    }

}
