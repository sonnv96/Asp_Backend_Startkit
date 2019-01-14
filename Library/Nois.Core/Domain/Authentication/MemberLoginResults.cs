
namespace Nois.Core.Domain.Authentication
{
    /// <summary>
    /// Represents the customer login result enumeration
    /// </summary>
    public enum MemberLoginResults
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,
        /// <summary>
        ///  Member dies not exist (email or username)
        /// </summary>
        MemberNotExist = 2,
        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword = 3,
        /// <summary>
        /// Account have not been activated
        /// </summary>
        NotActive = 4,
        /// <summary>
        ///  Member has been deleted 
        /// </summary>
        Deleted = 5
    }
}
