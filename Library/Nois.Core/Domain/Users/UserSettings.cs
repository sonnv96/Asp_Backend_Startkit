using Nois.Framework.Settings;

namespace Nois.Core.Domain.Users
{
    /// <summary>
    /// User entity
    /// </summary>
    public partial class UserSettings : ISettings
    {
        /// <summary>
        /// get or set Usernames Enabled
        /// </summary>
        public bool UsernamesEnabled { get; set; }
        /// <summary>
        /// get or set Gender Enabled
        /// </summary>
        public bool GenderEnabled { get; set; }
    }
}