﻿using System;
using System.Threading.Tasks;
using WaiterManagement.Common.Apps;
using WaiterManagement.Common.Models;

namespace WaiterManagement.Waiter.Bootstrapper
{
	public class WaiterApp : IWaiterApp , IWaiterAppSubscriber
	{
		#region IWaiterAppSubscriber
		public event EventHandler<OrderModel> NewOrderHandler;
		public event EventHandler<AcceptedOrderCurrentStateModel> AcceptedOrderInfoUpdatedHandler;
		#endregion

		#region IWaiterApp
		public void NewOrderMade(OrderModel order)
		{
			Task.Run(() => HandleSafely(NewOrderHandler, order)); //Should be awaited...
		}

		public void CallWaiter(string tableLogin)
		{
			throw new System.NotImplementedException();
		}

		public void AcceptedOrderInfoUpdated(AcceptedOrderCurrentStateModel acceptedOrder)
		{
			Task.Run(() => HandleSafely(AcceptedOrderInfoUpdatedHandler, acceptedOrder)); //Should be awaited...
		}
		#endregion

		#region Private Methods
		private void HandleSafely<T>(EventHandler<T> handler, T args)
		{
			if(handler != null)
			{
				try
				{
					handler(this, args);
				}
				catch
				{
					//Logging
				}
			}
		}
		#endregion
	}

	public interface IWaiterAppSubscriber
	{
		event EventHandler<OrderModel> NewOrderHandler;
		event EventHandler<AcceptedOrderCurrentStateModel> AcceptedOrderInfoUpdatedHandler;
	}
}