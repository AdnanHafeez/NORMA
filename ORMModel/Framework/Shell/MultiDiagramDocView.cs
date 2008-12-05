#region Common Public License Copyright Notice
/**************************************************************************\
* Neumont Object-Role Modeling Architect for Visual Studio                 *
*                                                                          *
* Copyright � Neumont University. All rights reserved.                     *
* Copyright � Matthew Curland. All rights reserved.                        *
*                                                                          *
* The use and distribution terms for this software are covered by the      *
* Common Public License 1.0 (http://opensource.org/licenses/cpl) which     *
* can be found in the file CPL.txt at the root of this distribution.       *
* By using this software in any fashion, you are agreeing to be bound by   *
* the terms of this license.                                               *
*                                                                          *
* You must not remove this notice, or any other, from this software.       *
\**************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Neumont.Tools.Modeling.Design;

namespace Neumont.Tools.Modeling.Shell
{
	/// <summary>
	/// Base class for <see cref="DiagramDocView"/> implementations that support multiple <see cref="Diagram"/>s.
	/// </summary>
	[CLSCompliant(false)]
	public abstract partial class MultiDiagramDocView : DiagramDocView, IModelingEventSubscriber
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <remarks>
		/// For parameter descriptions, see <see cref="DiagramDocView(ModelingDocData,IServiceProvider)"/>.
		/// </remarks>
		protected MultiDiagramDocView(ModelingDocData docData, IServiceProvider serviceProvider) : base(docData, serviceProvider)
		{
			myDiagramRefCounts = new Dictionary<Diagram, int>();
		}
		#endregion // Constructor
		#region Fields and Private Properties
		private readonly Dictionary<Diagram, int> myDiagramRefCounts;
		private MultiDiagramDocViewControl myDocViewControl;
		private bool myVerifyPageOrder;
		private MultiDiagramDocViewControl DocViewControl
		{
			get
			{
				return myDocViewControl ?? (myDocViewControl = new MultiDiagramDocViewControl(this));
			}
		}
		#endregion // Fields and Private Properties
		#region Constants
		/// <summary>
		/// The <see cref="Size.Width"/> of <see cref="Image"/>s displayed on tabs.
		/// </summary>
		public const int DiagramImageWidth = 28;
		/// <summary>
		/// The <see cref="Size.Height"/> of <see cref="Image"/>s displayed on tabs.
		/// </summary>
		public const int DiagramImageHeight = 14;
		/// <summary>
		/// The <see cref="Size"/> of <see cref="Image"/>s displayed on tabs.
		/// </summary>
		public static readonly Size DiagramImageSize = new Size(DiagramImageWidth, DiagramImageHeight);
		#endregion // Constants
		#region Public Properties
		#region Context Menu support
		/// <summary>
		/// A <see cref="ContextMenuStrip"/> that operates against a specific <see cref="DiagramView"/>
		/// and <see cref="Diagram"/>.
		/// </summary>
		protected class MultiDiagramContextMenuStrip : ContextMenuStrip
		{
			/// <summary>
			/// Initializes a new instance of <see cref="MultiDiagramContextMenuStrip"/>.
			/// </summary>
			public MultiDiagramContextMenuStrip()
				: base()
			{
			}

			private DiagramView mySelectedDesigner;
			/// <summary>
			/// Gets the <see cref="Diagram"/> against which this <see cref="MultiDiagramContextMenuStrip"/>
			/// is currently operating.
			/// </summary>
			public Diagram SelectedDiagram
			{
				get
				{
					DiagramView selectedDesigner = mySelectedDesigner;
					return (selectedDesigner != null) ? selectedDesigner.Diagram : null;
				}
			}

			/// <summary>See <see cref="ToolStripDropDown.OnOpening"/>.</summary>
			protected override void OnOpening(System.ComponentModel.CancelEventArgs e)
			{
				MultiDiagramDocViewControl docViewControl = base.SourceControl as MultiDiagramDocViewControl;
				if (docViewControl != null)
				{
					mySelectedDesigner = docViewControl.AcquireContextMenuDesigner;
				}
				base.OnOpening(e);
			}
		}
		/// <summary>
		/// Gets or sets the <see cref="MultiDiagramContextMenuStrip"/> for this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		protected MultiDiagramContextMenuStrip ContextMenuStrip
		{
			get
			{
				return (myDocViewControl != null) ? myDocViewControl.ContextMenuStrip as MultiDiagramContextMenuStrip : null;
			}
			set
			{
				DocViewControl.ContextMenuStrip = value;
			}
		}
		#endregion // Context Menu support
		#region Window property
		/// <summary>See <see cref="Microsoft.VisualStudio.Shell.WindowPane.Window"/>.</summary>
		public override IWin32Window Window
		{
			get
			{
				return DocViewControl.Parent;
			}
		}
		#endregion // Window property
		#region CurrentDesigner property
		/// <summary>See <see cref="DiagramDocView.CurrentDesigner"/>.</summary>
		public override VSDiagramView CurrentDesigner
		{
			[DebuggerStepThrough]
			get
			{
				if (myDocViewControl != null)
				{
					DiagramTabPage tabPage = myDocViewControl.SelectedDiagramTab;
					if (tabPage != null)
					{
						return (VSDiagramView)tabPage.Designer;
					}
				}
				return null;
			}
		}
		#endregion // CurrentDesigner property
		#region CurrentDiagram property
		/// <summary>See <see cref="DiagramDocView.CurrentDiagram"/>.</summary>
		public override Diagram CurrentDiagram
		{
			[DebuggerStepThrough]
			get
			{
				if (myDocViewControl != null)
				{
					DiagramTabPage tabPage = myDocViewControl.SelectedDiagramTab;
					if (tabPage != null)
					{
						return tabPage.Diagram;
					}
				}
				return null;
			}
		}
		#endregion // CurrentDiagram property
		#region Store property
		/// <summary>
		/// Returns the <see cref="Microsoft.VisualStudio.Modeling.Store"/> of the <see cref="ModelingDocData"/>
		/// associated with this <see cref="MultiDiagramDocView"/>. If no <see cref="ModelingDocData"/> is
		/// associated with this <see cref="MultiDiagramDocView"/>, or if
		/// <see cref="Microsoft.VisualStudio.Modeling.Store.ShuttingDown"/> or
		/// <see cref="Microsoft.VisualStudio.Modeling.Store.Disposed"/> return <see langword="true"/>,
		/// <see langword="null"/> is returned.
		/// </summary>
		public Store Store
		{
			get
			{
				ModelingDocData docData = DocData;
				if (docData != null)
				{
					Store store = docData.Store;
					if (store != null && !store.ShuttingDown && !store.Disposed)
					{
						return store;
					}
				}
				return null;
			}
		}
		#endregion // Store property
		#endregion // Public Properties
		#region Methods
		#region RegisterImageForDiagramType method
		/// <summary>
		/// Associates the <see cref="Image"/> specified by <paramref name="image"/> with the <see cref="Type"/>
		/// of <see cref="Diagram"/> specified by <paramref name="diagramType"/>.
		/// </summary>
		/// <param name="diagramType">
		/// The <see cref="Type"/> of <see cref="Diagram"/> for which <paramref name="image"/> is being registered.
		/// </param>
		/// <param name="image">
		/// The <see cref="Image"/> to be associated with <paramref name="diagramType"/>, or <see langword="null"/>
		/// to only remove all previous associated <see cref="Image"/>s for <paramref name="diagramType"/> without
		/// associating a new <see cref="Image"/>.
		/// </param>
		/// <remarks>
		/// Any and all previously associated <see cref="Image"/>s for <paramref name="diagramType"/> will be
		/// replaced by <paramref name="image"/> if it is not <see langword="null"/>.
		/// </remarks>
		public void RegisterImageForDiagramType(Type diagramType, Image image)
		{
			if (diagramType == null)
			{
				throw new ArgumentNullException("diagramType");
			}
			MultiDiagramDocViewControl docViewControl = DocViewControl;
			ImageList imageList = docViewControl.ImageList;
			if (imageList == null)
			{
				// If we don't have an ImageList and the caller only wants to clear existing images, we don't need to do anything
				if (image == null)
				{
					return;
				}
				imageList = docViewControl.ImageList = new ImageList();
				imageList.ColorDepth = ColorDepth.Depth32Bit;
				imageList.TransparentColor = Color.Transparent;
				imageList.ImageSize = DiagramImageSize;
			}
			string key = diagramType.GUID.ToString("N", null);
			ImageList.ImageCollection images = imageList.Images;
			int oldIndex;
			while ((oldIndex = images.IndexOfKey(key)) >= 0)
			{
				images.RemoveAt(oldIndex);
			}
			if (image != null)
			{
				images.Add(key, image);
			}
		}
		#endregion // RegisterImageForDiagramType method
		#region RenameDiagram method
		/// <summary>
		/// Activates the user interface for renaming the specified <see cref="Diagram"/>
		/// </summary>
		public void RenameDiagram(Diagram diagram)
		{
			MultiDiagramDocViewControl docViewControl;
			if (null != diagram &&
				null != (docViewControl = myDocViewControl))
			{
				docViewControl.RenameDiagram(diagram);
			}
		}
		#endregion // RenameDiagram method
		#region ReorderDiagrams method
		/// <summary>
		/// Reorder the diagram tab pages
		/// </summary>
		public void ReorderDiagrams()
		{
			MultiDiagramDocViewControl docViewControl;
			TabControl.TabPageCollection pages;
			int diagramCount;
			if (null != (docViewControl = myDocViewControl) &&
				0 != (diagramCount = (pages = docViewControl.TabPages).Count))
			{
				Diagram[] diagrams = new Diagram[diagramCount];
				for (int i = 0; i < diagramCount; ++i)
				{
					diagrams[i] = ((DiagramTabPage)pages[i]).Diagram;
				}
				DiagramOrderDialog.ShowDialog(ServiceProvider, DocData, diagrams, myDocViewControl.ImageList);
			}
		}
		/// <summary>
		/// Check if a diagram has multiple pages.
		/// </summary>
		public bool HasMultiplePages
		{
			get
			{
				MultiDiagramDocViewControl docViewControl = myDocViewControl;
				return null != docViewControl && 1 < docViewControl.TabPages.Count;
			}
		}
		#endregion // ReorderDiagrams method
		#region Add methods
		/// <summary>
		/// Adds the <see cref="Diagram"/> specified by <paramref name="diagram"/> to this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="diagram">The <see cref="Diagram"/> to be added to this <see cref="MultiDiagramDocView"/>.</param>
		public void AddDiagram(Diagram diagram)
		{
			AddDiagram(diagram, false);
		}
		/// <summary>
		/// Adds the <see cref="Diagram"/> specified by <paramref name="diagram"/> to this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="diagram">The <see cref="Diagram"/> to be added to this <see cref="MultiDiagramDocView"/>.</param>
		/// <param name="selectAsCurrent">Indicates whether focus should be given to the new <see cref="Diagram"/>.</param>
		public void AddDiagram(Diagram diagram, bool selectAsCurrent)
		{
			if (diagram == null)
			{
				throw new ArgumentNullException("diagram");
			}
			DiagramView designer = CreateDiagramView();
			diagram.Associate(designer);
			AddDesigner(designer, selectAsCurrent);
		}
		/// <summary>
		/// Adds the <see cref="DiagramView"/> specified by <paramref name="designer"/> to this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="designer">The <see cref="DiagramView"/> to be added to this <see cref="MultiDiagramDocView"/>.</param>
		public void AddDesigner(DiagramView designer)
		{
			AddDesigner(designer, false);
		}
		/// <summary>
		/// Adds the <see cref="DiagramView"/> specified by <paramref name="designer"/> to this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="designer">The <see cref="DiagramView"/> to be added to this <see cref="MultiDiagramDocView"/>.</param>
		/// <param name="selectAsCurrent">Indicates whether focus should be given to the new <see cref="DiagramView"/>.</param>
		public void AddDesigner(DiagramView designer, bool selectAsCurrent)
		{
			if (designer == null)
			{
				throw new ArgumentNullException("designer");
			}
			MultiDiagramDocViewControl docViewControl = DocViewControl;
			int tabCount = docViewControl.TabCount;
			DiagramTabPage tabPage = new DiagramTabPage(docViewControl, designer);
			Diagram diagram = designer.Diagram;
			Dictionary<Diagram, int> diagramRefCounts = myDiagramRefCounts;
			int refCount;
			if (!diagramRefCounts.TryGetValue(diagram, out refCount) || refCount <= 0)
			{
				diagram.DiagramRemoved += DiagramRemoved;
			}
			diagramRefCounts[diagram] = refCount + 1;
			if (selectAsCurrent)
			{
				// If this is not here then the tab is not correctly activated.
				docViewControl.SelectedIndex = -1;
				docViewControl.SelectTab(tabPage);
			}
		}
		#endregion // Add methods
		#region Remove methods
		/// <summary>
		/// Removes all <see cref="Diagram"/>s from this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		public void RemoveAllDiagrams()
		{
			MultiDiagramDocViewControl control = myDocViewControl;
			if (control == null)
			{
				return;
			}
			int tabCount;
			while ((tabCount = control.TabCount) > 0)
			{
				RemoveAt(tabCount - 1);
			}
		}
		/// <summary>
		/// Removes the <see cref="Diagram"/> specified by <see cref="CurrentDiagram"/> from this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		public void RemoveCurrentDiagram()
		{
			RemoveDiagram(CurrentDiagram);
		}
		/// <summary>
		/// Removes the <see cref="Diagram"/> specified by <paramref name="diagram"/> from this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="diagram">The <see cref="Diagram"/> to be removed.</param>
		public void RemoveDiagram(Diagram diagram)
		{
			if (diagram == null)
			{
				throw new ArgumentNullException("diagram");
			}
			MultiDiagramDocViewControl control = myDocViewControl;
			if (control == null)
			{
				return;
			}
			int lastIndex = 0;
			while ((lastIndex = control.IndexOf(diagram, lastIndex)) >= 0)
			{
				RemoveAt(lastIndex);
			}
		}
		/// <summary>
		/// Removes the <see cref="DiagramView"/> specified by <see cref="CurrentDesigner"/> from this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		public void RemoveCurrentDesigner()
		{
			RemoveDesigner(CurrentDesigner);
		}
		/// <summary>
		/// Removes the <see cref="DiagramView"/> specified by <paramref name="designer"/> from this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="designer">The <see cref="DiagramView"/> to be removed.</param>
		public void RemoveDesigner(DiagramView designer)
		{
			if (designer == null)
			{
				throw new ArgumentNullException("designer");
			}
			MultiDiagramDocViewControl control = myDocViewControl;
			if (control == null)
			{
				return;
			}
			int lastIndex = 0;
			while ((lastIndex = control.IndexOf(designer, lastIndex)) >= 0)
			{
				RemoveAt(lastIndex);
			}
		}
		private void RemoveAt(int index)
		{
			DiagramTabPage tabPage = (DiagramTabPage)myDocViewControl.TabPages[index];
			Diagram diagram = tabPage.Diagram;
			if (diagram != null)
			{
				Dictionary<Diagram, int> diagramRefCounts = myDiagramRefCounts;
				int refCount = diagramRefCounts[diagram] - 1;
				if (refCount <= 0)
				{
					diagram.DiagramRemoved -= DiagramRemoved;
					diagramRefCounts.Remove(diagram);
				}
				else
				{
					diagramRefCounts[diagram] = refCount;
				}
			}
			// HACK: Set Parent to null rather than removing the TabPage from the Parent's TabPages collection.
			// If we do the latter, for some reason another TabPage gets removed in addition to the TabPage we are processing.
			tabPage.Parent = null;
			tabPage.Dispose();
		}
		#endregion // Remove methods
		#region Select methods
		/// <summary>
		/// Selects the <see cref="Diagram"/> specified by <paramref name="diagram"/> for this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="diagram">The <see cref="Diagram"/> to be selected.</param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="Diagram"/> is successfully selected; otherwise, <see langword="false"/>.
		/// </returns>
		public bool SelectDiagram(Diagram diagram)
		{
			if (diagram == null)
			{
				throw new ArgumentNullException("diagram");
			}
			MultiDiagramDocViewControl control = myDocViewControl;
			int index;
			if (control != null && (index = control.IndexOf(diagram, 0)) >= 0)
			{
				// Although a single Diagram could theoretically appear on multiple tabs,
				// we only need to activate one.
				control.SelectedIndex = index;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Selects the <see cref="DiagramView"/> specified by <paramref name="designer"/> for this <see cref="MultiDiagramDocView"/>.
		/// </summary>
		/// <param name="designer">The <see cref="DiagramView"/> to be selected.</param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="DiagramView"/> is successfully selected; otherwise, <see langword="false"/>.
		/// </returns>
		public bool SelectDesigner(DiagramView designer)
		{
			if (designer == null)
			{
				throw new ArgumentNullException("designer");
			}
			MultiDiagramDocViewControl control = myDocViewControl;
			int index;
			if (control != null && (index = control.IndexOf(designer, 0)) >= 0)
			{
				// Although a single DiagramView could theoretically appear on multiple tabs,
				// we only need to activate one.
				control.SelectedIndex = index;
				return true;
			}
			return false;
		}
		#endregion // Select methods
		#region DiagramRemoved event handler
		private void DiagramRemoved(object sender, ElementDeletedEventArgs e)
		{
			RemoveDiagram(e.ModelElement as Diagram);
		}
		#endregion // DiagramRemoved event handler
		#region Dispose method
		/// <summary>See <see cref="DiagramDocView.Dispose"/>.</summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (myDocViewControl != null)
					{
						myDocViewControl.Parent.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		#endregion // Dispose method
		#endregion // Methods
		#region IModelingEventSubscriber Implementation
		/// <summary>
		/// Implement <see cref="IModelingEventSubscriber.ManageModelingEventHandlers"/>
		/// </summary>
		protected void ManageModelingEventHandlers(ModelingEventManager eventManager, EventSubscriberReasons reasons, EventHandlerAction action)
		{
			if (0 != (reasons & EventSubscriberReasons.DocumentLoaded))
			{
				Store store = this.Store;
				if (null != store.FindDomainModel(DiagramDisplayDomainModel.DomainModelId))
				{
					IPropertyProviderService providerService = ((IFrameworkServices)store).PropertyProviderService;
					if (providerService != null)
					{
						providerService.AddOrRemovePropertyProvider<Diagram>(DiagramDisplay.ProvideDiagramProperties, true, action);
					}
					if (0 != (reasons & EventSubscriberReasons.UserInterfaceEvents))
					{
						DomainDataDirectory dataDirectory = store.DomainDataDirectory;
						DomainClassInfo classInfo;
						classInfo = dataDirectory.FindDomainRelationship(DiagramDisplayHasDiagramOrder.DomainClassId);
						if (classInfo != null)
						{
							// DiagramDisplay is an optional domain model, it may not be loaded in the store
							eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementAddedEventArgs>(VerifyPageOrderEvent), action);
							eventManager.AddOrRemoveHandler(classInfo, new EventHandler<ElementDeletedEventArgs>(VerifyPageOrderEvent), action);
							eventManager.AddOrRemoveHandler(classInfo, new EventHandler<RolePlayerOrderChangedEventArgs>(VerifyPageOrderEvent), action);
							eventManager.AddOrRemoveHandler(classInfo, new EventHandler<RolePlayerChangedEventArgs>(VerifyPageOrderEvent), action);
							eventManager.AddOrRemoveHandler(new EventHandler<ElementEventsEndedEventArgs>(ElementEventsEndedEvent), action);
						}
					}
				}
			}
		}
		void IModelingEventSubscriber.ManageModelingEventHandlers(ModelingEventManager eventManager, EventSubscriberReasons reasons, EventHandlerAction action)
		{
			ManageModelingEventHandlers(eventManager, reasons, action);
		}
		/// <summary>
		/// Mark the tab order as potentially dirty
		/// </summary>
		private void VerifyPageOrderEvent(object sender, EventArgs e)
		{
			myVerifyPageOrder = true;
		}
		/// <summary>
		/// Verify tab order when events have complete
		/// </summary>
		private void ElementEventsEndedEvent(object sender, ElementEventsEndedEventArgs e)
		{
			if (myVerifyPageOrder)
			{
				myVerifyPageOrder = false;
				MultiDiagramDocViewControl control;
				Store store;
				IList<DiagramDisplay> containers;
				if (null != (control = myDocViewControl) &&
					null != (store = this.Store) &&
					0 != (containers = store.ElementDirectory.FindElements<DiagramDisplay>(false)).Count)
				{
					control.VerifyDiagramOrder(containers[0].OrderedDiagramCollection);
				}
			}
		}
		#endregion // IModelingEventSubscriber Implementation
	}
}
