using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Models.ViewModels.Nrls;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class DocumentReferenceFactoryExt
    {
        //TODO : Set up test
        public static PointerViewModel ToViewModel(this DocumentReference documentReference)
        {
            var viewModel = new PointerViewModel
            {
                Content = documentReference.Content.ToContentViewModelList(),
                Created = documentReference.CreatedElement?.ToDateTimeOffset(),
                Custodian = documentReference.Custodian.ToViewModel(),
                Id = documentReference.Id,
                Status = documentReference.Status?.ToString(),
                Identifier = documentReference.Identifier.ToViewModelList(),
                Subject = documentReference.Subject.ToViewModel(),
                Type = documentReference.Type.ToViewModel(),
                Author = documentReference.Author.FirstOrDefault()?.ToViewModel(),
                Indexed = documentReference.Indexed
            };

            return viewModel;
        }

        private static List<ContentViewModel> ToContentViewModelList(this List<DocumentReference.ContentComponent> contentComponents)
        {
            var viewModels = new List<ContentViewModel>();

            foreach (var contentComponent in contentComponents)
            {
                viewModels.Add(contentComponent.ToContentViewModel());
            }

            return viewModels;
        }

        private static ContentViewModel ToContentViewModel(this DocumentReference.ContentComponent contentComponent)
        {
            var attachment = contentComponent.Attachment;
            var viewModel = new ContentViewModel
            {
                Attachment = new AttachmentViewModel
                {
                    ContentType = attachment.ContentType,
                    Creation = attachment.CreationElement?.ToDateTimeOffset(),
                    Title = attachment.Title,
                    Url = attachment.Url
                }
            };

            return viewModel;
        }
    }
}
