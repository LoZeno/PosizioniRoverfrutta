using System;
using Prism.Mvvm;

namespace Microsoft.Practices.Prism.Commands
{
    public class DelegateCommand : global::Prism.Commands.DelegateCommand
    {
        public DelegateCommand(Action executeMethod) : base(executeMethod) { }
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : base(executeMethod, canExecuteMethod) { }
    }

    public class DelegateCommand<T> : global::Prism.Commands.DelegateCommand<T>
    {
        public DelegateCommand(Action<T> executeMethod) : base(executeMethod) { }
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : base(executeMethod, canExecuteMethod) { }
    }
}

namespace Microsoft.Practices.Prism.ViewModel
{
    public abstract class NotificationObject : BindableBase
    {
    }
}
