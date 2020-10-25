using System;
using System.Web.Mvc;

namespace PDCore.Web.Helpers.MultiLanguage
{
    public class MultiLanguageController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        { 
            //var languageHelper = new LanguageHelper();

            //languageHelper.SetLanguage(Request);

            return base.BeginExecuteCore(callback, state);
        }
    }
}
