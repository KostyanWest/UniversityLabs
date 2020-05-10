using System;

namespace Sem2Lab1
{
	public abstract class Cell
	{
		private static readonly long MAX_VALUE_FOR_MERGE = long.MaxValue / 2;
		private long value = 0;

		public event EventHandler<ValueChangeEventArgs> ValueChange;

		public long Value
		{
			get => value;
			set
			{
				if (value < 0) {
					throw new ArgumentOutOfRangeException ();
				}
				long oldValue = this.value;
				this.value = value;
				ChangeColor ();
				OnValueChange (new ValueChangeEventArgs (oldValue, value));
			}
		}

		protected virtual void OnValueChange (ValueChangeEventArgs e)
		{
			ValueChange?.Invoke (this, e);
		}

		protected abstract void ChangeColor ();

		public bool CanMergeWith (Cell cell)
		{
			long otherValue = cell.Value;
			if (Value == otherValue && Value <= MAX_VALUE_FOR_MERGE && otherValue <= MAX_VALUE_FOR_MERGE) {
				return true;
			} else {
				return false;
			}
		}

		public virtual void Destroy ()
		{
			ValueChange = null;
		}
	}
}
