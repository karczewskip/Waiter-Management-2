﻿using WaiterManagement.BLL.Commands.Base;

namespace WaiterManagement.BLL.Commands.Concrete.ManagerCommands
{
	public class EditCategoryCommand : ICommand
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}
}