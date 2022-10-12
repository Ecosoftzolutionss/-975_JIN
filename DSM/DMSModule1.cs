using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace DSM
{
    public class DMSModule : IModule
    {
        private IRegionManager regionManager;
        private IUnityContainer container;

        public DMSModule(IRegionManager regionManager, IUnityContainer uContainer)
        {
            this.regionManager = regionManager;
            this.container = uContainer;
        }

        public void Initialize()
        {
            if (Global.UserType == "User")
            {
                regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.DashBoardView));
            }
            else if (Global.UserType == "Api")
            {
                regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.DashBoardView));
            }
            else
            {
                regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.MasterView));

                //container.RegisterTypeForNavigation<Views.DashBoardView>();

                //regionManager.RequestNavigate("ChildRegion", "DashBoardView");
            }
        }
    }
}
