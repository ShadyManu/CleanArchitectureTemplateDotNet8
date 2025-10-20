using Carter;

namespace Presentation.Extensions;

public abstract class ApiModule(string basePath) : CarterModule($"/api{basePath}");