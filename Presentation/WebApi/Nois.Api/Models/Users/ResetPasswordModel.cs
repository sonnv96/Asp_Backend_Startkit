using FluentValidation;

namespace Nois.Api.Models.Users
{

    #region Models

    /// <summary>
    /// ResetPassword item model
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// ResetPassword identity
        /// </summary>
        public int Id { get; set; }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for ResetPasswordEditModel
    /// </summary>
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("User.IdRequired");
        }
    }

    #endregion
}