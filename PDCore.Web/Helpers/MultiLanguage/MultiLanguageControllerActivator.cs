using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace PDCore.Web.Helpers.MultiLanguage
{
    public class MultiLanguageControllerActivator : IControllerActivator 
    { 
        public IController Create(RequestContext requestContext, Type controllerType) 
        { 
            //LanguageHelper.SetLanguage(requestContext.HttpContext.Request); 

            return DependencyResolver.Current.GetService(controllerType) as IController; 
        } 
    }
}
