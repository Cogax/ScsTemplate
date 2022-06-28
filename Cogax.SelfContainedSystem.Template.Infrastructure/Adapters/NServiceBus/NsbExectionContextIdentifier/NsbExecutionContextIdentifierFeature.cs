using NServiceBus;
using NServiceBus.Features;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

public class NsbExecutionContextIdentifierFeature : Feature
{
    private NsbExecutionContextIdentifierCurrentSessionHolder sessionHolder = new();

    protected override void Setup(FeatureConfigurationContext context)
    {
        context.Pipeline.Register(new NsbExecutionContextIdentifierBehavior(sessionHolder), "Enabled identification of a NSB Message Handler Execution Context");
        context.Container.ConfigureComponent(() => sessionHolder.Identifier, DependencyLifecycle.InstancePerCall);
    }
}
