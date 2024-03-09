// ReSharper disable StringLiteralTypo
// ReSharper disable HeapView.DelegateAllocation
// ReSharper disable HeapView.ClosureAllocation
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable InvertIf

namespace Build;

internal class PublishTarget(
    Commands commands)
    : IInitializable, ITarget<int>
{
    public Task InitializeAsync() => commands.Register(
        this,
        "Publish app",
        "publish");

    [SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        var projectPath = Path.Combine("src", "DeduplicatorApp");
        var publishPath = Path.Combine(projectPath, "bin");
        var rootPath = Path.Combine(publishPath, "wwwroot");
        var result = await new DotNetPublish()
            .WithProject(projectPath)
            .WithConfiguration("Release")
            .WithOutput(publishPath)
            .RunAsync(cancellationToken: cancellationToken);
        
        // Change the base-tag in index.html from '/' to 'Deduplicator' to match GitHub Pages repository subdirectory
        var indexFile = Path.Combine(rootPath, "index.html");
        var indexContent = await File.ReadAllTextAsync(indexFile, cancellationToken);
        indexContent = indexContent.Replace("""<base href="/" />""", """<base href="/Deduplicator/" />""");
        await File.WriteAllTextAsync(indexFile, indexContent, cancellationToken);
        
        // Copy index.html to 404.html to serve the same file when a file is not found
        File.Copy(indexFile, Path.Combine(rootPath, "404.html"));
        
        // Add .nojekyll file to tell GitHub pages to not treat this as a Jekyll project. (Allow files and folders starting with an underscore)
        await File.AppendAllTextAsync(Path.Combine(rootPath, ".nojekyll"), "", cancellationToken);

        return result ?? 1;
    }
}