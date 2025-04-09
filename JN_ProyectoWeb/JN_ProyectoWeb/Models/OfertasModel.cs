namespace JN_ProyectoWeb.Models
{
    public class OfertasModel
    {
        public int Id { get; set; }
        public decimal Salario { get; set; }
        public string? Horario { get; set; }
        public int Cantidad { get; set; }
        public long IdPuesto { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public string? EstadoDescripcion { get; set; }
        public DateTime Fecha { get; set; }
        public long IdOferta { get; set; }
        public int CantidadAplicaciones { get; set; }


    }
}
