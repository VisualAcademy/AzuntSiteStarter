# Forms

Blazor includes form components that integrate validation and model binding.

## EditForm

```razor
<EditForm Model="model" OnValidSubmit="SaveAsync">
    <DataAnnotationsValidator />
    <ValidationSummary />
</EditForm>
```

## Validation

Keep validation rules close to the model when the same rules are shared across several forms.
