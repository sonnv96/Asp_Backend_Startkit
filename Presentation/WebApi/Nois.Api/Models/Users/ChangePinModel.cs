using FluentValidation;

namespace Nois.Api.Models.Users
{
    /// <summary>
    /// change pin
    /// </summary>
    public class ChangePinModel
    {
        /// <summary>
        /// Current pin, minimum length: 6
        /// </summary>
        public string CurrentPin { get; set; }

        /// <summary>
        /// New pin, minimum length: 6
        /// </summary>
        public string NewPin { get; set; }
        /// <summary>
        /// confirm pin
        /// </summary>
        public string ConfirmPin { get; set; }
    }
    #region Validators

    /// <summary>
    /// Validate for SupplierEditModel
    /// </summary>
    public class ChangePinValidator : AbstractValidator<ChangePinModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public ChangePinValidator()
        {
            RuleFor(x => x.CurrentPin).NotEmpty().WithMessage("Current pin is required");
            RuleFor(x => x.NewPin).NotEmpty().WithMessage("New pin is required");
            RuleFor(x => x.ConfirmPin).NotEmpty().WithMessage("Confirm pin is required");
            RuleFor(x => x.NewPin).Matches(x => x.ConfirmPin).WithMessage("The pin and confirmation pin do not match.");
        }
    }

    #endregion
}