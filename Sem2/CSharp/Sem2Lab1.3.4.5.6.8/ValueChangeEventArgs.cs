using System;

namespace Sem2Lab1
{
	public class ValueChangeEventArgs : EventArgs
	{
		public readonly long oldValue;
		public readonly long newValue;

		public ValueChangeEventArgs (long oldValue, long newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
}
