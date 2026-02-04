using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dereliction.ViewModels;

namespace Dereliction;

public class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public Control Build(object? data)
    {
        if (data == null)
        {
            return new TextBlock { Text = "Null" };
        }
        string name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);
        return type != null ? (Control)Activator.CreateInstance(type)! : new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
