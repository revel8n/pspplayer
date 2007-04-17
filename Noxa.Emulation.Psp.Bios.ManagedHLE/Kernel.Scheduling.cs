// ----------------------------------------------------------------------------
// PSP Player Emulation Suite
// Copyright (C) 2006 Ben Vanik (noxa)
// Licensed under the LGPL - see License.txt in the project root for details
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Noxa.Emulation.Psp;
using Noxa.Emulation.Psp.Bios;
using Noxa.Emulation.Psp.Cpu;
using Noxa.Emulation.Psp.Games;

namespace Noxa.Emulation.Psp.Bios.ManagedHLE
{
	partial class Kernel
	{
		public bool Schedule()
		{
			while( this.SchedulableThreads.Count == 0 )
			{
				// No threads to run? Check for delayed
				KThread earliest = null;
				for( int n = 0; n < this.Threads.Count; n++ )
				{
					KThread thread = this.Threads[ n ];
					if( ( thread.State == KThreadState.Waiting ) &&
						( thread.WaitingOn == KThreadWait.Delay ) )
					{
						if( ( earliest == null ) ||
							( ( thread.WaitTimestamp + thread.WaitTimeout ) > ( earliest.WaitTimestamp + earliest.WaitTimeout ) ) )
							earliest = thread;
					}
				}

				if( earliest != null )
				{
					// Wait on it
					while( earliest.State == KThreadState.Waiting )
						System.Threading.Thread.Sleep( 1 );
				}
				else
				{
					Debug.WriteLine( "Kernel::Schedule: ran out of threads to run - ending sim" );
					return false;
				}
			}

			// Find next thread to run
			KThread nextThread = this.SchedulableThreads.Head;
			Debug.Assert( nextThread != null );

			// If it didn't change, don't do anything
			if( nextThread == ActiveThread )
				return false;

			// Switch
			Cpu.SwitchContext( nextThread.ContextID );

			ActiveThread = nextThread;

			return true;
		}

		public void Execute()
		{
			if( ActiveThread != null )
			{
				// Execute active thread
				bool breakFlag;
				uint instructionsExecuted;
				Cpu.Execute( out breakFlag, out instructionsExecuted );
				if( breakFlag == true )
					this.Schedule();
				else
				{
					// Only if not broken by choice
					Debug.WriteLine( string.Format( "Kernel: CPU returned to us after {0} instructions", instructionsExecuted ) );
				}
			}
			else
			{
				// Load game

				// Clear everything (needed?)
				Emulator.LightReset();

				// Get bootstream
				Debug.Assert( Bios.BootStream == null );
				GameLoader gameLoader = new GameLoader();
				Bios.BootStream = gameLoader.FindBootStream( Bios.Game );
				Debug.Assert( Bios.BootStream != null );

				LoadParameters loadParams = new LoadParameters();
				loadParams.Path = Bios.Game.Folder;
				LoadResults results = Bios.Loader.LoadModule( ModuleType.Boot, Bios.BootStream, loadParams );

				Debug.Assert( results.Successful == true );
				if( results.Successful == false )
				{
					Debug.WriteLine( string.Format( "Kernel: load of game failed" ) );
					Bios.Game = null;
					return;
				}

				this.CurrentPath = Bios.Game.Folder;
				this.Cpu.SetupGame( Bios.Game, Bios.BootStream );

				// Start modules
				foreach( Module module in Bios._modules )
					module.Start();

				Debug.WriteLine( string.Format( "Kernel: game loaded" ) );
			}
		}

		private KThread _oldThread;
		private KCallback _runningCallback;
		private bool _runningUserCall;

		public bool IssueCallback( KCallback callback, uint argument )
		{
			Debug.Assert( callback != null );
			Debug.Assert( _runningUserCall == false );
			Debug.Assert( _runningCallback == null );

			_oldThread = ActiveThread;
			ActiveThread = callback.Thread;

			_runningCallback = callback;

			Cpu.MarshalCall( ActiveThread.ContextID, callback.Address, new uint[] { argument }, new MarshalCompleteDelegate( this.CallbackComplete ), 0 );

			return true;
		}

		private bool CallbackComplete( int tcsId, int state, int result )
		{
			Debug.Assert( _oldThread != null );
			Debug.Assert( _runningCallback != null );

			// Something ? return value something?

			ActiveThread = _oldThread;
			_oldThread = null;
			_runningCallback = null;

			return true;
		}

		public bool IssueUserCall( uint address, uint[] arguments )
		{
			Debug.Assert( _runningUserCall == false );
			Debug.Assert( _runningCallback == null );

			_runningUserCall = true;

			Cpu.MarshalCall( ActiveThread.ContextID, address, arguments, new MarshalCompleteDelegate( this.UserCallComplete ), ( int )address );

			return true;
		}

		private bool UserCallComplete( int tcsId, int state, int result )
		{
			Debug.Assert( _runningUserCall == true );

			uint address = ( uint )state;
			// Something ? return value?

			_runningUserCall = false;

			return true;
		}
	}
}
