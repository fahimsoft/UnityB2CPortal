using B2CPortal.Interfaces;
using B2CPortal.Services;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace B2CPortal
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<ICity, CityService>();
            container.RegisterType<ITest, TestService>();
            container.RegisterType<IUser, UserService>();
            container.RegisterType<IProductMaster, ProductMasterService>();
            container.RegisterType<IProductDetail, ProductDetailService>();
            container.RegisterType<IAccount, AccountServices>();
            container.RegisterType<ICart, CartService>();
            container.RegisterType<IOrders, OrderServices>();
            container.RegisterType<IOrderDetail, OrderDetailServices>();
            container.RegisterType<IShippingDetails, ShippingDetailsService>();
            container.RegisterType<IOrderTransection, OrderTransectionService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}