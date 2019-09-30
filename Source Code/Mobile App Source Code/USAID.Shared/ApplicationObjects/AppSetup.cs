using System;
using Autofac;
using USAID.Interfaces;
using USAID.Managers;
using USAID.Repositories;
using USAID.Respositories.Impl;
using USAID.Services;
using USAID.ViewModels;

namespace USAID.ApplicationObjects
{
	public class AppSetup
	{
		public IContainer CreateContainer()
		{
			var containerBuilder = new ContainerBuilder();
			RegisterDependencies(containerBuilder);

			return containerBuilder.Build();
		}

		protected virtual void RegisterDependencies(ContainerBuilder cb)
		{
            // Register Managers
            cb.RegisterType<AuthenticationManager>().As<IAuthenticationManager>().SingleInstance(); // this must be a singleton so services can access the auth token
           
            // Register Services
            cb.RegisterType<AuthenticationService>().As<IAuthenticationService>();
			cb.RegisterType<ObservationService>().As<IObservationService>();
           
			// Register View Models
            cb.RegisterType<LoginViewModel>();
			cb.RegisterType<LandingViewModel>();
			cb.RegisterType<SitesViewModel>();
			cb.RegisterType<AboutUsViewModel>();
			cb.RegisterType<SiteViewModel>();
			cb.RegisterType<ActivitiesViewModel>();
			cb.RegisterType<ChartViewModel>();
			cb.RegisterType<ObservationViewModel> ();
			cb.RegisterType<ActivityViewModel> ();
			cb.RegisterType<ChangeViewModel>();
			cb.RegisterType<CommentViewModel>();
			cb.RegisterType<AttachmentViewModel>();

            // Register Repositories
			cb.RegisterType<ObservationRepository>().As<IObservationRepository>();
			cb.RegisterType<SiteRepository>().As<ISiteRepository>();
			cb.RegisterType<ActivityRepository>().As<IActivityRepository>();
			cb.RegisterType<IndicatorRepository>().As<IIndicatorRepository>();
			cb.RegisterType<SiteIndicatorRepository>().As<ISiteIndicatorRepository>();
			cb.RegisterType<ObservationEntryRepository>().As<IObservationEntryRepository>();
			cb.RegisterType<ObservationChangeRepository>().As<IObservationChangeRepository>();
			cb.RegisterType<ObservationCommentRepository>().As<IObservationCommentRepository>();
			cb.RegisterType<ObservationAttachmentRepository>().As<IObservationAttachmentRepository>();
			cb.RegisterType<IndicatorAgeRepository>().As<IIndicatorAgeRepository>();


            // Register Builders
   //         cb.RegisterType<CreditAppBuilder>().As<ICreditAppBuilder>().SingleInstance();
			//cb.RegisterType<QuoteBuilder>().As<IQuoteBuilder>().SingleInstance();
		}
	}
}

