using System.Collections.Generic;
using Hydra.Core.Domain;

namespace Hydra.Core.Provider
{
	public interface IPeptideDataProvider
	{
		IList<Peptide> Read(string fileName);

		void Write(IList<Peptide> peptides, string fileName);
	}
}
