using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
	public partial class ProlazakVozilaTS
	{
		public int Id { get; set; }
		public string RegistracijskaOznaka { get; set; }
		public int KategorijaVozilaId { get; set; }
		public DateTime VrijemeProlaska { get; set; }
		public int NaplatnaKucicaId { get; set; }

		public virtual KategorijaVozila KategorijaVozila { get; set; }
		public virtual NaplatnaKucica NaplatnaKucica { get; set; }

		public ProlazakVozila ConvertToProlazakVozila()
		{
			ProlazakVozila pv = new ProlazakVozila();
			pv.Id = this.Id;
			pv.RegistracijskaOznaka = this.RegistracijskaOznaka;
			pv.KategorijaVozilaId = this.KategorijaVozilaId;
			pv.NaplatnaKucicaId = this.NaplatnaKucicaId;

			Console.WriteLine(this.VrijemeProlaska.Ticks);
			pv.VrijemeProlaska = BitConverter.GetBytes(this.VrijemeProlaska.Ticks);

			return pv;
		}
	}
}