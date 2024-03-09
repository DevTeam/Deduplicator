using Build;

DI.Setup(nameof(Composition))
    .Root<RootTarget>("RootTarget")
    
    .DefaultLifetime(Lifetime.PerBlock)
    
    .Bind().To<RootCommand>()
    
    // Targets
    .Bind(Tag.Type).To<PublishTarget>();

return await new Composition().RootTarget.RunAsync(CancellationToken.None);