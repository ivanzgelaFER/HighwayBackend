namespace RPPP_WebApp.ModelsPartial
{
    public class SadrzajPopratni
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public int? KoordinataX { get; set; }
        public int? KoordinataY { get; set; }
        public int? GodinaOtvaranja { get; set; }
        public string NazivPopratnog { get; set; }
        public TimeSpan RadnimDanomOd { get; set; }
        public TimeSpan RadninDanomDo { get; set; }
        public TimeSpan VikendimaDo { get; set; }
        public TimeSpan VikendimaOd { get; set; }

        public string NazivVrste { get; set; }

    }
}
