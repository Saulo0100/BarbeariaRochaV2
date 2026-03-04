namespace BarbeariaRocha.Modelos.Response.Mensalista
{
    public class MensalistaResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Tipo { get; set; }
        public required string Status { get; set; }
        public required string Dia { get; set; }
        public decimal Valor { get; set; }
        public int CortesNoMes { get; set; }
        public List<MensalistaCorteResponse> Cortes { get; set; } = [];
    }
}
