# Feature Master
This library can be used to implement feature toggles in an application that uses the (ASP).NET Core hosting model.

![Nuget](https://img.shields.io/nuget/v/FeatureMaster.svg?style=flat-square)


## How to Use

1. Create a feature
2. Configure the default toggle configuration for that feature
3. Configure toggling decisions based on application context
4. Inject feature toggles via DI

### Create a Feature

Create a new class that extends `Feature`. Add one or more toggles with public getters and setters.

```csharp
using FeatureMaster;

public class MyNewFeature : Feature
{
    public bool UseMyNewFeature { get; set; }
}
```

Here I only added one toggle, but you can have multiple toggles per feature. It's your choice.

### Configure Default Toggles

Use the `ConfigureFeatureToggles` helper method to register a feature and configure it.

_Note: I assume that you use IServiceCollection. If that's impossible for you, create an issue with the details._

```csharp
using FeatureMaster;
```

```csharp
public IConfiguration Configuration { get; }

public IHostingEnvironment Environment { get; }

public void ConfigureServices(IServiceCollection services)
{
    services.ConfigureFeatureToggles<MyNewFeature>(feature =>
    {
        // Enable in dev, disable everywhere else
        feature.UseMyNewFeature = Environment.IsDevelopment();
    });
}
```

### Configure Smart Toggles

Sometimes configuration is not enough and you need to enable or disable a feature based on application context.

In this example I bind my feature toggles from appsettings. Then I inspect an HttpContext and enable my new feature if the user is a beta tester, overriding the value from my appsettings.

```csharp
public IConfiguration Configuration { get; }

public IHostingEnvironment Environment { get; }

public void ConfigureServices(IServiceCollection services)
{
    // Bind defaults from appsettings this time
    services.ConfigureFeatureToggles<MyNewFeature>(
        Configuration.GetSection("Features:MyNewFeature").Bind);

    // Override defaults based on application context
    services.ConfigureFeatureToggleRouter<MyNewFeature, HttpContext>((feature, context) =>
    {
        if (context.User.IsInRole("BetaTester"))
        {
            feature.UseMyNewFeature = true;
        }
    });
}
```

### Using Feature Toggles

Using feature toggles is as easy as injecting `IFeatureToggles` in your services. In a web application, you'd typically do this inside a controller. Then you have to call `GetFeatureToggles()` or `GetFeatureToggles(context)` to get either the default toggles or the smart toggles.

```csharp
public class HomeController : Controller
{
    private readonly IFeatureToggles<MyNewFeature> _feature;

    public HomeController(IFeatureToggles<MyNewFeature> feature)
    {
        _feature = feature;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult MyNewFeature()
    {
        // Call GetFeatureToggles with a context to get smart toggling behavior
        var toggles = _feature.GetFeatureToggles(HttpContext);
        if (toggles.UseMyNewFeature)
        {
            return View();
        }
        else
        {
            return NotFound();
        }
    }
}
```

### Summary

#### Simple Toggles

1. Configure feature toggles with `ConfigureFeatureToggles<TFeature>()`
2. Inject `IFeatureToggles` and call `GetFeatureToggles()`

#### Smart Toggles

1. Configure feature toggles with `ConfigureFeatureToggles<TFeature>()`
1. Configure smart toggling decisions with `ConfigureFeatureToggleRouter<TFeature, TToggleContext>()`
2. Inject `IFeatureToggles` and call `GetFeatureToggles(toggleContext)`

Notes

- There is a limit of 1 toggle router per unique pair of `<TFeature, TToggleContext>`
- If you call `GetFeatureToggles()` and forget to pass a toggle context, you get default toggles instead of smart toggles.
- It's always safe to pass a context. If there is no toggle router for a given context, you just get default toggles.
