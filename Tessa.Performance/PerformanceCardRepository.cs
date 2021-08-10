using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Performance.EventSources;

namespace Tessa.Performance
{
    public sealed class PerformanceCardRepository : ICardRepository
    {
        private readonly ICardRepository baseCardRepository;

        public PerformanceCardRepository(ICardRepository baseCardRepository)
        {
            this.baseCardRepository = baseCardRepository;
        }

        public Task<CardDeleteResponse> DeleteAsync(CardDeleteRequest request, CancellationToken cancellationToken = default)
        {
            return this.baseCardRepository.DeleteAsync(request, cancellationToken);
        }

        public Task<CardGetFileVersionsResponse> GetFileVersionsAsync(CardGetFileVersionsRequest request, CancellationToken cancellationToken = default)
        {
            return this.baseCardRepository.GetFileVersionsAsync(request, cancellationToken);
        }

        public Task<CardResponse> RequestAsync(CardRequest request, CancellationToken cancellationToken = default)
        {
            return this.baseCardRepository.RequestAsync(request, cancellationToken);
        }

        public async Task<CardNewResponse> NewAsync(CardNewRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ServiceType == CardServiceType.Default)
                return await this.baseCardRepository.NewAsync(request, cancellationToken);

            CardRepositoryEventSource.Log.StartNew();

            try
            {
                var result = await this.baseCardRepository.NewAsync(request, cancellationToken);

                if (!result.ValidationResult.IsSuccessful())
                {
                    CardRepositoryEventSource.Log.FailNew();
                }

                return result;
            }
            catch
            {
                CardRepositoryEventSource.Log.FailNew();
                throw;
            }
            finally
            {
                CardRepositoryEventSource.Log.EndNew();
            }
        }

        public async Task<CardGetResponse> GetAsync(CardGetRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ServiceType == CardServiceType.Default)
                return await this.baseCardRepository.GetAsync(request, cancellationToken);

            CardRepositoryEventSource.Log.StartGet();

            try
            {
                var result = await this.baseCardRepository.GetAsync(request, cancellationToken);

                if (!result.ValidationResult.IsSuccessful())
                {
                    CardRepositoryEventSource.Log.FailGet();
                }

                return result;
            }
            catch
            {
                CardRepositoryEventSource.Log.FailGet();
                throw;
            }
            finally
            {
                CardRepositoryEventSource.Log.EndGet();
            }
        }

        public async Task<CardStoreResponse> StoreAsync(CardStoreRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ServiceType == CardServiceType.Default)
                return await this.baseCardRepository.StoreAsync(request, cancellationToken);

            CardRepositoryEventSource.Log.StartStore();

            try
            {
                var result = await this.baseCardRepository.StoreAsync(request, cancellationToken);

                if (!result.ValidationResult.IsSuccessful())
                {
                    CardRepositoryEventSource.Log.FailStore();
                }

                return result;
            }
            catch
            {
                CardRepositoryEventSource.Log.FailStore();
                throw;
            }
            finally
            {
                CardRepositoryEventSource.Log.EndStore();
            }
        }
    }
}
