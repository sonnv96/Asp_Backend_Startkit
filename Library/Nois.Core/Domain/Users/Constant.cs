using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.Core.Domain.Users
{
    public static class Constant
    {
        public const string DefaultPassword = "123456";
        public const string DefaultPIN = "1234";
        public const string PasswordSalt = "123456";

        /// <summary>
        /// DashbroadFilter
        /// </summary>
        public enum DashbroadFilter
        {
            TD = 0,
            YD = 1,
            TW = 2,
            LW = 3,
        }

        public enum Remark
        {
            None = 0,
            WrongSortedCode = 1,
            WrongDimentions = 2
        }
    }
}
