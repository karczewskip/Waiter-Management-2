﻿using Caliburn.Micro;
using WaiterManagement.Wpf.MVVM.Abstract;

namespace WaiterManagement.Manager.Bootstrapper
{
	public sealed class ViewModelResolver : IViewModelResolver
	{
		public T Resolve<T>() where T : IViewModel
		{
			return IoC.Get<T>();
		}
	}
}