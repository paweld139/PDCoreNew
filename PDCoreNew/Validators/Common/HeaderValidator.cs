namespace PDCoreNew.Validators.Common
{
    public abstract class HeaderValidator<TSheetModel> : Validator<TSheetModel>
    {
        protected abstract char CellChar { get; }

        protected abstract string ExpectedHeaderName { get; }

        protected abstract string ExpectedHeaderNameTranslationKey { get; }
    }
}
