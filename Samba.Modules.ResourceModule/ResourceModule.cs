﻿using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using Samba.Domain.Models.Resources;
using Samba.Localization.Properties;
using Samba.Modules.ResourceModule.WidgetCreators;
using Samba.Presentation.Common;
using Samba.Presentation.Common.Interaction;
using Samba.Presentation.Common.ModelBase;
using Samba.Presentation.Common.Widgets;
using Samba.Services;
using Samba.Services.Common;

namespace Samba.Modules.ResourceModule
{
    [ModuleExport(typeof(ResourceModule))]
    class ResourceModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ResourceEditorView _resourceEditorView;
        private readonly ResourceSwitcherView _resourceSwitcherView;

        [ImportingConstructor]
        public ResourceModule(IRegionManager regionManager,
            IUserService userService,
            ResourceSwitcherView resourceSwitcherView,
            ResourceEditorView resourceEditorView)
        {
            _resourceSwitcherView = resourceSwitcherView;
            _resourceEditorView = resourceEditorView;
            _regionManager = regionManager;

            AddDashboardCommand<EntityCollectionViewModelBase<ResourceViewModel, Resource>>(string.Format(Resources.List_f, Resources.Resource), Resources.Resourceses, 40);
            AddDashboardCommand<EntityCollectionViewModelBase<ResourceTemplateViewModel, ResourceTemplate>>(string.Format(Resources.List_f, Resources.ResourceTemplate), Resources.Resourceses, 40);
            AddDashboardCommand<EntityCollectionViewModelBase<ResourceStateViewModel, ResourceState>>(string.Format(Resources.List_f, Resources.ResourceState), Resources.Resourceses, 40);
            AddDashboardCommand<EntityCollectionViewModelBase<ResourceScreenViewModel, ResourceScreen>>(string.Format(Resources.List_f, Resources.ResourceScreen), Resources.Resourceses, 41);

            PermissionRegistry.RegisterPermission(PermissionNames.OpenLocations, PermissionCategories.Navigation, Resources.CanOpenLocationList);
            PermissionRegistry.RegisterPermission(PermissionNames.ChangeLocation, PermissionCategories.Ticket, Resources.CanChangeLocation);

           // WidgetCreatorRegistry.RegisterWidgetCreator(new ResourceButtonWidgetCreator());

        }

        protected override void OnInitialization()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(ResourceSwitcherView));
            _regionManager.RegisterViewWithRegion(RegionNames.ResourceScreenRegion, typeof(ResourceSelectorView));
            _regionManager.RegisterViewWithRegion(RegionNames.ResourceScreenRegion, typeof(ResourceSearchView));
            _regionManager.RegisterViewWithRegion(RegionNames.ResourceScreenRegion, typeof(ResourceEditorView));
            _regionManager.RegisterViewWithRegion(RegionNames.ResourceScreenRegion, typeof (ResourceDashboardView));

            EventServiceFactory.EventService.GetEvent<GenericEvent<EntityOperationRequest<Resource>>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.SelectResource) ActivateResourceSwitcher();
                if (x.Topic == EventTopicNames.EditResourceDetails) ActivateResourceEditor();
            });

            EventServiceFactory.EventService.GetEvent<GenericEvent<EntityOperationRequest<AccountData>>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.SelectResource) ActivateResourceSwitcher();
            });

            EventServiceFactory.EventService.GetEvent<GenericEvent<PopupData>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.PopupClicked && x.Value.EventMessage == EventTopicNames.SelectResource)
                {
                    ActivateResourceSwitcher();
                }
            });
        }

        private void ActivateResourceEditor()
        {
            _regionManager.Regions[RegionNames.ResourceScreenRegion].Activate(_resourceEditorView);
        }

        private void ActivateResourceSwitcher()
        {
            _regionManager.Regions[RegionNames.MainRegion].Activate(_resourceSwitcherView);
        }


    }
}