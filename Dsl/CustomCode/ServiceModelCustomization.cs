using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Partial class to attach the custom TypeDescriptionProvider to ServiceModel.
	/// This makes the Version property read-only in the Properties window.
	/// </summary>
	[TypeDescriptionProvider(typeof(ServiceModelTypeDescriptionProvider))]
	public partial class ServiceModel
	{
		[Browsable(false)]
		public List<string> ServiceUsingsList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.ServiceUsings))
					return new List<string>();

				return this.ServiceUsings
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.ToList();
			}
		}

		[Browsable(false)]
		public List<string> ControllerUsingsList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.ControllerUsings))
					return new List<string>();

				return this.ControllerUsings
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.ToList();
			}
		}
	}
}
