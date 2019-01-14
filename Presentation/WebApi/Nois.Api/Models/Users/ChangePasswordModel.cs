using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Nois.Api.Models.Users
{
    /// <summary>
    /// change password model
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Current password, minimum length: 6
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// New password, minimum length: 6
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// confirm password
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
    #region Validators

    /// <summary>
    /// Validate for SupplierEditModel
    /// </summary>
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required");
            RuleFor(x => x.NewPassword).Matches(x => x.ConfirmPassword).WithMessage("The password and confirmation password do not match.");
        }
    }

    #endregion
}