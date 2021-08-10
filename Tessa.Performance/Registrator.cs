using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions;
using Tessa.Platform;
using Tessa.Platform.Storage;
using Unity;
using Unity.Lifetime;

namespace Tessa.Performance
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        const string BaseDefaultCardRepository = nameof(BaseDefaultCardRepository);
        const string BaseExtendedCardRepository = nameof(BaseExtendedCardRepository);
        const string BasePlatformCardRepository = nameof(BasePlatformCardRepository);
        const string BaseDefaultWTCardRepository = nameof(BaseDefaultWTCardRepository);
        const string BaseExtendedWTCardRepository = nameof(BaseExtendedWTCardRepository);
        const string BasePlatformWTCardRepository = nameof(BasePlatformWTCardRepository);

        public override void RegisterUnity()
        {
            var defaultCardRepository = this.UnityContainer.Resolve<ICardRepository>(CardRepositoryNames.Default);
            var extendedCardRepository = this.UnityContainer.Resolve<ICardRepository>(CardRepositoryNames.Extended);
            var platformCardRepository = this.UnityContainer.Resolve<ICardRepository>(CardRepositoryNames.Platform);
            var defaultWTCardRepository = this.UnityContainer.Resolve<ICardRepository>(CardRepositoryNames.DefaultWithoutTransaction);
            var extendedWTCardRepository = this.UnityContainer.Resolve<ICardRepository>(CardRepositoryNames.ExtendedWithoutTransaction);
            var platformWTCardRepository = this.UnityContainer.Resolve<ICardRepository>(CardRepositoryNames.PlatformWithoutTransaction);

            this.UnityContainer
                .RegisterInstance(BaseDefaultCardRepository, defaultCardRepository)
                .RegisterInstance(BaseExtendedCardRepository, extendedCardRepository)
                .RegisterInstance(BasePlatformCardRepository, platformCardRepository)
                .RegisterInstance(BaseDefaultWTCardRepository, defaultWTCardRepository)
                .RegisterInstance(BaseExtendedWTCardRepository, extendedWTCardRepository)
                .RegisterInstance(BasePlatformWTCardRepository, platformWTCardRepository)

                .RegisterFactory<ICardRepository>(
                    CardRepositoryNames.Default,
                    c => new PerformanceCardRepository(c.Resolve<ICardRepository>(BaseDefaultCardRepository)),
                    new ContainerControlledLifetimeManager())

                .RegisterFactory<ICardRepository>(
                    CardRepositoryNames.Extended,
                    c => new PerformanceCardRepository(c.Resolve<ICardRepository>(BaseExtendedCardRepository)),
                    new ContainerControlledLifetimeManager())

                .RegisterFactory<ICardRepository>(
                    CardRepositoryNames.Platform,
                    c => new PerformanceCardRepository(c.Resolve<ICardRepository>(BasePlatformCardRepository)),
                    new ContainerControlledLifetimeManager())

                .RegisterFactory<ICardRepository>(
                    CardRepositoryNames.DefaultWithoutTransaction,
                    c => new PerformanceCardRepository(c.Resolve<ICardRepository>(BaseDefaultWTCardRepository)),
                    new ContainerControlledLifetimeManager())

                .RegisterFactory<ICardRepository>(
                    CardRepositoryNames.ExtendedWithoutTransaction,
                    c => new PerformanceCardRepository(c.Resolve<ICardRepository>(BaseExtendedWTCardRepository)),
                    new ContainerControlledLifetimeManager())

                .RegisterFactory<ICardRepository>(
                    CardRepositoryNames.PlatformWithoutTransaction,
                    c => new PerformanceCardRepository(c.Resolve<ICardRepository>(BasePlatformWTCardRepository)),
                    new ContainerControlledLifetimeManager());

            var defaultExtensionContainer = this.UnityContainer.Resolve<IExtensionContainer>();
            var platformExtensionContainer = this.UnityContainer.Resolve<IExtensionContainer>(ExtensionContainerNames.Platform);

            var configuration = this.UnityContainer.Resolve<IConfigurationManager>();
            var traceThreshold = (int)configuration.Configuration.Settings.TryGet(PerformanceExtensionTraceListener.TraceThresholdParamName, -1L);
            if (traceThreshold >= 0)
            {
                var listener = new PerformanceExtensionTraceListener(traceThreshold);

                defaultExtensionContainer
                    .RegisterTraceListener<ICardNewExtension>(listener)
                    .RegisterTraceListener<ICardGetExtension>(listener)
                    .RegisterTraceListener<ICardStoreExtension>(listener)
                    .RegisterTraceListener<ICardStoreTaskExtension>(listener);

                platformExtensionContainer
                    .RegisterTraceListener<ICardNewExtension>(listener)
                    .RegisterTraceListener<ICardGetExtension>(listener)
                    .RegisterTraceListener<ICardStoreExtension>(listener)
                    .RegisterTraceListener<ICardStoreTaskExtension>(listener);
            }
        }
    }
}
