using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Hydra.Core.Provider;
using Microsoft.Practices.ServiceLocation;

namespace Hydra.Core.Domain
{
	[DataContract]
	public class Peptides
	{
		private IList<Peptide> peptides;
		private IServiceLocator serviceLocator;

		public Peptides(string location)
		{
			try
			{
				serviceLocator = ServiceLocator.Current;
			}
			catch (NullReferenceException)
			{
			}

			Location = location;
		}

		public Peptides(string location, IServiceLocator serviceLocator)
			: this(location)
		{
			this.serviceLocator = serviceLocator;
		}

		public string Location { get; set; }

		[DataMember]
		public IList<Peptide> PeptideCollection
		{
			get
			{
				if (peptides == null)
				{
					peptides = new List<Peptide>();
				}
				return peptides;
			}
		}

		public static Peptides Open(string path)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Peptides));
			using (XmlReader stream = XmlReader.Create(path))
			{
				Peptides peptides = serializer.ReadObject(stream, true) as Peptides;
				peptides.Location = path;

				foreach (Peptide peptide in peptides.PeptideCollection)
				{
					foreach (FragmentIon fragment in peptide.FragmentIonList)
					{
						fragment.Peptide = peptide;
					}
				}

				return peptides;
			}
		}

		public void Save()
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(Peptides));
			using (FileStream stream = new FileStream(Location, FileMode.Create))
			{
				serializer.WriteObject(stream, this);
			}
		}

		public void Add(string importFileName)
		{
			IPeptideDataProvider dataProvider = serviceLocator.GetInstance<IPeptideDataProvider>();
			IList<Peptide> peptides = dataProvider.Read(importFileName);

			foreach (Peptide peptide in peptides)
			{
				PeptideCollection.Add(peptide);
				foreach (FragmentIon fragment in peptide.FragmentIonList)
				{
					fragment.Peptide = peptide;
				}
			}
		}
	}
}
