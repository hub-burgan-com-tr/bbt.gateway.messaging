using bbt.gateway.common.Models;
using bbt.gateway.common.Repositories;
using bbt.gateway.messaging.Api.Pusula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Workers
{
    public class HeaderManager
    {
        public List<Header> Headers = new List<Header>();
        private readonly ITransactionManager _transactionManager;
        private readonly IRepositoryManager _repositoryManager;
 
        public HeaderManager(IRepositoryManager repositoryManager, 
            ITransactionManager transactionManager
            )
        {
            _transactionManager = transactionManager;
            _repositoryManager = repositoryManager;
        }

        public async Task<Header[]> Get(int page, int pageSize)
        {
            Header[] returnValue;
            returnValue = (await _repositoryManager.Headers.GetWithPaginationAsync(page, pageSize)).ToArray();

            return returnValue;
        }

        public async Task<Header> Get(MessageContentType contentType, HeaderInfo headerInfo)
        {
            Header header = null;

            if (headerInfo.Sender != SenderType.AutoDetect)
            {
                _transactionManager.CustomerRequestInfo.BusinessLine = headerInfo.Sender == SenderType.On ? "X" : "B";
                var defaultHeader = new Header();
                defaultHeader.SmsSender = headerInfo.Sender;
                defaultHeader.SmsPrefix = headerInfo.SmsPrefix;
                defaultHeader.SmsSuffix = headerInfo.SmsSuffix;

                return defaultHeader;
            }

            string businessLine = string.IsNullOrEmpty(_transactionManager.CustomerRequestInfo.BusinessLine) ? null : _transactionManager.CustomerRequestInfo.BusinessLine;
            int? branch = _transactionManager.CustomerRequestInfo.BranchCode != 0 ? _transactionManager.CustomerRequestInfo.BranchCode : null;



            header = await get(contentType, businessLine, branch);

            return header;
        }

        public Header Get(common.Models.v2.SmsTypes smsType)
        {

            string businessLine = string.IsNullOrEmpty(_transactionManager.CustomerRequestInfo.BusinessLine) ? null : _transactionManager.CustomerRequestInfo.BusinessLine;
            int? branch = _transactionManager.CustomerRequestInfo.BranchCode != 0 ? _transactionManager.CustomerRequestInfo.BranchCode : null;

            return get(smsType, businessLine, branch);

        }

        public async Task Save(Header header)
        {

            if (header.Id == Guid.Empty)
            {
                header.Id = Guid.NewGuid();
                await _repositoryManager.Headers.AddAsync(header);
            }
            else
            {
                _repositoryManager.Headers.Update(header);
                await _repositoryManager.SaveChangesAsync();
            }
            
        }

        public async Task Delete(Guid id)
        {
            var itemToDelete = new Header { Id = id };
            _repositoryManager.Headers.Remove(itemToDelete);
            await _repositoryManager.SaveChangesAsync();
        }

        private async Task<Header> get(MessageContentType contentType, string businessLine, int? branch)
        {

            var firstPass = (await _repositoryManager.Headers.FindAsync(h => h.BusinessLine == businessLine && h.Branch == branch && h.ContentType == contentType)).FirstOrDefault();
            if (firstPass != null) return firstPass;

            // Check branch first
            var secondPass = (await _repositoryManager.Headers.FindAsync(h => h.BusinessLine == null && h.Branch == branch && h.ContentType == contentType)).FirstOrDefault();
            if (secondPass != null) return secondPass;

            var thirdPass = (await _repositoryManager.Headers.FindAsync(h => h.BusinessLine == businessLine && h.Branch == null && h.ContentType == contentType)).FirstOrDefault();
            if (thirdPass != null) return thirdPass;

            var lastPass = (await _repositoryManager.Headers.FindAsync(h => h.BusinessLine == null && h.Branch == null && h.ContentType == contentType)).FirstOrDefault();
            if (lastPass != null) return lastPass;


            var header = new Header()
            {
                SmsPrefix = "",
                SmsSuffix = "",
                SmsSender = SenderType.Burgan,
            };


            return header;
        }

        private Header get(common.Models.v2.SmsTypes smsType, string businessLine, int? branch)
        {

            /*
            var firstPass = _repositoryManager.Headers.Find(h => h.BusinessLine == businessLine && h.Branch == branch && h.ContentType == contentType).FirstOrDefault();
            if (firstPass != null) return firstPass;

            // Check branch first
            var secondPass = _repositoryManager.Headers.Find(h => h.BusinessLine == null && h.Branch == branch && h.ContentType == contentType).FirstOrDefault();
            if (secondPass != null) return secondPass;

            var thirdPass = _repositoryManager.Headers.Find(h => h.BusinessLine == businessLine && h.Branch == null && h.ContentType == contentType).FirstOrDefault();
            if (thirdPass != null) return thirdPass;

            var lastPass = _repositoryManager.Headers.Find(h => h.BusinessLine == null && h.Branch == null && h.ContentType == contentType).FirstOrDefault();
            if (lastPass != null) return lastPass;
            */

            var header = new Header()
            {
                SmsPrefix = "",
                SmsSuffix = "",

                SmsSender = string.IsNullOrEmpty(_transactionManager.CustomerRequestInfo.BusinessLine) ? SenderType.Burgan :
                (_transactionManager.CustomerRequestInfo.BusinessLine == "X" ? SenderType.On : SenderType.Burgan),
            };

            if (_transactionManager.Sender != common.Models.v2.SenderType.AutoDetect)
            {
                header.SmsSender = (SenderType)_transactionManager.Sender;
            }

            return header;
        }

        private async Task loadHeaders()
        {
            Headers = (await _repositoryManager.Headers.GetAllAsync()).ToList();

        }


    }
}
