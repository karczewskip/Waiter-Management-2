﻿using System.Configuration;
using Microsoft.AspNet.SignalR.Client;
using WaiterManagement.Common.Apps;
using WaiterManagement.Common.Models;
using WaiterManagement.Common.Security;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WaiterManagement.Waiter.Connection
{
	public class WaiterConnectionProvider : IWaiterConnectionProvider
	{
		#region Private Fields
		private HubConnection _hubConnection;
		private IHubProxy _hubProxy;
		#endregion

		#region Dependencies
		private readonly IAccessProvider _accessProvider;
		private readonly IWaiterApp _waiterApp;
		#endregion

		#region Constructors
		public WaiterConnectionProvider(IAccessProvider accessProvider, IWaiterApp waiterApp)
		{
			_accessProvider = accessProvider;
			_waiterApp = waiterApp;
		}
		#endregion

		#region IWaiterConnectionProvider
		public async Task Connect()
		{
			_hubConnection = new HubConnection(ConfigurationManager.AppSettings["ServerPath"]);
			_hubConnection.Headers.Add("login", _accessProvider.Login);
			_hubConnection.Headers.Add("token", _accessProvider.Token);
			_hubProxy = _hubConnection.CreateHubProxy("waiterHub");
			_hubProxy.On<OrderModel>("NewOrderMade", order => _waiterApp.NewOrderMade(order));
			_hubProxy.On<IEnumerable<OrderModel>>("OrdersAwaiting", awaitingOrders => _waiterApp.OrdersAwaiting(awaitingOrders));
			_hubProxy.On<AcceptedOrderCurrentStateModel>("AcceptedOrderInfoUpdated", acceptedOrder => _waiterApp.AcceptedOrderInfoUpdated(acceptedOrder));
			_hubProxy.On<AcceptOrderModel>("OrderWasAccepted", acceptedOrder => _waiterApp.OrderWasAccepted(acceptedOrder));
			_hubProxy.On<String>("CallWaiter", tableLogin => _waiterApp.CallWaiter(tableLogin));
			await _hubConnection.Start();
		}

		public void Disconnect()
		{
			_hubConnection.Stop();
		}

		public void AcceptOrder(int orderId)
		{
			_hubProxy.Invoke("AcceptOrder", new AcceptOrderModel()
			{
				OrderId = orderId
			});
		}

		public void EndOrder(int orderId, bool wasCancelled, string cancelledReason)
		{
			_hubProxy.Invoke("EndOrder", new EndOrderModel()
			{
				OrderId =  orderId,
				OrderCancelled = wasCancelled,
				OrderCancelledReason = cancelledReason
			});
		}

		public void ChangeOrderItemState(int orderId, AcceptedOrderMenuItemQuantity menuItem)
		{
			_hubProxy.Invoke("ChangeOrderItemState", new ChangeOrderItemStateModel()
			{
				OrderId = orderId,
				MenuItemQuantityId = menuItem.MenuItemQuantityId,
				Ready = menuItem.Ready
			});
		}

		public void UpdateAfterLogin()
		{
			_hubProxy.Invoke("UpdateAfterLogin");
		}
		#endregion
	}
}