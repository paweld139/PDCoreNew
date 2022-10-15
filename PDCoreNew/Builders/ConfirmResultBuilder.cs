using Microsoft.Extensions.Localization;
using PDCoreNew.Models.Results;
using System;

namespace PDCoreNew.Builders
{
    public class ConfirmResultBuilder : Builder<ConfirmResult>
    {
        private readonly IStringLocalizer stringLocalizer;

        public ConfirmResultBuilder(IStringLocalizer stringLocalizer = null)
        {
            _object = new ConfirmResult(false, null, null, null, null);

            this.stringLocalizer = stringLocalizer;
        }

        private ConfirmResultBuilder ExecuteAndReturn(Action action)
        {
            action();

            return this;
        }

        private string GetTranslated(string input)
        {
            if (stringLocalizer != null)
            {
                input = stringLocalizer[input];
            }

            return input;
        }

        public ConfirmResultBuilder WithConfirm(string yes, string no) => ExecuteAndReturn(() =>
        {
            _object.IsConfirm = true;
            _object.Yes = GetTranslated(yes);
            _object.No = GetTranslated(no);
        });

        public ConfirmResultBuilder WithMessage(string message) => ExecuteAndReturn(() => _object.Message = GetTranslated(message));

        public ConfirmResultBuilder WithTitle(string title) => ExecuteAndReturn(() => _object.Title = GetTranslated(title));

        public ConfirmResultBuilder WithTitleAndMessage(string title, string message) => WithTitle(title).WithMessage(message);
    }
}
