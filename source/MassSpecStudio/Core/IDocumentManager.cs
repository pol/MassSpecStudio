using System.Collections.Generic;
using MassSpecStudio.Core.Domain;
using Microsoft.Practices.Prism.Commands;

namespace MassSpecStudio.Core
{
	public interface IDocumentManager
	{
		IList<ExperimentBase> Experiments { get; }

		DelegateCommand<string> New { get; }

		DelegateCommand<string> Save { get; }

		DelegateCommand<string> Close { get; }

		bool IsDirty { get; }

		bool IsProjectOpen { get; }

		void Open(string path);

		void CloseProject();
	}
}
