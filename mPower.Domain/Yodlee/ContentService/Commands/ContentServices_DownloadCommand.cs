using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Paralect.Domain;
using Paralect.ServiceBus;
using com.yodlee.common;
using com.yodlee.soap.ext.traversal;
using mPower.Domain.Yodlee.Services;
using mPower.Domain.Yodlee.Storage.Documents;
using ContainerInfo = mPower.Domain.Yodlee.Storage.Documents.ContainerInfo;

namespace mPower.Domain.Yodlee.ContentService.Commands
{
    public class ContentServices_DownloadCommand : Command
    {
        
    }

    public class ContentServices_DownloadCommandHandler : BaseYodleeService, IMessageHandler<ContentServices_DownloadCommand>
    {
        private readonly ContentServiceDocumentService _documentService;

        public ContentServices_DownloadCommandHandler(ContentServiceDocumentService documentService)
        {
            _documentService = documentService;
        }


        public void Handle(ContentServices_DownloadCommand message)
        {
            ConnectToYodlee();

            var traversalService = new ContentServiceTraversalService();
            var services = traversalService.getAllContentServices(_cobrandContext).ToList();

            //Save Content Services
            Mapper.CreateMap<List<ContentServiceInfo>, List<ContentServiceDocument>>();
            Mapper.CreateMap<ContentServiceInfo, ContentServiceDocument>();
            Mapper.CreateMap<com.yodlee.common.ContainerInfo, ContainerInfo>();

            Mapper.AssertConfigurationIsValid();

            var docs = services.Select(Mapper.Map<ContentServiceInfo, ContentServiceDocument>).ToList();

            _documentService.InsertMany(docs);
        }
    }
}
