// ----------------------------------------------------------------------------
// PSP Player Emulation Suite
// Copyright (C) 2006 Ben Vanik (noxa)
// Licensed under the LGPL - see License.txt in the project root for details
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;

namespace Noxa.Emulation.Psp.Player.Development.Tools
{
	partial class ToolDocument : WeifenLuo.WinFormsUI.DockContent
	{
		public ToolDocument()
		{
			InitializeComponent();

			this.DockableAreas = DockAreas.Document | DockAreas.Float;
		}
	}
}

