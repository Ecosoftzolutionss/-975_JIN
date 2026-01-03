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
            // Determine which view to show based on user type
            else if (Global.UserType.ToUpper() == "ADMIN")
            {
                // Register views for Admin
                regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.MasterView));
                regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.DSMasterView));
            }
            else if (Global.UserType.ToUpper() == "REPORT")
            {
                // Register views for Report
                regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.MasterView));
                regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.ReportView));
            }
            else
            {
                regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.MasterView));
                regionManager.RegisterViewWithRegion("ChildRegion", typeof(Views.DSMasterView));
                //container.RegisterTypeForNavigation<Views.DashBoardView>();

                //regionManager.RequestNavigate("ChildRegion", "DashBoardView");
            }
        }
    }
}
