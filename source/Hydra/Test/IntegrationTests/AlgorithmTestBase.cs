using System.Collections.Generic;
using System.ComponentModel;
using Hydra.Core.Events;
using Hydra.Modules.Project;
using MassSpecStudio.Core;
using MassSpecStudio.Core.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Moq;
using ProteoWizard.MassSpecStudio.DataProvider;

namespace Hydra.Test.IntegrationTests
{
	public abstract class AlgorithmTestBase
	{
		protected Mock<IEventAggregator> MockEventAggregator { get; set; }

		protected Mock<IRegionManager> MockRegionManager { get; set; }

		protected Mock<IServiceLocator> MockServiceLocator { get; set; }

		protected Mock<BackgroundWorker> MockBackgroundWorker { get; set; }

		protected OutputEvent OutputEvent { get; set; }

		protected ClickableOutputEvent ClickableOutputEvent { get; set; }

		protected void Initialize()
		{
			MockEventAggregator = new Mock<IEventAggregator>();
			MockRegionManager = new Mock<IRegionManager>();
			MockServiceLocator = new Mock<IServiceLocator>();
			MockBackgroundWorker = new Mock<BackgroundWorker>();

			OutputEvent = new OutputEvent();
			ClickableOutputEvent = new ClickableOutputEvent();
			ProjectOpeningEvent projectOpeningEvent = new ProjectOpeningEvent();
			LoadProjectEvent loadProjectEvent = new LoadProjectEvent();
			ClearOutputEvent clearOutputEvent = new ClearOutputEvent();
			ResultAddedEvent resultAddedEvent = new ResultAddedEvent();
			ViewResultsEvent viewResultsEvent = new ViewResultsEvent();
			StatusUpdateEvent statusUpdateEvent = new StatusUpdateEvent();
			MockEventAggregator.Setup(mock => mock.GetEvent<ClearOutputEvent>()).Returns(clearOutputEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<OutputEvent>()).Returns(OutputEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<ClickableOutputEvent>()).Returns(ClickableOutputEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<ProjectOpeningEvent>()).Returns(projectOpeningEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<LoadProjectEvent>()).Returns(loadProjectEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<ResultAddedEvent>()).Returns(resultAddedEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<ViewResultsEvent>()).Returns(viewResultsEvent);
			MockEventAggregator.Setup(mock => mock.GetEvent<StatusUpdateEvent>()).Returns(statusUpdateEvent);

			List<IExperimentType> experimentTypes = new List<IExperimentType>();
			experimentTypes.Add(new HydraExperimentType(MockServiceLocator.Object));
			List<IDataProvider> dataProviders = new List<IDataProvider>();
			dataProviders.Add(new ProteoWizardDataProvider(MockEventAggregator.Object));
			MockServiceLocator.Setup(mock => mock.GetAllInstances<IExperimentType>()).Returns(experimentTypes);
			MockServiceLocator.Setup(mock => mock.GetAllInstances<IDataProvider>()).Returns(dataProviders);
		}
	}
}
