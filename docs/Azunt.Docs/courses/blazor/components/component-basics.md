# Component basics

A Razor component combines markup and C# behavior in one reusable unit.

## Parameters

Parameters let a parent component provide values to a child component.

```razor
@code {
    [Parameter]
    public string Title { get; set; } = string.Empty;
}
```

## Events

Event callbacks let child components notify their parent without introducing direct coupling.
