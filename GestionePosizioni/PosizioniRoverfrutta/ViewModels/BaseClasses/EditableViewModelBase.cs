using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace PosizioniRoverfrutta.ViewModels.BaseClasses
{
    public abstract class EditableModelBase<T> : IEditableObject
    {
        private T Cache { get; set; }

        private object CurrentModel
        {
            get { return this; }
        }

        public ICommand CancelEditCommand
        {
            get { return new DelegateCommand(CancelEdit); }
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            Cache = Activator.CreateInstance<T>();

            //Set Properties of Cache
            foreach (var info in CurrentModel.GetType().GetProperties())
            {
                if (!info.CanRead || !info.CanWrite) continue;
                var oldValue = info.GetValue(CurrentModel, null);
                Cache.GetType().GetProperty(info.Name).SetValue(Cache, oldValue, null);
            }
        }

        public void EndEdit()
        {
            Cache = default(T);
        }


        public void CancelEdit()
        {
            foreach (var info in CurrentModel.GetType().GetProperties())
            {
                if (!info.CanRead || !info.CanWrite) continue;
                var oldValue = info.GetValue(Cache, null);
                CurrentModel.GetType().GetProperty(info.Name).SetValue(CurrentModel, oldValue, null);
            }
        }

        #endregion
    }
}