using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MassSpecStudio.Core.Domain;
using MassSpecStudio.Core.Processing;

namespace Hydra.Processing.Algorithm
{
	public abstract class AlgorithmBase : IAlgorithm
	{
		private IList<IProcessingStep> _processingSteps;

		public AlgorithmBase()
		{
			_processingSteps = new List<IProcessingStep>();
		}

		public abstract string Name { get; }

		public bool IsEnabled
		{
			get { return true; }
		}

		public IList<IProcessingStep> ProcessingSteps
		{
			get { return _processingSteps; }
		}

		public abstract void Execute(BackgroundWorker worker, ExperimentBase experimentBase);

		public void SetParameters(MassSpecStudio.Core.Domain.Algorithm algorithm)
		{
			if (algorithm != null)
			{
				foreach (MassSpecStudio.Core.Domain.ProcessingStep processingParameters in algorithm.ProcessingSteps)
				{
					IProcessingStep processingStep = _processingSteps.Where(item => item.GetType().FullName == processingParameters.Type).FirstOrDefault();
					if (processingStep != null)
					{
						foreach (ProcessingParameter processingParameter in processingParameters.Parameters)
						{
							PropertyInfo propertyInfo = processingStep.GetType().GetProperty(processingParameter.Name);
							if (propertyInfo.PropertyType.IsEnum)
							{
								object enumValue = Enum.Parse(propertyInfo.PropertyType, processingParameter.Value);
								propertyInfo.SetValue(processingStep, enumValue, null);
							}
							else
							{
								propertyInfo.SetValue(processingStep, Convert.ChangeType(processingParameter.Value, propertyInfo.PropertyType), null);
							}
						}
					}
				}
			}
		}
	}
}
