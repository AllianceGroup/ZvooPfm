using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using StructureMap;

namespace mPower.Framework.Environment
{
    public class StructureMapFilterProvider : FilterAttributeFilterProvider
    {
        public StructureMapFilterProvider(IContainer container)
        {
            _container = container;
        }

        private IContainer _container;

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(
               ControllerContext controllerContext,
               ActionDescriptor actionDescriptor)
        {

            var attributes = base.GetControllerAttributes(controllerContext,
                                                          actionDescriptor);
            foreach (var attribute in attributes)
            {
                _container.BuildUp(attribute);
            }

            return attributes;
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(
                    ControllerContext controllerContext,
                    ActionDescriptor actionDescriptor)
        {

            var attributes = base.GetActionAttributes(controllerContext,
                                                      actionDescriptor);
            foreach (var attribute in attributes)
            {
                _container.BuildUp(attribute);
            }

            return attributes;
        }
    }
}
