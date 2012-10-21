using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.UI.Resources;
using Vandelay.Industries.Models;
using Orchard.ContentManagement.Handlers;

namespace Vandelay.Industries.Drivers {
    [OrchardFeature("Vandelay.Industries")]
    public class MetaDriver : ContentPartDriver<MetaPart> {
        private readonly IWorkContextAccessor _wca;

        public MetaDriver(IWorkContextAccessor workContextAccessor) {
            _wca = workContextAccessor;
        }

        protected override DriverResult Display(MetaPart part, string displayType, dynamic shapeHelper) {
            if (displayType != "Detail") return null;
            var resourceManager = _wca.GetContext().Resolve<IResourceManager>();
            if (!String.IsNullOrWhiteSpace(part.Description)) {
                resourceManager.SetMeta(new MetaEntry {
                    Name = "description",
                    Content = part.Description
                });
            }
            if (!String.IsNullOrWhiteSpace(part.Keywords)) {
                resourceManager.SetMeta(new MetaEntry {
                    Name = "keywords",
                    Content = part.Keywords
                });
            }
            return null;
        }

        //GET
        protected override DriverResult Editor(MetaPart part, dynamic shapeHelper) {

            return ContentShape("Parts_Meta_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Meta",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(MetaPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(MetaPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Keywords", part.Keywords);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
        }

        protected override void Importing(MetaPart part, ImportContentContext context)
        {
            var keywords = context.Attribute(part.PartDefinition.Name, "Keywords");
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                part.Keywords = keywords;
            }
            var description = context.Attribute(part.PartDefinition.Name, "Description");
            if (!String.IsNullOrWhiteSpace(description))
            {
                part.Description = description;
            }
        }
    }
}