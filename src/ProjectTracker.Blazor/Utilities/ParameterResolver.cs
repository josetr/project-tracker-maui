namespace ProjectTracker;

using Microsoft.AspNetCore.Components;
using System.Reflection;

internal partial class ParameterResolver
{
    private readonly Dictionary<(Type, Type), Action<ComponentBase, ViewModelBase>> resolvers = new();

    public void Resolve(ComponentBase component, ViewModelBase viewModel)
    {
        var key = (component.GetType(), viewModel.GetType());
        if (resolvers.TryGetValue(key, out var resolver))
        {
            resolver(component, viewModel);
            return;
        }

        var mapping = new List<(PropertyInfo, PropertyInfo)>();
        var viewModelProperties = GetProperties(viewModel.GetType());

        foreach (var property in GetProperties(component.GetType()))
        {
            var viewModelProperty = viewModelProperties.Find(x => x.Name == property.Name);
            if (viewModelProperty != null)
                mapping.Add((property, viewModelProperty));
        }

        resolver = new Action<ComponentBase, ViewModelBase>((c, vm) =>
        {
            foreach (var (prop, vmProperty) in mapping)
                vmProperty.SetValue(vm, prop.GetValue(c));
        });

        resolvers[key] = resolver;
        resolver(component, viewModel);

        static List<PropertyInfo> GetProperties(Type type)
        {
            var result = new List<PropertyInfo>();

            foreach (var property in type.GetProperties().Where(x => x.GetSetMethod() != null))
            {
                if (property.GetCustomAttribute<ParameterAttribute>() != null || property.GetCustomAttribute<ViewModels.ParameterAttribute>() != null)
                    result.Add(property);
            }

            return result;
        }
    }
}
