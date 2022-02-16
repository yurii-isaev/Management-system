﻿using System.Collections.Generic;

namespace WebAPI.Utils.Constants
{
    public static class RoleNameTypes
    {
        public const string Employee = "Employee";
        public const string Manager = "Manager";
        public const string Administrator = "Administrator";
        
        public static IEnumerable<string> AllRoles
        {
            get
            {
                yield return Employee;
                yield return Manager;
                yield return Administrator;
            }
        }
    }
}