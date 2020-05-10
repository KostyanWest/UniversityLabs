using System;

namespace Sem2Lab1
{
	public interface ISimpleGame
	{
		event EventHandler GameEnd;
		void PressKey (ConsoleKey key);
	}
}
